using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Assman
{
    internal static class CompiledResourcePairExtensions
    {
        public static CompiledResourcePairCollection ExternallyCompiled(this IResource[] resources)
        {
            return (from resource1 in resources
                   from resource2 in resources
                   where CompiledResourcePair.IsExternallyCompiledPair(resource1, resource2)
                   select new CompiledResourcePair { DebugResource = resource2, ReleaseResource = resource1 })
                   .ToResourcePairCollection();
        }

        public static bool IsExternallyCompiled(this IResource resource)
        {
            return resource is ExternallyCompiledResource;
        }

        public static CompiledResourcePair SingleOrDefault(this IEnumerable<CompiledResourcePair> resources, IResource resource)
        {
            return resources.SingleOrDefault(r => r.Matches(resource));
        }

        internal static CompiledResourcePairCollection ToResourcePairCollection(this IEnumerable<CompiledResourcePair> pairs)
        {
            return new CompiledResourcePairCollection(pairs);
        }
    }

    internal class CompiledResourcePairCollection : IEnumerable<CompiledResourcePair>
    {
        private readonly List<CompiledResourcePair> _pairs = new List<CompiledResourcePair>();
        private readonly Dictionary<string,CompiledResourcePair> _debugIndex = new Dictionary<string, CompiledResourcePair>(Comparers.VirtualPath); 
        private readonly Dictionary<string,CompiledResourcePair> _releaseIndex = new Dictionary<string, CompiledResourcePair>(Comparers.VirtualPath);

        public CompiledResourcePairCollection(IEnumerable<CompiledResourcePair> pairs)
        {
            foreach (var pair in pairs)
            {
                _pairs.Add(pair);
                _debugIndex.Add(pair.DebugResource.VirtualPath, pair);
                _releaseIndex.Add(pair.ReleaseResource.VirtualPath, pair);
            }
        }

        public CompiledResourcePair SingleOrDefault(string virtualPath)
        {
            CompiledResourcePair value;

            if (_debugIndex.TryGetValue(virtualPath, out value))
                return value;
            if (_releaseIndex.TryGetValue(virtualPath, out value))
                return value;

            return null;
        }
        
        public IEnumerator<CompiledResourcePair> GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}