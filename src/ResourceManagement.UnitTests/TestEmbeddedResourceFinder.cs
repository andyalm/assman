using System;

using AlmWitt.Web.ResourceManagement.UnitTests.TestSupport;
using AlmWitt.Web.Test.ResourceManagement.TestObjects;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
    [TestFixture]
    public class TestEmbeddedResourceFinder
    {
        private const string _embeddedNs = "AlmWitt.Web.ResourceManagement.UnitTests.EmbeddedResources";
        private IResourceFinder _instance;

        [SetUp]
        public void Init()
        {
            _instance = ResourceFinderFactory.GetInstance(this.GetType().Assembly);
        }
        
        [Test]
        public void FindsFile()
        {
            ResourceCollection resources = _instance.FindResources(ResourceType.Css);

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(1));
            Assert.That(resources[0].Name, Is.EqualTo(_embeddedNs + ".Embedded1.css"));
        }

        [Test]
        public void FindsMultipleFiles()
        {
            ResourceCollection resources = _instance.FindResources(ResourceType.ClientScript);

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(2));
            AssertContains(resources, _embeddedNs + ".Embedded1.js");
            AssertContains(resources, _embeddedNs + ".Embedded2.js");
        }

[Test]
		public void FindsFilesForAllFileExtensions()
		{
			var resourceType = new StubResourceType("text/css", ".css");
			resourceType.AddFileExtension(".txt");

			var resources = _instance.FindResources(resourceType);

			resources.Count.ShouldEqual(2);
			AssertContains(resources, _embeddedNs + ".Embedded1.css");
			AssertContains(resources, _embeddedNs + ".Embedded1.txt");
		}
        private void AssertContains(ResourceCollection collection, string name)
        {
            IResource foundResource = collection.FindOne(delegate(IResource resource)
                                {
                                    return resource.Name == name;
                                });

            Assert.That(foundResource, Is.Not.Null);
        }
    }
}
