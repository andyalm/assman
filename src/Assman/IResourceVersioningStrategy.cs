using System;

namespace Assman
{
    public interface IResourceVersioningStrategy
    {
        string ApplyVersion(string virtualPath);
    }

    public class NoVersioningStrategy : IResourceVersioningStrategy
    {
        public string ApplyVersion(string virtualPath)
        {
            return virtualPath;
        }
    }

    public class ConfiguredVersioningStrategy : IResourceVersioningStrategy
    {
        private readonly Func<string> _getVersion;

        public ConfiguredVersioningStrategy(Func<string> getVersion)
        {
            _getVersion = getVersion;
        }

        public string ApplyVersion(string virtualPath)
        {
            var version = _getVersion();
            if (!String.IsNullOrEmpty(version))
                return virtualPath.AddQueryParam("v", version);
            else
                return virtualPath;
        }
    }
}