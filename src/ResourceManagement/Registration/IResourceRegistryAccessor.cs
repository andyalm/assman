using System;

using AlmWitt.Web.ResourceManagement.Registration;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceRegistryAccessor
	{
		IResourceRegistry ScriptRegistry { get; }

		IResourceRegistry NamedScriptRegistry(string name);

		IResourceRegistry StyleRegistry { get; }

		IResourceRegistry NamedStyleRegistry(string name);

		RegisteredResources GetRegisteredScripts(string registryName);

		RegisteredResources GetRegisteredStyles(string registryName);
	}
}