namespace AlmWitt.Web.ResourceManagement.Registration.WebForms
{
	/// <summary>
	/// Includes a client script file on a page.
	/// </summary>
	public class ClientScriptInclude : WebResourceIncludeControl
	{
		/// <summary>
		/// Gets or sets the url of the client script.
		/// </summary>
		public string ScriptUrl
		{
			get { return ResourceUrl; }
			set { ResourceUrl = value; }
		}

		protected override IResourceRegistry GetRegistryWithName(string name)
		{
			return ResourceRegistries.NamedScriptRegistry(name);
		}
	}
}
