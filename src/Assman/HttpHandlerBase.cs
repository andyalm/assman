using System.Web;

namespace Assman
{
    public abstract class HttpHandlerBase : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var wrapper = new HttpContextWrapper(context);
            wrapper.Items[typeof (HttpContext)] = context;
            ProcessRequest(wrapper);
        }

        public abstract void ProcessRequest(HttpContextBase context);

        public bool IsReusable
        {
            get { return true; }
        }
    }

    internal static class HttpContextBaseExtensions
    {
        public static HttpContext ToHttpContext(this HttpContextBase context)
        {
            return (HttpContext) context.Items[typeof (HttpContext)];
        }
    }
}