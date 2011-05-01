using System;
using System.Collections.Generic;
using System.Configuration;

namespace Assman.Configuration
{
	public class PluginElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new PluginElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			var finderElement = (PluginElement) element;

			return finderElement.Type ?? String.Empty;
		}

		public IEnumerable<IResourceManagementPlugin> GetPlugins()
		{
			foreach (PluginElement finderElement in this)
			{
				yield return finderElement.CreatePlugin();
			}
		}
	}
}