using System;
using System.Collections.Generic;

namespace Assman
{
	public interface IResourceCache
	{
	    bool TryGetResources(ResourceType resourceType, out IEnumerable<IResource> cachedResources);
		bool TryGetGroup(string consolidatedUrl, out IResourceGroup cachedGroup);
		void StoreGroup(string consolidatedUrl, IResourceGroup @group);
		void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources);
	}

	public static class ResourceCacheExtensions
	{
		public static IResourceGroup GetOrAddGroup(this IResourceCache resourceCache, string consolidatedUrl, Func<IResourceGroup> getGroup)
		{
			IResourceGroup group;
			if (resourceCache.TryGetGroup(consolidatedUrl, out group))
				return group;

			group = getGroup();

			if(group != null)
			{
				resourceCache.StoreGroup(consolidatedUrl, group);
			}

			return group;
		}

		public static IEnumerable<IResource> GetOrAddResources(this IResourceCache resourceCache, ResourceType resourceType, Func<IEnumerable<IResource>> findResources)
		{
			IEnumerable<IResource> resources;
			if (resourceCache.TryGetResources(resourceType, out resources))
				return resources;

			resources = findResources();

			if(resources != null)
			{
				resourceCache.StoreResources(resourceType, resources);
			}

			return resources;
		}
	}

	public class NullResourceCache : IResourceCache
	{
		private static readonly IResourceCache _instance = new NullResourceCache();
		public static IResourceCache Instance
		{
			get { return _instance; }
		}
		
		private NullResourceCache() {}
		
		public bool TryGetResources(ResourceType resourceType, out IEnumerable<IResource> cachedResources)
		{
			cachedResources = null;
			return false;
		}

		public bool TryGetGroup(string consolidatedUrl, out IResourceGroup cachedGroup)
		{
			cachedGroup = null;
			return false;
		}

		public void StoreGroup(string consolidatedUrl, IResourceGroup group) {}
		public void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources) {}
	}
}