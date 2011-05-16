using System;
using System.IO;
using System.Web;

namespace Assman.Handlers
{
    public abstract class ResourceHandlerBase : HttpHandlerBase
    {
        public override void ProcessRequest(HttpContextBase context)
        {
            HandleRequest(new HttpRequestContext(context));
        }

        internal void HandleRequest(IRequestContext context)
        {
            var resource = GetResource();
            DateTime lastModified = resource.GetLastModified().ToUniversalTime();
            DateTime ifModifiedSince = (context.IfModifiedSince ?? DateTime.MinValue).ToUniversalTime();
            lastModified = RoundToSeconds(lastModified);
            ifModifiedSince = RoundToSeconds(ifModifiedSince);
            if (lastModified <= ifModifiedSince)
            {
                context.StatusCode = 304;
                context.StatusDescription = "Not Modified";
                return;
            }
            else
            {
                context.SetLastModified(lastModified);
                context.ContentType = resource.ResourceType.ContentType;
                resource.WriteContent(context.OutputStream);
            }
        }

        private static DateTime RoundToSeconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        protected abstract IHandlerResource GetResource();
    }

    public interface IHandlerResource
    {
        DateTime GetLastModified();
        ResourceType ResourceType { get; }
        void WriteContent(Stream outputStream);
    }

    
}