using System;
using System.Configuration;
using System.IO;

namespace Assman.Configuration
{
    internal static class ConfigurationHelper
    {
        public static DateTime LastModified(this ConfigurationSection configSection, IPathResolver pathResolver)
        {
            string configSource = configSection.SectionInformation.ConfigSource;
            string configFile;
            if (String.IsNullOrEmpty(configSource))
                configFile = "~/web.config";
            else
                configFile = "~/" + configSource;

            string filePath = pathResolver.MapPath(configFile);
            return File.GetLastWriteTime(filePath).ToUniversalTime();
        }
    }
}
