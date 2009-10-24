using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.Controls
{
	/// <summary>
	/// Represents a control that includes a web resource.
	/// </summary>
	[Obsolete("Please use AlmWitt.Web.ResourceManagement.WebForms.WebResourceIncludeControl instead.")]
	public abstract class WebResourceInclude : Control
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			//Page.PreRenderComplete += delegate { RegisterInclude(); };
			RegisterInclude();
		}

		/// <summary>
		/// When overridden in the inheriting class, registers the web resource on the page.
		/// </summary>
		protected abstract void RegisterInclude();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			//nothing to render
		}
	}
}
