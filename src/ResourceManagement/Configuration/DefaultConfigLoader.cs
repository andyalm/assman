using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
    public class DefaultConfigLoader : IConfigLoader
    {
        public TSection GetSection<TSection>(string sectionName) where TSection : class
        {
            return WebConfigurationManager.GetSection(sectionName) as TSection;
        }

        public TSection GetSectionForEditing<TSection>(string sectionName, out System.Configuration.Configuration configuration) where TSection : ConfigurationSection, new() 
        {
            configuration = ConfigurationHelper.OpenWebConfiguration(HttpContext.Current.Server.MapPath("~"));

            return ConfigurationHelper.OpenForEditing<TSection>(sectionName, configuration);
        }
    }
}