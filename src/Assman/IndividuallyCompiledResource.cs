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
            get { return new DateTime(); }
        }

        public IEnumerable<IResource> Resources
        {
            get { yield return Resource; }
        }

        public void WriteTo(Stream outputStream) {}

        private string CalculateCompiledPath()
        {
            var lastDotIndex = Resource.VirtualPath.LastIndexOf(".");
            var beforePeriod = Resource.VirtualPath.Substring(0, lastDotIndex);
            var afterPeriod = Resource.VirtualPath.Substring(lastDotIndex + 1);

            return beforePeriod + ".min." + afterPeriod;
        }
    }
}