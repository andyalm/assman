using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public class StaticResourceGroup : IResourceGroup
	{
		private readonly ResourceCollection _resources;

		public StaticResourceGroup(string consolidatedUrl, IEnumerable<IResource> resources)
		{
			ConsolidatedUrl = consolidatedUrl;
			_resources = resources.ToResourceCollection();
		}

		public string ConsolidatedUrl { get; private set; }

		public bool Compress { get; set; }

		public IEnumerable<IResource> GetResources()
		{
			return _resources;
		}

		public bool IsMatch(IResource resource)
		{
			return _resources.Contains(resource);
		}
	}
}