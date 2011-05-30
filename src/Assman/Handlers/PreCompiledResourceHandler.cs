using System;
using System.IO;

using Assman.IO;

namespace Assman.Handlers
{
    internal class PreCompiledResourceHandler : ResourceHandlerBase
    {
        private readonly string _physicalResourcePath;
        private readonly ResourceType _resourceType;
        private readonly IFileAccess _fileAccess;
        private readonly ResourceMode _resourceMode;

        public PreCompiledResourceHandler(string physicalResourcePath, ResourceType resourceType, ResourceMode resourceMode) : this(physicalResourcePath, resourceType, resourceMode, new FileAccessWrapper())
        {
            
        }
        
        public PreCompiledResourceHandler(string physicalResourcePath, ResourceType resourceType, ResourceMode resourceMode, IFileAccess fileAccess)
        {
            _physicalResourcePath = physicalResourcePath;
            _resourceType = resourceType;
            _fileAccess = fileAccess;
            _resourceMode = resourceMode;
        }

        protected override IHandlerResource GetResource()
        {
            string relatedResourcePath;
            if(CompiledResourcePair.TryGetRelatedPath(_physicalResourcePath, _resourceMode, _fileAccess.Exists, out relatedResourcePath))
            {
                return CreateHandler(relatedResourcePath);
            }
            else
            {
                return CreateHandler(_physicalResourcePath);
            }
        }

        private IHandlerResource CreateHandler(string physicalResourcePath)
        {
            return new PreCompiledHandlerResource(physicalResourcePath, _resourceType, _fileAccess);
        }

        private class PreCompiledHandlerResource : IHandlerResource
        {
            private readonly string _physicalFilePath;
            private readonly ResourceType _resourceType;
            private readonly IFileAccess _fileAccess;

            public PreCompiledHandlerResource(string physicalFilePath, ResourceType resourceType, IFileAccess fileAccess)
            {
                _physicalFilePath = physicalFilePath;
                _resourceType = resourceType;
                _fileAccess = fileAccess;
            }

            public string PhysicalFilePath
            {
                get { return _physicalFilePath; }
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
                using (var fileStream = _fileAccess.OpenStream(_physicalFilePath))
                {
                    fileStream.CopyTo(outputStream);
                }
            }
        }
    }
}