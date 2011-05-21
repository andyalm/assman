using System;

namespace Assman
{
    internal class ExternallyCompiledResource
    {
        public IResource DebugResource { get; set; }
        public IResource ReleaseResource { get; set; }

        public bool Matches(IResource resource)
        {
            return DebugResource.VirtualPath.Equals(resource.VirtualPath, StringComparison.OrdinalIgnoreCase)
                   || ReleaseResource.VirtualPath.Equals(resource.VirtualPath, StringComparison.OrdinalIgnoreCase);
        }

        public bool WasMinifiedFrom(IResource resource)
        {
            return resource.VirtualPath.Equals(DebugResource.VirtualPath, StringComparison.OrdinalIgnoreCase);
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