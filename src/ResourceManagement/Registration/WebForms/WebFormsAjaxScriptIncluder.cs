using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal class WebFormsAjaxScriptIncluder : WebFormsClientScriptRegistry
	{
		private readonly ScriptManager _scriptManager;

		public WebFormsAjaxScriptIncluder(Control control, ScriptManager scriptManager)
			: base(control)
		{
			_scriptManager = scriptManager;
		}

		public override void IncludePath(string urlToInclude)
		{
			//we have to check to see if a reference exists already
			//because the ScriptManager is case-sensitive when removing
			//duplicate references.  We could work around this by doing a
			//ToLower for all of our url's, but I think that's ugly.  I want
			//to be able to include my js files with proper casing without
			//including duplicates if someone accidentally uses a different casing.
			if (!ContainsReference(urlToInclude))
				_scriptManager.Scripts.Add(CreateFileReference(urlToInclude));
		}

		private bool ContainsReference(string url)
		{
			foreach (ScriptReference reference in _scriptManager.Scripts)
			{
				if (url.Equals(reference.Path, StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return false;
		}

		private static ScriptReference CreateFileReference(string url)
		{
			ScriptReference scriptReference = new ScriptReference();
			scriptReference.Path = url;
			return scriptReference;
		}
	}
}