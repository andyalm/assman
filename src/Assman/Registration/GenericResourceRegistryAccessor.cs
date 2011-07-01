using System;
using System.Linq;

namespace Assman.Registration
{
	public class GenericResourceRegistryAccessor : IResourceRegistryAccessor
	{
		private readonly ResourceRegistryMap _scriptRegistries = new ResourceRegistryMap(() => new GenericResourceRegistry());
		private readonly ResourceRegistryMap _styleRegistries = new ResourceRegistryMap(() => new GenericResourceRegistry());
		
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
			return GetRegisteredResources(_scriptRegistries, registryName);
		}

		public RegisteredResources GetRegisteredStyles(string registryName)
		{
			return GetRegisteredResources(_styleRegistries, registryName);
		}

		private RegisteredResources GetRegisteredResources(ResourceRegistryMap registryMap, string registryName)
		{
			return new RegisteredResources
			{
				Includes = registryMap.GetIncludesFor(registryName).ToList(),
				InlineBlocks = registryMap.GetInlineBlocksFor(registryName).ToList()
			};
		}
	}
}