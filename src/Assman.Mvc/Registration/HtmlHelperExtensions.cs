using System;
using System.Web.Mvc;

using Assman.Registration;

namespace Assman.Mvc.Registration
{
	//NOTE: Many of these helper methods should have a void return value.  However, I have made them return a DummyStringResult
	//because some ViewEngines (*cough* Razor *cough*) have a very ugly one line statement syntax.  Making them return a dummy
	//return value allows you to call the helper methods with either expression or statement syntax.
	public static class HtmlHelperExtensions
	{	
		public static DummyStringResult RequireScript(this HtmlHelper html, string virtualPath)
		{
			html.ScriptRegistry().Require(virtualPath);
			return DummyStringResult.Instance;
		}

		public static DummyStringResult RequireScript(this HtmlHelper html, string virtualPath, string registryName)
		{
			html.ScriptRegistry(registryName).Require(virtualPath);
			return DummyStringResult.Instance;
		}

		public static DummyStringResult RequireStylesheet(this HtmlHelper html, string virtualPath)
		{
			html.StyleRegistry().Require(virtualPath);
			return DummyStringResult.Instance;
		}

		public static DummyStringResult RequireStylesheet(this HtmlHelper html, string virtualPath, string registryName)
		{
			html.StyleRegistry(registryName).Require(virtualPath);
			return DummyStringResult.Instance;
		}

		public static DummyStringResult RenderScripts(this HtmlHelper html)
		{
			html.RenderScripts(ResourceRegistryConfiguration.DefaultRegistryName);
			return DummyStringResult.Instance;
		}

		public static DummyStringResult RenderScripts(this HtmlHelper html, string registryName)
		{
			var renderer = html.ResourceRegistries().ScriptRenderer(registryName);
			renderer.Render(html.ViewContext.Writer);
			return DummyStringResult.Instance;
		}

		public static DummyStringResult RenderStyles(this HtmlHelper html)
		{
			html.RenderStyles(ResourceRegistryConfiguration.DefaultRegistryName);

			return DummyStringResult.Instance;
		}

		public static DummyStringResult RenderStyles(this HtmlHelper html, string registryName)
		{
			var renderer = html.ResourceRegistries().StyleRenderer(registryName);
			renderer.Render(html.ViewContext.Writer);

			return DummyStringResult.Instance;
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
	}
}