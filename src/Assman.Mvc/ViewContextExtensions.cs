using System;
using System.Web.Mvc;

using Assman.Registration;

namespace Assman.Mvc
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