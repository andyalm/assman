using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal class WebFormsCssIncluder : WebFormsIncluderBase
	{
		const string linkTemplate = "<link type=\"text/css\" rel=\"Stylesheet\" href=\"{0}\" />";
		
		public WebFormsCssIncluder(Control control) : base(control) { }

		public override void IncludeUrl(string urlToInclude)
		{
			if(Page.Header != null)
			{
				Page.PreRenderComplete += delegate
				{
					Page.Header.Controls.Add(new LiteralControl(String.Format(linkTemplate, urlToInclude)));
				};
			}
			else
			{
				Page.ClientScript.RegisterClientScriptBlock(typeof(Page), urlToInclude, urlToInclude, false);
			}
		}
	}
}