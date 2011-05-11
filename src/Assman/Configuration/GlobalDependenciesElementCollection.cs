using System;
using System.Configuration;

namespace Assman.Configuration
{
    public class GlobalDependenciesElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new GlobalDependenciesElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((GlobalDependenciesElement)element).Path;
        }
    }
}