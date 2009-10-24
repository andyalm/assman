using System;

using AlmWitt.Web.ResourceManagement.UnitTests.TestObjects;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
	[TestFixture]
	public class TestCachingResourceCollector
	{
		private ConsolidatedResource _resource;
		private StubResourceCollector _stub;
		private CachingResourceCollector _instance;

		[SetUp]
		public void Init()
		{
			ResourceCollection resources = new ResourceCollection();
			resources.Add(new StubResource("content1"));
			resources.Add(new StubResource("content2"));
			_resource = ConsolidatedResource.FromResources(resources);
			_stub = new StubResourceCollector(_resource);
			_instance = new CachingResourceCollector(_stub);
		}

		[Test]
		public void CallsGetFirstTimeAndReturnsResults()
		{
			ConsolidatedResource resource = _instance.GetResource(ResourceFinderFactory.Null, String.Empty, null);

            Assert.That(_stub.GetCount, Is.EqualTo(1));
            Assert.That(resource, Is.EqualTo(_resource));
		}

		[Test]
		public void InternallyCallsGetOnceWhenCalledMultipleTimes()
		{
            ConsolidatedResource resource1 = _instance.GetResource(ResourceFinderFactory.Null, String.Empty, null);
            ConsolidatedResource resource2 = _instance.GetResource(ResourceFinderFactory.Null, String.Empty, null);

            Assert.That(_stub.GetCount, Is.EqualTo(1));
            Assert.That(resource2, Is.EqualTo(resource1));
            Assert.That(resource1, Is.EqualTo(_resource));
		}
	}
}