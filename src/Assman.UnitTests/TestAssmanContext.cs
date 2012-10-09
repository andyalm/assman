using System;
using System.Linq;

using Assman.Configuration;
using Assman.DependencyManagement;
using Assman.PreCompilation;
using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestAssmanContext
	{
		private StubResourceFinder _finder;
		private AssmanContext _context;

		[SetUp]
		public void SetupContext()
		{
			DependencyManagerFactory.ClearDependencyCache();
			_finder = new StubResourceFinder();
			_context = new AssmanContext(ResourceMode.Release);
			_context.AddFinder(_finder);
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
			var group1Template = new StubResourceGroupTemplate(group1) { ResourceType = ResourceType.Script };
			var group2Template = new StubResourceGroupTemplate(group2) { ResourceType = ResourceType.Script };
			_context.ScriptGroups.Add(group1Template);
			_context.ScriptGroups.Add(group2Template);
			var preConsolidationReport = CompileAll();

			var group2Consolidated = preConsolidationReport.Scripts.Groups.Single(g => g.ConsolidatedUrl == "~/scripts/everything-else.js");

			group2Consolidated.Resources.CountShouldEqual(1);
			group2Consolidated.Resources[0].ShouldEqual("~/scripts/otherscript.js");
		}

		[Test]
		public void ResourceCanOnlyBeInOneGroupByDefault()
		{
			var jquery = StubResource.WithPath("~/scripts/jquery.js");
			var componentA = StubResource.WithPath("~/scripts/componentA.js");
			var componentB = StubResource.WithPath("~/scripts/componentB.js");
			var finder = new StubResourceFinder();
			finder.AddResources(jquery, componentA, componentB);
			_context.AddFinder(finder);

			var pageAGroup = new ResourceGroup("~/scripts/compiled/pageA.js", new[]
			{
				jquery,
				componentA
			});
			_context.ScriptGroups.Add(new StubResourceGroupTemplate(pageAGroup) { ResourceType = ResourceType.Script });
			var pageBGroup = new ResourceGroup("~/scripts/compiled/pageB.js", new[]
			{
				jquery,
				componentB
			});
			_context.ScriptGroups.Add(new StubResourceGroupTemplate(pageBGroup) { ResourceType = ResourceType.Script });
			var report = CompileAll();

			var pageAGroupReport = report.Scripts.Groups.Single(g => g.ConsolidatedUrl == "~/scripts/compiled/pageA.js");
			var pageBGroupReport = report.Scripts.Groups.Single(g => g.ConsolidatedUrl == "~/scripts/compiled/pageB.js");

			pageAGroupReport.Resources.CountShouldEqual(2);
			pageBGroupReport.Resources.CountShouldEqual(1);
		}

		[Test]
		public void ResourceCanBeInMultipleGroupsIfExplicitlyPrevented()
		{
			var jquery = StubResource.WithPath("~/scripts/jquery.js");
			var componentA = StubResource.WithPath("~/scripts/componentA.js");
			var componentB = StubResource.WithPath("~/scripts/componentB.js");
			var finder = new StubResourceFinder();
			finder.AddResources(jquery, componentA, componentB);
			_context.AddFinder(finder);
			_context.MutuallyExclusiveGroups = false;

			var pageAGroup = new ResourceGroup("~/scripts/compiled/pageA.js", new[]
			{
				jquery,
				componentA
			});
			_context.ScriptGroups.Add(new StubResourceGroupTemplate(pageAGroup) { ResourceType = ResourceType.Script });
			var pageBGroup = new ResourceGroup("~/scripts/compiled/pageB.js", new[]
			{
				jquery,
				componentB
			});
			_context.ScriptGroups.Add(new StubResourceGroupTemplate(pageBGroup) { ResourceType = ResourceType.Script });
			var report = CompileAll();

			var pageAGroupReport = report.Scripts.Groups.Single(g => g.ConsolidatedUrl == "~/scripts/compiled/pageA.js");
			var pageBGroupReport = report.Scripts.Groups.Single(g => g.ConsolidatedUrl == "~/scripts/compiled/pageB.js");

			pageAGroupReport.Resources.CountShouldEqual(2);
			pageBGroupReport.Resources.CountShouldEqual(2);
		}

		private PreCompilationReport CompileAll()
		{
			var compiler = _context.GetCompiler();
			var preConsolidationReport = compiler.CompileAll((resource) => { });
			return preConsolidationReport;
		}
	}
}