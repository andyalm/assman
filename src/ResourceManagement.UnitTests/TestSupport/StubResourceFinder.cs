using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement.TestSupport
{
    public class StubResourceFinder : IResourceFinder
    {
        private readonly ResourceCollection _resources = new ResourceCollection();
        
        public StubResource CreateResource(string virtualPath)
        {
        	var resource = StubResource.WithPath(virtualPath);
			AddResource(resource);

        	return resource;
        }
		
		public void AddResource(IResource resource)
        {
        	_resources.Add(resource);
        }
		
		public void AddResource(string virtualPath, string content)
        {
            StubResource resource = new StubResource(content);
            resource.VirtualPath = virtualPath;
            _resources.Add(resource);
        }

    	public void AddResources(params IResource[] resources)
    	{
    		_resources.AddRange(resources);
    	}

    	public ResourceCollection Resources { get { return _resources; } }

    	public IEnumerable<IResource> FindResources(ResourceType resourceType)
    	{
    		return _resources;
    	}

    	public IResource FindResource(string virtualPath)
    	{
    		return _resources.Where(r => r.VirtualPath != null && r.VirtualPath.Equals(virtualPath, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
    	}
    }
}
