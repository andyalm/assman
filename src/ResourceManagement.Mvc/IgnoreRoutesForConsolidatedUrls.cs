using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using AlmWitt.Web.ResourceManagement.Mvc;
#if NET_40
[assembly: PreApplicationStartMethod(typeof(IgnoreRoutesForConsolidatedUrls), "IgnoreWebResourceRoutes")]
#endif
namespace AlmWitt.Web.ResourceManagement.Mvc
{
	public static class IgnoreRoutesForConsolidatedUrls
	{
		public static void IgnoreWebResourceRoutes()
		{
			RouteTable.Routes.IgnoreWebResourceRoutes();
		}

        public static void IgnoreWebResourceRoutes(this RouteCollection routes)
        {
            routes.IgnoreRoute("{*consolidatedurls}", new { consolidatedurls = @".*(\.jsx|\.cssx|\.js|\.css)(/.*)?" });   
        }
	}
}