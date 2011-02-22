using System;
using System.Collections.Generic;
using System.Web;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public static class ResourceRegistryConfiguration
	{
		public const string DefaultRegistryName = "Default";

		private static readonly IIncludeOrderingStrategy _defaultOrderingStrategy = new FirstRenderedStrategy();
		private static IIncludeOrderingStrategy _orderingStrategy;

		public static IIncludeOrderingStrategy OrderingStrategy
		{
			get { return _orderingStrategy ?? _defaultOrderingStrategy; }
			set { _orderingStrategy = value; }
		}
	}

	public class FirstRenderedStrategy : IIncludeOrderingStrategy
	{
		private static readonly Type _httpItemsKey = typeof (FirstRenderedStrategy);
		
		public bool ShouldRegistryRenderInclude(string virtualPath, OrderingStategyContext context)
		{
			var renderedUrls = GetRenderedUrls(context.HttpContext);

			return !renderedUrls.Contains(virtualPath);
		}

		public void IncludeHasBeenRendered(string virtualPath, OrderingStategyContext context)
		{
			var renderedUrls = GetRenderedUrls(context.HttpContext);

			if (!renderedUrls.Contains(virtualPath))
				renderedUrls.Add(virtualPath);
		}

		private HashSet<string> GetRenderedUrls(HttpContextBase httpContext)
		{
			return httpContext.Items.GetOrCreate<HashSet<string>>(_httpItemsKey, () => new HashSet<string>(StringComparer.OrdinalIgnoreCase));
		}
	}
}