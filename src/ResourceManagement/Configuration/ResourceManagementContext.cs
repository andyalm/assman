using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using AlmWitt.Web.ResourceManagement.ContentFiltering;
using AlmWitt.Web.ResourceManagement.PreConsolidation;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class ResourceManagementContext
	{
		public static ResourceManagementContext Create()
		{
			return new ResourceManagementContext();
		}

		private static ResourceManagementContext _current;

		public static ResourceManagementContext Current
		{
			get
			{
				if(_current == null)
				{
					_current = ResourceManagementConfiguration.Current.BuildContext();
				}

				return _current;
			}
			set { _current = value; }
		}

		private readonly CompositeResourceFinder _finder;
		private readonly ContentFilterMap _filterMap;
		private readonly ResourceGroupTemplateCollection _scriptGroups;
		private readonly ResourceGroupTemplateCollection _styleGroups;
		private readonly List<Assembly> _assemblies;
		private readonly DependencyManager _dependencyManager;
		private readonly ThreadSafeInMemoryCache<string, string> _resolvedResourceUrls = new ThreadSafeInMemoryCache<string, string>(StringComparer.OrdinalIgnoreCase);

		internal ResourceManagementContext()
		{
			_finder = new CompositeResourceFinder();
			_filterMap = new ContentFilterMap();
			_scriptGroups = new ResourceGroupTemplateCollection();
			_styleGroups = new ResourceGroupTemplateCollection();
			_assemblies = new List<Assembly>();
			_dependencyManager = DependencyManagerFactory.GetDependencyManager(_finder, _scriptGroups, _styleGroups);
			ConsolidateClientScripts = true;
			ConsolidateCssFiles = true;
		}

		public DateTime ConfigurationLastModified { get; set; }
		public bool PreConsolidated { get; private set; }
		public bool ConsolidateClientScripts { get; set; }
		public bool ConsolidateCssFiles { get; set; }

		public bool ManageDependencies { get; set; }

		public string Version { get; set; }

		public void MapExtensionToFilter(string fileExtension, IContentFilterFactory filterFactory)
		{
			_filterMap.MapExtension(fileExtension, filterFactory);
		}

		public ResourceGroupTemplateCollection ScriptGroups
		{
			get { return _scriptGroups; }
		}

		public ResourceGroupTemplateCollection StyleGroups
		{
			get { return _styleGroups; }
		}

		public IContentFilterFactory GetFilterFactoryForExtension(string fileExtension)
		{
			return _filterMap.GetFilterFactoryForExtension(fileExtension);
		}

		public void AddFinder(IResourceFinder finder)
		{
			_finder.AddFinder(finder);
		}

		public void AddFinders(IEnumerable<IResourceFinder> finders)
		{
			_finder.AddFinders(finders);
		}

		public void AddAssembly(Assembly assembly)
		{
			_assemblies.Add(assembly);
			_finder.AddFinder(ResourceFinderFactory.GetInstance(assembly));
		}

		public void AddAssemblies(IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
			{
				AddAssembly(assembly);
			}
		}

		public IEnumerable<Assembly> GetAssemblies()
		{
			return _assemblies;
		}

		public ConsolidatedResource ConsolidateGroup(string groupConsolidatedUrl, GroupTemplateContext groupTemplateContext, ResourceMode mode)
		{
			var group = groupTemplateContext.FindGroupOrDefault(_finder, groupConsolidatedUrl, mode);
			
			if(group == null)
			{
				throw new Exception("No group with consolidatedUrl '" + groupConsolidatedUrl + "' could be found.");
			}

			return ConsolidateGroup(group, mode);
		}

		public ConsolidatedResource ConsolidateGroup(IResourceGroup group, ResourceMode mode)
		{
			Func<IResource, IContentFilter> createContentFilter = resource =>
			{
				var contentFilterFactory = GetFilterFactoryForExtension(resource.FileExtension);
				return contentFilterFactory.CreateFilter(group, mode);
			};

			return group.GetResources()
				.SortByDependencies(_dependencyManager)
				.Consolidate(createContentFilter, group.ResourceType.Separator);
		}

		public PreConsolidationReport ConsolidateAll(Action<ConsolidatedResource, IResourceGroup> handleConsolidatedResource, ResourceMode mode)
		{
			return new PreConsolidationReport
			{
				ClientScriptGroups = ConsolidateAllInternal(ScriptGroups, handleConsolidatedResource, mode).ToList(),
				CssGroups = ConsolidateAllInternal(StyleGroups, handleConsolidatedResource, mode).ToList(),
				Dependencies = GetAllDependencies(mode).ToList()
			};
		}

		private IEnumerable<PreConsolidatedResourceDependencies> GetAllDependencies(ResourceMode mode)
		{
			var allResources = _finder.FindResources(ResourceType.ClientScript)
									  .Union(_finder.FindResources(ResourceType.Css));

			//we must gather the dependencies for the consolidated url's before we gather them for specific url's
			//because the consolidated url's could actually exist on disk if pre-consolidation was run before.
			//In that case, if we did the specific resources first, it would use the IDependencyProvider against the
			//consolidated resource and not find any dependencies.  Then that value is cached and prevents us from finding
			//the dependencies for the group.
			var scriptDependenciesByConsolidatedUrl = GetDependenciesForConsolidatedUrls(ScriptGroups, allResources, mode);
			var styleDependenciesByConsolidatedUrl = GetDependenciesForConsolidatedUrls(StyleGroups, allResources, mode);

			var specificResourceDependencies = from resource in allResources
											   let dependencies = _dependencyManager.GetDependencies(resource)
											   where dependencies.Any()
											   select new PreConsolidatedResourceDependencies
											   {
												   ResourcePath = resource.VirtualPath,
												   Dependencies = dependencies.ToList()
											   };

			return scriptDependenciesByConsolidatedUrl.Union(styleDependenciesByConsolidatedUrl).Union(specificResourceDependencies);
		}

		public GroupTemplateContext FindGroupTemplate(string consolidatedUrl)
		{
			var resourceType = ResourceType.FromPath(consolidatedUrl);

			return GroupTemplatesOfType(resourceType).GetGroupTemplateOrDefault(consolidatedUrl);
		}

		public IEnumerable<string> GetResourceDependencies(string virtualPath)
		{
			return _dependencyManager.GetDependencies(virtualPath);
		}

		public void MapExtensionToDependencyProvider(string fileExtension, IDependencyProvider dependencyProvider)
		{
			_dependencyManager.MapProvider(fileExtension, dependencyProvider);
		}

		public string GetScriptUrl(string scriptUrl)
		{
			return GetResourceUrl(ConsolidateClientScripts, ScriptGroups, scriptUrl);
		}

		public string GetStylesheetUrl(string stylesheetUrl)
		{
			return GetResourceUrl(ConsolidateCssFiles, StyleGroups, stylesheetUrl);
		}

		public void LoadPreCompilationReport(PreConsolidationReport preConsolidationReport)
		{
			PopulateResourceUrlMap(preConsolidationReport.ClientScriptGroups);
			PopulateResourceUrlMap(preConsolidationReport.CssGroups);
			PopulateDependencyCache(preConsolidationReport.Dependencies);
			Version = preConsolidationReport.Version;
			PreConsolidated = true;
		}

		private void PopulateDependencyCache(IEnumerable<PreConsolidatedResourceDependencies> dependencies)
		{
			var cache = new PreConsolidatedDependencyCache();
			foreach (var resourceWithDependency in dependencies)
			{
				cache.SetDependencies(resourceWithDependency.ResourcePath, resourceWithDependency.Dependencies);
			}

			_dependencyManager.SetCache(cache);
		}

		private void PopulateResourceUrlMap(IEnumerable<PreConsolidatedResourceGroup> groups)
		{
			var resourceUrlMap = from @group in groups
								 from resourcePath in @group.Resources
								 select new KeyValuePair<string, string>(resourcePath, @group.ConsolidatedUrl);

			_resolvedResourceUrls.AddRange(resourceUrlMap);

			var consolidatedUrls = from @group in groups
			                       select new KeyValuePair<string, string>(@group.ConsolidatedUrl, @group.ConsolidatedUrl);

			_resolvedResourceUrls.AddRange(consolidatedUrls);
		}

		internal IResourceFinder Finder
		{
			get { return _finder; }
		}

		private string GetResourceUrl(bool consolidate, IEnumerable<IResourceGroupTemplate> groupTemplates, string resourceUrl)
		{
			if (consolidate)
			{
				string unresolvedUrl = resourceUrl;
				var resolvedUrl = _resolvedResourceUrls.GetOrAdd(resourceUrl,
				                                                 () => CalculateResourceUrl(groupTemplates, unresolvedUrl));

				if (resolvedUrl != unresolvedUrl || IsConsolidatedUrl(groupTemplates, resolvedUrl))
				{
					if (!PreConsolidated)
						resolvedUrl = UrlType.Dynamic.ConvertUrl(resolvedUrl);
					else
						resolvedUrl = UrlType.Static.ConvertUrl(resolvedUrl);
				}

				resourceUrl = resolvedUrl;
			}
			if (!String.IsNullOrEmpty(Version) && !resourceUrl.Contains("?"))
				resourceUrl += "?v=" + HttpUtility.UrlEncode(Version);
			return resourceUrl;
		}

		private bool IsConsolidatedUrl(IEnumerable<IResourceGroupTemplate> groupTemplates, string path)
		{
			return groupTemplates.Any(t => t.MatchesConsolidatedUrl(path));
		}

		private string CalculateResourceUrl(IEnumerable<IResourceGroupTemplate> groupTemplates, string resourceUrl)
		{
			foreach (var groupTemplate in groupTemplates)
			{
				string consolidatedUrl;
				if (groupTemplate.TryGetConsolidatedUrl(resourceUrl, out consolidatedUrl))
				{
					return consolidatedUrl;
				}
			}

			return resourceUrl;
		}

		private IEnumerable<PreConsolidatedResourceGroup> ConsolidateAllInternal(ResourceGroupTemplateCollection groupTemplates,
		                                    Action<ConsolidatedResource, IResourceGroup> handleConsolidatedResource,
		                                    ResourceMode mode)
		{
			if (!groupTemplates.Any())
				return Enumerable.Empty<PreConsolidatedResourceGroup>();

			var allResources = _finder.FindResources(groupTemplates.First().ResourceType);

			var preConsolidatedGroups = new List<PreConsolidatedResourceGroup>();
			groupTemplates.EachGroup(allResources, mode, group =>
			{
				var consolidatedResource = ConsolidateGroup(group, mode);
				handleConsolidatedResource(consolidatedResource, group);
				var preConsolidatedGroup = new PreConsolidatedResourceGroup
				{
					ConsolidatedUrl = group.ConsolidatedUrl,
					Resources = consolidatedResource.Resources.Select(resource => resource.VirtualPath).ToList()
				};
				preConsolidatedGroups.Add(preConsolidatedGroup);	
			});

			return preConsolidatedGroups;
		}

		private IEnumerable<PreConsolidatedResourceDependencies> GetDependenciesForConsolidatedUrls(IEnumerable<IResourceGroupTemplate> groupTemplates, IEnumerable<IResource> allResources, ResourceMode mode)
		{
			return from @group in groupTemplates.SelectMany(g => g.GetGroups(allResources, mode))
			       let dependencies = _dependencyManager.GetDependencies(@group.ConsolidatedUrl)
			       where dependencies.Any()
			       select new PreConsolidatedResourceDependencies
			       {
			       	ResourcePath = @group.ConsolidatedUrl,
			       	Dependencies = dependencies.ToList()
			       };
		}

		private ResourceGroupTemplateCollection GroupTemplatesOfType(ResourceType resourceType)
		{
			if (resourceType == ResourceType.ClientScript)
				return _scriptGroups;
			else
				return _styleGroups;
		}
	}
}