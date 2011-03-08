using System;
using System.IO;
using System.Web.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
    public class MappedConfigLoader : IConfigLoader
    {
        private readonly string _basePath;

        public MappedConfigLoader(string basePath)
        {
            _basePath = basePath;
        }

        public TSection GetSection<TSection>(string sectionName) where TSection : class
        {
        	var map = new WebConfigurationFileMap();
        	map.VirtualDirectories.Add("/", GetVDirMapping());
        	var configuration = WebConfigurationManager.OpenMappedWebConfiguration(map, "/");

            return configuration.GetSection(sectionName) as TSection;
        }

    	private VirtualDirectoryMapping GetVDirMapping()
		{
    		string directory;
    		if (_basePath.EndsWith(".config"))
    			directory = Path.GetDirectoryName(_basePath);
    		else
    			directory = _basePath;
    		return new VirtualDirectoryMapping(directory, isAppRoot : true);
    	}
    }
}