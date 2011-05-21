using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
{
    internal static class ExternallyCompiledResourceExtensions
    {
        public static IEnumerable<ExternallyCompiledResource> ExternallyCompiled(this IEnumerable<IResource> resources)
        {
            //prevent multiple enumerations over potentially slow enumerable
            resources = resources.ToList();

            return from resource1 in resources
                   from resource2 in resources
                   where ExternallyCompiledResource.IsPair(resource1, resource2)
                   select new ExternallyCompiledResource {DebugResource = resource2, ReleaseResource = resource1};

        }

        public static IResource WithMode(this IResource resource, ResourceMode mode, IEnumerable<ExternallyCompiledResource> resources)
        {
            var externallyCompiledResource = resources.SingleOrDefault(resource);
            if (externallyCompiledResource != null)
                return externallyCompiledResource.WithMode(mode);
            
            return resource;
        }

        public static ExternallyCompiledResource SingleOrDefault(this IEnumerable<ExternallyCompiledResource> resources, IResource resource)
        {
            return resources.SingleOrDefault(r => r.Matches(resource));
        }

        public static IEnumerable<IResource> WithMode(this IEnumerable<ExternallyCompiledResource> externallyCompiledResources, ResourceMode mode)
        {
            if (mode == ResourceMode.Debug)
                return externallyCompiledResources.Select(r => r.DebugResource);
            else
                return externallyCompiledResources.Select(r => r.ReleaseResource);
        }
    }
}