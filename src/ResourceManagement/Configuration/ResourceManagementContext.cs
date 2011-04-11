using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

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
		private IResourceGroupManager _scriptGroups;
		private IResourceGroupManager _styleGroups;
		private readonly List<Assembly> _assemblies;
		private readonly DependencyManager _dependencyManager;
	    private readonly IResourceModeProvider _resourceModeProvider;

		internal ResourceManagementContext()
		{
			_finder = new CompositeResourceFinder();
			_filterMap = new ContentFilterMap();
			_scriptGroups = ResourceGroupManager.GetInstance();
			_styleGroups = ResourceGroupManager.GetInstance();
			_assemblies = new List<Assembly>();
			_dependencyManager = DependencyManagerFactory.GetDependencyManager(_finder, _scriptGroups, _styleGroups);
		    _resourceModeProvider = ConfigDrivenResourceModeProvider.GetInstance();
		}

		public DateTime ConfigurationLastModified { get; set; }
		public bool PreConsolidated { get; private set; }
	    public bool ConsolidateClientScripts
	    {
            get { return _scriptGroups.Consolidate; }
            set { _scriptGroups.Consolidate = value; }
	    }

	    public bool ConsolidateCssFiles
	    {
            get { return _styleGroups.Consolidate; }
            set { _styleGroups.Consolidate = value; }
	    }

		public bool ManageDependencies { get; set; }

		public string Version { get; set; }

		public void MapExtensionToFilter(string fileExtension, IContentFilterFactory filterFactory)
		{
			_filterMap.MapExtension(fileExtension, filterFactory);
		}

		public IResourceGroupManager ScriptGroups
		{
			get { return _scriptGroups; }
		}

		public IResourceGroupManager StyleGroups
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

		public GroupTemplateContext FindGroupTemplate(string consolidatedUrl)
		{
			var resourceType = ResourceType.FromPath(consolidatedUrl);

			return GroupManagerOfType(resourceType).GetGroupTemplateOrDefault(consolidatedUrl);
		}

		public ResourceConsolidator GetConsolidator()
		{
			return new ResourceConsolidator(_filterMap, _dependencyManager, _scriptGroups, _styleGroups, _finder);
		}

		public IEnumerable<string> GetResourceDependencies(string virtualPath)
		{
			return _dependencyManager.GetDependencies(virtualPath);
		}

		public void MapExtensionToDependencyProvider(string fileExtension, IDependencyProvider dependencyProvider)
		{
			_dependencyManager.MapProvider(fileExtension, dependencyProvider);
		}

		public IEnumerable<string> GetScriptUrls(string scriptUrl)
		{
			return GetResourceUrls(ScriptGroups, scriptUrl);
		}

		public IEnumerable<string> GetStylesheetUrls(string stylesheetUrl)
		{
			return GetResourceUrls(StyleGroups, stylesheetUrl);
		}

		public void LoadPreCompilationReport(PreConsolidationReport preConsolidationReport)
		{
			_scriptGroups = new PreConsolidatedGroupManager(preConsolidationReport.ScriptGroups);
			_styleGroups = new PreConsolidatedGroupManager(preConsolidationReport.StyleGroups);
			_dependencyManager.SetCache(new PreConsolidatedDependencyCache(preConsolidationReport.Dependencies));
			Version = preConsolidationReport.Version;
			PreConsolidated = true;
		}

		internal IResourceFinder Finder
		{
			get { return _finder; }
		}

		private IEnumerable<string> GetResourceUrls(IResourceGroupManager groupManager, string resourceUrl)
		{
		    IEnumerable<string> resolvedResourceUrls;
            if(!groupManager.IsGroupUrlWithConsolidationDisabled(resourceUrl))
		    {
		        resolvedResourceUrls = new [] {groupManager.ResolveResourceUrl(resourceUrl)};
		    }
            else
            {
                var resourceMode = _resourceModeProvider.GetCurrentResourceMode();
                resolvedResourceUrls = groupManager.GetResourceUrlsInGroup(resourceUrl, resourceMode, _finder)
                    .SortByDependencies(_dependencyManager);
            }
            
            if (!String.IsNullOrEmpty(Version))
                return resolvedResourceUrls.Select(u => u.AddQueryParam("v", Version));
            else
                return resolvedResourceUrls;
		}

	    private IResourceGroupManager GroupManagerOfType(ResourceType resourceType)
		{
			if (resourceType == ResourceType.ClientScript)
				return _scriptGroups;
			else
				return _styleGroups;
		}
	}
}