using System;
using System.Web;

namespace Assman
{
	public static class ResourceCacheFactory
	{
		public static IResourceCache GetCache(ResourceMode resourceMode)
		{
			if(resourceMode == ResourceMode.Release)
				return new InMemoryThreadSafeResourceCache();
			else
				return new AspNetShortLivedResourceCache(GetHttpContext);
		}

		private static HttpContextBase GetHttpContext()
		{
			var httpContext = HttpContext.Current;
			if (httpContext == null)
				return null;

			return new HttpContextWrapper(httpContext);
		}
	}
}