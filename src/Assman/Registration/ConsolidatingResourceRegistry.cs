using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assman.Registration
{
    /// <summary>
    /// Represents a <see cref="IResourceRegistry"/> that consolidates script or css includes that
    /// are registered through it.
    /// </summary>
    /// <remarks>
    /// This is a proxy for an inner <see cref="IResourceRegistry"/> which means it can wrap any implementation of a
    /// <see cref="IResourceRegistry"/>.
    /// </remarks>
    public class ConsolidatingResourceRegistry : IReadableResourceRegistry
    {
        private readonly IResourceRegistry _inner;
        private readonly Func<string, IEnumerable<string>> _getResourceUrls;
        private readonly Func<string, IEnumerable<string>> _getDependencies;

        internal ConsolidatingResourceRegistry(IResourceRegistry inner, Func<string, IEnumerable<string>> getResourceUrls, Func<string,IEnumerable<string>> getDependencies)
        {
            _inner = inner;
            _getResourceUrls = getResourceUrls;
            _getDependencies = getDependencies;
        }

        public IResourceRegistry Inner
        {
            get { return _inner; }
        }

        public bool TryResolvePath(string resourcePath, out IEnumerable<string> resolvedResourcePaths)
        {
            var resolvedPaths = _getResourceUrls(resourcePath);
            if(resolvedPaths.HasAtLeast(2) || !resolvedPaths.Single().EqualsVirtualPath(resourcePath))
            {
                resolvedResourcePaths = resolvedPaths;
                return true;
            }
            else
            {
                resolvedResourcePaths = null;
                return false;
            }
        }

        public void Require(string resourcePath)
        {
            resourcePath = ToCanonicalUrl(resourcePath);
            foreach (var dependency in _getDependencies(resourcePath))
            {
                foreach (var resolvedDependency in _getResourceUrls(dependency))
                {
                    _inner.Require(resolvedDependency);
                } 
            }
            foreach(var pathToInclude in _getResourceUrls(resourcePath))
            {
                _inner.Require(pathToInclude);
            }
        }

        public void RegisterInlineBlock(Action<TextWriter> block, object key)
        {
            _inner.RegisterInlineBlock(block, key);
        }

        public bool IsInlineBlockRegistered(object key)
        {
            return _inner.IsInlineBlockRegistered(key);
        }

        public IEnumerable<string> GetIncludes()
        {
            return _inner.AsReadable().GetIncludes();
        }

        public IEnumerable<Action<TextWriter>> GetInlineBlocks()
        {
            return _inner.AsReadable().GetInlineBlocks();
        }

        private string ToCanonicalUrl(string url)
        {
            return url.ToAppRelativePath();
        }
    }
}