using System;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.UnitTests.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
	[TestFixture]
	public class TestResourceManagementConfiguration
	{
		private ResourceManagementConfiguration _instance;
		private const string myScript = "myscript.js";
		private const string mySecondScript = "mysecondscript.js";
		private const string consolidatedScript = "~/consolidated.jsx";
		private const string consolidatedScriptStatic = "~/consolidated.js";
		private const string excludedScript = "excluded.js";
        private string embeddedScript;

		[SetUp]
		public void Init()
		{
			_instance = new ResourceManagementConfiguration();
			_instance.ClientScripts.Consolidate = true;
			_instance.ClientScripts.Add(new ClientScriptGroupElement());
			_instance.ClientScripts[0].ConsolidatedUrl = consolidatedScript;
			_instance.ClientScripts[0].Exclude.Add(excludedScript);

		    string embeddedNs = "AlmWitt.Web.ResourceManagement.UnitTests.EmbeddedResources";
		    embeddedScript = EmbeddedResource.GetVirtualPath(this.GetType().Assembly.GetName().Name, embeddedNs + ".Embedded1.js");
		}
		
		[Test]
		public void ReturnsConsolidatedScriptFileWhenScriptConsolidationEnabled()
		{
			_instance.Consolidate = true;
			_instance.ClientScripts.Consolidate = true;
			
			string scriptToInclude = _instance.GetScriptUrl(myScript);

            Assert.That(scriptToInclude, Is.EqualTo(consolidatedScript).IgnoreCase);
		}

		[Test]
		public void ReturnsSelfWhenScriptConsolidationDisabled()
		{
			_instance.Consolidate = false;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

            Assert.That(scriptToInclude, Is.EqualTo(myScript));
		}

[Test]
		public void AppendsVersionNumberToConsolidatedUrl()
		{
			const string version = "2";
			_instance.EnableConsolidation = true;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.AreEqual(consolidatedScript + "?v=" + version, scriptToInclude);
		}

		[Test]
		public void AppendsVersionNumberToUnConsolidatedUrl()
		{
			const string version = "2";
			_instance.EnableConsolidation = false;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.AreEqual(myScript + "?v=" + version, scriptToInclude);
		}

		[Test]
		public void VersionParameterIsUrlEncoded()
		{
			const string version = "2 2";
			_instance.EnableConsolidation = true;
			_instance.Version = version;
			string scriptToInclude = _instance.GetScriptUrl(myScript);

			Assert.AreEqual(consolidatedScript + "?v=" + HttpUtility.UrlEncode(version), scriptToInclude);
		}


		[Test]
		public void NeverReturnsConsolidatedScriptFileWhenScriptIsExcluded()
		{
			_instance.Consolidate = true;
			string scriptToInclude = _instance.GetScriptUrl(excludedScript);

            Assert.That(scriptToInclude, Is.EqualTo(excludedScript));
		}

		[Test]
		public void ExclusionsAreCaseInsensitive()
		{
			_instance.Consolidate = true;
			string scriptToInclude = _instance.GetScriptUrl(excludedScript.ToUpperInvariant());

            Assert.That(scriptToInclude, Is.EqualTo(excludedScript).IgnoreCase);
		}

		[Test]
		public void FilesInSecondGroupAndNotInFirstReturnSecondGroupUrl()
		{
			const string secondGroupUrl = "~/mysecondconsolidation.jsx";
			_instance.Consolidate = true;
			_instance.ClientScripts[0].Exclude.Add(mySecondScript);
			_instance.ClientScripts.Add(new ClientScriptGroupElement());
			_instance.ClientScripts[1].ConsolidatedUrl = secondGroupUrl;

			string scriptToInclude = _instance.GetScriptUrl(mySecondScript);

            Assert.That(scriptToInclude, Is.EqualTo(secondGroupUrl).IgnoreCase);
		}

		[Test]
		public void ConsolidatedUrlIsStaticWhenPreConsolidated()
		{
			_instance.Consolidate = true;
			_instance.PreConsolidated = true;
			_instance.ClientScripts.Consolidate = true;

			string scriptToInclude = _instance.GetScriptUrl(myScript);

            Assert.That(scriptToInclude, Is.EqualTo(consolidatedScriptStatic).IgnoreCase);
		}

        [Test]
		public void WhenBuildingContext_EmbeddedResourcesCanBeFoundForConfiguredAssemblies()
		{
			_instance.Assemblies.Add(this.GetType().Assembly.GetName().Name);
			var fileFinder = ResourceFinderFactory.Null;
			var context = _instance.BuildContext(fileFinder);
			ResourceCollection resources = context.Finder.FindResources(ResourceType.Css);
			Assert.IsNotNull(resources);
			Assert.IsTrue(resources.Count > 0, "Resource count should be greater than zero.");
		}
	}
}
