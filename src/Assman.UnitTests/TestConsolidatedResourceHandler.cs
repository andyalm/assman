using System;

using Assman.Configuration;
using Assman.Handlers;
using Assman.TestSupport;

using Moq;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestConsolidatedResourceHandler
	{
		private DateTime _lastModified;
		private ConsolidatedResourceHandler _instance;
		private Mock<IResourceFinder> _finder;
		private static readonly ResourceType _resourceType = ResourceType.Script;
		private const string VirtualPath = "~/myfile.js";

		[SetUp]
		public void Init()
		{
			_lastModified = DateTime.Now.AddYears(-1);
			var resources = new ResourceCollection
			{
				CreateResource("Content1", _lastModified.AddDays(-3)),
				CreateResource("Content2", _lastModified),
				CreateResource("Content3", _lastModified.AddDays(-10))
			};
			_finder = new Mock<IResourceFinder>();
			_finder.Setup(f => f.FindResources(_resourceType)).Returns(resources);
			
			var groupTemplate = new StubResourceGroupTemplate(new ResourceGroup(VirtualPath, resources));
			groupTemplate.ResourceType = _resourceType;

			var configContext = AssmanContext.Create();
			configContext.AddFinder(_finder.Object);
			
			_instance = new ConsolidatedResourceHandler(VirtualPath, configContext.GetConsolidator(), groupTemplate.WithEmptyContext());
		}

		[Test]
		public void NoIfModifiedSinceOutputsResource()
		{
			StubRequestContext context = new StubRequestContext();
			context.IfModifiedSince = null;

			_instance.HandleRequest(context);

			AssertContentReturned(context);
		}

		[Test]
		public void OldIfModifiedSinceOutputsResource()
		{
			StubRequestContext context = new StubRequestContext();
			context.IfModifiedSince = _lastModified.AddDays(-1);

			_instance.HandleRequest(context);

			AssertContentReturned(context);
		}

		[Test]
		public void RecentIfModifiedSinceReturns304()
		{
			StubRequestContext context = new StubRequestContext();
			context.IfModifiedSince = _lastModified.AddDays(1);

			_instance.HandleRequest(context);

			AssertNoContentReturned(context);
		}

		[Test]
		public void IdenticleIfModifiedByReturns304()
		{
			StubRequestContext context = new StubRequestContext();
			context.IfModifiedSince = _lastModified;

			_instance.HandleRequest(context);

			AssertNoContentReturned(context);
		}

		[Test]
		public void MinimumLastModifiedValueIsHonored()
		{
			StubRequestContext context = new StubRequestContext();
			context.IfModifiedSince = _lastModified.AddDays(1);

			_instance.MinLastModified = _lastModified.AddDays(2);
			_instance.HandleRequest(context);

			AssertContentReturned(context);
		}

		[Test]
		public void WhenDebugIsTrue_ConolidatatedResourceIsNotCached()
		{
			_instance.Mode = ResourceMode.Debug;
			var firstRequest = new StubRequestContext();
			_instance.HandleRequest(firstRequest);
			var secondRequest = new StubRequestContext();
			_instance.HandleRequest(secondRequest);

			AssertContentReturned(firstRequest);
			AssertContentReturned(secondRequest);

			AssertConsolidatedResourceWasNotCached();
		}

		[Test]
		public void WhenDebugIsFalse_ConolidatatedResourceIsCached()
		{
			_instance.Mode = ResourceMode.Release;
			var firstRequest = new StubRequestContext();
			_instance.HandleRequest(firstRequest);
			var secondRequest = new StubRequestContext();
			_instance.HandleRequest(secondRequest);

			AssertContentReturned(firstRequest);
			AssertContentReturned(secondRequest);

			AssertConsolidatedResourceWasCached();
		}

		private static IResource CreateResource(string content, DateTime lastModified)
		{
			StubResource resource1 = new StubResource(content);
			resource1.LastModified = lastModified;
			resource1.VirtualPath = "~/" + content + ".js";

			return resource1;
		}

		private static void AssertContentReturned(StubRequestContext context)
		{
			context.StatusCode.ShouldEqual(200);
			context.SetLastModifiedCalled.ShouldEqual(1);
			context.OutputStream.Length.ShouldNotEqual(0L);
		}

		private static void AssertNoContentReturned(StubRequestContext context)
		{
			context.StatusCode.ShouldEqual(304);
			context.OutputStream.Length.ShouldEqual(0L);
			context.SetLastModifiedCalled.ShouldEqual(0);
		}

		private void AssertConsolidatedResourceWasCached()
		{
			_finder.Verify(f => f.FindResources(It.IsAny<ResourceType>()), Times.Exactly(1));
		}

		private void AssertConsolidatedResourceWasNotCached()
		{
			_finder.Verify(g => g.FindResources(It.IsAny<ResourceType>()), Times.AtLeast(2));
		}
	}
}
