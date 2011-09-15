using System;
using System.IO;

namespace Assman
{
    internal class CompiledResourcePair
    {
        public IResource DebugResource { get; set; }
        public IResource ReleaseResource { get; set; }

        public bool Matches(IResource resource)
        {
            return Matches(resource.VirtualPath);
        }

        public bool Matches(string virtualPath)
        {
            return DebugResource.VirtualPath.EqualsVirtualPath(virtualPath)
                   || ReleaseResource.VirtualPath.EqualsVirtualPath(virtualPath);
        }

        public bool WasMinifiedFrom(IResource resource)
        {
            return resource.VirtualPath.EqualsVirtualPath(DebugResource.VirtualPath);
        }

        public IResource WithMode(ResourceMode mode)
        {
            return mode == ResourceMode.Debug ? DebugResource : ReleaseResource;
        }

        public static bool IsExternallyCompiledPair(IResource potentialReleaseResource, IResource potentialDebugResource)
        {
            return potentialReleaseResource.VirtualPath == ToMinPath(potentialDebugResource.VirtualPath, potentialDebugResource.FileExtension)
                   || potentialDebugResource.VirtualPath == ToDebugPath(potentialReleaseResource.VirtualPath, potentialReleaseResource.FileExtension);
        }

        public static bool TryGetRelatedPath(string resourcePath, ResourceMode mode, Func<string, bool> resourceExists, out string pathToOtherResourceInPair)
        {
            string fileExtension = Path.GetExtension(resourcePath);
            if(mode == ResourceMode.Release)
            {
                var compiledResource = ToCompiledPath(resourcePath, fileExtension);
                if(resourceExists(compiledResource))
                {
                    pathToOtherResourceInPair = compiledResource;
                    return true;
                }
                
                var minResource = ToMinPath(resourcePath, fileExtension);
                if(resourceExists(minResource))
                {
                    pathToOtherResourceInPair = minResource;
                    return true;
                }
            }
            else
            {
                var debugResource = ToDebugPath(resourcePath, fileExtension);
                if(resourceExists(debugResource))
                {
                    pathToOtherResourceInPair = debugResource;
                    return true;
                }
            }

            pathToOtherResourceInPair = null;
            return false;
        }

        private static string ToMinPath(string standardPath, string fileExtension)
        {
            return standardPath.ChangeExtension(".min" + fileExtension);
        }

        private static string ToCompiledPath(string standardPath, string fileExtension)
        {
            return standardPath.ChangeExtension(".compiled" + fileExtension);
        }

        private static string ToDebugPath(string standardPath, string fileExtension)
        {
            return standardPath.ChangeExtension(".debug" + fileExtension);
        }
    }
}