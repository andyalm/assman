using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class CssFilesConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty(PropertyNames.Groups)]
		[ConfigurationCollection(typeof(CssGroupElementCollection), AddItemName = "group")]
		public CssGroupElementCollection Groups
		{
			get { return (CssGroupElementCollection)this[PropertyNames.Groups]; }
			set { this[PropertyNames.Groups] = value; }
		}

		[ConfigurationProperty(PropertyNames.InclusionRedirects)]
		[ConfigurationCollection(typeof(InclusionRedirectElementCollection))]
		public InclusionRedirectElementCollection InclusionRedirects
		{
			get { return (InclusionRedirectElementCollection)this[PropertyNames.InclusionRedirects]; }
			set { this[PropertyNames.InclusionRedirects] = value; }
		}
	}
}