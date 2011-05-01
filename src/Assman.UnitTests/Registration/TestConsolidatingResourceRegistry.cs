using System.Collections.Generic;
using System.Linq;

using Assman.Configuration;
using Assman.TestSupport;

using Moq;

using NUnit.Framework;

namespace Assman.Registration
{
	[TestFixture]
	public class TestConsolidatingResourceRegistry
	{
		private Mock<IResourceRegistry> _innerRegistry;
		private ResourceManagementContext _context;
		private ConsolidatingResourceRegistry _consolidatingRegistry;
		private const string UrlNotToBeConsolidated = "~/random/location/script.js";
		private const string UrlToBeConsolidated = "~/configured/location/script.js";
		private const string ConsolidatedUrl = "~/scripts/consolidated/core.js";

		[SetUp]
		public void SetupContext()
		{
			_innerRegistry = new Mock<IResourceRegistry>();
			_context = ResourceManagementContext.Create();
			_context.ScriptGroups.Add(new ClientScriptGroupElement
			{
				ConsolidatedUrl = ConsolidatedUrl,
				Include = new ResourceMatchElementCollection
				{
					new ResourceMatchElement
					{
						Pattern = "~/configured/location/.+"
					}
				}
			});

			_consolidatingRegistry = new ConsolidatingResourceRegistry(_innerRegistry.Object, _context.GetScriptUrls);
		}

		[Test]
		public void WhenIncludingAUrlThatIsNotConfiguredToBeConsolidated_ItIsIncludedAsIs()
		{
			_consolidatingRegistry.IncludePath(UrlNotToBeConsolidated);

			_innerRegistry.Verify(r => r.IncludePath(UrlNotToBeConsolidated));
		}

		[Test]
		public void WhenIncludingAUrlThaIsConfiguredToBeConsolidated_TheConsolidatedUrlIsIncludedInstead()
		{
			_consolidatingRegistry.IncludePath(UrlToBeConsolidated);

			_innerRegistry.Verify(r => r.IncludePath(ConsolidatedUrl));
		}

		[Test]
		public void WhenTryingToResolveAPathThatIsNotConfiguredToBeConsolidated_ItIsNotResolved()
		{
			IEnumerable<string> resolvedVirtualPath;
			_consolidatingRegistry.TryResolvePath(UrlNotToBeConsolidated, out resolvedVirtualPath).ShouldBeFalse();
			resolvedVirtualPath.ShouldBeNull();
		}

		[Test]
		public void WhenTryingToResolveAPathThatIsConfiguredToBeConsolidated_ItIsResolved()
		{
            IEnumerable<string> resolvedVirtualPath;
			_consolidatingRegistry.TryResolvePath(UrlToBeConsolidated, out resolvedVirtualPath).ShouldBeTrue();
			resolvedVirtualPath.Single().ShouldEqual(ConsolidatedUrl);
		}
	}
}