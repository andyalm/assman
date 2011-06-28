using System.Web.Configuration;

namespace Assman.Configuration
{
    public interface IResourceModeProvider
    {
        ResourceMode GetCurrentResourceMode();
    }

    public static class ResourceModeProvider
    {
        private static IResourceModeProvider _instance = ConfigDrivenResourceModeProvider.GetInstance();

        public static IResourceModeProvider Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }
    }

    public class ConfigDrivenResourceModeProvider : IResourceModeProvider
    {
        public static ConfigDrivenResourceModeProvider GetInstance()
        {
            return new ConfigDrivenResourceModeProvider(DefaultConfigLoader.GetInstance());
        }
        
        private readonly IConfigLoader _configLoader;

        protected internal ConfigDrivenResourceModeProvider(IConfigLoader configLoader)
        {
            _configLoader = configLoader;
        }

        public ResourceMode GetCurrentResourceMode()
        {
            var compilationSection = _configLoader.GetSection<CompilationSection>("system.web/compilation");
            if(compilationSection == null)
                return ResourceMode.Release;
            else
                return compilationSection.Debug ? ResourceMode.Debug : ResourceMode.Release;
        }
    }
}