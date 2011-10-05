namespace Assman.TestSupport
{
    public static class ExternallyCompiledExtensions
    {
         public static IResource ExternallyCompiledWith(this IResource debugResource, IResource releaseResource, ResourceMode mode)
         {
             var pair = new CompiledResourcePair
             {
                 DebugResource = debugResource,
                 ReleaseResource = releaseResource
             };
             return new ExternallyCompiledResource(pair, mode == ResourceMode.Release ? releaseResource : debugResource);
         }
    }
}