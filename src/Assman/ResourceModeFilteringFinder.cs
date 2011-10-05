using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
{
    public class ResourceModeFilteringFinder : IResourceFinder
    {
        private readonly ResourceMode _resourceMode;
        private readonly IResourceFinder _inner;

        public ResourceModeFilteringFinder(ResourceMode resourceMode, IResourceFinder inner)
        {
            _resourceMode = resourceMode;
            _inner = inner;
        }

        public IEnumerable<IResource> FindResources(ResourceType resourceType)
        {
            var resources = _inner.FindResources(resourceType).ToArray();
            var externallyCompiledPairs = resources.ExternallyCompiled();

            foreach (var resource in resources)
            {
                var externallyCompiledPair = externallyCompiledPairs.SingleOrDefault(resource.VirtualPath);
                if(externallyCompiledPair == null)
                {
                    yield return resource;
                }
                else
                {
                    var resourceWithProperMode = externallyCompiledPair.WithMode(_resourceMode);
                    if(resourceWithProperMode.VirtualPath.EqualsVirtualPath(resource.VirtualPath))
                        yield return new ExternallyCompiledResource(externallyCompiledPair, resourceWithProperMode);
                }
            }
        }

        public IResource FindResource(string virtualPath)
        {
            if (CompiledResourcePair.MatchesMode(virtualPath, _resourceMode))
                return _inner.FindResource(virtualPath);
            
            RelatedResourceInfo otherResourceInfo;
            if(CompiledResourcePair.TryGetRelatedResource(virtualPath, _resourceMode, potentialPath => _inner.FindResource(potentialPath) != null, out otherResourceInfo))
            {
                var resource = _inner.FindResource(virtualPath);
                var relatedResource = _inner.FindResource(otherResourceInfo.RelatedPath);
                CompiledResourcePair pair;
                if(otherResourceInfo.ModeOfRelatedPath == ResourceMode.Release)
                    pair = new CompiledResourcePair { DebugResource = resource, ReleaseResource = relatedResource };
                else
                    pair = new CompiledResourcePair { DebugResource = relatedResource, ReleaseResource = resource };

                return new ExternallyCompiledResource(pair, pair.WithMode(_resourceMode));
            }

            return _inner.FindResource(virtualPath);
        }
    }

    internal class ExternallyCompiledResource : IResource
    {
        private readonly CompiledResourcePair _pair;
        private readonly IResource _inner;

        public ExternallyCompiledResource(CompiledResourcePair pair, IResource inner)
        {
            _pair = pair;
            _inner = inner;
        }

        public string Name
        {
            get { return _pair.ResourceWithCanonicalPath.Name; }
        }

        public string VirtualPath
        {
            get { return _pair.CanonicalPath; }
        }

        public DateTime LastModified
        {
            get { return _inner.LastModified; }
        }

        public string FileExtension
        {
            get { return _pair.ResourceWithCanonicalPath.FileExtension; }
        }

        public string GetContent()
        {
            return _inner.GetContent();
        }
    }
}