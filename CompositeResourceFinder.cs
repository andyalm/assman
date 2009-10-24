using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
    internal class CompositeResourceFinder : IResourceFinder
    {
        List<IResourceFinder> _finders = new List<IResourceFinder>();
        
        public ResourceCollection FindResources(string extension)
        {
            ResourceCollection combined = new ResourceCollection();
            foreach (IResourceFinder finder in _finders)
            {
                ResourceCollection found = finder.FindResources(extension);
                combined.AddRange(found);
            }

            return combined;
        }

        public void AddFinder(IResourceFinder finder)
        {
            _finders.Add(finder);
        }
    }
}
