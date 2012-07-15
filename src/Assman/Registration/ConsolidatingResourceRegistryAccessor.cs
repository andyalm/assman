using System;
using System.Linq;

using Assman.Configuration;

namespace Assman.Registration
{
	public static class ConsolidationResourceRegistryExtensions
	{
		//TODO: collapse this with the inner IResourceRegistryAccessor to simplify things
		public static IResourceRegistryAccessor UseConsolidation(this IResourceRegistryAccessor registryAccessor)
		{
			if(registryAccessor is ConsolidatingResourceRegistryAccessor)
			{
				return registryAccessor;
			}
			else
			{
				return new ConsolidatingResourceRegistryAccessor(AssmanContext.Current);
			}
		}

		private class ConsolidatingResourceRegistryAccessor : IResourceRegistryAccessor
		{
			private readonly ResourceRequirementCollection _requirements;
			private readonly ResourceRegistryMap _scriptRegistries;
			private readonly ResourceRegistryMap _styleRegistries;
			
			public ConsolidatingResourceRegistryAccessor(AssmanContext context)
			{
				//TODO: Consider lessening dependency from AssmanContext to just the path resolvers
				_requirements = new ResourceRequirementCollection();
			    var versioningStrategy = new ConfiguredVersioningStrategy(() => context.Version);
				_scriptRegistries = new ResourceRegistryMap(registryName => new ConsolidatingResourceRegistry(_requirements, registryName, context.ScriptPathResolver, versioningStrategy));
				_styleRegistries = new ResourceRegistryMap(registryName => new ConsolidatingResourceRegistry(_requirements, registryName, context.StylePathResolver, versioningStrategy));
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
	
}