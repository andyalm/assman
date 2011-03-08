using System;
using System.Web.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
    public class DefaultConfigLoader : IConfigLoader
    {
        public TSection GetSection<TSection>(string sectionName) where TSection : class
        {
            return WebConfigurationManager.GetSection(sectionName) as TSection;
        }
    }
}