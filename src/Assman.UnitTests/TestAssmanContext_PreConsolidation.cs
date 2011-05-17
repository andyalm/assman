using System.Collections.Generic;
using System.Linq;

using Assman.Configuration;
using Assman.PreConsolidation;

using Moq;

using NUnit.Framework;

using Assman.TestSupport;

namespace Assman
{
	[TestFixture]
	public class TestAssmanContext_PreConsolidation
	{
		private AssmanContext _context;
		private Mock<IDependencyProvider> _dependencyProvider;
		private Mock<IResourceFinder> _finder;

		[SetUp]
		public void SetupContext()
		{
			_dependencyProvider = new Mock<IDependencyProvider>();
			_finder = new Mock<IResourceFinder>();
			
			_context = new AssmanContext();
			_context.MapExtensionToDependencyProvider(".js", _dependencyProvider.Object);
			_context.AddFinder(_finder.Object);
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
			var preConsolidationReport = new PreConsolidationReport
			{
				Dependencies = new List<PreConsolidatedResourceDependencies>
				{
					new PreConsolidatedResourceDependencies
					{
						ResourcePath = "~/scripts/myscript.js",
						Dependencies = new List<string>
						{
							"~/scripts/jquery.js"
						}
					}
				}
			};
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
			_context.ScriptGroups.Add(groupTemplate.Object);

			var preConsolidationReport = new PreConsolidationReport();
			var scriptGroup = new PreConsolidatedResourceGroup
			{
				ConsolidatedUrl = "~/scripts/consolidated/common.js",
				Resources = new List<string>
				{
					 "~/scripts/myscript.js"
				}
			};
			preConsolidationReport.Scripts.Groups.Add(scriptGroup);

			_context.LoadPreCompilationReport(preConsolidationReport);

			var resolvedUrl = _context.GetScriptUrls("~/scripts/myscript.js").Single();
			resolvedUrl.ShouldEqual("~/scripts/consolidated/common.js");
			
			//verify that the group template was not looked at (that proves the value came from the prepopulated cache)
			string consolidatedUrl;
			groupTemplate.Verify(t => t.TryGetConsolidatedUrl(It.IsAny<string>(), out consolidatedUrl), Times.Never());
		}

		[Test]
		public void WhenPreConsolidatedReportIsLoaded_ResourceUrlCacheIsPrepopulatedWithIndividuallyCompiledResources()
		{
			var preConsolidationReport = new PreConsolidationReport
			{
				Scripts = new PreConsolidatedResourceReport
				{
					SingleResources = new List<PreCompiledSingleResource>
					{
						new PreCompiledSingleResource
						{
							OriginalPath = "~/myfile.js",
							CompiledPath = "~/myfile.min.js"
						}
					}
				}
			};
			_context.LoadPreCompilationReport(preConsolidationReport);

			var resolvedUrl = _context.GetScriptUrls("~/myfile.js").Single();
			resolvedUrl.ShouldEqual("~/myfile.min.js");
		}

		[Test]
		public void WhenPreConsolidatedReportIsLoaded_ConsolidatedUrlsAreCachedAsThemselvesSoGroupsCanBeIncludedDirectly()
		{
			var groupTemplate = new Mock<IResourceGroupTemplate>();
			_context.ScriptGroups.Add(groupTemplate.Object);

			var preConsolidationReport = new PreConsolidationReport();
			var scriptGroup = new PreConsolidatedResourceGroup
			{
				ConsolidatedUrl = "~/scripts/consolidated/common.js",
				Resources = new List<string>
				{
					 "~/scripts/myscript.js"
				}
			};
			preConsolidationReport.Scripts.Groups.Add(scriptGroup);

			_context.LoadPreCompilationReport(preConsolidationReport);

			var resolvedUrl = _context.GetScriptUrls("~/scripts/consolidated/common.js").Single();
			resolvedUrl.ShouldEqual("~/scripts/consolidated/common.js");

			//verify that the group template was not looked at (that proves the value came from the prepopulated cache)
			string consolidatedUrl;
			groupTemplate.Verify(t => t.TryGetConsolidatedUrl(It.IsAny<string>(), out consolidatedUrl), Times.Never());
		}
	}
}