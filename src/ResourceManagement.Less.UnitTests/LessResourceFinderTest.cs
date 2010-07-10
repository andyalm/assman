using System;

using AlmWitt.Web.ResourceManagement.IO;

using Moq;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.Less.UnitTests
{
    [TestFixture]
    public class LessResourceFinderTest
    {
        private LessResourceFinder _finder;
        private Mock<IFileFinder> _fileFinder;
        private const string _directory = "c:\\mydir";

        [SetUp]
        public void SetupContext()
        {
            _fileFinder = new Mock<IFileFinder>();

            _finder = new LessResourceFinder(_directory, _fileFinder.Object);
        }

        [Test]
        public void WhenFileExtensionEndsWithJs_EmptyCollectionIsReturned()
        {
            var resources = _finder.FindResources(".js");

            Assert.That(resources.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenFileExtensionEndsWithCss_LessFilesAreFound()
        {
            _fileFinder.Setup(f => f.FindFilesRecursively(_directory, ".less")).Returns(new[] { "myfile1.less", "myfile2.less" });
            
            var resources = _finder.FindResources(".css");

            Assert.That(resources.Count, Is.EqualTo(2));
            Assert.IsInstanceOf<LessResource>(resources[0]);
            Assert.IsInstanceOf<LessResource>(resources[1]);
        }
    }
}