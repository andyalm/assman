using System;
using System.Collections.Generic;
using System.Web.UI;

namespace AlmWitt.Web.ResourceManagement
{
	internal class ResourceRegistryMap<TResourceRegistry>
	{
		private readonly Dictionary<string, TResourceRegistry> _registries = new Dictionary<string, TResourceRegistry>(StringComparer.OrdinalIgnoreCase);
		private readonly Func<TResourceRegistry> _createRegistry;
		private const string Default = "Default";

		public ResourceRegistryMap(Func<TResourceRegistry> createRegistry)
		{
			_createRegistry = createRegistry;
		}

		public TResourceRegistry GetDefaultRegistry()
		{
			return GetRegistryWithName(Default);
		}

		public TResourceRegistry GetRegistryWithName(string name)
		{
			TResourceRegistry registry;
			if (_registries.TryGetValue(name, out registry))
				return registry;

			registry = _createRegistry();
			_registries[name] = registry;

			return registry;
		}
	}
}