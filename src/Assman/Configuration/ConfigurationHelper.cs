using System;
using System.Configuration;
using System.IO;

namespace Assman.Configuration
{
	internal static class ConfigurationHelper
	{
		public static DateTime LastModified(this ConfigurationSection configSection, VirtualPathResolver pathResolver)
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

        public static TAbstraction CreateInstanceFromTypeString<TAbstraction>(string typeName)
        {
            var type = Type.GetType(typeName, false);
            if(type == null)
            {
                throw new ConfigurationErrorsException("The type '" + typeName + "' could not be found.");
            }

            var instance = Activator.CreateInstance(type);

            if(!(instance is TAbstraction))
            {
                throw new ConfigurationErrorsException("The type '" + typeName + "' must implement or inherit from '" +
                                                       typeof (TAbstraction).FullName + "'.");
            }

            return (TAbstraction) instance;
        }
	}
}
