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
				return new ConsolidatingResourceRegistryAccessor(registryAccessor, ResourceManagementContext.Current);
			}
		}

		private class ConsolidatingResourceRegistryAccessor : IResourceRegistryAccessor
		{
			private readonly IResourceRegistryAccessor _inner;
			private readonly ResourceManagementContext _context;
			private IResourceRegistry _scriptRegistry;
			private IResourceRegistry _styleRegistry;
			private readonly IDictionary<string, IResourceRegistry> _namedScriptRegistries = new Dictionary<string, IResourceRegistry>(StringComparer.OrdinalIgnoreCase);
			private readonly IDictionary<string, IResourceRegistry> _namedStyleRegistries = new Dictionary<string, IResourceRegistry>(StringComparer.OrdinalIgnoreCase);
			
			//static cache to keep track of resolved url's.  Since resolving can be a relatively expensive operation when you have a large ResourceManagement.config file, we
			//cache the results here so that you only need to resolve a given url once in the app domain.
			//TODO: move caching out of this class and into somewhere else encapsulated behind ResourceManagementContext
			private static readonly IDictionary<string,string> _resolvedUrlCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			
			public ConsolidatingResourceRegistryAccessor(IResourceRegistryAccessor inner, ResourceManagementContext context)
			{
				_inner = inner;
				_context = context;
			}

			public IResourceRegistry ScriptRegistry
			{
				get { return WrapDefaultWithConsolidation(_inner.ScriptRegistry, _context.GetScriptUrl, ref _scriptRegistry); }
			}

			public IResourceRegistry NamedScriptRegistry(string name)
			{
				return WrapNamedWithConsolidation(name, _inner.NamedScriptRegistry, _context.GetScriptUrl, _namedScriptRegistries);
			}

			public IResourceRegistry StyleRegistry
			{
				get { return WrapDefaultWithConsolidation(_inner.StyleRegistry, _context.GetStylesheetUrl, ref _styleRegistry); }
			}

			public IResourceRegistry NamedStyleRegistry(string name)
			{
				return WrapNamedWithConsolidation(name, _inner.NamedStyleRegistry, _context.GetStylesheetUrl, _namedStyleRegistries);
			}

			private IResourceRegistry WrapNamedWithConsolidation(string name, Func<string,IResourceRegistry> getNamedRegistry, Func<string, string> getResourceUrl, IDictionary<string, IResourceRegistry> registryCache)
			{
				IResourceRegistry consolidatingRegistry;
				if(registryCache.TryGetValue(name, out consolidatingRegistry))
				{
					return consolidatingRegistry;
				}

				consolidatingRegistry = WrapWithConsolidation(getNamedRegistry(name), getResourceUrl);
				registryCache[name] = consolidatingRegistry;

				return consolidatingRegistry;
			}

			private IResourceRegistry WrapDefaultWithConsolidation(IResourceRegistry registry, Func<string, string> getResourceUrl, ref IResourceRegistry field)
			{
				if (field != null)
					return field;

				field = WrapWithConsolidation(registry, getResourceUrl);

				return field;
			}

			private IResourceRegistry WrapWithConsolidation(IResourceRegistry registry, Func<string, string> getResourceUrl)
			{
				if (registry is ConsolidatingResourceRegistry || registry is DependencyResolvingResourceRegistry)
				{
					return registry;
				}
				else
				{
					IResourceRegistry consolidatingRegistry = new ConsolidatingResourceRegistry(registry, getResourceUrl, _resolvedUrlCache);
					if(_context.ManageDependencies)
					{
						consolidatingRegistry = new DependencyResolvingResourceRegistry(consolidatingRegistry, _context);
					}

					return consolidatingRegistry;
				}
			}
		}
	}
	
}