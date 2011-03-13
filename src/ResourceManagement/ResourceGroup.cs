using System.Collections.Generic;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	public class ResourceGroup : IResourceGroup
	{
		private readonly List<IResource> _resources;

		public ResourceGroup(string consolidatedUrl, IEnumerable<IResource> resources)
		{
			ConsolidatedUrl = consolidatedUrl;
			_resources = resources.ToList();
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