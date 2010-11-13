using System.Collections.Generic;

using AlmWitt.Web.ResourceManagement;

namespace AlmWitt.Web.ResourceManagement.TestObjects
{
	public class StubResourceGroup : IResourceGroup
	{
		public StubResourceGroup()
		{
			Resources = new ResourceCollection();
		}

		public string ConsolidatedUrl { get; set; }

		public bool Compress { get; set; }

		public ResourceType ResourceType { get; set; }

		public ResourceCollection Resources { get; private set; }

		public IEnumerable<IResource> GetResources()
		{
			return Resources;
		}

		public bool IsMatch(IResource resource)
		{
			return Resources.Contains(resource);
		}
	}
}