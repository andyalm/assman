using System;
using System.Collections.Generic;

namespace Assman
{
    internal class InMemoryThreadSafeResourceCache : IResourceCache
    {
        private readonly IThreadSafeInMemoryCache<ResourceType, IEnumerable<IResource>> _resources = new ThreadSafeInMemoryCache<ResourceType, IEnumerable<IResource>>();
        private readonly IThreadSafeInMemoryCache<string, IResourceGroup> _groups = new ThreadSafeInMemoryCache<string, IResourceGroup>(Comparers.VirtualPath);

        public bool TryGetResources(ResourceType resourceType, out IEnumerable<IResource> cachedResources)
        {
            return _resources.TryGetValue(resourceType, out cachedResources);
        }

        public bool TryGetGroup(string consolidatedUrl, out IResourceGroup cachedGroup)
        {
            return _groups.TryGetValue(consolidatedUrl, out cachedGroup);
        }

        public void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources)
        {
            _resources.Set(resourceType, resources);
        }

        public void StoreGroup(string consolidatedUrl, IResourceGroup group)
        {
            _groups.Set(consolidatedUrl, group);
        }
    }
}