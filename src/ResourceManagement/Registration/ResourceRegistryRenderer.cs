using System;
using System.IO;
using System.Linq;
using System.Web;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public abstract class ResourceRegistryRenderer
	{
		private readonly IReadableResourceRegistry _resourceRegistry;
		private readonly IRegistryRenderingContext _renderingContext;

		protected ResourceRegistryRenderer(IReadableResourceRegistry resourceRegistry, IRegistryRenderingContext renderingContext)
		{
			_resourceRegistry = resourceRegistry;
			_renderingContext = renderingContext;
		}

		public void RenderTo(TextWriter writer)
		{
			foreach (var includeUrl in _resourceRegistry.GetIncludes())
			{
				if(_renderingContext.ShouldRenderInclude(includeUrl))
				{
					var resolvedUrl = _renderingContext.ResolveUrl(includeUrl);
					WriteIncludeTag(writer, resolvedUrl);
					_renderingContext.MarkIncludeAsRendered(includeUrl);
				}
			}

			var scriptBlocks = _resourceRegistry.GetInlineBlocks();
			if (scriptBlocks.Any())
			{
				WriteBeginBlock(writer);
				foreach (var scriptBlock in scriptBlocks)
				{
					scriptBlock(writer);
				}
				WriteEndBlock(writer);
			}
		}

		protected abstract void WriteIncludeTag(TextWriter writer, string url);
		protected abstract void WriteBeginBlock(TextWriter writer);
		protected abstract void WriteEndBlock(TextWriter writer);
	}

	public static class ResourceRegistryRendererExtensions
	{
		public static ResourceRegistryRenderer ScriptRenderer(this IResourceRegistryAccessor registryAccessor, string registryName, HttpContextBase httpContext, Func<string,string> urlResolver)
		{
			var orderingStrategyContext = new OrderingStategyContext(registryName, registryAccessor.NamedScriptRegistry, httpContext);

			var renderingContext = new RegistryRenderingContext(orderingStrategyContext,
			                                                    ResourceRegistryConfiguration.OrderingStrategy, urlResolver);
			
			return new ScriptRegistryRenderer(orderingStrategyContext.CurrentRegistry.AsReadable(), renderingContext);
		}

		public static ResourceRegistryRenderer StyleRenderer(this IResourceRegistryAccessor registryAccessor, string registryName, HttpContextBase httpContext, Func<string,string> urlResolver)
		{
			var orderingStrategyContext = new OrderingStategyContext(registryName, registryAccessor.NamedStyleRegistry, httpContext);

			var renderingContext = new RegistryRenderingContext(orderingStrategyContext,
																ResourceRegistryConfiguration.OrderingStrategy, urlResolver);

			return new StyleRegistryRenderer(orderingStrategyContext.CurrentRegistry.AsReadable(), renderingContext);
		}
		
		private class ScriptRegistryRenderer : ResourceRegistryRenderer
		{
			public ScriptRegistryRenderer(IReadableResourceRegistry resourceRegistry, RegistryRenderingContext renderingContext) : base(resourceRegistry, renderingContext) {}
			
			protected override void WriteIncludeTag(TextWriter writer, string url)
			{
				writer.Write("<script type=\"text/javascript\" src=\"{0}\"></script>", url);
			}

			protected override void WriteBeginBlock(TextWriter writer)
			{
				writer.Write("<script type=\"text/javascript\">");
			}
			protected override void WriteEndBlock(TextWriter writer)
			{
				writer.Write("</script>");
			}
		}

		private class StyleRegistryRenderer : ResourceRegistryRenderer
		{
			public StyleRegistryRenderer(IReadableResourceRegistry resourceRegistry, RegistryRenderingContext renderingContext) : base(resourceRegistry, renderingContext) { }

			protected override void WriteIncludeTag(TextWriter writer, string url)
			{
				writer.Write("<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}\"></link>", url);
			}
			protected override void WriteBeginBlock(TextWriter writer)
			{
				writer.Write("<style type=\"text/css\">");
			}
			protected override void WriteEndBlock(TextWriter writer)
			{
				writer.Write("</style>");
			}
		}
	}
}