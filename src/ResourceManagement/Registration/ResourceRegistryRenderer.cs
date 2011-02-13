using System;
using System.IO;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public abstract class ResourceRegistryRenderer
	{
		private readonly IReadableResourceRegistry _resourceRegistry;
		private readonly Func<string, string> _urlResolver;

		protected ResourceRegistryRenderer(IReadableResourceRegistry resourceRegistry, Func<string,string> urlResolver)
		{
			_resourceRegistry = resourceRegistry;
			_urlResolver = urlResolver;
		}

		public void RenderTo(TextWriter writer)
		{
			foreach (var includeUrl in _resourceRegistry.GetIncludes())
			{
				var resolvedUrl = _urlResolver(includeUrl);
				WriteIncludeTag(writer, resolvedUrl);
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
		public static ResourceRegistryRenderer Renderer(this IScriptRegistry scriptRegistry, Func<string,string> urlResolver)
		{
			return new ScriptRegistryRenderer(scriptRegistry.AsReadable(), urlResolver);
		}

		public static ResourceRegistryRenderer Renderer(this IStyleRegistry styleRegistry, Func<string,string> urlResolver)
		{
			return new StyleRegistryRenderer(styleRegistry.AsReadable(), urlResolver);
		}

		private class ScriptRegistryRenderer : ResourceRegistryRenderer
		{
			public ScriptRegistryRenderer(IReadableResourceRegistry resourceRegistry, Func<string,string> urlResolver) : base(resourceRegistry, urlResolver) {}
			
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
			public StyleRegistryRenderer(IReadableResourceRegistry resourceRegistry, Func<string,string> urlResolver) : base(resourceRegistry, urlResolver) {}

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