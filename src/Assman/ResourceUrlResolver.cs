using System;
using System.Collections.Generic;

using Assman.DependencyManagement;

namespace Assman
{
    public interface IResourcePathResolver
    {
        IEnumerable<string> GetDependencyUrls(string resourceUrl);
        IEnumerable<string> GetMatchingGroupUrls(string virtualPath);
        IEnumerable<string> ResolveGroupUrl(string virtualPath);
    }
    
    public class ResourcePathResolver : IResourcePathResolver
    {
        private readonly IResourceGroupManager _groupManager;
        private readonly DependencyManager _dependencyManager;
        private readonly IResourceFinder _finder;

        public ResourcePathResolver(IResourceGroupManager groupManager, DependencyManager dependencyManager, IResourceFinder finder)
        {
            _groupManager = groupManager;
            _dependencyManager = dependencyManager;
            _finder = finder;
        }

        public IEnumerable<string> GetDependencyUrls(string resourceUrl)
        {
            return _dependencyManager.GetDependencies(resourceUrl);
        }

        public IEnumerable<string> ResolveGroupUrl(string virtualPath)
        {
            if(_groupManager.IsGroupUrlWithConsolidationDisabled(virtualPath))
            {
                return _groupManager.GetResourceUrlsInGroup(virtualPath, _finder).SortByDependencies(_dependencyManager);
            }
            else
            {
                return new[] {virtualPath};
            }
        } 

        public IEnumerable<string> GetMatchingGroupUrls(string virtualPath)
        {
            if (_groupManager.IsConsolidatedUrl(virtualPath))
            {
                return new[] { virtualPath };
            }
            else
            {
                return _groupManager.GetMatchingConsolidatedUrls(virtualPath);
            }
        }
    }
}