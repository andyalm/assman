using System;
using System.Web.Configuration;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.UnitTests.TestSupport;
using AlmWitt.Web.Test.ResourceManagement.TestObjects;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
	[TestFixture]
	public class TestResourceHandler
	{
		private DateTime _lastModified;
		private ResourceHandler _instance;
		private Mock<IResourceFinder> _finder;
		private CompilationSection _compilationSection;
		private static readonly ResourceType _resourceType = ResourceType.ClientScript;
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
			
			var groupTemplate = new StubResourceGroupTemplate(new StaticResourceGroup(VirtualPath, resources));
			groupTemplate.ResourceType = _resourceType;
			
			_compilationSection = new CompilationSection();
			_compilationSection.Debug = true;
			var configLoader = new Mock<IConfigLoader>();
			configLoader.Setup(l => l.GetSection<CompilationSection>(It.IsAny<string>())).Returns(_compilationSection);

			var configContext = ResourceManagementContext.Create();
			configContext.AddFinder(_finder.Object);
			
			_instance = new ResourceHandler(VirtualPath, configContext, new GroupTemplateContext(groupTemplate));
			_instance.GetConfigurationLoader = () => configLoader.Object;
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
			_compilationSection.Debug = true;
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
			_compilationSection.Debug = false;
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
			Assert.AreEqual(200, context.StatusCode);
			Assert.AreEqual(1, context.SetLastModifiedCalled);
			Assert.AreNotEqual(0L, context.OutputStream.Length);
		}

		private static void AssertNoContentReturned(StubRequestContext context)
		{
			Assert.AreEqual(304, context.StatusCode);
			Assert.AreEqual(0L, context.OutputStream.Length);
			Assert.AreEqual(0, context.SetLastModifiedCalled);
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
