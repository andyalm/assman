using System;

using AlmWitt.Web.ResourceManagement.Configuration;
using AlmWitt.Web.ResourceManagement.PreConsolidation;
using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceManagementConfiguration
	{
		private ResourceManagementConfiguration _instance;
		private const string consolidatedScript = "~/consolidated.jsx";
		private const string excludedScript = "excluded.js";

		[SetUp]
		public void Init()
		{
			_instance = new ResourceManagementConfiguration();
			_instance.ClientScripts.Consolidate = true;
			_instance.ClientScripts.Add(new ClientScriptGroupElement());
			_instance.ClientScripts[0].ConsolidatedUrl = consolidatedScript;
			_instance.ClientScripts[0].Exclude.AddPattern(excludedScript);
			_instance.LastModified(DateTime.MinValue);
		}

        [Test]
		public void WhenBuildingContext_EmbeddedResourcesCanBeFoundForConfiguredAssemblies()
		{
			_instance.Assemblies.Add(this.GetType().Assembly.GetName().Name);
			var fileFinder = ResourceFinderFactory.Null;
			var context = _instance.BuildContext(fileFinder, NullPreConsolidationPersister.Instance);
			ResourceCollection resources = context.Finder.FindResources(ResourceType.Css);
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
