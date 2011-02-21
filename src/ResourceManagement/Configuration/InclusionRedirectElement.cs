using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class InclusionRedirectElement : ConfigurationElement
	{
		[ConfigurationProperty(PropertyNames.Pattern)]
		public string Pattern
		{
			get { return (string) this[PropertyNames.Pattern]; }
			set { this[PropertyNames.Pattern] = value; }
		}

		[ConfigurationProperty(PropertyNames.RedirectTo)]
		public string RedirectTo
		{
			get { return (string) this[PropertyNames.RedirectTo]; }
			set { this[PropertyNames.RedirectTo] = value; }
		}
	}
}