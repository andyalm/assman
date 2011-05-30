using System;
using System.IO;
using System.Web;

namespace Assman.Handlers
{
    public abstract class ResourceHandlerBase : HttpHandlerBase
    {
        private IHandlerResource _cachedResource;
        
        protected ResourceHandlerBase()
        {
            Mode = ResourceMode.Debug;
        }
        
        public override void ProcessRequest(HttpContextBase context)
        {
            HandleRequest(new HttpRequestContext(context));
        }

        internal void HandleRequest(IRequestContext context)
        {
            var resource = GetResourceInternal();
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
                context.LastModified = lastModified;
                context.ContentType = resource.ResourceType.ContentType;
                string version;
                if(context.IsRequestVersioned())
                {
                    context.Expires = Now().AddYears(1);
                    context.Cacheability = HttpCacheability.Public;
                }
                resource.WriteContent(context.OutputStream);
            }
        }

        public ResourceMode Mode { get; set; }

        private static DateTime RoundToSeconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        private bool CachingEnabled
        {
            get { return Mode == ResourceMode.Release; }
        }

        internal Func<DateTime> Now = () => DateTime.Now; 

        protected abstract IHandlerResource GetResource();

        private IHandlerResource GetResourceInternal()
        {
            if (_cachedResource != null)
            {
                return _cachedResource;
            }

            var resource = GetResource();
            if (CachingEnabled)
            {
                _cachedResource = resource;
            }

            return resource;
        }
    }

    public interface IHandlerResource
    {
        DateTime GetLastModified();
        ResourceType ResourceType { get; }
        void WriteContent(Stream outputStream);
    }

    
}