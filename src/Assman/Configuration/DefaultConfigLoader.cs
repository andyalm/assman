using System;
using System.Web.Configuration;

namespace Assman.Configuration
{
    public class DefaultConfigLoader : IConfigLoader
    {
        public static DefaultConfigLoader GetInstance()
        {
            return new DefaultConfigLoader();
        }

        protected internal DefaultConfigLoader() {}
        
        public TSection GetSection<TSection>(string sectionName) where TSection : class
        {
            return WebConfigurationManager.GetSection(sectionName) as TSection;
        }
    }
}