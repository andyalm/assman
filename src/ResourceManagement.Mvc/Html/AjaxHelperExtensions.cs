using System.Web.Mvc;

namespace AlmWitt.Web.ResourceManagement.Mvc.Html
{
	public static class AjaxHelperExtensions
	{
		public static IResourceRegistry ScriptRegistry(this AjaxHelper ajax, string registryName)
		{
			return ajax.ResourceRegistries().NamedScriptRegistry(registryName);
		}

		public static IResourceRegistry ScriptRegistry(this AjaxHelper ajax)
		{
			return ajax.ResourceRegistries().ScriptRegistry;
		}

		public static IResourceRegistry StyleRegistry(this AjaxHelper ajax)
		{
			return ajax.ResourceRegistries().StyleRegistry;
		}

		public static IResourceRegistry StyleRegistry(this AjaxHelper ajax, string registryName)
		{
			return ajax.ResourceRegistries().NamedStyleRegistry(registryName);
		}

		public static IResourceRegistryAccessor ResourceRegistries(this AjaxHelper ajax)
		{
			return ajax.ViewContext.ResourceRegistries();
		}
	}
}