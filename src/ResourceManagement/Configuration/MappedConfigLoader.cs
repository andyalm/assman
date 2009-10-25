using System;
using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
    public class MappedConfigLoader : IConfigLoader
    {
        private string _basePath;

        public MappedConfigLoader(string basePath)
        {
            _basePath = basePath;
        }

        public TSection GetSection<TSection>(string sectionName) where TSection : class
        {
            var configuration = ConfigurationHelper.OpenWebConfiguration(_basePath);

            return configuration.GetSection(sectionName) as TSection;
        }

        public TSection GetSectionForEditing<TSection>(string sectionName, out System.Configuration.Configuration configuration) where TSection : ConfigurationSection, new()
        {
            configuration = ConfigurationHelper.OpenWebConfiguration(_basePath);

            return ConfigurationHelper.OpenForEditing<TSection>(sectionName, configuration);
        }
    }
}