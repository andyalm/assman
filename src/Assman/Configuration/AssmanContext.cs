using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Assman.ContentFiltering;
using Assman.DependencyManagement;
using Assman.PreCompilation;

namespace Assman.Configuration
{
    public class AssmanContext
    {
        public static AssmanContext Create(ResourceMode resourceMode)
        {
            return new AssmanContext(resourceMode);
        }

        private static AssmanContext _current;

        public static AssmanContext Current
        {
            get
            {
                if(_current == null)
                {
                    var resourceMode = ResourceModeProvider.Instance.GetCurrentResourceMode();
                    _current = AssmanConfiguration.Current.BuildContext(resourceMode);
                }

                return _current;
            }
            set { _current = value; }
        }

        private readonly IResourceFinder _finder;
        private readonly CompositeResourceFinder _compositeFinder;
        private readonly ContentFilterPipelineMap _filterPipelineMap;
        private IResourceGroupManager _scriptGroups;
        private IResourceGroupManager _styleGroups;
        private readonly List<Assembly> _assemblies;
        private readonly DependencyManager _dependencyManager;
        private readonly ResourceMode _resourceMode;

        internal AssmanContext(ResourceMode resourceMode)
        {
            var resourceCache = ResourceCacheFactory.GetCache(resourceMode);
            
            _scriptGroups = ResourceGroupManager.GetInstance(resourceMode, resourceCache);
            _styleGroups = ResourceGroupManager.GetInstance(resourceMode, resourceCache);
            _compositeFinder = new CompositeResourceFinder();
            _compositeFinder.Exclude(new ConsolidatedResourceExcluder(_scriptGroups));
            _compositeFinder.Exclude(new ConsolidatedResourceExcluder(_styleGroups));
            _compositeFinder.Exclude(new PreCompiledResourceExcluder());
            _compositeFinder.Exclude(new VsDocResourceExcluder());
            _finder = new ResourceModeFilteringFinder(resourceMode, new CachingResourceFinder(resourceCache, _compositeFinder));
            _filterPipelineMap = new ContentFilterPipelineMap();
            _assemblies = new List<Assembly>();
            _dependencyManager = DependencyManagerFactory.GetDependencyManager(_finder, _scriptGroups, _styleGroups, resourceMode);
            _resourceMode = resourceMode;
        }

        public DateTime ConfigurationLastModified { get; set; }
        public bool PreCompiled { get; private set; }
        public bool GZip { get; set; }
        public bool ConsolidateScripts
        {
            get { return _scriptGroups.Consolidate; }
            set { _scriptGroups.Consolidate = value; }
        }

        public bool ConsolidateStylesheets
        {
            get { return _styleGroups.Consolidate; }
            set { _styleGroups.Consolidate = value; }
        }

        public bool ManageDependencies { get; set; }

        public string Version { get; set; }

        public void MapExtensionToContentPipeline(string fileExtension, ContentFilterPipeline filterPipeline)
        {
            _filterPipelineMap.MapExtension(fileExtension, filterPipeline);
        }

        public IResourceGroupManager ScriptGroups
        {
            get { return _scriptGroups; }
        }

        public IResourceGroupManager StyleGroups
        {
            get { return _styleGroups; }
        }

        public ContentFilterPipeline GetContentPipelineForExtension(string fileExtension)
        {
            return _filterPipelineMap.GetPipelineForExtension(fileExtension);
        }

        public void AddFinder(IResourceFinder finder)
        {
            _compositeFinder.AddFinder(finder);
        }

        public void AddFinders(IEnumerable<IResourceFinder> finders)
        {
            _compositeFinder.AddFinders(finders);
        }

        public void AddExcluder(IFinderExcluder excluder)
        {
            _compositeFinder.Exclude(excluder);
        }

        public void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            _compositeFinder.AddFinder(ResourceFinderFactory.GetInstance(assembly));
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

        [Obsolete("Please use GetCompiler instead")]
        public ResourceCompiler GetConsolidator()
        {
            return GetCompiler();
        }

        public ResourceCompiler GetCompiler()
        {
            return new ResourceCompiler(_filterPipelineMap, _dependencyManager, _scriptGroups, _styleGroups, _finder, _resourceMode);
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

        public void LoadPreCompilationReport(PreCompilationReport preCompilationReport)
        {
            _scriptGroups = new PreCompiledGroupManager(preCompilationReport.Scripts, _scriptGroups);
            _styleGroups = new PreCompiledGroupManager(preCompilationReport.Stylesheets, _styleGroups);
            _dependencyManager.SetCache(new PreCompiledDependencyCache(preCompilationReport.Dependencies));
            Version = preCompilationReport.Version;
            PreCompiled = true;
        }

        internal IResourceFinder Finder
        {
            get { return _finder; }
        }

        private IEnumerable<string> GetResourceUrls(IResourceGroupManager groupManager, string resourceUrl)
        {
            IEnumerable<string> resolvedResourceUrls;
            if (groupManager.IsGroupUrlWithConsolidationDisabled(resourceUrl))
            {
                resolvedResourceUrls = groupManager.GetResourceUrlsInGroup(resourceUrl, _finder)
                    .SortByDependencies(_dependencyManager);
            }
            else
            {
                resolvedResourceUrls = new[] {groupManager.ResolveResourceUrl(resourceUrl)};
            }

            if (!String.IsNullOrEmpty(Version))
                return resolvedResourceUrls.Select(u => u.AddQueryParam("v", Version));
            else
                return resolvedResourceUrls;
        }

        private IResourceGroupManager GroupManagerOfType(ResourceType resourceType)
        {
            if (resourceType == ResourceType.Script)
                return _scriptGroups;
            else
                return _styleGroups;
        }
    }
}