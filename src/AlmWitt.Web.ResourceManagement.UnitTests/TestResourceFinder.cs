using System;

using AlmWitt.Web.ResourceManagement.UnitTests.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
	[TestFixture]
	public class TestResourceFinder
	{
		private TempDirectoryManager _dirMgr;

		[SetUp]
		public void Setup()
		{
			_dirMgr = new TempDirectoryManager();
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

			ResourceCollection resources = ResourceFinderFactory.GetInstance(_dirMgr.DirectoryName).FindResources(".js");
			
			Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(2));
			Assert.That(resources[0].Name, Is.EqualTo(resourceName1));
            Assert.That(resources[1].Name, Is.EqualTo(resourceName2));
		}

		[Test]
		public void FindsJavascriptFileWithJsExtensionInSubDirectory()
		{
			string resourceName = "myscriptfile.js";
			_dirMgr.CreateFile("mysubdir", resourceName, "");

			ResourceCollection resources = ResourceFinderFactory.GetInstance(_dirMgr.DirectoryName).FindResources(".js");

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

			ResourceCollection resources = ResourceFinderFactory.GetInstance(_dirMgr.DirectoryName).FindResources(".js").Where(delegate(IResource r)
			                                                                                	{
			                                                                                		return r.Name != filteredResourceName;
			                                                                                	});

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(1));
            Assert.That(resources[0].Name, Is.EqualTo(resourceName));
		}
	}
}
