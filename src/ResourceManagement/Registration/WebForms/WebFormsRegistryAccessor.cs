using System;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement.WebForms
{
	internal class WebFormsRegistryAccessor : IResourceRegistryAccessor
	{
		private const string Default = "Default";
		private readonly ResourceRegistryMap<IResourceRegistry> _scriptRegistries;
		private readonly ResourceRegistryMap<IResourceRegistry> _styleRegistries;

		private readonly Control _control;

		public static WebFormsRegistryAccessor GetInstance(Control control)
		{
			return new WebFormsRegistryAccessor(control);
		}

		internal WebFormsRegistryAccessor(Control control)
		{
			_control = control;
			_scriptRegistries = new ResourceRegistryMap<IResourceRegistry>(CreateScriptRegistry);
			_styleRegistries = new ResourceRegistryMap<IResourceRegistry>(CreateStyleRegistry);
		}

		public IResourceRegistry ScriptRegistry
		{
			get { return _scriptRegistries.GetDefaultRegistry(); }
		}

		public IResourceRegistry NamedScriptRegistry(string name)
		{
			return _scriptRegistries.GetRegistryWithName(name);
		}

		public IResourceRegistry StyleRegistry
		{
			get { return _styleRegistries.GetDefaultRegistry(); }
		}

		public IResourceRegistry NamedStyleRegistry(string name)
		{
			return _styleRegistries.GetRegistryWithName(name);
		}

		private IResourceRegistry CreateScriptRegistry()
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

		private IResourceRegistry CreateStyleRegistry()
		{
			return new WebFormsCssRegistry(_control);
		}
	}
}