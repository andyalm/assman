using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public class ResourceGroup : IResourceGroup
	{
		private readonly ResourceCollection _resources;

		public ResourceGroup(string consolidatedUrl, IEnumerable<IResource> resources)
		{
			ConsolidatedUrl = consolidatedUrl;
			_resources = resources.ToResourceCollection();
		}

		public string ConsolidatedUrl { get; private set; }

		public bool Minify { get; set; }

		public IEnumerable<IResource> GetResources()
		{
			return _resources;
		}

		public bool Contains(IResource resource)
		{
			return _resources.Contains(resource);
		}
	}
}