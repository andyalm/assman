using System;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceManagementContext
	{
	    private StubResourceFinder _finder;
	    private ResourceManagementContext _context;

	    [SetUp]
	    public void SetupContext()
	    {
	        DependencyManagerFactory.ClearDependencyCache();
            _finder = new StubResourceFinder();
            _context = new ResourceManagementContext();
            _context.AddFinder(_finder);
	    }
        
        [Test]
		public void ConsolidateGroupExcludesResourcesMatchingGivenExcludeFilter()
		{
			var stubFinder = new StubResourceFinder();
			stubFinder.AddResource("~/file1.js", "");
			stubFinder.AddResource("~/file2.js", "");
			stubFinder.AddResource("~/file3.js", "");
			
			var group = new ResourceGroup("~/consolidated.js", stubFinder.Resources);
			var groupTemplate = new StubResourceGroupTemplate(group);
			groupTemplate.ResourceType = ResourceType.ClientScript;

            _context.AddFinder(stubFinder);
			var excludeFilter = ToFilter(r => r.VirtualPath.Contains("file2"));
			var consolidator = _context.GetConsolidator();
			var consolidatedResource = consolidator.ConsolidateGroup(group.ConsolidatedUrl, groupTemplate.WithContext(excludeFilter), ResourceMode.Debug);
			
			consolidatedResource.ShouldNotBeNull();
			consolidatedResource.Resources.Count().ShouldEqual(2);
		}

		[Test]
		public void ConsolidateGroupSortsResourcesByDependencies()
		{
		    var dependencyLeaf = _finder.CreateResource("~/dependency-leaf.js");
            var dependencyRoot = _finder.CreateResource("~/dependency-root.js");
            var dependencyMiddle = _finder.CreateResource("~/dependency-middle.js");
            
            var group = new ResourceGroup("~/consolidated.js", new IResource[]
			{
				dependencyLeaf,
				dependencyRoot,
				dependencyMiddle
			});
            
			var groupTemplate = new StubResourceGroupTemplate(group);
			groupTemplate.ResourceType = ResourceType.ClientScript;

			var dependencyProvider = new StubDependencyProvider();
			dependencyProvider.SetDependencies(dependencyLeaf, "~/dependency-middle.js");
			dependencyProvider.SetDependencies(dependencyMiddle, "~/dependency-root.js");
			
			_context.MapExtensionToDependencyProvider(".js", dependencyProvider);

			var consolidator = _context.GetConsolidator();
			var consolidatedResource = consolidator.ConsolidateGroup(group, ResourceMode.Debug);
			var resources = consolidatedResource.Resources.ToList();
			resources[0].VirtualPath.ShouldEqual("~/dependency-root.js");
			resources[1].VirtualPath.ShouldEqual("~/dependency-middle.js");
			resources[2].VirtualPath.ShouldEqual("~/dependency-leaf.js");
		}

		[Test]
		public void WhenConsolidatedUrlMatchesPatternOfAGroup_ItIsNotIncludedInTheConsolidatedResource()
		{
			var jquery = StubResource.WithPath("~/scripts/jquery.js");
			var component = StubResource.WithPath("~/scripts/component.js");
			var core = StubResource.WithPath("~/scripts/core.js");
			var otherscript = StubResource.WithPath("~/scripts/otherscript.js");
			
			var finder = new StubResourceFinder();
			finder.AddResources(jquery, component, core, otherscript);
			
			var group1 = new ResourceGroup("~/scripts/core.js", new IResource[]
			{
				jquery,
				component
			});
			var group2 = new ResourceGroup("~/scripts/everything-else.js", new IResource[]
			{
				core,
				otherscript
			});

		    _context.AddFinder(finder);
			var group1Template = new StubResourceGroupTemplate(group1) { ResourceType = ResourceType.ClientScript };
			var group2Template = new StubResourceGroupTemplate(group2) { ResourceType = ResourceType.ClientScript };
			_context.ScriptGroups.Add(group1Template);
			_context.ScriptGroups.Add(group2Template);
			var consolidator = _context.GetConsolidator();
			var preConsolidationReport = consolidator.ConsolidateAll((resource, @group) => { }, ResourceMode.Release);

			var group2Consolidated = preConsolidationReport.ScriptGroups.Where(g => g.ConsolidatedUrl == "~/scripts/everything-else.js").Single();

			group2Consolidated.Resources.CountShouldEqual(1);
			group2Consolidated.Resources[0].ShouldEqual("~/scripts/otherscript.js");
		}

		private IResourceFilter ToFilter(Predicate<IResource> predicate)
		{
			return ResourceFilters.Predicate(predicate);
		}
	}
}