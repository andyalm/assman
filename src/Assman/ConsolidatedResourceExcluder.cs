namespace Assman
{
    public class ConsolidatedResourceExcluder : IFinderExcluder
    {
        private readonly IResourceGroupManager _groupManager;

        public ConsolidatedResourceExcluder(IResourceGroupManager groupManager)
        {
            _groupManager = groupManager;
        }

        public bool ShouldExclude(IResource resource)
        {
            return _groupManager.IsConsolidatedUrl(resource.VirtualPath);
        }
    }
}