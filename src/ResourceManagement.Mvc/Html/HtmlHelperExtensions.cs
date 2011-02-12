using System.Linq;
using System.Web.Mvc;

using AlmWitt.Web.ResourceManagement.Registration;

namespace AlmWitt.Web.ResourceManagement.Mvc.Html
{
	public static class HtmlHelperExtensions
	{	
		public static void IncludeScript(this HtmlHelper html, string virtualPath)
		{
			html.ScriptRegistry().IncludeUrl(virtualPath);
		}

		public static void RenderScripts(this HtmlHelper html)
		{
			var scriptRegistry = html.ScriptRegistry().AsReadable();
			RenderScripts(html, scriptRegistry);
		}

		public static void RenderScripts(this HtmlHelper html, string registryName)
		{
			var scriptRegistry = html.ScriptRegistry(registryName).AsReadable();
			RenderScripts(html, scriptRegistry);
		}

		public static void RenderStyles(this HtmlHelper html)
		{
			var styleRegistry = html.StyleRegistry().AsReadable();
			RenderStyles(html, styleRegistry);
		}

		private static void RenderScripts(HtmlHelper html, IReadableResourceRegistry scriptRegistry)
		{
			var url = new UrlHelper(html.ViewContext.RequestContext);
			var writer = html.ViewContext.Writer;
			foreach(var includeUrl in scriptRegistry.GetIncludes())
			{
				var scriptTag = GetScriptTag();
				scriptTag.Attributes["src"] = url.Content(includeUrl);
				scriptTag.WriteTo(writer);
			}

			var scriptBlocks = scriptRegistry.GetInlineBlocks();
			if(scriptBlocks.Any())
			{
				var scriptTag = GetScriptTag();
				scriptTag.WriteStartTag(writer);
				foreach (var scriptBlock in scriptBlocks)
				{
					scriptBlock(writer);
				}
				scriptTag.WriteEndTag(writer);
			}
		}

		private static void RenderStyles(HtmlHelper html, IReadableResourceRegistry styleRegistry)
		{
			var url = new UrlHelper(html.ViewContext.RequestContext);
			var writer = html.ViewContext.Writer;
			foreach (var includeUrl in styleRegistry.GetIncludes())
			{
				var linkTag = new TagBuilder("link");
				linkTag.Attributes["rel"] = "Stylesheet";
				linkTag.Attributes["type"] = "text/css";
				linkTag.Attributes["href"] = url.Content(includeUrl);
				linkTag.WriteTo(writer);
			}

			var styleBlocks = styleRegistry.GetInlineBlocks();
			if(styleBlocks.Any())
			{
				var styleTag = new TagBuilder("style");
				styleTag.Attributes["type"] = "text/css";
				styleTag.WriteStartTag(writer);
				foreach (var styleBlock in styleBlocks)
				{
					styleBlock(writer);
				}
				styleTag.WriteEndTag(writer);
			}
		}

		public static IScriptRegistry ScriptRegistry(this HtmlHelper html, string registryName)
		{
			return html.ResourceRegistries().NamedScriptRegistry(registryName);
		}

		public static IScriptRegistry ScriptRegistry(this HtmlHelper html)
		{
			return html.ResourceRegistries().ScriptRegistry;
		}

		public static IStyleRegistry StyleRegistry(this HtmlHelper html)
		{
			return html.ResourceRegistries().StyleRegistry;
		}

		public static IStyleRegistry StyleRegistry(this HtmlHelper html, string registryName)
		{
			return html.ResourceRegistries().NamedStyleRegistry(registryName);
		}

		public static IResourceRegistryAccessor ResourceRegistries(this HtmlHelper html)
		{
			var viewDataKey = "__ResourceRegistries" + html.ViewContext.View.GetHashCode();
			var resourceRegistries = html.ViewContext.ViewData[viewDataKey] as IResourceRegistryAccessor;
			if(resourceRegistries == null)
			{
				resourceRegistries = new GenericResourceRegistryAccessor().UseConsolidation();
				html.ViewContext.ViewData[viewDataKey] = resourceRegistries;
			}

			return resourceRegistries;
		}

		private static TagBuilder GetScriptTag()
		{
			var scriptTag = new TagBuilder("script");
			scriptTag.Attributes["type"] = "text/javascript";
			return scriptTag;
		}
	}
}