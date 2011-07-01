using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Assman
{
	public class AspNetShortLivedResourceCache : IResourceCache
	{
		public const string QueryStringKey = "rid";
		private static readonly object _resourceCacheMutex = new object();
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
				lock(_resourceCacheMutex)
				{
					innerCache = httpContext.Cache.Get(aspNetCacheKey) as IResourceCache;
					if (innerCache == null)
					{
						innerCache = new InMemoryThreadSafeResourceCache();
						httpContext.Cache.Insert(aspNetCacheKey, innerCache, null, DateTime.Now.AddMinutes(2),
												 Cache.NoSlidingExpiration);
					}
				}
			}

			return innerCache;
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