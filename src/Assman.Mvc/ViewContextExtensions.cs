using System;
using System.Web.Mvc;

using Assman.Registration;

namespace Assman.Mvc
{
    public static class ViewContextExtensions
    {
        public static IResourceRegistryAccessor ResourceRegistries(this ViewContext viewContext)
        {
            //generate the key off of the CurrentExecutionFilePath.  This should stay the same unless a Server.Transfer happens, in which
            //case, it will change and we will want to start clean anyways.  Originally we used the HashCode of the current view, but that
            //broke RenderAction.
            var key = "__ResourceRegistries" + viewContext.HttpContext.Request.CurrentExecutionFilePath;
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