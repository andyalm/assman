using System;
using System.IO;
using System.Web;

using Assman.IO;

namespace Assman.Handlers
{
    public abstract class ResourceHandlerBase : HttpHandlerBase
    {
        private IHandlerResource _cachedResource;
        private IHandlerResource _cachedGZippedResource;
        
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
            var gzip = ShouldGZip(context);
            var resource = GetResourceInternal(gzip);
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
                if(context.IsRequestVersioned())
                {
                    context.Expires = Now().AddYears(1);
                    context.Cacheability = HttpCacheability.Public;
                }
                if(gzip)
                {
                    context.ContentEncoding = "gzip";
                    context.Vary = "Accept-Encoding";
                }
                resource.WriteContent(context.OutputStream);
            }
        }

        public ResourceMode Mode { get; set; }

        public bool EnableGZip { get; set; }

        private bool ShouldGZip(IRequestContext context)
        {
            return EnableGZip && Mode == ResourceMode.Release && context.AcceptsGZip;
        }

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

        private IHandlerResource GetResourceInternal(bool gzip)
        {
            if (gzip)
            {
                return GetResourceInternal(ref _cachedGZippedResource, r => new CachedGZippedHandlerResource(r));
            }
            else
            {
                return GetResourceInternal(ref _cachedResource, r => new CachedHandlerResource(r));
            }
        }

        private IHandlerResource GetResourceInternal(ref IHandlerResource cachedResource, Func<IHandlerResource,IHandlerResource> createCachedResource)
        {
            if (cachedResource != null)
            {
                return cachedResource;
            }

            var resource = GetResource();
            if (CachingEnabled)
            {
                resource = createCachedResource(resource);
                cachedResource = resource;
            }

            return resource;
        }

        private class CachedGZippedHandlerResource : CachedHandlerResource
        {
            public CachedGZippedHandlerResource(IHandlerResource inner) : base(inner, CompressContent) {}
            
            private static void CompressContent(IHandlerResource inner, MemoryStream outputStream)
            {
                var compressingStream = outputStream.Compress(leaveOpen : true);
                inner.WriteContent(compressingStream);
                //for some stupid reason, Flush() doesn't work here, so you have to call Close or your content
                //can end up truncated
                compressingStream.Close();
            }
        }

        private class CachedHandlerResource : IHandlerResource
        {
            private readonly DateTime _lastModified;
            private readonly ResourceType _resourceType;
            private readonly MemoryStream _content;

            public CachedHandlerResource(IHandlerResource inner) : this(inner, (r,s) => r.WriteContent(s)) {}

            protected CachedHandlerResource(IHandlerResource inner, Action<IHandlerResource,MemoryStream> writeContent)
            {
                _lastModified = inner.GetLastModified();
                _resourceType = inner.ResourceType;
                _content = new MemoryStream();
                writeContent(inner, _content);
            }

            public DateTime GetLastModified()
            {
                return _lastModified;
            }

            public ResourceType ResourceType
            {
                get { return _resourceType; }
            }

            public void WriteContent(Stream outputStream)
            {
                _content.WriteTo(outputStream);
            }
        }
    }

    public interface IHandlerResource
    {
        DateTime GetLastModified();
        ResourceType ResourceType { get; }
        void WriteContent(Stream outputStream);
    }

    
}