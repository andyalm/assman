using System.Collections.Generic;

namespace Assman
{
    public class CachingResourceFinder : IResourceFinder
    {
        private readonly IResourceCache _cache;
        private readonly IResourceFinder _inner;

        public CachingResourceFinder(IResourceCache cache, IResourceFinder inner)
        {
            _cache = cache;
            _inner = inner;
        }

        public IEnumerable<IResource> FindResources(ResourceType resourceType)
        {
            return _cache.GetOrAddResources(resourceType, () => _inner.FindResources(resourceType));
        }

        public IResource FindResource(string virtualPath)
        {
            //no need to cache for now
            return _inner.FindResource(virtualPath);
        }
    }
}