using System;
using System.Web;

using Assman.Configuration;
using Assman.Handlers;
using Assman.TestSupport;

using Moq;

using NUnit.Framework;

using Assman.IO;

namespace Assman
{
	[TestFixture]
	public class TestConsolidatedResourceHandler
	{
		private DateTime _lastModified;
		private DynamicallyConsolidatedResourceHandler _instance;
		private Mock<IResourceFinder> _finder;
		private static readonly ResourceType _resourceType = ResourceType.Script;
		private DateTime _now;
		private const string VirtualPath = "~/myfile.js";

		[SetUp]
		public void Init()
		{
			_now = DateTime.Now;
			_lastModified = _now.AddYears(-1);
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

		    var configContext = new AssmanContext(ResourceMode.Debug);
			configContext.AddFinder(_finder.Object);

			_instance = new DynamicallyConsolidatedResourceHandler(VirtualPath, configContext.GetConsolidator(), groupTemplate.WithEmptyContext())
			{
				Now = () => _now
			};
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

		[Test]
		public void WhenVersionIsSpecifiedInRequest_ExpiresHeaderIsSetToOneYearFromNow()
		{
			var request = new StubRequestContext();
			request.QueryString["v"] = "123";
			_instance.HandleRequest(request);

			request.Expires.ShouldEqual(_now.AddYears(1));
			request.Cacheability.ShouldEqual(HttpCacheability.Public);
		}

		[Test]
		public void WhenVersionIsNotSpecifiedInRequest_ExpiresHeaderIsNotSet()
		{
			var request = new StubRequestContext();
			_instance.HandleRequest(request);

			request.Expires.ShouldEqual(DateTime.MinValue);
			((int)request.Cacheability).ShouldEqual(0);
		}

		[Test]
		public void WhenGZipEncodingIsAcceptedAndItIsEnabledAndItIsReleaseMode_ContentIsReturnedGZipped()
		{
			var request = new StubRequestContext();
			request.AcceptsGZip = true;
			_instance.EnableGZip = true;
			_instance.Mode = ResourceMode.Release;
			_instance.HandleRequest(request);

			request.Vary.ShouldEqual("Content-Encoding");
			request.ContentEncoding.ShouldEqual("gzip");
			request.OutputStream.Position = 0;
			var content = request.OutputStream.Decompress().ReadToEnd();
			content.ShouldEqual("Content1\r\nContent2\r\nContent3");
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
			context.OutputStream.Length.ShouldNotEqual(0L);
		}

		private static void AssertNoContentReturned(StubRequestContext context)
		{
			context.StatusCode.ShouldEqual(304);
			context.OutputStream.Length.ShouldEqual(0L);
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
