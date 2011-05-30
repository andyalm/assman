using System;
using System.IO;

using Assman.ContentFiltering;
using Assman.IO;

namespace Assman.Handlers
{
    internal class UnconsolidatedResourceHandler : ResourceHandlerBase
    {
        private readonly string _physicalPathToResource;
        private readonly IFileAccess _fileAccess;
        private readonly ResourceType _resourceType;
        private readonly ContentFilterPipeline _contentFilterPipeline;
        private readonly ContentFilterContext _contentFilterContext;

        public UnconsolidatedResourceHandler(string physicalPathToResource,
            ResourceType resourceType,
            ContentFilterPipeline contentFilterPipeline,
            ContentFilterContext contentFilterContext)
            : this(physicalPathToResource, resourceType, contentFilterPipeline, contentFilterContext, new FileAccessWrapper()) { }
        
        internal UnconsolidatedResourceHandler(string physicalPathToResource,
                                               ResourceType resourceType,
                                               ContentFilterPipeline contentFilterPipeline,
                                               ContentFilterContext contentFilterContext,
                                               IFileAccess fileAccess)
        {
            _physicalPathToResource = physicalPathToResource;
            _resourceType = resourceType;
            _contentFilterPipeline = contentFilterPipeline;
            _contentFilterContext = contentFilterContext;
            _fileAccess = fileAccess;
        }

        protected override IHandlerResource GetResource()
        {
            return new UnconsolidatedHandlerResource(_fileAccess, _resourceType, _physicalPathToResource, _contentFilterPipeline, _contentFilterContext);
        }

        private class UnconsolidatedHandlerResource : IHandlerResource
        {
            private readonly IFileAccess _fileAccess;
            private readonly ResourceType _resourceType;
            private readonly string _physicalFilePath;
            private readonly ContentFilterPipeline _contentFilterPipeline;
            private readonly ContentFilterContext _contentFilterContext;

            public UnconsolidatedHandlerResource(IFileAccess fileAccess, ResourceType resourceType,
                                                 string physicalFilePath, ContentFilterPipeline contentFilterPipeline,
                                                 ContentFilterContext contentFilterContext)
            {
                _fileAccess = fileAccess;
                _resourceType = resourceType;
                _physicalFilePath = physicalFilePath;
                _contentFilterPipeline = contentFilterPipeline;
                _contentFilterContext = contentFilterContext;
            }

            public DateTime GetLastModified()
            {
                return _fileAccess.LastModified(_physicalFilePath);
            }

            public ResourceType ResourceType
            {
                get { return _resourceType; }
            }

            public void WriteContent(Stream outputStream)
            {
                string contentToWrite;
                using(var reader = _fileAccess.OpenReader(_physicalFilePath))
                {
                    var fileContents = reader.ReadToEnd();
                    contentToWrite = _contentFilterPipeline.FilterContent(fileContents, _contentFilterContext);
                }

                var writer = new StreamWriter(outputStream);
                writer.Write(contentToWrite);
                writer.Flush();
            }
        }
    }
}