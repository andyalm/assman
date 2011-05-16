using System;
using System.IO;

using Assman.ContentFiltering;
using Assman.IO;

namespace Assman.Handlers
{
    public class UnconsolidatedResourceHandler : ResourceHandlerBase
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

        protected override IHandlerResource GetResource()
        {
            return new UnconsolidatedHandlerResource(_fileAccess, _resourceType, _physicalPathToResource, _contentFilter);
        }

        private class UnconsolidatedHandlerResource : IHandlerResource
        {
            private readonly IFileAccess _fileAccess;
            private readonly ResourceType _resourceType;
            private readonly string _physicalFilePath;
            private readonly IContentFilter _contentFilter;

            public UnconsolidatedHandlerResource(IFileAccess fileAccess, ResourceType resourceType,
                                                 string physicalFilePath, IContentFilter contentFilter)
            {
                _fileAccess = fileAccess;
                _resourceType = resourceType;
                _physicalFilePath = physicalFilePath;
                _contentFilter = contentFilter;
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
                    contentToWrite = _contentFilter.FilterContent(fileContents);
                }

                var writer = new StreamWriter(outputStream);
                writer.Write(contentToWrite);
                writer.Flush();
            }
        }
    }
}