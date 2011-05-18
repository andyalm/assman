using System;

namespace Assman
{
    public class PreCompiledResourceExcluder : IFinderExcluder
    {
        public bool ShouldExclude(IResource resource)
        {
            return resource.VirtualPath.EndsWith(".compiled.js", StringComparison.OrdinalIgnoreCase)
                || resource.VirtualPath.EndsWith(".compiled.css", StringComparison.OrdinalIgnoreCase);
        }
    }
}