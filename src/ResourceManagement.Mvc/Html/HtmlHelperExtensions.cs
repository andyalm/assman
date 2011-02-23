using System;
using System.Web.Mvc;

using AlmWitt.Web.ResourceManagement.Registration;

namespace AlmWitt.Web.ResourceManagement.Mvc.Html
{
	public static class HtmlHelperExtensions
	{	
		public static void IncludeScript(this HtmlHelper html, string virtualPath)
		{
			html.ScriptRegistry().IncludePath(virtualPath);
		}

		public static void IncludeScript(this HtmlHelper html, string virtualPath, string registryName)
		{
			html.ScriptRegistry(registryName).IncludePath(virtualPath);
		}

		public static void IncludeStylesheet(this HtmlHelper html, string virtualPath)
		{
			html.StyleRegistry().IncludePath(virtualPath);
		}

		public static void IncludeStylesheet(this HtmlHelper html, string virtualPath, string registryName)
		{
			html.StyleRegistry(registryName).IncludePath(virtualPath);
		}

		public static void RenderScripts(this HtmlHelper html)
		{
			html.RenderScripts(ResourceRegistryConfiguration.DefaultRegistryName);
		}

		public static void RenderScripts(this HtmlHelper html, string registryName)
		{
			var renderer = html.ResourceRegistries().ScriptRenderer(registryName, html.Resolver());
			renderer.Render(html.ViewContext.Writer);
		}

		public static void RenderStyles(this HtmlHelper html)
		{
			html.RenderStyles(ResourceRegistryConfiguration.DefaultRegistryName);
		}

		public static void RenderStyles(this HtmlHelper html, string registryName)
		{
			var renderer = html.ResourceRegistries().StyleRenderer(registryName, html.Resolver());
			renderer.Render(html.ViewContext.Writer);
		}

		public static IResourceRegistry ScriptRegistry(this HtmlHelper html, string registryName)
		{
			return html.ResourceRegistries().NamedScriptRegistry(registryName);
		}

		public static IResourceRegistry ScriptRegistry(this HtmlHelper html)
		{
			return html.ResourceRegistries().ScriptRegistry;
		}

		public static IResourceRegistry StyleRegistry(this HtmlHelper html)
		{
			return html.ResourceRegistries().StyleRegistry;
		}

		public static IResourceRegistry StyleRegistry(this HtmlHelper html, string registryName)
		{
			return html.ResourceRegistries().NamedStyleRegistry(registryName);
		}

		public static IResourceRegistryAccessor ResourceRegistries(this HtmlHelper html)
		{
			return html.ViewContext.ResourceRegistries();
		}

		private static Func<string,string> Resolver(this HtmlHelper html)
		{
			var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
			Func<string, string> resolverDelegate = url =>
			{
				return ResourceIncludeResolver.Instance.ResolveUrl(urlHelper, url);
			};
			return resolverDelegate;
		}
	}
}