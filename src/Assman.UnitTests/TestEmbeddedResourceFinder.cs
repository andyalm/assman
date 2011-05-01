using System;
using System.Collections.Generic;
using System.Linq;

using Assman.TestSupport;
using Assman.TestObjects;

using NUnit.Framework;

namespace Assman
{
    [TestFixture]
    public class TestEmbeddedResourceFinder
    {
        private const string EmbeddedNs = "Assman.EmbeddedResources";
        private IResourceFinder _instance;

        [SetUp]
        public void Init()
        {
            _instance = ResourceFinderFactory.GetInstance(this.GetType().Assembly);
        }
        
        [Test]
        public void FindsFile()
        {
            var resources = _instance.FindResources(ResourceType.Css).ToList();

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(1));
            Assert.That(resources[0].Name, Is.EqualTo(EmbeddedNs + ".Embedded1.css"));
        }

        [Test]
        public void FindsMultipleFiles()
        {
			var resources = _instance.FindResources(ResourceType.ClientScript).ToList();

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(2));
            AssertContains(resources, EmbeddedNs + ".Embedded1.js");
            AssertContains(resources, EmbeddedNs + ".Embedded2.js");
        }

[Test]
		public void FindsFilesForAllFileExtensions()
		{
			var resourceType = new StubResourceType("text/css", ".css");
			resourceType.AddFileExtension(".txt");

			var resources = _instance.FindResources(resourceType).ToList();

			resources.Count.ShouldEqual(2);
			AssertContains(resources, EmbeddedNs + ".Embedded1.css");
			AssertContains(resources, EmbeddedNs + ".Embedded1.txt");
		}

        private void AssertContains(IEnumerable<IResource> collection, string name)
        {
            collection.ShouldContain(r => r.Name == name);
        }
    }
}
