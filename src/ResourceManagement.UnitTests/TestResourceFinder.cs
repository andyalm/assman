using System;
using System.Linq;

using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceFinder
	{
		private TempDirectoryManager _dirMgr;
		private IResourceFinder _finder;

		[SetUp]
		public void Setup()
		{
			_dirMgr = new TempDirectoryManager();
			_finder = ResourceFinderFactory.GetInstance(_dirMgr.DirectoryName);
		}
		
		[TearDown]
		public void TearDown()
		{
			_dirMgr.Dispose();
		}
		
		[Test]
		public void FindsJavascriptFileWithJsExtension()
		{
			string resourceName1 = "myscriptfile1.js";
			_dirMgr.CreateFile(resourceName1, "");
			string resourceName2 = "myscriptfile2.js";
			_dirMgr.CreateFile(resourceName2, "");

			var resources = _finder.FindResources(ResourceType.ClientScript).ToList();

			Assert.IsNotNull(resources);
			Assert.AreEqual(2, resources.Count);
			Assert.AreEqual(resourceName1, resources[0].Name);
			Assert.AreEqual(resourceName2, resources[1].Name);
		}

		[Test]
		public void FindsJavascriptFileWithJsExtensionInSubDirectory()
		{
			string resourceName = "myscriptfile.js";
			_dirMgr.CreateFile("mysubdir", resourceName, "");

			var resources = _finder.FindResources(ResourceType.ClientScript).ToList();

			Assert.That(resources, Is.Not.Null);
			Assert.That(resources.Count, Is.EqualTo(1));
            Assert.That(resources[0].Name, Is.EqualTo(resourceName));
		}

		[Test]
		public void FindAppliesFilter()
		{
			string resourceName = "myscriptfile.js";
			_dirMgr.CreateFile(resourceName, "");
			string filteredResourceName = "filtered.js";
			_dirMgr.CreateFile(filteredResourceName, "");

			_finder = ResourceFinderFactory.GetInstance(_dirMgr.DirectoryName);
			var resources = _finder.FindResources(ResourceType.ClientScript)
				.Where(r => r.Name != filteredResourceName).ToList();

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(1));
            Assert.That(resources[0].Name, Is.EqualTo(resourceName));
		}
	}
}
