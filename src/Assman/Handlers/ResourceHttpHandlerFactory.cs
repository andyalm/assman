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
            var appRelativePath = url.ToAppRelativePath();

            return _pathToHandlerCache.GetOrAdd(appRelativePath, () => GetHandlerUncached(pathTranslated, appRelativePath));
        }

        public IHttpHandler GetHandlerUncached(string physicalPathToResource, string appRelativePathToResource)
        {
            var resourceType = ResourceType.FromPath(physicalPathToResource);
            if (resourceType == null)
            {
                return null;
            }
            var resourceMode = GetResourceMode();

            if (_assmanContext.PreCompiled)
            {
                return new PreCompiledResourceHandler(physicalPathToResource, resourceType, resourceMode)
                {
                    EnableGZip = _assmanContext.GZip,
                    Mode = resourceMode
                };
            }
            
            var groupTemplate = _assmanContext.FindGroupTemplate(appRelativePathToResource);
            if (groupTemplate != null)
            {
                return new DynamicallyConsolidatedResourceHandler(appRelativePathToResource, _assmanContext.GetConsolidator(),
                                                       groupTemplate)
                {
                    MinLastModified = _assmanContext.ConfigurationLastModified,
                    Mode = resourceMode,
                    EnableGZip = _assmanContext.GZip
                };
            }
            else
            {
                var fileExtension = Path.GetExtension(physicalPathToResource);
                
                var contentFilterPipeline = _assmanContext.GetContentPipelineForExtension(fileExtension);
                var contentFilterContext = new ContentFilterContext
                {
                    Minify = resourceMode == ResourceMode.Release, //TODO: base this off of global minification setting
                    ResourceVirtualPath = appRelativePathToResource
                };

                return new DynamicallyCompiledIndividualResourceHandler(physicalPathToResource, resourceType, contentFilterPipeline, contentFilterContext)
                {
                    Mode = resourceMode,
                    EnableGZip = _assmanContext.GZip
                };
            }
        }

        private static IThreadSafeInMemoryCache<string, IHttpHandler> CreateHandlerCache()
        {
            return new ThreadSafeInMemoryCache<string, IHttpHandler>(Comparers.VirtualPath);
        }

        private ResourceMode GetResourceMode()
        {
            var compilationSection = _configLoader.GetSection<CompilationSection>("system.web/compilation");
            return compilationSection.Debug ? ResourceMode.Debug : ResourceMode.Release;
        }

        public void ReleaseHandler(IHttpHandler handler) {}
    }
}