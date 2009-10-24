using System;
using System.Configuration;
using System.Reflection;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> used to specify an Assembly that the resource management library will
    /// search for resources in.
    /// </summary>
    public class AssemblyElement : ConfigurationElement
    {
        ///<summary>
        ///</summary>
        public AssemblyElement()
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="name"></param>
        public AssemblyElement(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        [ConfigurationProperty(PropertyNames.Name, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[PropertyNames.Name]; }
            set { this[PropertyNames.Name] = value; }
        }

        /// <summary>
        /// Gets the assembly that the element is pointing to.
        /// </summary>
        /// <returns></returns>
        public Assembly GetAssembly()
        {
            try
            {
                return Assembly.Load(Name);
            }
            catch (Exception ex)
            {
                throw new Exception("The assembly '" + Name + "' could not be loaded.", ex);
            }
        }

        private static class PropertyNames
        {
            public const string Name = "name";
        }
    }
}
