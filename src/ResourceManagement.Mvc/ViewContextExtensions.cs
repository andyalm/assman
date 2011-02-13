using System;
using System.Web.Mvc;

using AlmWitt.Web.ResourceManagement.Registration;

namespace AlmWitt.Web.ResourceManagement.Mvc
{
	public static class ViewContextExtensions
	{
		public static IResourceRegistryAccessor ResourceRegistries(this ViewContext viewContext)
		{
			var key = "__ResourceRegistries" + viewContext.View.GetHashCode();
			var resourceRegistries = viewContext.HttpContext.Items[key] as IResourceRegistryAccessor;
			if (resourceRegistries == null)
			{
				resourceRegistries = new GenericResourceRegistryAccessor().UseConsolidation();
				viewContext.HttpContext.Items[key] = resourceRegistries;
			}

			return resourceRegistries;
		}
	}
}