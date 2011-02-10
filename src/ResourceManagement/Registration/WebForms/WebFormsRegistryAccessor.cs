using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal class WebFormsRegistryAccessor : IResourceRegistryAccessor
	{
		private const string Default = "Default";
		private readonly ResourceRegistryMap<IScriptRegistry> _scriptRegistries;
		private readonly ResourceRegistryMap<IStyleRegistry> _styleRegistries;

		private readonly Control _control;

		public static WebFormsRegistryAccessor GetInstance(Control control)
		{
			return new WebFormsRegistryAccessor(control);
		}

		internal WebFormsRegistryAccessor(Control control)
		{
			_control = control;
			_scriptRegistries = new ResourceRegistryMap<IScriptRegistry>(CreateScriptRegistry);
			_styleRegistries = new ResourceRegistryMap<IStyleRegistry>(CreateStyleRegistry);
		}

		public IScriptRegistry ScriptRegistry
		{
			get { return _scriptRegistries.GetDefaultRegistry(); }
		}

		public IScriptRegistry NamedScriptRegistry(string name)
		{
			return _scriptRegistries.GetRegistryWithName(name);
		}

		public IStyleRegistry StyleRegistry
		{
			get { return _styleRegistries.GetDefaultRegistry(); }
		}

		public IStyleRegistry NamedStyleRegistry(string name)
		{
			return _styleRegistries.GetRegistryWithName(name);
		}

		private IScriptRegistry CreateScriptRegistry()
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(_control.Page);
			if (scriptManager == null)
			{
				return new WebFormsClientScriptRegistry(_control);
			}
			else
			{
				return new WebFormsAjaxScriptIncluder(_control, scriptManager);
			}
		}

		private IStyleRegistry CreateStyleRegistry()
		{
			return new WebFormsCssRegistry(_control);
		}
	}
}