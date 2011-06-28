using System;
using System.Collections.Generic;

namespace Assman
{
    internal class InMemoryThreadSafeResourceCache : IResourceCache
    {
        private readonly IThreadSafeInMemoryCache<ResourceType, IEnumerable<IResource>> _resources = new ThreadSafeInMemoryCache<ResourceType, IEnumerable<IResource>>();
        private readonly IThreadSafeInMemoryCache<GroupKey, IResourceGroup> _groups = new ThreadSafeInMemoryCache<GroupKey, IResourceGroup>();

        public string CurrentCacheKey
        {
            get { return null; }
        }

        public bool TryGetResources(ResourceType resourceType, out IEnumerable<IResource> cachedResources)
        {
            return _resources.TryGetValue(resourceType, out cachedResources);
        }

        public bool TryGetGroup(string consolidatedUrl, ResourceMode mode, out IResourceGroup cachedGroup)
        {
            return _groups.TryGetValue(new GroupKey {ConsolidatedUrl = consolidatedUrl, Mode = mode}, out cachedGroup);
        }

        public void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources)
        {
            _resources.Set(resourceType, resources);
        }

        public void StoreGroup(string consolidatedUrl, ResourceMode mode, IResourceGroup group)
        {
            _groups.Set(new GroupKey {ConsolidatedUrl = consolidatedUrl, Mode = mode}, group);
        }

        private class GroupKey
        {
            public string ConsolidatedUrl { get; set; }
            public ResourceMode Mode { get; set; }

            #region ReSharper Generated

            public bool Equals(GroupKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.ConsolidatedUrl, ConsolidatedUrl) && Equals(other.Mode, Mode);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(GroupKey)) return false;
                return Equals((GroupKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ConsolidatedUrl.GetHashCode() * 397) ^ Mode.GetHashCode();
                }
            }

            #endregion

            public override string ToString()
            {
                return ConsolidatedUrl + "|" + Mode;
            }
        }
    }
}