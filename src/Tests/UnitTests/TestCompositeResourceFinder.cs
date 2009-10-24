using System;
using System.Collections.Generic;
using System.Text;

using AlmWitt.Web.ResourceManagement.UnitTests.TestObjects;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
    [TestFixture]
    public class TestCompositeResourceFinder
    {
        private CompositeResourceFinder _instance;

        [SetUp]
        public void Init()
        {
            _instance = new CompositeResourceFinder();
        }
        
        [Test]
        public void FindResourcesCombinesResultsOfChildren()
        {
            StubResourceFinder finder1 = new StubResourceFinder();
            finder1.AddResource("~/myfile1.js", "content1");
            finder1.AddResource("~/myfile2.js", "content2");
            _instance.AddFinder(finder1);

            StubResourceFinder finder2 = new StubResourceFinder();
            finder2.AddResource("assembly://MyAssembly/MyNamespace.Myfile1.js", "content3");
            _instance.AddFinder(finder2);


            ResourceCollection resources = _instance.FindResources(".js");
            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(3));
            Assert.That(resources[0].VirtualPath, Is.EqualTo("~/myfile1.js"));
            Assert.That(resources[1].VirtualPath, Is.EqualTo("~/myfile2.js"));
            Assert.That(resources[2].VirtualPath, Is.EqualTo("assembly://MyAssembly/MyNamespace.Myfile1.js"));
        }
    }
}