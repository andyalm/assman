using System;

namespace AlmWitt.Web.ResourceManagement.TestSupport
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

		public ResourceCollection Resources { get { return _resources; } }

    	public ResourceCollection FindResources(ResourceType resourceType)
    	{
    		return _resources;
    	}

    	public IResource FindResource(string virtualPath)
    	{
    		return null;
    	}
    }
}
