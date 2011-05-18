using System;
using System.Linq;

using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestResourceCompiler
	{
		private ResourceTestContext _context;
		private ResourceCompiler _compiler;
		
		[SetUp]
		public void SetupContext()
		{
			_context = new ResourceTestContext();
			_compiler = _context.GetConsolidator();
		}
		
		[Test]
		public void WhenPreConsolidatedReportIsGenerated_ConsolidatedUrlsDependenciesAreIncludedInReport()
		{
			var coreGroup = _context.CreateGroup("~/scripts/consolidated/core.js");
			var sharedGroup = _context.CreateGroup("~/scripts/consolidated/shared.js");
			var homeGroup = _context.CreateGroup("~/scripts/consolidated/home.js");

			var jquery = _context.CreateResource("~/scripts/jquery.js")
				.InGroup(coreGroup).Resource;
			var site = _context.CreateResource("~/scripts/site.js")
				.WithDependencies(jquery)
				.InGroup(coreGroup).Resource;
			var mycomponent = _context.CreateResource("~/scripts/mycomponent.js")
				.InGroup(sharedGroup)
				.WithDependencies(site).Resource;
			var myothercomponent = _context.CreateResource("~/scripts/myothercomponent.js")
				.InGroup(sharedGroup)
				.WithDependencies(site).Resource;
			var homeIndex = _context.CreateResource("~/Views/Home/Index.js")
				.InGroup(homeGroup)
				.WithDependencies(jquery, mycomponent).Resource;
			var homePartial = _context.CreateResource("~/Views/Home/_MyPartial.js")
				.InGroup(homeGroup)
				.WithDependencies(jquery, site).Resource;
			var accountIndex = _context.CreateResource("~/Views/Account/Index.js")
				.WithDependencies(myothercomponent).Resource;

			var preConsolidatedReport = _compiler.CompileAll(resource => { }, ResourceMode.Release);
			var homeGroupDepends = preConsolidatedReport.Dependencies.ShouldContain(d => d.ResourcePath == homeGroup.ConsolidatedUrl);
			
			homeGroupDepends.Dependencies.CountShouldEqual(3);
			homeGroupDepends.Dependencies[0].ShouldEqual(jquery.VirtualPath);
			homeGroupDepends.Dependencies[1].ShouldEqual(site.VirtualPath);
			homeGroupDepends.Dependencies[2].ShouldEqual(mycomponent.VirtualPath);
		}

		[Test]
		public void WhenPreConsolidatedReportIsGenerated_UnconsolidatedResourcesAreIncluded()
		{
			_context.CreateGroup("~/group1.js", "~/file1.js", "~/file2.js");
			_context.CreateGroup("~/group2.js", "~/file3.js", "~/file4.js");

			_context.CreateResource("~/file5.js");
			_context.CreateResource("~/file6.js");

			var report = _compiler.CompileAll(r => {}, ResourceMode.Release);

			report.Scripts.SingleResources.CountShouldEqual(2);
			report.Scripts.SingleResources[0].OriginalPath.ShouldEqual("~/file5.js");
			report.Scripts.SingleResources[0].CompiledPath.ShouldEqual("~/file5.min.js");
			report.Scripts.SingleResources[1].OriginalPath.ShouldEqual("~/file6.js");
			report.Scripts.SingleResources[1].CompiledPath.ShouldEqual("~/file6.min.js");
		}

	    [Test]
	    public void WhenPreConsolidetedReportIsGenerated_CompiledFilesAreNot()
	    {
	        
	    }

		[Test]
		public void ConsolidateGroupExcludesResourcesMatchingGivenExcludeFilter()
		{
			var group = _context.CreateGroup("~/consolidated.js");
			
			_context.CreateResource("~/file1.js")
				.InGroup(group);
			_context.CreateResource("~/file2.js")
				.InGroup(group);
			_context.CreateResource("~/file3.js")
				.InGroup(group);

			var groupTemplate = new StubResourceGroupTemplate(group);
			groupTemplate.ResourceType = ResourceType.Script;

			var excludeFilter = ToFilter(r => r.VirtualPath.Contains("file2"));
			var consolidatedResource = _compiler.CompileGroup(group.ConsolidatedUrl, groupTemplate.WithContext(excludeFilter), ResourceMode.Debug);

			consolidatedResource.ShouldNotBeNull();
			consolidatedResource.Resources.Count().ShouldEqual(2);
		}

		[Test]
		public void ConsolidateGroupSortsResourcesByDependencies()
		{
			var dependencyLeaf = _context.CreateResource("~/dependency-leaf.js").Resource;
			var dependencyRoot = _context.CreateResource("~/dependency-root.js").Resource;
			var dependencyMiddle = _context.CreateResource("~/dependency-middle.js").Resource;

			var group = new ResourceGroup("~/consolidated.js", new IResource[]
			{
				dependencyLeaf,
				dependencyRoot,
				dependencyMiddle
			});

			var groupTemplate = new StubResourceGroupTemplate(group);
			groupTemplate.ResourceType = ResourceType.Script;

			_context.DependencyProvider.SetDependencies(dependencyLeaf, "~/dependency-middle.js");
			_context.DependencyProvider.SetDependencies(dependencyMiddle, "~/dependency-root.js");

			var consolidatedResource = _compiler.CompileGroup(group, ResourceMode.Debug);
			var resources = consolidatedResource.Resources.ToList();
			resources[0].VirtualPath.ShouldEqual("~/dependency-root.js");
			resources[1].VirtualPath.ShouldEqual("~/dependency-middle.js");
			resources[2].VirtualPath.ShouldEqual("~/dependency-leaf.js");
		}

		[Test]
		public void CompileUnconsolidatedResourcesOnlyCompilesUnconsolidatedResources()
		{
			var group = _context.CreateGroup("~/consolidated.js");

			_context.CreateResource("~/file1.js")
				.InGroup(group);
			_context.CreateResource("~/file2.js")
				.InGroup(group);
			_context.CreateResource("~/file3.js")
				.InGroup(group);

			var unconsolidatedResource1 = _context.CreateResource("~/unconsolidated1.js").Resource;
			var unconsolidatedResource2 = _context.CreateResource("~/unconsolidated2.js").Resource;

			var unconsolidatedResourceCompilations = _compiler.CompileUnconsolidatedResources(ResourceType.Script, ResourceMode.Debug, r => {}).ToList();

			unconsolidatedResourceCompilations.CountShouldEqual(2);
			unconsolidatedResourceCompilations[0].Resources.Single().VirtualPath.ShouldEqual(unconsolidatedResource1.VirtualPath);
			unconsolidatedResourceCompilations[1].Resources.Single().VirtualPath.ShouldEqual(unconsolidatedResource2.VirtualPath);
		}

		private IResourceFilter ToFilter(Predicate<IResource> predicate)
		{
			return ResourceFilters.Predicate(predicate);
		}
	}
}