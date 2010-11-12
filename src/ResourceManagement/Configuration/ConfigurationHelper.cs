using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	internal class ConfigurationHelper
	{
		/// <summary>
		/// Opens a <see cref="T:System.Configuration.Configuration"/> object from the given config file.
		/// </summary>
		/// <param name="physicalPath">The physical path to the configuration file.  This can be either a path to a file or a directory.  If it is a directory, then it will try to open up the web.config file in that directory.</param>
		/// <returns>A <see cref="T:System.Configuration.Configuration"/> object loaded from the given config file.</returns>
		public static System.Configuration.Configuration OpenWebConfiguration(string physicalPath)
		{
			bool isAppRoot = true;
			
			System.Configuration.Configuration configuration;
			WebConfigurationFileMap map = new WebConfigurationFileMap();
			string directory;
			if (physicalPath.EndsWith(".config"))
				directory = Path.GetDirectoryName(physicalPath);
			else
				directory = physicalPath;
			VirtualDirectoryMapping vMapping = new VirtualDirectoryMapping(directory, isAppRoot);
			map.VirtualDirectories.Add("/", vMapping);
			configuration = WebConfigurationManager.OpenMappedWebConfiguration(map, "/");
			return configuration;
		}

		public static TConfigSection OpenForEditing<TConfigSection>(string sectionName, System.Configuration.Configuration configuration) where TConfigSection : ConfigurationSection, new()
		{
			TConfigSection section = configuration.Sections[sectionName] as TConfigSection;
			if (section == null)
			{
				section = new TConfigSection();
				configuration.Sections.Add(sectionName, section);
			}

			return section;
		}

		public static DateTime GetLastModified(ConfigurationSection configSection)
		{
			string configSource = configSection.SectionInformation.ConfigSource;
			string configFile;
			if (String.IsNullOrEmpty(configSource))
				configFile = "~/web.config";
			else
				configFile = "~/" + configSource;

			string filePath = HttpContext.Current.Server.MapPath(configFile);
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
