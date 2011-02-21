using System.Configuration;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class InclusionRedirectElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new InclusionRedirectElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return null;
		}
	}
}