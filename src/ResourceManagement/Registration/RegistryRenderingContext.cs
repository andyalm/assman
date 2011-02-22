using System;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public class RegistryRenderingContext : IRegistryRenderingContext
	{
		private readonly IIncludeOrderingStrategy _orderingStrategy;
		private readonly Func<string, string> _resolveUrl;
		private readonly OrderingStategyContext _orderingStrategyContext;

		public RegistryRenderingContext(OrderingStategyContext orderingStrategyContext, IIncludeOrderingStrategy orderingStrategy, Func<string, string> resolveUrl)
		{
			_orderingStrategyContext = orderingStrategyContext;
			_orderingStrategy = orderingStrategy;
			_resolveUrl = resolveUrl;
		}

		public string ResolveUrl(string virtualPath)
		{
			return _resolveUrl(virtualPath);
		}

		public bool ShouldRenderInclude(string virtualPath)
		{
			return _orderingStrategy.ShouldRegistryRenderInclude(virtualPath, _orderingStrategyContext);
		}

		public void MarkIncludeAsRendered(string virtualPath)
		{
			_orderingStrategy.IncludeHasBeenRendered(virtualPath, _orderingStrategyContext);
		}
	}
}