using System;
using System.Collections.Generic;

using Assman.Configuration;

namespace Assman.Registration
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
				return new ConsolidatingResourceRegistryAccessor(registryAccessor, AssmanContext.Current);
			}
		}

		private class ConsolidatingResourceRegistryAccessor : IResourceRegistryAccessor
		{
			private readonly IResourceRegistryAccessor _inner;
			private readonly AssmanContext _context;
			private IResourceRegistry _scriptRegistry;
			private IResourceRegistry _styleRegistry;
			private readonly IDictionary<string, IResourceRegistry> _namedScriptRegistries = new Dictionary<string, IResourceRegistry>(Comparers.RegistryNames);
			private readonly IDictionary<string, IResourceRegistry> _namedStyleRegistries = new Dictionary<string, IResourceRegistry>(Comparers.RegistryNames);
			
			public ConsolidatingResourceRegistryAccessor(IResourceRegistryAccessor inner, AssmanContext context)
			{
				_inner = inner;
				_context = context;
			}

			public IResourceRegistry ScriptRegistry
			{
				get { return WrapDefaultWithConsolidation(_inner.ScriptRegistry, _context.GetScriptUrls, ref _scriptRegistry); }
			}

			public IResourceRegistry NamedScriptRegistry(string name)
			{
				return WrapNamedWithConsolidation(name, _inner.NamedScriptRegistry, _context.GetScriptUrls, _namedScriptRegistries);
			}

			public IResourceRegistry StyleRegistry
			{
				get { return WrapDefaultWithConsolidation(_inner.StyleRegistry, _context.GetStylesheetUrls, ref _styleRegistry); }
			}

			public IResourceRegistry NamedStyleRegistry(string name)
			{
				return WrapNamedWithConsolidation(name, _inner.NamedStyleRegistry, _context.GetStylesheetUrls, _namedStyleRegistries);
			}

			public RegisteredResources GetRegisteredScripts(string registryName)
			{
				return _inner.GetRegisteredScripts(registryName);
			}

			public RegisteredResources GetRegisteredStyles(string registryName)
			{
				return _inner.GetRegisteredStyles(registryName);
			}

			private IResourceRegistry WrapNamedWithConsolidation(string name, Func<string,IResourceRegistry> getNamedRegistry, Func<string, IEnumerable<string>> getResourceUrls, IDictionary<string, IResourceRegistry> registryCache)
			{
				IResourceRegistry consolidatingRegistry;
				if(registryCache.TryGetValue(name, out consolidatingRegistry))
				{
					return consolidatingRegistry;
				}

				consolidatingRegistry = WrapWithConsolidation(getNamedRegistry(name), getResourceUrls);
				registryCache[name] = consolidatingRegistry;

				return consolidatingRegistry;
			}

			private IResourceRegistry WrapDefaultWithConsolidation(IResourceRegistry registry, Func<string, IEnumerable<string>> getResourceUrls, ref IResourceRegistry field)
			{
				if (field != null)
					return field;

				field = WrapWithConsolidation(registry, getResourceUrls);

				return field;
			}

			private IResourceRegistry WrapWithConsolidation(IResourceRegistry registry, Func<string, IEnumerable<string>> getResourceUrls)
			{
				if (registry is ConsolidatingResourceRegistry || registry is DependencyResolvingResourceRegistry)
				{
					return registry;
				}
				else
				{
					IResourceRegistry consolidatingRegistry = new ConsolidatingResourceRegistry(registry, getResourceUrls);
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