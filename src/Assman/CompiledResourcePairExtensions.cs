using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
{
    internal static class CompiledResourcePairExtensions
    {
        public static IEnumerable<CompiledResourcePair> ExternallyCompiled(this IEnumerable<IResource> resources)
        {
            return resources.CompiledPair(CompiledResourcePair.IsExternallyCompiledPair);
        }

        private static IEnumerable<CompiledResourcePair> CompiledPair(this IEnumerable<IResource> resources, Func<IResource,IResource,bool> isPairPredicate)
        {
            //prevent multiple enumerations over potentially slow enumerable
            resources = resources.ToList();

            return from resource1 in resources
                   from resource2 in resources
                   where isPairPredicate(resource1, resource2)
                   select new CompiledResourcePair { DebugResource = resource2, ReleaseResource = resource1 };
        } 

        public static IResource WithMode(this IResource resource, ResourceMode mode, IEnumerable<CompiledResourcePair> resources)
        {
            var compiledResourcePair = resources.SingleOrDefault(resource);
            if (compiledResourcePair != null)
                return compiledResourcePair.WithMode(mode);
            
            return resource;
        }

        public static IEnumerable<string> WithMode(this IEnumerable<string> resourcePaths, ResourceMode mode, IEnumerable<CompiledResourcePair> externallyCompiledResources)
        {
            return resourcePaths.Select(path =>
            {
                var compiledResourcePair = externallyCompiledResources.SingleOrDefault(path);
                if (compiledResourcePair != null)
                    return compiledResourcePair.WithMode(mode).VirtualPath;
                else
                    return path;
            });
        }

        public static CompiledResourcePair SingleOrDefault(this IEnumerable<CompiledResourcePair> resources, IResource resource)
        {
            return resources.SingleOrDefault(r => r.Matches(resource));
        }

        public static CompiledResourcePair SingleOrDefault(this IEnumerable<CompiledResourcePair> resources, string virtualPath)
        {
            return resources.SingleOrDefault(r => r.Matches(virtualPath));
        }

        public static IEnumerable<IResource> WithMode(this IEnumerable<CompiledResourcePair> compiledResourcePair, ResourceMode mode)
        {
            if (mode == ResourceMode.Debug)
                return compiledResourcePair.Select(r => r.DebugResource);
            else
                return compiledResourcePair.Select(r => r.ReleaseResource);
        }
    }
}