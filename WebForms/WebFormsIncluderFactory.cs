using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal class WebFormsIncluderFactory : IResourceIncluderFactory
	{
		private Control _control;

		public WebFormsIncluderFactory(Control control)
		{
			_control = control;
		}

		public IResourceIncluder CreateClientScriptIncluder()
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(_control.Page);
			if (scriptManager == null)
			{
				return new WebFormsClientScriptIncluder(_control);
			}
			else
			{
				return new WebFormsAjaxScriptIncluder(_control, scriptManager);
			}
		}

		public IResourceIncluder CreateCssIncluder()
		{
			return new WebFormsCssIncluder(_control);
		}
	}
}