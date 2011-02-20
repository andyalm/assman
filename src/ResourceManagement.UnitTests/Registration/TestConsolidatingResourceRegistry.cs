using System.Collections.Generic;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.TestSupport;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	[TestFixture]
	public class TestConsolidatingResourceRegistry
	{
		private Mock<IResourceRegistry> _innerRegistry;
		private ResourceManagementContext _config;
		private Dictionary<string, string> _resolvedUrlCache;
		private ConsolidatingResourceRegistry _consolidatingRegistry;
		private const string UrlNotToBeConsolidated = "~/random/location/script.js";
		private const string UrlToBeConsolidated = "~/configured/location/script.js";
		private const string ConsolidatedUrl = "~/scripts/consolidated/core.js";

		[SetUp]
		public void SetupContext()
		{
			_innerRegistry = new Mock<IResourceRegistry>();
			_config = ResourceManagementContext.Create();
			_config.PreConsolidated = true;
			_config.ClientScriptGroups.Add(new ClientScriptGroupElement
			{
				ConsolidatedUrl = ConsolidatedUrl,
				Include = new ResourceMatchElementCollection
				{
					"~/configured/location/.+"
				}
			});
			_resolvedUrlCache = new Dictionary<string, string>();

			_consolidatingRegistry = new ConsolidatingResourceRegistry(_innerRegistry.Object, _config.GetScriptUrl, _resolvedUrlCache);
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
			string resolvedVirtualPath;
			_consolidatingRegistry.TryResolvePath(UrlNotToBeConsolidated, out resolvedVirtualPath).ShouldBeFalse();
			resolvedVirtualPath.ShouldBeNull();
		}

		[Test]
		public void WhenTryingToResolveAPathThatIsConfiguredToBeConsolidated_ItIsResolved()
		{
			string resolvedVirtualPath;
			_consolidatingRegistry.TryResolvePath(UrlToBeConsolidated, out resolvedVirtualPath).ShouldBeTrue();
			resolvedVirtualPath.ShouldEqual(ConsolidatedUrl);
		}
	}
}