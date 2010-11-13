using System;
using System.IO;

using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
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

			ConsolidatedResource consolidated = new ConsolidatedResource(resources, new MemoryStream());
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
