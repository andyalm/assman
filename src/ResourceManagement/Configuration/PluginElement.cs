using System;
using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class PluginElement : ConfigurationElement
	{
		[ConfigurationProperty(PropertyNames.Type)]
		public string Type
		{
			get { return (string) this[PropertyNames.Type]; }
			set { this[PropertyNames.Type] = value; }
		}

		public IResourceManagementPlugin CreatePlugin()
		{
			if (!String.IsNullOrEmpty(this.Type))
			{
				return CreatePluginFromType();
			}

			throw ConfigException("The custom finder element must either specify the 'type' or 'factory' attribute.");
		}

		private IResourceManagementPlugin CreatePluginFromType()
		{
			var pluginType = System.Type.GetType(this.Type);
			AssertTypeImplements<IResourceManagementPlugin>(pluginType, "type");

			return (IResourceManagementPlugin)Activator.CreateInstance(pluginType);
		}

		private void AssertTypeImplements<TInterface>(Type pluginType, string attributeName)
		{
			if(pluginType == null)
			{
				throw ConfigException("The ResourceManagement Plugin " + attributeName + " '" + this.Type + "' could not be found.");
			}
			if(!typeof(TInterface).IsAssignableFrom(pluginType))
			{
				throw ConfigException("The type '" + this.Type + "' must implement the '" +
				                      typeof (IResourceManagementPlugin).FullName + "' interface.");
			}
		}

		private Exception ConfigException(string message)
		{
			return new ConfigurationErrorsException(message, this.ElementInformation.Source, this.ElementInformation.LineNumber);
		}
	}
}