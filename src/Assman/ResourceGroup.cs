using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
{
	public class ResourceGroup : IResourceGroup
	{
		private readonly List<IResource> _resources;
		private readonly ResourceType _resourceType;

		public ResourceGroup(string consolidatedUrl, IEnumerable<IResource> resources)
		{
			ConsolidatedUrl = consolidatedUrl;
			_resources = resources.ToList();
			_resourceType = ResourceType.FromPath(ConsolidatedUrl);
		}

		public string ConsolidatedUrl { get; private set; }

		public bool Minify { get; set; }

		public ResourceType ResourceType
		{
			get { return _resourceType; }
		}

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