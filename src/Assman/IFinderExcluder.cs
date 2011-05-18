using System.Collections.Generic;

namespace Assman
{
    public interface IFinderExcluder
    {
        bool ShouldExclude(IResource resource);
    }
}