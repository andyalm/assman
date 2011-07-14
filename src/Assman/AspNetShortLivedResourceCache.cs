using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Assman
{
	public class AspNetShortLivedResourceCache : IResourceCache
	{
	    private static readonly object _resourceCacheMutex = new object();
		private readonly Func<HttpContextBase> _getHttpContext;
	    private const string CacheKey = "AssmanAspNetShortLivedCacheKey";

	    public AspNetShortLivedResourceCache(Func<HttpContextBase> getHttpContext)
		{
			_getHttpContext = getHttpContext;
		}

		public bool TryGetResources(ResourceType resourceType, out IEnumerable<IResource> cachedResources)
		{
			return GetInnerCache().TryGetResources(resourceType, out cachedResources);
		}

		public bool TryGetGroup(string consolidatedUrl, out IResourceGroup cachedGroup)
		{
			return GetInnerCache().TryGetGroup(consolidatedUrl, out cachedGroup);
		}

		public void StoreGroup(string consolidatedUrl, IResourceGroup group)
		{
			GetInnerCache().StoreGroup(consolidatedUrl, group);
		}

		public void StoreResources(ResourceType resourceType, IEnumerable<IResource> resources)
		{
			GetInnerCache().StoreResources(resourceType, resources);
		}

	    private IResourceCache GetInnerCache()
		{
			var httpContext = _getHttpContext();
			if (httpContext == null)
				return NullResourceCache.Instance;

		    var innerCache = httpContext.Cache.Get(CacheKey) as IResourceCache;
			if(innerCache == null)
			{
				lock(_resourceCacheMutex)
				{
					innerCache = httpContext.Cache.Get(CacheKey) as IResourceCache;
					if (innerCache == null)
					{
						innerCache = new InMemoryThreadSafeResourceCache();
						httpContext.Cache.Insert(CacheKey, innerCache, null, DateTime.Now.AddSeconds(15),
												 Cache.NoSlidingExpiration);
					}
				}
			}

			return innerCache;
		}
	}
}