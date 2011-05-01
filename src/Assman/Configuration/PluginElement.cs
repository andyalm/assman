using System;
using System.Configuration;

namespace Assman.Configuration
{
	public class PluginElement : ConfigurationElement
	{
		[ConfigurationProperty(PropertyNames.Type)]
		public string Type
		{
			get { return (string) this[PropertyNames.Type]; }
			set { this[PropertyNames.Type] = value; }
		}

		public IAssmanPlugin CreatePlugin()
		{
			if (!String.IsNullOrEmpty(this.Type))
			{
				return CreatePluginFromType();
			}

			throw ConfigException("The custom finder element must either specify the 'type' or 'factory' attribute.");
		}

		private IAssmanPlugin CreatePluginFromType()
		{
			var pluginType = System.Type.GetType(this.Type);
			AssertTypeImplements<IAssmanPlugin>(pluginType, "type");

			return (IAssmanPlugin)Activator.CreateInstance(pluginType);
		}

		private void AssertTypeImplements<TInterface>(Type pluginType, string attributeName)
		{
			if(pluginType == null)
			{
				throw ConfigException("The Assman Plugin " + attributeName + " '" + this.Type + "' could not be found.");
			}
			if(!typeof(TInterface).IsAssignableFrom(pluginType))
			{
				throw ConfigException("The type '" + this.Type + "' must implement the '" +
				                      typeof (IAssmanPlugin).FullName + "' interface.");
			}
		}

		private Exception ConfigException(string message)
		{
			return new ConfigurationErrorsException(message, this.ElementInformation.Source, this.ElementInformation.LineNumber);
		}
	}
}