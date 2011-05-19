namespace Assman
{
    public class VsDocResourceExcluder : IFinderExcluder
    {
        public bool ShouldExclude(IResource resource)
        {
            return resource.VirtualPath.EndsWith("-vsdoc.js");
        }
    }
}