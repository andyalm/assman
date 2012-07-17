using System;
using System.Collections.Generic;
using System.IO;

namespace Assman.Registration
{
	public class ResourceRegistryMap
	{
		private readonly IDictionary<string, IResourceRegistry> _registries = new Dictionary<string, IResourceRegistry>(Comparers.RegistryNames);
		private readonly Func<string,IResourceRegistry> _createRegistry;

		public ResourceRegistryMap(Func<string,IResourceRegistry> createRegistry)
		{
			_createRegistry = createRegistry;
		}

		public IResourceRegistry GetDefaultRegistry()
		{
			return GetRegistryWithName(ResourceRegistryConfiguration.DefaultRegistryName);
		}

		public IResourceRegistry GetRegistryWithName(string name)
		{
			IResourceRegistry registry;
			if (_registries.TryGetValue(name, out registry))
				return registry;

			registry = _createRegistry(name);
			_registries[name] = registry;

			return registry;
		}

		public IEnumerable<string> GetIncludesFor(string registryName)
		{
			return GetRegistryWithName(registryName).GetIncludes();
		}

		public IEnumerable<Action<TextWriter>> GetInlineBlocksFor(string registryName)
		{
			return GetRegistryWithName(registryName).GetInlineBlocks();
		}
	}
}