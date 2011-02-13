using System;
using System.Collections.Generic;

using AlmWitt.Web.ResourceManagement.Configuration;

namespace AlmWitt.Web.ResourceManagement.Registration
{
	public static class ConsolidationResourceRegistryExtensions
	{
		public static IResourceRegistryAccessor UseConsolidation(this IResourceRegistryAccessor registryAccessor)
		{
			if(registryAccessor is ConsolidatingResourceRegistryAccessor)
			{
				return registryAccessor;
			}
			else
			{
				return new ConsolidatingResourceRegistryAccessor(registryAccessor, ResourceManagementConfiguration.Current);
			}
		}

		private class ConsolidatingResourceRegistryAccessor : IResourceRegistryAccessor
		{
			private readonly IResourceRegistryAccessor _inner;
			private readonly ResourceManagementConfiguration _config;
			private ConsolidatingResourceRegistry _scriptRegistry;
			private ConsolidatingResourceRegistry _styleRegistry;
			private readonly IDictionary<string, ConsolidatingResourceRegistry> _namedScriptRegistries = new Dictionary<string, ConsolidatingResourceRegistry>(StringComparer.OrdinalIgnoreCase);
			private readonly IDictionary<string, ConsolidatingResourceRegistry> _namedStyleRegistries = new Dictionary<string, ConsolidatingResourceRegistry>(StringComparer.OrdinalIgnoreCase);
			
			//static cache to keep track of resolved url's.  Since resolving can be a relatively expensive operation when you have a large ResourceManagement.config file, we
			//cache the results here so that you only need to resolve a given url once in the app domain.
			private static readonly IDictionary<string,string> _resolvedUrlCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			
			public ConsolidatingResourceRegistryAccessor(IResourceRegistryAccessor inner, ResourceManagementConfiguration config)
			{
				_inner = inner;
				_config = config;
			}

			public IScriptRegistry ScriptRegistry
			{
				get { return WrapDefaultWithConsolidation(_inner.ScriptRegistry, _config.GetScriptUrl, ref _scriptRegistry); }
			}

			public IScriptRegistry NamedScriptRegistry(string name)
			{
				return WrapNamedWithConsolidation(name, _inner.NamedScriptRegistry, _config.GetScriptUrl, _namedScriptRegistries);
			}

			public IStyleRegistry StyleRegistry
			{
				get { return WrapDefaultWithConsolidation(_inner.StyleRegistry, _config.GetStylesheetUrl, ref _styleRegistry); }
			}

			public IStyleRegistry NamedStyleRegistry(string name)
			{
				return WrapNamedWithConsolidation(name, _inner.NamedStyleRegistry, _config.GetStylesheetUrl, _namedStyleRegistries);
			}

			private ConsolidatingResourceRegistry WrapNamedWithConsolidation(string name, Func<string,IResourceRegistry> getNamedRegistry, Func<string, string> getResourceUrl, IDictionary<string, ConsolidatingResourceRegistry> registryCache)
			{
				ConsolidatingResourceRegistry consolidatingRegistry;
				if(registryCache.TryGetValue(name, out consolidatingRegistry))
				{
					return consolidatingRegistry;
				}

				consolidatingRegistry = WrapWithConsolidation(getNamedRegistry(name), getResourceUrl);
				registryCache[name] = consolidatingRegistry;

				return consolidatingRegistry;
			}

			private ConsolidatingResourceRegistry WrapDefaultWithConsolidation(IResourceRegistry registry, Func<string, string> getResourceUrl, ref ConsolidatingResourceRegistry field)
			{
				if (field != null)
					return field;

				field = WrapWithConsolidation(registry, getResourceUrl);

				return field;
			}

			private ConsolidatingResourceRegistry WrapWithConsolidation(IResourceRegistry registry, Func<string, string> getResourceUrl)
			{
				if (registry is ConsolidatingResourceRegistry)
				{
					return (ConsolidatingResourceRegistry)registry;
				}
				else
				{
					return new ConsolidatingResourceRegistry(registry, _config, getResourceUrl, _resolvedUrlCache);
				}
			}
		}
	}
	
}