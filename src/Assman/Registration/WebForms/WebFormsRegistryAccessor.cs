using System;
using System.Web.UI;

namespace Assman.Registration.WebForms
{
	internal class WebFormsRegistryAccessor : IResourceRegistryAccessor
	{
		private readonly ResourceRegistryMap _scriptRegistries;
		private readonly ResourceRegistryMap _styleRegistries;

		private readonly Control _control;

		public static IResourceRegistryAccessor GetInstance(Control control)
		{	
			return new WebFormsRegistryAccessor(control).UseConsolidation();
		}

		internal WebFormsRegistryAccessor(Control control)
		{
			_control = control;
			_scriptRegistries = new ResourceRegistryMap(CreateScriptRegistry);
			_styleRegistries = new ResourceRegistryMap(CreateStyleRegistry);
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

		public RegisteredResources GetRegisteredScripts(string registryName)
		{
			throw RegistryIsWriteOnlyException();
		}

		public RegisteredResources GetRegisteredStyles(string registryName)
		{
			throw RegistryIsWriteOnlyException();
		}

		private Exception RegistryIsWriteOnlyException()
		{
			return new NotSupportedException("The WebForms implementations of IResourceRegistry write to the ScriptManager and/or ClientScriptManager, which take care of rendering the scripts themselves.  Thus methods of the IResourceRegistryAccessor that read includes and script blocks are not supported in this scenario.");
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