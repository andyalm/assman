using System;
using System.IO;
using System.Web;
using System.Web.Configuration;

using Assman.Configuration;
using Assman.ContentFiltering;

namespace Assman.Handlers
{
    public class ResourceHttpHandlerFactory : IHttpHandlerFactory
    {
        private static readonly IHttpHandler _staticHandler = CreateStaticFileHandler();
        private readonly IThreadSafeInMemoryCache<string, IHttpHandler> _pathToHandlerCache = CreateHandlerCache();
        private readonly AssmanContext _assmanContext;
        private readonly IConfigLoader _configLoader;

        public ResourceHttpHandlerFactory() : this(AssmanContext.Current, new DefaultConfigLoader()) {}

        internal ResourceHttpHandlerFactory(AssmanContext assmanContext, IConfigLoader configLoader)
        {
            _assmanContext = assmanContext;
            _configLoader = configLoader;
        }

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            if (_assmanContext.PreConsolidated)
                return _staticHandler;

            var appRelativePath = "~" + url;

            return _pathToHandlerCache.GetOrAdd(pathTranslated, () => GetHandlerUncached(pathTranslated, appRelativePath));
        }

        public IHttpHandler GetHandlerUncached(string physicalPathToResource, string appRelativePathToResource)
        {
            var resourceMode = GetResourceMode();
            var groupTemplate = _assmanContext.FindGroupTemplate(appRelativePathToResource);
            if (groupTemplate != null)
            {
                return new ConsolidatedResourceHandler(appRelativePathToResource, _assmanContext.GetConsolidator(),
                                                       groupTemplate, resourceMode)
                {
                    MinLastModified = _assmanContext.ConfigurationLastModified
                };
            }
            else
            {
                var fileExtension = Path.GetExtension(physicalPathToResource);
                var resourceType = ResourceType.FromPath(physicalPathToResource);
                if (resourceType == null)
                {
                    return null;
                }
                var contentFilterPipeline = _assmanContext.GetFilterPipelineForExtension(fileExtension);
                var contentFilterContext = new ContentFilterContext
                {
                    Minify = resourceMode == ResourceMode.Release,
                    Mode = resourceMode,
                    ResourceVirtualPath = appRelativePathToResource
                };
                
                return new UnconsolidatedResourceHandler(physicalPathToResource, resourceType, contentFilterPipeline, contentFilterContext);
            }
        }


        private static IThreadSafeInMemoryCache<string, IHttpHandler> CreateHandlerCache()
        {
            return new ThreadSafeInMemoryCache<string, IHttpHandler>(Comparers.FileSystemPath);
        }

        private static IHttpHandler CreateStaticFileHandler()
        {
            var webAssembly = typeof (IHttpHandler).Assembly;
            var staticHandlerType = webAssembly.GetType("System.Web.StaticFileHandler", true);
            return (IHttpHandler) Activator.CreateInstance(staticHandlerType, nonPublic : true);
        }

        private ResourceMode GetResourceMode()
        {
            var compilationSection = _configLoader.GetSection<CompilationSection>("system.web/compilation");
            return compilationSection.Debug ? ResourceMode.Debug : ResourceMode.Release;
        }

        public void ReleaseHandler(IHttpHandler handler) {}
    }
}