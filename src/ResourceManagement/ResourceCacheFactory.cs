using System;
using System.Web;

namespace AlmWitt.Web.ResourceManagement
{
	public static class ResourceCacheFactory
	{
		public static IResourceCache GetCache()
		{
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