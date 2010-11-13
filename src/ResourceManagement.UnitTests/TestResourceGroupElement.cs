using System;
using System.Linq;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceGroupElement
	{
		private ResourceCollection _allResources;
		private ResourceGroupElement _element;

		[SetUp]
		public void Init()
		{
			_allResources = new ResourceCollection();
			_element = new ClientScriptGroupElement();
			_element.ConsolidatedUrl = "~/consolidated.js";
		}

		[Test]
		public void WhenOnlyExcludeSpecified_AllRemainingResourcesAreImplicitlyIncluded()
		{
			CreateResources("include1.js", "exclude.js", "include2.js");

			_element.Exclude.Add("exclude");
			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(2, resources.Count());
		}

		[Test]
		public void WhenOnlyIncludesSpecified_OnlyMatchingResourcesIncluded()
		{
			CreateResources("include1.js", "notincluded.js", "include2.js");

			_element.Include.Add("include1");
			_element.Include.Add("include2");
			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(2, resources.Count());
		}

		[Test]
		public void ExcludeTrumpsInclude()
		{
			CreateResources("include.js", "both.js", "exclude.js");

			_element.Include.Add("include");
			_element.Include.Add("both");
			_element.Exclude.Add("exclude");
			_element.Exclude.Add("both");
			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(1, resources.Count());
		}

		[Test]
		public void WhenNoInludeOrExcludeIsSpecified_EverythingIsImplicitlyIncluded()
		{
			CreateResources("file1.js", "file2.js", "file3.js");

			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(3, resources.Count());
		}

		[Test]
		public void GetResourcesSortsByIncludeOrder()
		{
			CreateResources("file1.js", "file2.js", "file3.js",
				"file4.js", "file5.js");

			_element.Consolidate = true;
			_element.Include.Add("file3.js");
			_element.Include.Add("file2.js");
			_element.Include.Add("file1.js");
			_element.Include.Add("file4.js");
			_element.Include.Add("file5.js");

			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(5, resources.Count);
			Assert.AreEqual("file3.js", resources[0].Name);
			Assert.AreEqual("file2.js", resources[1].Name);
			Assert.AreEqual("file1.js", resources[2].Name);
			Assert.AreEqual("file4.js", resources[3].Name);
			Assert.AreEqual("file5.js", resources[4].Name);
		}

		[Test]
		public void GetResourceUrlHonorsIncludeAndExclude()
		{
			CreateResources("file1.js", "file2.js", "file3.js", "file4.js", "file5.js");

			_element.Consolidate = true;
			_element.Include.Add("file2.js");
			_element.Include.Add("file3.js");
			_element.Include.Add("file4.js");
			_element.Exclude.Add("file3.js");

			_element.GetResourceUrl("file1.js", UrlType.Static).ShouldEqual("file1.js");
			_element.GetResourceUrl("file2.js", UrlType.Static).ShouldEqual(_element.ConsolidatedUrl);
			_element.GetResourceUrl("file3.js", UrlType.Static).ShouldEqual("file3.js");
			_element.GetResourceUrl("file4.js", UrlType.Static).ShouldEqual(_element.ConsolidatedUrl);
			_element.GetResourceUrl("file5.js", UrlType.Static).ShouldEqual("file5.js");
		}

		[Test]
		public void WhenConsolidatedUrlIsNotTemplated_OnlyOneGroupIsReturnedFromGetGroups()
		{
			CreateResources("file1.js", "file2.js");

			_element.Include.Add(@"file\d+\.js");

			var groups = _element.GetGroups(_allResources);
			groups.Count().ShouldEqual(1);
			groups.First().ConsolidatedUrl.ShouldEqual(_element.ConsolidatedUrl);
			groups.First().GetResources().ShouldContainAll(_allResources.ToArray());
		}

		[Test]
		public void WhenConsolidatedUrlIsTemplated_ThereAreGroupsReturnedForEveryMatchiCombinationOfTheTemplate()
		{
			CreateResources("Views/Search/Landing.js", "Views/Search/Results.js", "Views/Purchase/Landing.js");

			_element.ConsolidatedUrl = "~/consolidated/bycontroller/{controller}.js";
			_element.Include.Add(@"~/Views/(?<controller>\w+)/.+\.js");

			var groups = _element.GetGroups(_allResources).ToList();
			groups.Count.ShouldEqual(2);
			groups.ShouldContain(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Search.js");
			groups.ShouldContain(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Purchase.js");
			groups.Single(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Search.js").GetResources().Count().ShouldEqual(2);
			groups.Single(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Purchase.js").GetResources().Count().ShouldEqual(1);
		}

		[Test]
		public void MatchesUrlTemplate_NonTemplatePositiveCase()
		{
			_element.MatchesConsolidatedUrl(_element.ConsolidatedUrl).ShouldEqual(true);
		}

		[Test]
		public void MatchesUrlTemplate_NonTemplateNegativeCase()
		{
			_element.MatchesConsolidatedUrl("~/somethingelse.js").ShouldEqual(false);
		}

		[Test]
		public void MatchesUrlTemplate_TemplatePositiveCase()
		{
			_element.ConsolidatedUrl = "~/consolidated/bycontroller/{controller}.js";
			_element.MatchesConsolidatedUrl("~/consolidated/bycontroller/search.js").ShouldBeTrue();
		}

		[Test]
		public void MatchesUrlTemplate_TemplateNegativeCase()
		{
			_element.ConsolidatedUrl = "~/consolidated/bycontroller/{controller}.js";
			_element.MatchesConsolidatedUrl("~/consolidated/bycontroller/search/other.js").ShouldBeFalse();
		}

		private void CreateResources(params string[] paths)
		{
			foreach (var path in paths)
			{
				_allResources.Add(StubResource.WithPath("~/" + path));
			}
		}

		private ResourceCollection GetResources(ResourceGroupElement groupElement)
		{
			return groupElement.GetGroups(_allResources)
				.Single().GetResources().ToResourceCollection();
		}
	}
}
