using System;
using System.Web;

using Assman.ContentFiltering;
using Assman.IO;

namespace Assman.Handlers
{
    public class UnconsolidatedResourceHandler : HttpHandlerBase
    {
        private readonly string _physicalPathToResource;
        private readonly IFileAccess _fileAccess;
        private readonly ResourceType _resourceType;
        private readonly IContentFilter _contentFilter;

        public UnconsolidatedResourceHandler(string physicalPathToResource,
            ResourceType resourceType,
            IContentFilter contentFilter) : this(physicalPathToResource, resourceType, contentFilter, new FileAccessWrapper()) {}
        
        internal UnconsolidatedResourceHandler(string physicalPathToResource,
                                                        ResourceType resourceType,
                                                        IContentFilter contentFilter,
                                                        IFileAccess fileAccess)
        {
            _physicalPathToResource = physicalPathToResource;
            _resourceType = resourceType;
            _contentFilter = contentFilter;
            _fileAccess = fileAccess;
        }

        public override void ProcessRequest(HttpContextBase context)
        {
            using(var reader = _fileAccess.OpenReader(_physicalPathToResource))
            {
                var fileContents = reader.ReadToEnd();
                var filteredContent = _contentFilter.FilterContent(fileContents);

                context.Response.ContentType = _resourceType.ContentType;
                context.Response.Write(fileContents);
                //TODO: Add Last-Modified and 304 handling
            }

        }
    }
}