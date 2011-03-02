using AlmWitt.Web.ResourceManagement.Configuration;

using NUnit.Framework;

using AlmWitt.Web.ResourceManagement.TestSupport;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceManagementContext_PreConsolidation
	{
		private ResourceManagementContext _context;

		[SetUp]
		public void SetupContext()
		{
			_context = new ResourceManagementContext();
		}

		[Test]
		public void DefaultStateOfPreConsolidationFlagIsFalse()
		{
			_context.PreConsolidated.ShouldBeFalse();
		}

		[Test]
		public void WhenPreConsolidationReportIsLoaded_PreConsolidatedFlagIsSetToTrue()
		{
			var preConsolidationInfo = new PreConsolidationReport();
			_context.LoadPreCompilationReport(preConsolidationInfo);

			_context.PreConsolidated.ShouldBeTrue();
		}
	}
}