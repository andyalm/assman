using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class ClientScriptsConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty(PropertyNames.Groups)]
		[ConfigurationCollection(typeof(ClientScriptGroupElementCollection), AddItemName = "group")]
		public ClientScriptGroupElementCollection Groups
		{
			get { return (ClientScriptGroupElementCollection) this[PropertyNames.Groups]; }
			set { this[PropertyNames.Groups] = value; }
		}

		[ConfigurationProperty(PropertyNames.InclusionRedirects)]
		[ConfigurationCollection(typeof(InclusionRedirectElementCollection))]
		public InclusionRedirectElementCollection InclusionRedirects
		{
			get { return (InclusionRedirectElementCollection) this[PropertyNames.InclusionRedirects]; }
			set { this[PropertyNames.InclusionRedirects] = value; }
		}
	}
}