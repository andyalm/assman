using System;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
            ResourceCollection resources = _instance.FindResources(".css");

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(1));
            Assert.That(resources[0].Name, Is.EqualTo(_embeddedNs + ".Embedded1.css"));
        }

        [Test]
        public void FindsMultipleFiles()
        {
            ResourceCollection resources = _instance.FindResources(".js");

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(2));
            AssertContains(resources, _embeddedNs + ".Embedded1.js");
            AssertContains(resources, _embeddedNs + ".Embedded2.js");
        }

        private void AssertContains(ResourceCollection collection, string name)
        {
            IResource foundResource = collection.Find(delegate(IResource resource)
                                {
                                    return resource.Name == name;
                                });

            Assert.That(foundResource, Is.Not.Null);
        }
    }
}