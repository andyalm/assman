using System;
using System.Configuration;

namespace Assman.Configuration
{
    public abstract class ResourceConfigurationElement<TGroupElementCollection> : ConfigurationElement where TGroupElementCollection : ResourceGroupElementCollection, new()
    {
        /// <summary>
        /// Gets or sets whether consolidation is enabled for this type of resource.
        /// </summary>
        [ConfigurationProperty(PropertyNames.Consolidate, IsRequired = false, DefaultValue = ResourceModeCondition.Always)]
        public ResourceModeCondition Consolidate
        {
            get { return (ResourceModeCondition)this[PropertyNames.Consolidate]; }
            set { this[PropertyNames.Consolidate] = value; }
        }
        
        /// <summary>
        /// Gets the <see cref="ResourceGroupElementCollection"/> used to configure resource groups.
        /// </summary>
        [ConfigurationProperty(PropertyNames.Groups, IsRequired = false)]
        [ConfigurationCollection(typeof(ResourceGroupElementCollection), AddItemName = "group")]
        public TGroupElementCollection Groups
        {
            get
            {
                var element = this[PropertyNames.Groups] as TGroupElementCollection;
                if(element == null)
                {
                    element = new TGroupElementCollection();
                    this[PropertyNames.Groups] = element;
                }

                return element;
            }
        }

        /// <summary>
        /// Gets the <see cref="GlobalDependenciesElementCollection"/> used to configure global dependencies.
        /// </summary>
        [ConfigurationProperty(PropertyNames.GlobalDependencies, IsRequired = false)]
        [ConfigurationCollection(typeof(GlobalDependenciesElementCollection), AddItemName = "add")]
        public GlobalDependenciesElementCollection GlobalDependencies
        {
            get
            {
                var element = this[PropertyNames.GlobalDependencies] as GlobalDependenciesElementCollection;
                if(element == null)
                {
                    element = new GlobalDependenciesElementCollection();
                    this[PropertyNames.GlobalDependencies] = element;
                }

                return element;
            }
        }
    }
}