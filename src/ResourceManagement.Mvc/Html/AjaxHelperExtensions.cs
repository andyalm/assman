using System.Web.Mvc;

namespace AlmWitt.Web.ResourceManagement.Mvc.Html
{
	public static class AjaxHelperExtensions
	{
		public static IScriptRegistry ScriptRegistry(this AjaxHelper ajax, string registryName)
		{
			return ajax.ResourceRegistries().NamedScriptRegistry(registryName);
		}

		public static IScriptRegistry ScriptRegistry(this AjaxHelper ajax)
		{
			return ajax.ResourceRegistries().ScriptRegistry;
		}

		public static IStyleRegistry StyleRegistry(this AjaxHelper ajax)
		{
			return ajax.ResourceRegistries().StyleRegistry;
		}

		public static IStyleRegistry StyleRegistry(this AjaxHelper ajax, string registryName)
		{
			return ajax.ResourceRegistries().NamedStyleRegistry(registryName);
		}

		public static IResourceRegistryAccessor ResourceRegistries(this AjaxHelper ajax)
		{
			return ajax.ViewContext.ResourceRegistries();
		}
	}
}