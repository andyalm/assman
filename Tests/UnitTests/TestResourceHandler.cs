using System;

using AlmWitt.Web.ResourceManagement.UnitTests.TestObjects;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
    [TestFixture]
    public class TestResourceHandler
    {
        private StubResourceCollector _collector;
        private DateTime _lastModified;
        private ResourceHandler _instance;

        [SetUp]
        public void Init()
        {
            _lastModified = DateTime.Now.AddYears(-1);
            ResourceCollection resources = new ResourceCollection();
            resources.Add(CreateResource("Content1", _lastModified.AddDays(-3)));
            resources.Add(CreateResource("Content2", _lastModified));
            resources.Add(CreateResource("Content3", _lastModified.AddDays(-10)));
            _collector = new StubResourceCollector(ConsolidatedResource.FromResources(resources));
            _instance = new ClientScriptResourceHandler(ResourceFinderFactory.Null, _collector, null);
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
        public void OldIfModifiedByMillisecondsReturns304()
        {
            StubRequestContext context = new StubRequestContext();
            context.IfModifiedSince = _lastModified.AddMilliseconds(-5);

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

        private static IResource CreateResource(string content, DateTime lastModified)
        {
            StubResource resource1 = new StubResource(content);
            resource1.LastModified = lastModified;

            return resource1;
        }

        private static void AssertContentReturned(StubRequestContext context)
        {
            Assert.That(context.StatusCode, Is.EqualTo(200));
            Assert.That(context.SetLastModifiedCalled, Is.EqualTo(1));
            Assert.That(context.OutputStream.Length, Is.Not.EqualTo(0L));
        }

        private static void AssertNoContentReturned(StubRequestContext context)
        {
            Assert.That(context.StatusCode, Is.EqualTo(304));
            Assert.That(context.SetLastModifiedCalled, Is.EqualTo(0));
            Assert.That(context.OutputStream.Length, Is.EqualTo(0L));
        }
    }
}