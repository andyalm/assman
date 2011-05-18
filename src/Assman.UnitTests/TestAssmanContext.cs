using System;
using System.Linq;

using Assman.Configuration;
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
			_context = new AssmanContext();
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
			var consolidator = _context.GetConsolidator();
			var preConsolidationReport = consolidator.CompileAll((resource) => { }, ResourceMode.Release);

			var group2Consolidated = preConsolidationReport.Scripts.Groups.Where(g => g.ConsolidatedUrl == "~/scripts/everything-else.js").Single();

			group2Consolidated.Resources.CountShouldEqual(1);
			group2Consolidated.Resources[0].ShouldEqual("~/scripts/otherscript.js");
		}
	}
}