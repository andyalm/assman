namespace AlmWitt.Web.ResourceManagement.Registration.WebForms
{
	/// <summary>
	/// Includes a css file on a page.
	/// </summary>
	public class CssInclude : WebResourceIncludeControl
	{
		/// <summary>
		/// Gets or sets the url of the css file.
		/// </summary>
		public string StylesheetUrl
		{
			get { return ResourceUrl; }
			set { ResourceUrl = value; }
		}

		protected override IResourceRegistry GetRegistryWithName(string name)
		{
			return ResourceRegistries.NamedStyleRegistry(name);
		}
	}
}
