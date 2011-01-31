using System.Linq;

using AlmWitt.Web.ResourceManagement.TestObjects;
using AlmWitt.Web.ResourceManagement.TestSupport;

using NUnit.Framework;

namespace AlmWitt.Web.ResourceManagement
{
	[TestFixture]
	public class TestResourceType
	{
		[Test]
		public void WhenFileExtensionIsAlreadyAdded_ItIsNotAddedAgain()
		{
			var resourceType = new StubResourceType("test/plain", ".txt");
			resourceType.AddFileExtension(".doc");
			resourceType.AddFileExtension(".doc");

			resourceType.FileExtensions.ElementAt(0).ShouldEqual(".txt");
			resourceType.FileExtensions.ElementAt(1).ShouldEqual(".doc");
			resourceType.FileExtensions.Count().ShouldEqual(2);
		}
	}
}