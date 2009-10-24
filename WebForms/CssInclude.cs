using System;

namespace AlmWitt.Web.ResourceManagement.WebForms
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

		/// <summary>
		/// Gets the type of resource the control will register on the page.
		/// </summary>
		protected override ResourceType ResourceType
		{
			get { return ResourceType.Css; }
		}
	}
}
