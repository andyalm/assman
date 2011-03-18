using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceCache
	{
		string CurrentCacheKey { get; }
		bool TryGetResources(ResourceType resourceType, out IEnumerable<IResource> cachedResources);
		bool TryGetGroup(string consolidatedUrl, ResourceMode mode, out IResourceGroup cachedGroup);
		void StoreGroup(string consolidatedUrl, ResourceMode mode, IResourceGroup @group);
		void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources);
	}

	public static class ResourceCacheExtensions
	{
		public static IResourceGroup GetOrAddGroup(this IResourceCache resourceCache, string consolidatedUrl, ResourceMode mode, Func<IResourceGroup> getGroup)
		{
			IResourceGroup group;
			if (resourceCache.TryGetGroup(consolidatedUrl, mode, out group))
				return group;

			group = getGroup();

			if(group != null)
			{
				resourceCache.StoreGroup(consolidatedUrl, mode, group);
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

		public bool TryGetGroup(string consolidatedUrl, ResourceMode mode, out IResourceGroup cachedGroup)
		{
			cachedGroup = null;
			return false;
		}

		public void StoreGroup(string consolidatedUrl, ResourceMode mode, IResourceGroup group) {}
		public void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources) {}

		public string CurrentCacheKey
		{
			get { return null; }
		}
	}
}