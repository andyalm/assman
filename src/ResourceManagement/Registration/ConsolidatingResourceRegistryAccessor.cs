using System;

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

			public ConsolidatingResourceRegistryAccessor(IResourceRegistryAccessor inner, ResourceManagementConfiguration config)
			{
				_inner = inner;
				_config = config;
			}

			public IScriptRegistry ScriptRegistry
			{
				get { return WrapWithConsolidation(_inner.ScriptRegistry, _config.GetScriptUrl, ref _scriptRegistry); }
			}

			public IScriptRegistry NamedScriptRegistry(string name)
			{
				return WrapWithConsolidation(_inner.NamedScriptRegistry(name), _config.GetScriptUrl);
			}

			public IStyleRegistry StyleRegistry
			{
				get { return WrapWithConsolidation(_inner.StyleRegistry, _config.GetStylesheetUrl, ref _styleRegistry); }
			}

			public IStyleRegistry NamedStyleRegistry(string name)
			{
				return WrapWithConsolidation(_inner.NamedScriptRegistry(name), _config.GetStylesheetUrl);
			}

			private ConsolidatingResourceRegistry WrapWithConsolidation(IResourceRegistry registry, Func<string, string> getResourceUrl, ref ConsolidatingResourceRegistry field)
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
					return new ConsolidatingResourceRegistry(registry, _config, getResourceUrl);
				}
			}
		}
	}
	
}