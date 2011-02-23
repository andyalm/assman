using System;
using System.Web;

using AlmWitt.Web.ResourceManagement.Configuration;

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
			_instance.ClientScriptGroups.Add(_groupElement);
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
			_instance.ClientScriptGroups.Add(secondGroupElement);
			secondGroupElement.ConsolidatedUrl = secondGroupUrl;

			string scriptToInclude = _instance.GetScriptUrl(mySecondScript);

			Assert.That(scriptToInclude, Is.EqualTo(secondGroupUrl).IgnoreCase);
		}

		[Test]
		public void ConsolidatedUrlIsStaticWhenPreConsolidated()
		{
			_instance.ConsolidateClientScripts = true;
			_instance.PreConsolidated = true;

			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.That(scriptToInclude, Is.EqualTo(consolidatedScriptStatic).IgnoreCase);
		}
	}
}