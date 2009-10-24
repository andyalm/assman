using System;

namespace AlmWitt.Web.ResourceManagement.WebForms
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

		/// <summary>
		/// Gets the type of resource the control will register on the page.
		/// </summary>
		protected override ResourceType ResourceType
		{
			get { return ResourceType.ClientScript; }
		}
	}
}
