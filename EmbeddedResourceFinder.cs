using System;
using System.Reflection;

namespace AlmWitt.Web.ResourceManagement
{
    internal class EmbeddedResourceFinder : IResourceFinder
    {
        private Assembly _assembly;

        internal EmbeddedResourceFinder(Assembly assembly)
        {
            _assembly = assembly;
        }

        public ResourceCollection FindResources(string extension)
        {
            ResourceCollection resources = new ResourceCollection();
            foreach(string resourceName in _assembly.GetManifestResourceNames())
            {
                if(resourceName.EndsWith(extension))
                    resources.Add(new EmbeddedResource(_assembly, resourceName));
            }

            return resources;
        }
    }
}
