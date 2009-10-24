using System;

namespace AlmWitt.Web.ResourceManagement.Controls
{
	/// <summary>
	/// Includes a css file on a page.
	/// </summary>
	[Obsolete("Please use AlmWitt.Web.ResourceManagement.WebForms.CssInclude instead.")]
	public class CssInclude : WebResourceInclude
	{
		private string _stylesheetUrl;

		/// <summary>
		/// Gets or sets the url of the css file.
		/// </summary>
		public string StylesheetUrl
		{
			get { return _stylesheetUrl; }
			set { _stylesheetUrl = value; }
		}

		/// <summary>
		/// Registers the css file on the page.
		/// </summary>
		protected override void RegisterInclude()
		{
			WebResourceManager.IncludeCssFile(StylesheetUrl, this);
		}
	}
}
