using System;

using AlmWitt.Web.ResourceManagement.UnitTests.TestObjects;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement.UnitTests
{
	[TestFixture]
	public class TestConsolidatedResource
	{
		[Test]
		public void LastModifiedEqualsLastModifiedOfCollection()
		{
			ResourceCollection resources = new ResourceCollection();
			resources.Add(CreateResource(DateTime.Now));
			resources.Add(CreateResource(DateTime.Now.AddDays(-1)));

			ConsolidatedResource consolidated = ConsolidatedResource.FromResources(resources);
			Assert.That(consolidated.LastModified, Is.EqualTo(resources.LastModified));
		}

		private IResource CreateResource(DateTime lastModified)
		{
			StubResource resource = new StubResource(String.Empty);
			resource.LastModified = lastModified;

			return resource;
		}
	}
}