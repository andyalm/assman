using System.Collections.Generic;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.PreConsolidation;

using Moq;

using NUnit.Framework;

using AlmWitt.Web.ResourceManagement.TestSupport;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceManagementContext_PreConsolidation
	{
		private ResourceManagementContext _context;
		private Mock<IDependencyProvider> _dependencyProvider;

		[SetUp]
		public void SetupContext()
		{
			_dependencyProvider = new Mock<IDependencyProvider>();
			
			_context = new ResourceManagementContext();
			_context.MapExtensionToDependencyProvider(".js", _dependencyProvider.Object);
		}

		[Test]
		public void DefaultStateOfPreConsolidationFlagIsFalse()
		{
			_context.PreConsolidated.ShouldBeFalse();
		}

		[Test]
		public void WhenPreConsolidationReportIsLoaded_PreConsolidatedFlagIsSetToTrue()
		{
			var preConsolidationReport = new PreConsolidationReport();
			_context.LoadPreCompilationReport(preConsolidationReport);

			_context.PreConsolidated.ShouldBeTrue();
		}

		[Test]
		public void WhenPreConsolidationReportIsLoaded_VersionIsSet()
		{
			var preConsolidationReport = new PreConsolidationReport
			{
				Version = "311"
			};
			_context.LoadPreCompilationReport(preConsolidationReport);

			_context.Version.ShouldEqual("311");
		}

		[Test]
		public void WhenPreConsolidatedReportIsLoaded_DependenciesAreCachedSoThatDependencyProviderIsNotCalled()
		{
			var preConsolidationReport = new PreConsolidationReport();
			var scriptGroup = new PreConsolidatedResourceGroup
			{
				ConsolidatedUrl = "~/scripts/consolidated/common.js",
				Resources = new List<PreConsolidatedResourcePiece>
				{
					new PreConsolidatedResourcePiece
					{
						Path = "~/scripts/myscript.js",
						Dependencies = new List<string>
						{
							"~/scripts/jquery.js"
						}
					}
				}
			};
			preConsolidationReport.ClientScriptGroups.Add(scriptGroup);
			_context.LoadPreCompilationReport(preConsolidationReport);

			var dependencies = _context.GetResourceDependencies("~/scripts/myscript.js");
			dependencies.CountShouldEqual(1);
			dependencies.First().ShouldEqual("~/scripts/jquery.js");

			_dependencyProvider.Verify(p => p.GetDependencies(It.IsAny<IResource>()), Times.Never());
		}

		[Test]
		public void WhenPreConsolidatedReportIsLoaded_ResourceUrlCacheIsPrepopulated()
		{
			var groupTemplate = new Mock<IResourceGroupTemplate>();
			_context.ClientScriptGroups.Add(groupTemplate.Object);

			var preConsolidationReport = new PreConsolidationReport();
			var scriptGroup = new PreConsolidatedResourceGroup
			{
				ConsolidatedUrl = "~/scripts/consolidated/common.js",
				Resources = new List<PreConsolidatedResourcePiece>
				{
					new PreConsolidatedResourcePiece
					{
						Path = "~/scripts/myscript.js"
					}
				}
			};
			preConsolidationReport.ClientScriptGroups.Add(scriptGroup);

			_context.LoadPreCompilationReport(preConsolidationReport);

			var resolvedUrl = _context.GetScriptUrl("~/scripts/myscript.js");
			resolvedUrl.ShouldEqual("~/scripts/consolidated/common.js");
			
			//verify that the group template was not looked it (that proves the value came from the prepopulated cache)
			string consolidatedUrl;
			groupTemplate.Verify(t => t.TryGetConsolidatedUrl(It.IsAny<string>(), out consolidatedUrl), Times.Never());
		}
	}

	
}