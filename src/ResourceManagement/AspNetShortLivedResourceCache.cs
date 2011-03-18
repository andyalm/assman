using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace AlmWitt.Web.ResourceManagement
{
	public class AspNetShortLivedResourceCache : IResourceCache
	{
		private readonly Func<HttpContextBase> _getHttpContext;

		public AspNetShortLivedResourceCache(Func<HttpContextBase> getHttpContext)
		{
			_getHttpContext = getHttpContext;
		}

		public bool TryGetResources(ResourceType resourceType, out IEnumerable<IResource> cachedResources)
		{
			return GetInnerCache().TryGetResources(resourceType, out cachedResources);
		}

		public bool TryGetGroup(string consolidatedUrl, ResourceMode mode, out IResourceGroup cachedGroup)
		{
			return GetInnerCache().TryGetGroup(consolidatedUrl, mode, out cachedGroup);
		}

		public void StoreGroup(string consolidatedUrl, ResourceMode mode, IResourceGroup group)
		{
			GetInnerCache().StoreGroup(consolidatedUrl, mode, group);
		}

		public void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources)
		{
			GetInnerCache().StoreResources(resourceType, resources);
		}

		public string CurrentCacheKey
		{
			get
			{
				var httpContext = _getHttpContext();
				if (httpContext != null)
					return httpContext.GetResourceCacheKey();
				else
					return null;
			}
		}

		private IResourceCache GetInnerCache()
		{
			var httpContext = _getHttpContext();
			if (httpContext == null)
				return NullResourceCache.Instance;

			var aspNetCacheKey = httpContext.GetResourceCacheKey();

			var innerCache = httpContext.Cache.Get(aspNetCacheKey) as IResourceCache;
			if(innerCache == null)
			{
				innerCache = new InMemoryResourceCache();
				httpContext.Cache.Insert(aspNetCacheKey, innerCache, null, DateTime.Now.AddMinutes(2), Cache.NoSlidingExpiration);
			}

			return innerCache;
		}

		private class InMemoryResourceCache : IResourceCache
		{
			private readonly IDictionary<ResourceType,IEnumerable<IResource>> _resources = new Dictionary<ResourceType, IEnumerable<IResource>>();
			private readonly IDictionary<GroupKey, IResourceGroup> _groups = new Dictionary<GroupKey, IResourceGroup>();

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
				_resources[resourceType] = resources;
			}

			public void StoreGroup(string consolidatedUrl, ResourceMode mode, IResourceGroup group)
			{
				_groups[new GroupKey {ConsolidatedUrl = consolidatedUrl, Mode = mode}] = group;
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

	internal static class AspNetShortLivedCacheHttpContextExtensions
	{
		private static readonly object _httpItemsKey = typeof(AspNetShortLivedResourceCache);
		
		public static void SetResourceCacheKey(this HttpContextBase httpContext, string key)
		{
			httpContext.Items[_httpItemsKey] = key;
		}

		public static string GetResourceCacheKey(this HttpContextBase httpContext)
		{
			var key = httpContext.Items[_httpItemsKey] as string;
			if (key == null)
			{
				key = CreateNewAspNetCacheKey();
				httpContext.Items[_httpItemsKey] = key;
			}

			return key;
		}

		private static string CreateNewAspNetCacheKey()
		{
			return DateTime.Now.ToString("mmssff"); //unique string to the hundredths of a second
		}
	}
}