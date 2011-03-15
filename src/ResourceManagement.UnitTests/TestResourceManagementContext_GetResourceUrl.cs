using System;
using System.Collections.Generic;
using System.Web;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.PreConsolidation;
using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	public class TestResourceManagementContext_GetResourceUrl
	{
		private ResourceManagementContext _instance;
		private const string myScript = "myscript.js";
		private const string mySecondScript = "mysecondscript.js";
		private const string consolidatedScript = "~/consolidated.jsx";
		private const string consolidatedScriptStatic = "~/consolidated.js";
		private const string excludedScript = "excluded.js";
		private ClientScriptGroupElement _groupElement;

		[SetUp]
		public void Init()
		{
			_instance = new ResourceManagementContext();
			_instance.ConsolidateClientScripts = true;
			_instance.ConfigurationLastModified = DateTime.MinValue;
			_groupElement = new ClientScriptGroupElement();
			_groupElement.ConsolidatedUrl = consolidatedScript;
			_groupElement.Exclude.AddPattern(excludedScript);
			_instance.ScriptGroups.Add(_groupElement);
		}

		[Test]
		public void ReturnsConsolidatedScriptFileWhenScriptConsolidationEnabled()
		{
			_instance.ConsolidateClientScripts = true;

			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.That(scriptToInclude, Is.EqualTo(consolidatedScript).IgnoreCase);
		}

		[Test]
		public void ReturnsSelfWhenScriptConsolidationDisabled()
		{
			_instance.ConsolidateClientScripts = false;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.That(scriptToInclude, Is.EqualTo(myScript));
		}

		[Test]
		public void AppendsVersionNumberToConsolidatedUrl()
		{
			const string version = "2";
			_instance.ConsolidateClientScripts = true;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.AreEqual(consolidatedScript + "?v=" + version, scriptToInclude);
		}

		[Test]
		public void AppendsVersionNumberToUnConsolidatedUrl()
		{
			const string version = "2";
			_instance.ConsolidateClientScripts = false;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.AreEqual(myScript + "?v=" + version, scriptToInclude);
		}

		[Test]
		public void VersionParameterIsUrlEncoded()
		{
			const string version = "2 2";
			_instance.ConsolidateClientScripts = true;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.AreEqual(consolidatedScript + "?v=" + HttpUtility.UrlEncode(version), scriptToInclude);
		}


		[Test]
		public void NeverReturnsConsolidatedScriptFileWhenScriptIsExcluded()
		{
			_instance.ConsolidateClientScripts = true;
			string scriptToInclude = _instance.GetScriptUrl(excludedScript);

			Assert.That(scriptToInclude, Is.EqualTo(excludedScript));
		}

		[Test]
		public void ExclusionsAreCaseInsensitive()
		{
			_instance.ConsolidateClientScripts = true;
			string scriptToInclude = _instance.GetScriptUrl(excludedScript.ToUpperInvariant());

			Assert.That(scriptToInclude, Is.EqualTo(excludedScript).IgnoreCase);
		}

		[Test]
		public void FilesInSecondGroupAndNotInFirstReturnSecondGroupUrl()
		{
			const string secondGroupUrl = "~/mysecondconsolidation.jsx";
			_instance.ConsolidateClientScripts = true;
			_groupElement.Exclude.AddPattern(mySecondScript);
			var secondGroupElement = new ClientScriptGroupElement();
			_instance.ScriptGroups.Add(secondGroupElement);
			secondGroupElement.ConsolidatedUrl = secondGroupUrl;

			string scriptToInclude = _instance.GetScriptUrl(mySecondScript);

			Assert.That(scriptToInclude, Is.EqualTo(secondGroupUrl).IgnoreCase);
		}

		[Test]
		public void ConsolidatedUrlIsStaticWhenPreConsolidated()
		{
			_instance.ConsolidateClientScripts = true;
			var preConsolidationReport = new PreConsolidationReport
			{
				ClientScriptGroups = new List<PreConsolidatedResourceGroup>
				{
					new PreConsolidatedResourceGroup
					{
						ConsolidatedUrl = consolidatedScriptStatic,
						Resources = new List<string>
						{
							myScript
						}
					}
				}
			};
			_instance.LoadPreCompilationReport(preConsolidationReport);

			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.That(scriptToInclude, Is.EqualTo(consolidatedScriptStatic).IgnoreCase);
		}

		[Test]
		public void ScriptUrlIsLazilyCached()
		{
			var resolvedScriptPath1 = _instance.GetScriptUrl(myScript);
			if(resolvedScriptPath1 != consolidatedScript)
				Assert.Inconclusive("The first call to GetScriptUrl did not return the expected result");
			_instance.ScriptGroups.Clear();
			var resolvedScriptPath2 = _instance.GetScriptUrl(myScript);

			resolvedScriptPath2.ShouldEqual(resolvedScriptPath1);
		}

		[Test]
		public void WhenGroupUrlIsPassedIntoGetResourceUrlAndConsolidationIsEnabled_GroupUrlIsReturned()
		{
			_groupElement.Include.AddPath(myScript);
			_groupElement.Include.AddPath(mySecondScript);
			
			var resolvedScriptPath = _instance.GetScriptUrl(consolidatedScriptStatic);
			
			resolvedScriptPath.ShouldEqual(consolidatedScript);
		}
	}
}