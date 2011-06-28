using System;
using System.Linq;
using System.Web;

using Assman.Configuration;
using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
	public class TestAssmanContext_GetResourceUrl
	{
		private AssmanContext _instance;
		private const string myScript = "~/myscript.js";
		private const string mySecondScript = "~/mysecondscript.js";
		private const string consolidatedScript = "~/consolidated.js";
		private const string excludedScript = "~/excluded.js";
		private ScriptGroupElement _groupElement;
	    private StubResourceFinder _finder;
	    private StubDependencyProvider _dependencyProvider;

		[SetUp]
		public void Init()
		{
		    _finder = new StubResourceFinder();
		    _finder.CreateResource(myScript);
		    _finder.CreateResource(mySecondScript);
		    _finder.CreateResource(excludedScript);

            _dependencyProvider = new StubDependencyProvider();
            DependencyManagerFactory.ClearDependencyCache();

            _instance = AssmanContext.Create();
			_instance.ConsolidateScripts = true;
			_instance.ConfigurationLastModified = DateTime.MinValue;
		    _instance.AddFinder(_finder);
		    _instance.MapExtensionToDependencyProvider(".js", _dependencyProvider);
			_groupElement = new ScriptGroupElement();
			_groupElement.ConsolidatedUrl = consolidatedScript;
			_groupElement.Exclude.AddPattern(excludedScript);
			_instance.ScriptGroups.Add(_groupElement);
		}

		[Test]
		public void ReturnsConsolidatedScriptFileWhenScriptConsolidationEnabled()
		{
			_instance.ConsolidateScripts = true;

			var scriptToInclude = _instance.GetScriptUrls(myScript).Single();

			Assert.That(scriptToInclude, Is.EqualTo(consolidatedScript).IgnoreCase);
		}

		[Test]
		public void ReturnsSelfWhenScriptConsolidationDisabled()
		{
			_instance.ConsolidateScripts = false;
			string scriptToInclude = _instance.GetScriptUrls(myScript).Single();

			Assert.That(scriptToInclude, Is.EqualTo(myScript));
		}

		[Test]
		public void AppendsVersionNumberToConsolidatedUrl()
		{
			const string version = "2";
			_instance.ConsolidateScripts = true;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrls(myScript).Single();

			Assert.AreEqual(consolidatedScript + "?v=" + version, scriptToInclude);
		}

		[Test]
		public void AppendsVersionNumberToUnConsolidatedUrl()
		{
			const string version = "2";
			_instance.ConsolidateScripts = false;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrls(myScript).Single();

			Assert.AreEqual(myScript + "?v=" + version, scriptToInclude);
		}

		[Test]
		public void VersionParameterIsUrlEncoded()
		{
			const string version = "2 2";
			_instance.ConsolidateScripts = true;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrls(myScript).Single();

			Assert.AreEqual(consolidatedScript + "?v=" + HttpUtility.UrlEncode(version), scriptToInclude);
		}


		[Test]
		public void NeverReturnsConsolidatedScriptFileWhenScriptIsExcluded()
		{
			_instance.ConsolidateScripts = true;
			string scriptToInclude = _instance.GetScriptUrls(excludedScript).Single();

			Assert.That(scriptToInclude, Is.EqualTo(excludedScript));
		}

		[Test]
		public void ExclusionsAreCaseInsensitive()
		{
			_instance.ConsolidateScripts = true;
			string scriptToInclude = _instance.GetScriptUrls(excludedScript.ToUpperInvariant()).Single();

			Assert.That(scriptToInclude, Is.EqualTo(excludedScript).IgnoreCase);
		}

		[Test]
		public void FilesInSecondGroupAndNotInFirstReturnSecondGroupUrl()
		{
			const string secondGroupUrl = "~/mysecondconsolidation.js";
			_instance.ConsolidateScripts = true;
			_groupElement.Exclude.AddPattern(mySecondScript);
			var secondGroupElement = new ScriptGroupElement();
			_instance.ScriptGroups.Add(secondGroupElement);
			secondGroupElement.ConsolidatedUrl = secondGroupUrl;

			string scriptToInclude = _instance.GetScriptUrls(mySecondScript).Single();

			Assert.That(scriptToInclude, Is.EqualTo(secondGroupUrl).IgnoreCase);
		}

		[Test]
		public void ScriptUrlIsLazilyCached()
		{
			var resolvedScriptPath1 = _instance.GetScriptUrls(myScript).Single();
			if(resolvedScriptPath1 != consolidatedScript)
				Assert.Inconclusive("The first call to GetScriptUrl did not return the expected result");
			_instance.ScriptGroups.Clear();
			var resolvedScriptPath2 = _instance.GetScriptUrls(myScript).Single();

			resolvedScriptPath2.ShouldEqual(resolvedScriptPath1);
		}

		[Test]
		public void WhenGroupUrlIsPassedIntoGetResourceUrlAndConsolidationIsEnabled_GroupUrlIsReturned()
		{
			_groupElement.Include.AddPath(myScript);
			_groupElement.Include.AddPath(mySecondScript);
			
			var resolvedScriptPath = _instance.GetScriptUrls(consolidatedScript).Single();
			
			resolvedScriptPath.ShouldEqual(consolidatedScript);
		}

	    [Test]
	    public void WhenGroupUrlIsPassedInAndConsolidationForThatGroupIsDisabled_PathToEachResourceInGroupIsReturned()
	    {
            _groupElement.Include.AddPath(myScript);
            _groupElement.Include.AddPath(mySecondScript);
	        _groupElement.Consolidate = false;

	        var resolvedScriptPaths = _instance.GetScriptUrls(consolidatedScript).ToList();

            resolvedScriptPaths.CountShouldEqual(2);
            resolvedScriptPaths[0].ShouldEqual(myScript);
            resolvedScriptPaths[1].ShouldEqual(mySecondScript);
	    }

	    [Test]
	    public void WhenGroupUrlIsPassedInAndConsolidationForThatGroupIsDisabled_PathToEachResourceIsReturnedRespectingDependencies()
	    {
	        _dependencyProvider.SetDependencies(myScript, mySecondScript);
            _groupElement.Include.AddPath(myScript);
            _groupElement.Include.AddPath(mySecondScript);
            _groupElement.Consolidate = false;

            var resolvedScriptPaths = _instance.GetScriptUrls(consolidatedScript).ToList();

            resolvedScriptPaths.CountShouldEqual(2);
            resolvedScriptPaths[0].ShouldEqual(mySecondScript);
            resolvedScriptPaths[1].ShouldEqual(myScript);
	    }
	}
}