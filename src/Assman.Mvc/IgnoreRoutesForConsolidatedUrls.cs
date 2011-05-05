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
            routes.IgnoreRoute("{*consolidatedurls}", new { consolidatedurls = @".*(\.js|\.css)(/.*)?" });   
        }
	}
}