using System;
using System.Configuration;

namespace Assman.Configuration
{
    public class GlobalDependenciesElement : ConfigurationElement
    {
        [ConfigurationProperty(PropertyNames.Path, IsKey = true, IsRequired = true)]
        public string Path
        {
            get { return (string) this[PropertyNames.Path]; }
            set { this[PropertyNames.Path] = value; }
        } 
    }
}