using System;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.UnitTests.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
    [TestFixture]
    public class TestResourceGroupElement
    {
        private TempDirectoryManager _dirMgr;

        [SetUp]
        public void Init()
        {
            _dirMgr = new TempDirectoryManager();
        }

        [TearDown]
        public void Dispose()
        {
            _dirMgr.Dispose();
        }

        [Test]
        public void GetResourceExcludes()
        {
            _dirMgr.CreateFile("include1.js", "");
            _dirMgr.CreateFile("exclude.js", "");
            _dirMgr.CreateFile("include2.js", "");

            ResourceGroupElement element = new ClientScriptGroupElement();
            element.Exclude.Add("exclude");
            ResourceCollection resources = GetConsolidatedResource(element).Resources;

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetResourceIncludes()
        {
            _dirMgr.CreateFile("include1.js", "");
            _dirMgr.CreateFile("notincluded.js", "");
            _dirMgr.CreateFile("include2.js", "");

            ResourceGroupElement element = new ClientScriptGroupElement();
            element.Include.Add("include1");
            element.Include.Add("include2");
            ResourceCollection resources = GetConsolidatedResource(element).Resources;

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(2));
        }

        [Test]
        public void ExcludeTrumpsInclude()
        {
            _dirMgr.CreateFile("include.js");
            _dirMgr.CreateFile("both.js");
            _dirMgr.CreateFile("exclude.js");

            ResourceGroupElement element = new ClientScriptGroupElement();
            element.Include.Add("include");
            element.Include.Add("both");
            element.Exclude.Add("exclude");
            element.Exclude.Add("both");
            ResourceCollection resources = GetConsolidatedResource(element).Resources;

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(1));
        }

    	[Test]
        public void GetResourcesWithNoIncludeOrExcludeReturnsEverything()
        {
            _dirMgr.CreateFile("file1.js");
            _dirMgr.CreateFile("file2.js");
            _dirMgr.CreateFile("file3.js");

            ResourceGroupElement element = new ClientScriptGroupElement();
            ResourceCollection resources = GetConsolidatedResource(element).Resources;

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(3));
        }

    	[Test]
        public void GetResourcesExcludesGivenMatchFilter()
        {
            _dirMgr.CreateFile("file1.js");
            _dirMgr.CreateFile("file2.js");
            _dirMgr.CreateFile("file3.js");

            ResourceGroupElement element = new ClientScriptGroupElement();
            element.Consolidate = true;

    		ResourceCollection resources = GetConsolidatedResource(element, ToFilter(delegate(IResource resource)
    		{
    			return resource.VirtualPath.Contains("file2");
    		})).Resources;

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Count, Is.EqualTo(2));
        }

    	[Test]
		public void GetResourcesHonorsOrderOfIncludes()
		{
			_dirMgr.CreateFile("file1.js");
			_dirMgr.CreateFile("file2.js");
			_dirMgr.CreateFile("file3.js");
			_dirMgr.CreateFile("file4.js");
			_dirMgr.CreateFile("file5.js");

			ResourceGroupElement element = new ClientScriptGroupElement();
			element.Consolidate = true;
			element.Include.Add("~/file4.js");
			element.Include.Add("~/file1.js");
			element.Include.Add("~/file2.js");
			element.Include.Add("~/file3.js");
			element.Include.Add("~/file5.js");

    		ResourceCollection resources = GetConsolidatedResource(element).Resources;

			Assert.That(resources.Count, Is.EqualTo(5));
			Assert.That(resources[0].Name, Is.EqualTo("file4.js"));
			Assert.That(resources[1].Name, Is.EqualTo("file1.js"));
			Assert.That(resources[2].Name, Is.EqualTo("file2.js"));
			Assert.That(resources[3].Name, Is.EqualTo("file3.js"));
			Assert.That(resources[4].Name, Is.EqualTo("file5.js"));
		}

    	[Test]
        public void GetResourceUrlHonorsIncludeAndExclude()
        {
            _dirMgr.CreateFile("file1.js");
            _dirMgr.CreateFile("file2.js");
            _dirMgr.CreateFile("file3.js");
            _dirMgr.CreateFile("file4.js");
            _dirMgr.CreateFile("file5.js");

            ResourceGroupElement element = new ClientScriptGroupElement();
            element.Consolidate = true;
            element.Include.Add("file2.js");
            element.Include.Add("file3.js");
            element.Include.Add("file4.js");
            element.Exclude.Add("file3.js");

            Assert.That(element.GetResourceUrl("file1.js", UrlType.Dynamic), Is.EqualTo("file1.js"));
            Assert.That(element.GetResourceUrl("file2.js", UrlType.Dynamic), Is.Not.EqualTo("file2.js"));
            Assert.That(element.GetResourceUrl("file3.js", UrlType.Dynamic), Is.EqualTo("file3.js"));
            Assert.That(element.GetResourceUrl("file4.js", UrlType.Dynamic), Is.Not.EqualTo("file4.js"));
            Assert.That(element.GetResourceUrl("file5.js", UrlType.Dynamic), Is.EqualTo("file5.js"));
        }

    	private ConsolidatedResource GetConsolidatedResource(ResourceGroupElement element)
    	{
    		return element.GetResource(ResourceFinderFactory.GetInstance(_dirMgr.DirectoryName), ".js");
    	}

		private ConsolidatedResource GetConsolidatedResource(ResourceGroupElement element, IResourceFilter filter)
		{
			return element.GetResource(ResourceFinderFactory.GetInstance(_dirMgr.DirectoryName), ".js", filter);
		}

    	private static IResourceFilter ToFilter(Predicate<IResource> match)
        {
            return ResourceFilters.Predicate(match);
        }
    }
}