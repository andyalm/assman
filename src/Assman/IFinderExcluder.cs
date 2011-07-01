using System;

namespace Assman
{
    public interface IFinderExcluder
    {
        bool ShouldExclude(IResource resource);
    }
}