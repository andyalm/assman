using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using AlmWitt.Web.ResourceManagement.Mvc;
#if Net4
[assembly: PreApplicationStartMethod(typeof(IgnoreRoutesForConsolidatedUrls), "AddIgnoreRoute")]
#endif
namespace AlmWitt.Web.ResourceManagement.Mvc
{
	public static class IgnoreRoutesForConsolidatedUrls
	{
		public static void AddIgnoreRoute()
		{
			RouteTable.Routes.IgnoreRoute("{*consolidatedurls}", new { consolidatedurls = @".*(\.jsx|\.cssx)(/.*)?" });
		}
	}
}