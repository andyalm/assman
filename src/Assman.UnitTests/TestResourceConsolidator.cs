using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestResourceConsolidator
	{
		private ResourceTestContext _context;
		private ResourceConsolidator _consolidator;

		[SetUp]
		public void SetupContext()
		{
			_context = new ResourceTestContext();
			_consolidator = _context.GetConsolidator();
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

			var preConsolidatedReport = _consolidator.ConsolidateAll((resource, @group) => { }, ResourceMode.Release);
			var homeGroupDepends = preConsolidatedReport.Dependencies.ShouldContain(d => d.ResourcePath == homeGroup.ConsolidatedUrl);
			
			homeGroupDepends.Dependencies.CountShouldEqual(3);
			homeGroupDepends.Dependencies[0].ShouldEqual(jquery.VirtualPath);
			homeGroupDepends.Dependencies[1].ShouldEqual(site.VirtualPath);
			homeGroupDepends.Dependencies[2].ShouldEqual(mycomponent.VirtualPath);
		}
	}
}