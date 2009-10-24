using System;

namespace AlmWitt.Web.ResourceManagement.Controls
{
	/// <summary>
	/// Includes a client script file on a page.
	/// </summary>
	[Obsolete("Please use AlmWitt.Web.ResourceManagement.WebForms.ClientScriptInclude instead.")]
	public class ClientScriptInclude : WebResourceInclude
	{
		private string _scriptUrl;

		/// <summary>
		/// Gets or sets the url of the client script.
		/// </summary>
		public string ScriptUrl
		{
			get { return _scriptUrl; }
			set { _scriptUrl = value; }
		}

		/// <summary>
		/// Registers the client script file on the current page.
		/// </summary>
		protected override void RegisterInclude()
		{
			WebResourceManager.IncludeClientScriptFile(ScriptUrl, this);
		}
	}
}
