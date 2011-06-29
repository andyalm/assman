using System;
using System.Collections.Generic;
using System.Linq;

using Assman.Configuration;
using Assman.PreCompilation;
using Assman.TestSupport;

using NUnit.Framework;

namespace Assman
{
	[TestFixture]
	public class TestAssmanConfiguration
	{
		private AssmanConfiguration _instance;
		private const string consolidatedScript = "~/consolidated.js";
		private const string excludedScript = "excluded.js";

		[SetUp]
		public void Init()
		{
			_instance = new AssmanConfiguration();
			_instance.Scripts.Groups.Consolidate = true;
			_instance.Scripts.Groups.Add(new ScriptGroupElement());
			_instance.Scripts.Groups[0].ConsolidatedUrl = consolidatedScript;
			_instance.Scripts.Groups[0].Exclude.AddPattern(excludedScript);
			_instance.LastModified(DateTime.MinValue);
		}

		[Test]
		public void WhenBuildingContext_EmbeddedResourcesCanBeFoundForConfiguredAssemblies()
		{
			_instance.Assemblies.Add(this.GetType().Assembly.GetName().Name);
			var fileFinder = ResourceFinderFactory.Null;
			var context = _instance.BuildContext(ResourceMode.Debug, fileFinder, NullPreCompiledPersister.Instance);
			var resources = context.Finder.FindResources(ResourceType.Stylesheet).ToList();
			Assert.IsNotNull(resources);
			Assert.IsTrue(resources.Count > 0, "Resource count should be greater than zero.");
		}

		[Test]
		public void WhenBuildingContext_LastModifiedIsSet()
		{
			var lastModified = DateTime.Now;
			_instance.LastModified(lastModified);

			var context = _instance.BuildContext(ResourceMode.Debug, ResourceFinderFactory.Null, NullPreCompiledPersister.Instance);

			context.ConfigurationLastModified.ShouldEqual(lastModified);
		}

		[Test]
		public void WhenBuildingContext_GlobalDependenciesAreAddedToGroupManager()
		{
			_instance.Scripts.GlobalDependencies.Add("~/scripts/shared.js");
			_instance.Scripts.GlobalDependencies.Add("~/scripts/otherglobal.js");

			var context = _instance.BuildContext(ResourceMode.Debug, ResourceFinderFactory.Null, NullPreCompiledPersister.Instance);

			List<string> globalDependencies = context.ScriptGroups.GetGlobalDependencies().ToList();
			globalDependencies.CountShouldEqual(2);
			globalDependencies[0].ShouldEqual("~/scripts/shared.js");
			globalDependencies[1].ShouldEqual("~/scripts/otherglobal.js");
		}
	}
}
