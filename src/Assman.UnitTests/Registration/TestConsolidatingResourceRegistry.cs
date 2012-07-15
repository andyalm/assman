using System;
using System.Linq;
using System.Web;

using Assman.Configuration;
using Assman.DependencyManagement;
using Assman.TestSupport;

using NUnit.Framework;

namespace Assman.Registration
{
	public class TestConsolidatingResourceRegistry
	{
		private AssmanContext _context;
		private const string myScript = "~/myscript.js";
		private const string mySecondScript = "~/mysecondscript.js";
		private const string consolidatedScript = "~/consolidated.js";
		private const string excludedScript = "~/excluded.js";
		private ScriptGroupElement _groupElement;
		private StubResourceFinder _finder;
		private StubDependencyProvider _dependencyProvider;
		private ConsolidatingResourceRegistry _registry;

		[SetUp]
		public void Init()
		{
			_finder = new StubResourceFinder();
			_finder.CreateResource(myScript);
			_finder.CreateResource(mySecondScript);
			_finder.CreateResource(excludedScript);

			_dependencyProvider = new StubDependencyProvider();
			DependencyManagerFactory.ClearDependencyCache();

			_context = AssmanContext.Create(ResourceMode.Debug);
			_context.ConsolidateScripts = true;
			_context.ConfigurationLastModified = DateTime.MinValue;
			_context.AddFinder(_finder);
			_context.MapExtensionToDependencyProvider(".js", _dependencyProvider);
			_groupElement = new ScriptGroupElement();
			_groupElement.ConsolidatedUrl = consolidatedScript;
			_groupElement.Exclude.AddPattern(excludedScript);
			_context.ScriptGroups.Add(_groupElement);

			_registry = new ConsolidatingResourceRegistry(new ResourceRequirementCollection(), "Default", _context.ScriptPathResolver, new ConfiguredVersioningStrategy(() => _context.Version));
		}

		[Test]
		public void IncludesConsolidatedScriptFileWhenScriptConsolidationEnabled()
		{
			_context.ConsolidateScripts = true;

			_registry.Require(myScript);
			var scriptToInclude = _registry.GetIncludes().Single();

			Assert.That(scriptToInclude, Is.EqualTo(consolidatedScript).IgnoreCase);
		}

		[Test]
		public void IncludesAllFilesInGroupWhenScriptConsolidationDisabled()
		{
			_context.ConsolidateScripts = false;
			_registry.Require(myScript);
			var scriptsToInclude = _registry.GetIncludes().ToList();
			scriptsToInclude.CountShouldEqual(2);
			scriptsToInclude.ShouldContain(p => p.EqualsVirtualPath(myScript));
			scriptsToInclude.ShouldContain(p => p.EqualsVirtualPath(mySecondScript));
		}

		[Test]
		public void AppendsVersionNumberToConsolidatedUrl()
		{
			const string version = "2";
			_context.ConsolidateScripts = true;
			_context.Version = version;
			_registry.Require(myScript);
			string scriptToInclude = _registry.GetIncludes().Single();

			Assert.AreEqual(consolidatedScript + "?v=" + version, scriptToInclude);
		}

		[Test]
		public void AppendsVersionNumberToUnConsolidatedUrl()
		{
			const string version = "2";
			_context.ConsolidateScripts = false;
			_context.Version = version;
			_registry.Require(myScript);
			var scriptsToInclude = _registry.GetIncludes();
			scriptsToInclude.All(path => path.EndsWith("?v=" + version)).ShouldBeTrue();
		}

		[Test]
		public void VersionParameterIsUrlEncoded()
		{
			const string version = "2 2";
			_context.ConsolidateScripts = true;
			_context.Version = version;
			_registry.Require(myScript);
			string scriptToInclude = _registry.GetIncludes().Single();

			Assert.AreEqual(consolidatedScript + "?v=" + HttpUtility.UrlEncode(version), scriptToInclude);
		}


		[Test]
		public void IncludesIndividualFileWhenItIsNotPartOfAGroup()
		{
			_context.ConsolidateScripts = true;
			_registry.Require(excludedScript);
			string scriptToInclude = _registry.GetIncludes().Single();

			Assert.That(scriptToInclude, Is.EqualTo(excludedScript));
		}

		[Test]
		public void ExclusionsAreCaseInsensitive()
		{
			_context.ConsolidateScripts = true;
			_registry.Require(excludedScript.ToUpperInvariant());
			string scriptToInclude = _registry.GetIncludes().Single();

			Assert.That(scriptToInclude, Is.EqualTo(excludedScript).IgnoreCase);
		}

		[Test]
		public void FilesInSecondGroupAndNotInFirstReturnSecondGroupUrl()
		{
			const string secondGroupUrl = "~/mysecondconsolidation.js";
			_context.ConsolidateScripts = true;
			_groupElement.Exclude.AddPattern(mySecondScript);
			var secondGroupElement = new ScriptGroupElement();
			_context.ScriptGroups.Add(secondGroupElement);
			secondGroupElement.ConsolidatedUrl = secondGroupUrl;

			_registry.Require(mySecondScript);
			string scriptToInclude = _registry.GetIncludes().Single();

			Assert.That(scriptToInclude, Is.EqualTo(secondGroupUrl).IgnoreCase);
		}

		[Test]
		public void ScriptUrlIsLazilyCached()
		{
			_registry.Require(myScript);
			var resolvedScriptPath1 = _registry.GetIncludes().Single();
			if(resolvedScriptPath1 != consolidatedScript)
				Assert.Inconclusive("The first call to GetScriptUrl did not return the expected result");
			_context.ScriptGroups.Clear();
			_registry.Require(myScript);
			var resolvedScriptPath2 = _registry.GetIncludes().Single();

			resolvedScriptPath2.ShouldEqual(resolvedScriptPath1);
		}

		[Test]
		public void WhenGroupUrlIsPassedIntoGetResourceUrlAndConsolidationIsEnabled_GroupUrlIsReturned()
		{
			_groupElement.Include.AddPattern("~/Scripts/.+");

			var secondGroup = new ScriptGroupElement();
			secondGroup.ConsolidatedUrl = "~/Scripts/Consolidated/SecondGroup.js";
			_context.ScriptGroups.Add(secondGroup);
			
			_registry.Require(secondGroup.ConsolidatedUrl);
			var resolvedScriptPath = _registry.GetIncludes().Single();
			
			resolvedScriptPath.ShouldEqual(secondGroup.ConsolidatedUrl);
		}

		[Test]
		public void WhenGroupUrlIsRequiredThatMatchesThePatternOfAPreviousGroup_TheGroupUrlIsReturned()
		{
			var anotherGroup = new ScriptGroupElement();
			anotherGroup.ConsolidatedUrl = "~/another_consolidated_url.js";
			anotherGroup.Include.AddPath(myScript);
		    _context.ScriptGroups.Add(anotherGroup);
			_groupElement.Include.AddPattern("another_consolidated_url");

			_registry.Require(anotherGroup.ConsolidatedUrl);
			var resolvedScriptPath = _registry.GetIncludes().Single();

			resolvedScriptPath.ShouldEqual(anotherGroup.ConsolidatedUrl);
		}

		[Test]
		public void WhenGroupUrlIsRequiredAndConsolidationForThatGroupIsDisabled_PathToEachResourceInGroupIsReturned()
		{
			_groupElement.Include.AddPath(myScript);
			_groupElement.Include.AddPath(mySecondScript);
			_groupElement.Consolidate = ResourceModeCondition.Never;

			_registry.Require(consolidatedScript);
			var resolvedScriptPaths = _registry.GetIncludes().ToList();

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
			_groupElement.Consolidate = ResourceModeCondition.Never;

			_registry.Require(consolidatedScript);
			var resolvedScriptPaths = _registry.GetIncludes().ToList();

			resolvedScriptPaths.CountShouldEqual(2);
			resolvedScriptPaths[0].ShouldEqual(mySecondScript);
			resolvedScriptPaths[1].ShouldEqual(myScript);
		}
	}
}