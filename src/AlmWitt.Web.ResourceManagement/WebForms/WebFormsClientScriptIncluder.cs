using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal class WebFormsClientScriptIncluder : WebFormsIncluderBase
	{
		public WebFormsClientScriptIncluder(Control control) : base(control)
		{
		}

		public override void IncludeUrl(string urlToInclude)
		{
			Page.ClientScript.RegisterClientScriptInclude(typeof(WebFormsClientScriptIncluder), urlToInclude, urlToInclude);
		}
	}
}