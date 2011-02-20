using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using AlmWitt.Web.ResourceManagement.ContentFiltering;

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
		private readonly ResourceGroupTemplateCollection _clientScriptGroups;
		private readonly ResourceGroupTemplateCollection _cssFileGroups;
		private readonly List<Assembly> _assemblies;
		private readonly DependencyManager _dependencyManager;

		internal ResourceManagementContext()
		{
			_finder = new CompositeResourceFinder();
			_filterMap = new ContentFilterMap();
			_clientScriptGroups = new ResourceGroupTemplateCollection();
			_cssFileGroups = new ResourceGroupTemplateCollection();
			_assemblies = new List<Assembly>();
			_dependencyManager = new DependencyManager(_finder);
			ConsolidateClientScripts = true;
			ConsolidateCssFiles = true;
		}

		public DateTime ConfigurationLastModified { get; set; }
		public bool PreConsolidated { get; set; }
		public bool ConsolidateClientScripts { get; set; }
		public bool ConsolidateCssFiles { get; set; }
		public string Version { get; set; }

		public void MapExtensionToFilter(string fileExtension, IContentFilterFactory filterFactory)
		{
			_filterMap.MapExtension(fileExtension, filterFactory);
		}

		public ResourceGroupTemplateCollection ClientScriptGroups
		{
			get { return _clientScriptGroups; }
		}

		public ResourceGroupTemplateCollection CssFileGroups
		{
			get { return _cssFileGroups; }
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

		public ConsolidatedResource ConsolidateGroup(string groupConsolidatedUrl, GroupTemplateContext groupTemplateContext)
		{
			var group = GetGroupsFromTemplate(groupTemplateContext)
				.Where(g => UrlType.ArePathsEqual(g.ConsolidatedUrl, groupConsolidatedUrl))
				.SingleOrDefault();
			
			if(group == null)
			{
				throw new Exception("No group with consolidatedUrl '" + groupConsolidatedUrl + "' could be found.");
			}

			return ConsolidateGroup(group, groupTemplateContext);
		}

		public ConsolidatedResource ConsolidateGroup(IResourceGroup group, GroupTemplateContext groupTemplateContext)
		{
			Func<IResource, IContentFilter> createContentFilter = resource =>
			{
				var contentFilterFactory = GetFilterFactoryForExtension(resource.FileExtension);
				return contentFilterFactory.CreateFilter(group);
			};

			return group.GetResources()
				.ToResourceCollection()
				.WhereNot(groupTemplateContext.ExcludeFilter)
				.Consolidate(createContentFilter, groupTemplateContext.GroupTemplate.ResourceType.Separator);
		}

		public void ConsolidateAll(Action<ConsolidatedResource, IResourceGroup> handleConsolidatedResource)
		{
			ConsolidateAllInternal(ClientScriptGroups, handleConsolidatedResource);
			ConsolidateAllInternal(CssFileGroups, handleConsolidatedResource);
		}

		public GroupTemplateContext FindGroupTemplate(string consolidatedUrl)
		{
			var resourceType = ResourceType.FromPath(consolidatedUrl);

			return GroupTemplatesOfType(resourceType).FindGroupTemplate(consolidatedUrl);
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
			return GetResourceUrl(ConsolidateClientScripts, ClientScriptGroups, scriptUrl);
		}

		public string GetStylesheetUrl(string stylesheetUrl)
		{
			return GetResourceUrl(ConsolidateCssFiles, CssFileGroups, stylesheetUrl);
		}

		private string GetResourceUrl(bool consolidate, IEnumerable<IResourceGroupTemplate> groupTemplates, string resourceUrl)
		{
			if (consolidate)
			{
				foreach (var groupElement in groupTemplates)
				{
					string consolidatedUrl;
					if (groupElement.TryGetConsolidatedUrl(resourceUrl, out consolidatedUrl))
					{
						if (!PreConsolidated)
							resourceUrl = UrlType.Dynamic.ConvertUrl(consolidatedUrl);
						else
							resourceUrl = UrlType.Static.ConvertUrl(consolidatedUrl);
						break;
					}
				}
			}
			if (!String.IsNullOrEmpty(Version) && !resourceUrl.Contains("?"))
				resourceUrl += "?v=" + HttpUtility.UrlEncode(Version);
			return resourceUrl;
		}

		private void ConsolidateAllInternal(ResourceGroupTemplateCollection groupTemplates, Action<ConsolidatedResource, IResourceGroup> handleConsolidatedResource)
		{
			if(!groupTemplates.Any())
				return;

			var allResources = _finder.FindResources(groupTemplates.First().ResourceType);

			groupTemplates.ForEach(templateContext =>
			{
				var groups = templateContext.GroupTemplate.GetGroups(allResources);
				foreach (var group in groups)
				{
					var consolidatedResource = ConsolidateGroup(group, templateContext);
					handleConsolidatedResource(consolidatedResource, group);
				}

				return true;
			});
		}

		private IEnumerable<IResourceGroup> GetGroupsFromTemplate(GroupTemplateContext groupTemplateContext)
		{
			var resources = _finder
				.FindResources(groupTemplateContext.GroupTemplate.ResourceType)
				.WhereNot(groupTemplateContext.ExcludeFilter);

			return groupTemplateContext.GroupTemplate.GetGroups(resources);
		}

		private ResourceGroupTemplateCollection GroupTemplatesOfType(ResourceType resourceType)
		{
			if (resourceType == ResourceType.ClientScript)
				return _clientScriptGroups;
			else
				return _cssFileGroups;
		}

		internal IResourceFinder Finder
		{
			get { return _finder; }
		}
	}

	public class ContentFilterMap
	{
		private readonly IDictionary<string,IContentFilterFactory> _map = new Dictionary<string, IContentFilterFactory>();

		public void MapExtension(string fileExtension, IContentFilterFactory filterFactory)
		{
			ValidateFileExtensionArgument(fileExtension);

			_map[fileExtension] = filterFactory;
		}

		public IContentFilterFactory GetFilterFactoryForExtension(string fileExtension)
		{
			ValidateFileExtensionArgument(fileExtension);
			
			if (_map.ContainsKey(fileExtension))
				return _map[fileExtension];
			else
				return NullContentFilterFactory.Instance;
		}

		private void ValidateFileExtensionArgument(string fileExtension)
		{
			if(!fileExtension.StartsWith("."))
			{
				throw new ArgumentException("The fileExtension argument must begine with a dot (e.g. .js, .css)", "fileExtension");
			}
		}
	}
}