using System;

namespace AlmWitt.Web.ResourceManagement.UnitTests.TestObjects
{
    public class StubResourceFinder : IResourceFinder
    {
        private ResourceCollection _resources = new ResourceCollection();
        
        public void AddResource(string virtualPath, string content)
        {
            StubResource resource = new StubResource(content);
            resource.VirtualPath = virtualPath;
            _resources.Add(resource);
        }
        
        public ResourceCollection FindResources(string extension)
        {
            return _resources;
        }
    }
}
