using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman.TestSupport
{
	public class StubResourceGroup : IResourceGroup
	{
		private readonly List<IResource> _resources = new List<IResource>();

		public StubResourceGroup(string consolidatedUrl)
		{
			ConsolidatedUrl = consolidatedUrl;
			ResourceType = ResourceType.FromPath(consolidatedUrl);
		}

		public string ConsolidatedUrl { get; private set; }
		public bool Minify { get; private set; }
		public ResourceType ResourceType { get; private set; }
		public bool Contains(IResource resource)
		{
			return _resources.Any(r => r.VirtualPath.Equals(resource.VirtualPath, StringComparison.OrdinalIgnoreCase));
		}

		public void AddResource(IResource resource)
		{
			_resources.Add(resource);
		}

		public IEnumerable<IResource> GetResources()
		{
			return _resources;
		}
	}
}