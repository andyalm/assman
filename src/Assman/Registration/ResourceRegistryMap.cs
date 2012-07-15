using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assman.Registration
{
	public class ResourceRegistryMap
	{
		private readonly IDictionary<string, IResourceRegistry> _registries = new Dictionary<string, IResourceRegistry>(Comparers.RegistryNames);
		private readonly IDictionary<string, string> _includeOwnerMap = new Dictionary<string, string>(Comparers.VirtualPath);
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
			var includes = from include in GetRegistryWithName(registryName).AsReadable().GetIncludes()
						   where !_includeOwnerMap.ContainsKey(include) 
								 || _includeOwnerMap[include].EqualsVirtualPath(registryName)
						   select include;

			foreach (var include in includes)
			{
				_includeOwnerMap[include] = registryName;
				yield return include;
			}
		}

		public IEnumerable<Action<TextWriter>> GetInlineBlocksFor(string registryName)
		{
			return GetRegistryWithName(registryName).AsReadable().GetInlineBlocks();
		}
	}
}