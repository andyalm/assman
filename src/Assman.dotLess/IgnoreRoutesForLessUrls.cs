using System;
using System.Web;
using System.Web.Routing;

using Assman.dotLess;

#if NET_40
[assembly: PreApplicationStartMethod(typeof(IgnoreRoutesForLessUrls), "IgnoreLessRoutes")]
#endif
namespace Assman.dotLess
{
    public static class IgnoreRoutesForLessUrls
    {
        public static void IgnoreLessRoutes()
        {
            RouteTable.Routes.IgnoreLessRoutes();
        }

        public static void IgnoreLessRoutes(this RouteCollection routes)
        {
            routes.IgnoreRoute("{*resourceurls}", new { resourceurls = @".*(\.less)(/.*)?" });
        }
    }

    internal static class RouteExtensions
    {
        //this code adapted from System.Web.Mvc...I don't want to have to take a dependency on it here
        public static void IgnoreRoute(this RouteCollection routes, string url, object constraints)
        {
            var route = new IgnoredRoute(url)
            {
                Constraints = new RouteValueDictionary(constraints)
            };
            routes.Add(route);
        }

        private class IgnoredRoute : Route
        {
            public IgnoredRoute(string url)
                : base(url, new StopRoutingHandler())
              {
              }

            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
            {
                return null;
            }
        }
    }
}