using System;
using System.Collections.Generic;
using System.IO;

namespace Assman
{
    internal class IndividuallyCompiledResource : ICompiledResource
    {
        public IResource Resource { get; set; }
        public ResourceMode Mode { get; set; }
        public string CompiledContent { get; set; }
        
        private string _compiledPath;
        public string CompiledPath
        {
            get
            {
                if(_compiledPath == null && Resource != null)
                {
                    _compiledPath = CalculateCompiledPath();
                }
                
                return _compiledPath;
            }
            set { _compiledPath = value; }
        }

        public DateTime LastModified
        {
            get { return Resource.LastModified; }
        }

        public IEnumerable<IResource> Resources
        {
            get { yield return Resource; }
        }

        public void WriteTo(Stream outputStream)
        {
            var writer = new StreamWriter(outputStream);
            writer.Write(CompiledContent);
            writer.Flush();
        }

        private string CalculateCompiledPath()
        {
            var resourceType = ResourceType.FromPath(Resource.VirtualPath);

            return Resource.VirtualPath.ChangeExtension(".compiled" + resourceType.DefaultFileExtension);
        }
    }
}