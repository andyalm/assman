using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Assman.Mvc;
#if NET_40
[assembly: PreApplicationStartMethod(typeof(IgnoreRoutesForConsolidatedUrls), "IgnoreWebResourceRoutes")]
#endif
namespace Assman.Mvc
{
	public static class IgnoreRoutesForConsolidatedUrls
	{
		public static void IgnoreWebResourceRoutes()
		{
			RouteTable.Routes.IgnoreWebResourceRoutes();
		}

        public static void IgnoreWebResourceRoutes(this RouteCollection routes)
        {
            var autoIgnoreJsRoutes = ConfigurationManager.AppSettings.AsBool("Assman.AutoIgnoreJsRoutes", false);
            if(!autoIgnoreJsRoutes)
                routes.IgnoreRoute("{*scripturls}", new { scripturls = @".*(\.js)(/.*)?" });

            var autoIgnoreCssRoutes = ConfigurationManager.AppSettings.AsBool("Assman.AutoIgnoreCssRoutes", false);
            if (!autoIgnoreCssRoutes)
                routes.IgnoreRoute("{*stylesheeturls}", new { stylesheeturls = @".*(\.css)(/.*)?" });
        }

        private static bool AsBool(this NameValueCollection appSettings, string key, bool defaultValue)
        {
            var strValue = appSettings[key];
            if (strValue == null)
                return defaultValue;

            bool bValue;
            if (Boolean.TryParse(strValue, out bValue))
                return bValue;
            else
                return defaultValue;
        }
	}
}