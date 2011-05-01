using System;
using System.Linq;

using Assman.Configuration;
using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
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

			_element.Exclude.AddPattern("exclude");
			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(2, resources.Count());
		}

		[Test]
		public void WhenOnlyIncludesSpecified_OnlyMatchingResourcesIncluded()
		{
			CreateResources("include1.js", "notincluded.js", "include2.js");

			_element.Include.AddPattern("include1");
			_element.Include.AddPattern("include2");
			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(2, resources.Count());
		}

		[Test]
		public void WhenPathIsSpecifiedInsteadOfPattern_OnlyExactMatchesMatch()
		{
			CreateResources("include1.js", "notincluded.js", "include2.js");

			_element.Include.AddPath("~/include1.js");
			_element.Include.AddPath("notincluded");
			_element.Include.AddPath("~/include2.js");
			var resources = GetResources(_element);

			Assert.IsNotNull(resources);
			Assert.AreEqual(2, resources.Count());
		}

		[Test]
		public void ExcludeTrumpsInclude()
		{
			CreateResources("include.js", "both.js", "exclude.js");

			_element.Include.AddPattern("include");
			_element.Include.AddPattern("both");
			_element.Exclude.AddPattern("exclude");
			_element.Exclude.AddPattern("both");
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
		public void WhenGroupContainsPatternForDebugAndReleaseVersionsOfScript_DebugVersionIsExcludedFromReleaseGroupAndReleaseIsExcludedFromDebugGroup()
		{
			CreateResources("neutralscript.js", "MicrosoftAjax.debug.js", "MicrosoftAjax.js");

			_element.Include.Add(new ResourceMatchElement
			{
				Pattern = "neutralscript.js"
			});
			_element.Include.Add(new ResourceMatchElement
			{
				Path = "~/MicrosoftAjax.debug.js",
				Mode = ResourceMode.Debug
			});
			_element.Include.Add(new ResourceMatchElement
			{
				Path = "~/MicrosoftAjax.js",
				Mode = ResourceMode.Release
			});

			var debugResources = _element.GetGroups(_allResources, ResourceMode.Debug).Single().GetResources().ToList();
			debugResources.CountShouldEqual(2);
			debugResources[0].VirtualPath.ShouldEqual("~/neutralscript.js");
			debugResources[1].VirtualPath.ShouldEqual("~/MicrosoftAjax.debug.js");

			var releaseResources = _element.GetGroups(_allResources, ResourceMode.Release).Single().GetResources().ToList();
			releaseResources.CountShouldEqual(2);
			releaseResources[0].VirtualPath.ShouldEqual("~/neutralscript.js");
			releaseResources[1].VirtualPath.ShouldEqual("~/MicrosoftAjax.js");
		}

		[Test]
		public void GetResourcesSortsByIncludeOrder()
		{
			CreateResources("file1.js", "file2.js", "file3.js",
				"file4.js", "file5.js");

			_element.Consolidate = true;
			_element.Include.AddPattern("file3.js");
			_element.Include.AddPattern("file2.js");
			_element.Include.AddPattern("file1.js");
			_element.Include.AddPattern("file4.js");
			_element.Include.AddPattern("file5.js");

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
		public void TryGetConsolidatedUrlHonorsIncludeAndExclude()
		{
			CreateResources("file1.js", "file2.js", "file3.js", "file4.js", "file5.js");

			_element.Consolidate = true;
			_element.Include.AddPattern("file2.js");
			_element.Include.AddPattern("file3.js");
			_element.Include.AddPattern("file4.js");
			_element.Exclude.AddPattern("file3.js");

			VerifyUrlIsNotConsolidated("file1.js");
			VerifyUrlIsConsolidated("file2.js");
			VerifyUrlIsNotConsolidated("file3.js");
			VerifyUrlIsConsolidated("file4.js");
			VerifyUrlIsNotConsolidated("file5.js");
		}

		[Test]
		public void WhenConsolidatedUrlIsNotTemplated_OnlyOneGroupIsReturnedFromGetGroups()
		{
			CreateResources("file1.js", "file2.js");

			_element.Include.AddPattern(@"file\d+\.js");

			var groups = _element.GetGroups(_allResources, ResourceMode.Release);
			groups.Count().ShouldEqual(1);
			groups.First().ConsolidatedUrl.ShouldEqual(_element.ConsolidatedUrl);
			groups.First().GetResources().ShouldContainAll(_allResources.ToArray());
		}

		[Test]
		public void WhenConsolidatedUrlIsTemplated_ThereAreGroupsReturnedForEveryMatchiCombinationOfTheTemplate()
		{
			CreateResources("Views/Search/Landing.js", "Views/Search/Results.js", "Views/Purchase/Landing.js");

			_element.ConsolidatedUrl = "~/consolidated/bycontroller/{controller}.js";
			_element.Include.AddPattern(@"~/Views/(?<controller>\w+)/.+\.js");

			var groups = _element.GetGroups(_allResources, ResourceMode.Release).ToList();
			groups.Count.ShouldEqual(2);
			groups.ShouldContain(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Search.js");
			groups.ShouldContain(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Purchase.js");
			groups.Single(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Search.js").GetResources().Count().ShouldEqual(2);
			groups.Single(g => g.ConsolidatedUrl == "~/consolidated/bycontroller/Purchase.js").GetResources().Count().ShouldEqual(1);
			VerifyUrlIsConsolidated("~/Views/Search/Landing.js", "~/consolidated/bycontroller/Search.js");
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

		[Test]
		public void GroupHasMinificationSetCorrectly()
		{
			CreateResources("file1.js", "file2.js");
			_element.Minify = true;

			var group = _element.GetGroups(_allResources, ResourceMode.Release).Single();

			group.Minify.ShouldBeTrue();
		}

		private void VerifyUrlIsConsolidated(string virtualPath)
		{
			VerifyUrlIsConsolidated(virtualPath, _element.ConsolidatedUrl);
		}

		private void VerifyUrlIsConsolidated(string virtualPath, string expectedConsolidatedUrl)
		{
			string consolidatedUrl;
			_element.TryGetConsolidatedUrl(virtualPath, out consolidatedUrl).ShouldBeTrue();
			consolidatedUrl.ShouldEqual(expectedConsolidatedUrl);
		}

		private void VerifyUrlIsNotConsolidated(string virtualPath)
		{
			string consolidatedUrl;
			_element.TryGetConsolidatedUrl(virtualPath, out consolidatedUrl).ShouldBeFalse();
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
			return groupElement.GetGroups(_allResources, ResourceMode.Release)
				.Single().GetResources().ToResourceCollection();
		}
	}
}
