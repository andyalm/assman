using System;

namespace Assman
{
    internal class ExternallyCompiledResource
    {
        public IResource DebugResource { get; set; }
        public IResource ReleaseResource { get; set; }

        public bool Matches(IResource resource)
        {
            return DebugResource.VirtualPath.EqualsVirtualPath(resource.VirtualPath)
                   || ReleaseResource.VirtualPath.EqualsVirtualPath(resource.VirtualPath);
        }

        public bool WasMinifiedFrom(IResource resource)
        {
            return resource.VirtualPath.EqualsVirtualPath(DebugResource.VirtualPath);
        }

        public static bool IsPair(IResource potentialReleaseResource, IResource potentialDebugResource)
        {
            return potentialReleaseResource.VirtualPath == potentialDebugResource.VirtualPath.ChangeExtension(".min" + potentialDebugResource.FileExtension)
                   || potentialDebugResource.VirtualPath == potentialReleaseResource.VirtualPath.ChangeExtension(".debug" + potentialReleaseResource.FileExtension);
        }

        public IResource WithMode(ResourceMode mode)
        {
            return mode == ResourceMode.Debug ? DebugResource : ReleaseResource;
        }
    }
}