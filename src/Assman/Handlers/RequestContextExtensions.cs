using System;

namespace Assman.Handlers
{
    public static class RequestContextExtensions
    {
         internal static bool TryGetVersion(this IRequestContext requestContext, out string version)
         {
             version = requestContext.QueryString["v"];

             return !string.IsNullOrEmpty(version);
         }

        internal static bool IsRequestVersioned(this IRequestContext requestContext)
        {
            string version;
            return requestContext.TryGetVersion(out version);
        }
    }
}