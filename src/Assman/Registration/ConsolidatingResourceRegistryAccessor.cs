using System;
using System.Linq;

using Assman.Configuration;

namespace Assman.Registration
{
    public class ConsolidatingResourceRegistryAccessor : IResourceRegistryAccessor
    {
        private readonly ResourceRequirementCollection _scriptRequirements;
        private readonly ResourceRequirementCollection _styleRequirements;
        private readonly ResourceRegistryMap _scriptRegistries;
        private readonly ResourceRegistryMap _styleRegistries;

        public ConsolidatingResourceRegistryAccessor(AssmanContext context)
        {
            var versioningStrategy = new ConfiguredVersioningStrategy(() => context.Version);

            //TODO: Consider lessening dependency from AssmanContext to just the path resolvers
            _scriptRequirements = new ResourceRequirementCollection();
            _scriptRegistries =
                new ResourceRegistryMap(registryName =>
                    new ConsolidatingResourceRegistry(_scriptRequirements, registryName, context.ScriptPathResolver,
                                                      versioningStrategy));
            _styleRequirements = new ResourceRequirementCollection();
            _styleRegistries = new ResourceRegistryMap(registryName =>
                    new ConsolidatingResourceRegistry(_styleRequirements, registryName, context.StylePathResolver,
                                                      versioningStrategy));
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

    [Obsolete("This class exists simply so that older versions of the Assman.Mvc package will continue to work.  It will be removed in the next version.")]
    public class GenericResourceRegistryAccessor : ConsolidatingResourceRegistryAccessor
    {
        public GenericResourceRegistryAccessor() : base(AssmanContext.Current) {}
    }

    [Obsolete("This class exists simply so that older versions of the Assman.Mvc package will continue to work.  It will be removed in the next version.")]
    public static class ConsolidationResourceRegistryExtensions
    {
        public static IResourceRegistryAccessor UseConsolidation(this IResourceRegistryAccessor registryAccessor)
        {
            if (registryAccessor is ConsolidatingResourceRegistryAccessor)
            {
                return registryAccessor;
            }
            else
            {
                return new ConsolidatingResourceRegistryAccessor(AssmanContext.Current);
            }
        }
    }
}