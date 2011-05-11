using System;
using System.Linq;

using Assman.Configuration;
using Assman.PreConsolidation;
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
			var context = _instance.BuildContext(fileFinder, NullPreConsolidationPersister.Instance);
			var resources = context.Finder.FindResources(ResourceType.Css).ToList();
			Assert.IsNotNull(resources);
			Assert.IsTrue(resources.Count > 0, "Resource count should be greater than zero.");
		}

		[Test]
		public void WhenBuildingContext_LastModifiedIsSet()
		{
			var lastModified = DateTime.Now;
			_instance.LastModified(lastModified);

			var context = _instance.BuildContext(ResourceFinderFactory.Null, NullPreConsolidationPersister.Instance);

			context.ConfigurationLastModified.ShouldEqual(lastModified);
		}
	}
}
