using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;

namespace Assman.Registration
{
    public class ResourceRequirement
    {
        public ResourceRequirement(string virtualPath, string registryName, ResourceRequirementReason reason)
        {
            VirtualPath = virtualPath;
            RegistryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {registryName};
            Reasons = new HashSet<ResourceRequirementReason> { reason };
        }
        
        public string VirtualPath { get; private set; }

        public string ClaimingRegistry { get; private set; }

        public string[] ResolvedGroupUrls { get; set; }

        public string ChosenGroupUrl { get; set; }
        
        public string[] ResolvedIncludeUrls { get; set; }

        private HashSet<string> RegistryNames { get; set; }

        public HashSet<ResourceRequirementReason> Reasons { get; private set; }

        public void RequireRegistry(string registryName, ResourceRequirementReason reason)
        {
            if (!RegistryNames.Contains(registryName))
                RegistryNames.Add(registryName);
            
            if (!Reasons.Contains(reason))
                Reasons.Add(reason);
        }

        public bool RequiredInRegistry(string registryName)
        {
            return RegistryNames.Contains(registryName);
        }

        public void Claim(string registryName)
        {
            if(IsClaimed && !ClaimingRegistry.Equals(registryName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The registry '" + registryName + "' cannot claim the resource requriment '" + VirtualPath + "' because it has already been claimed by another registry ('" + ClaimingRegistry + "').");
            ClaimingRegistry = registryName;
        }

        public bool IsClaimedBy(string registryName)
        {
            return IsClaimed && ClaimingRegistry.Equals(registryName, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsClaimed
        {
            get { return ClaimingRegistry != null; }
        }

        public bool IsUnclaimed
        {
            get { return !IsClaimed; }
        }
    }

    public class ResourceRequirementCollection : KeyedCollection<string,ResourceRequirement>
    {
        public ResourceRequirementCollection() : base(StringComparer.OrdinalIgnoreCase) {}
        
        protected override string GetKeyForItem(ResourceRequirement item)
        {
            return item.VirtualPath;
        }

        public bool TryGetRequirement(string virtualPath, out ResourceRequirement requirement)
        {
            if (Contains(virtualPath))
            {
                requirement = this[virtualPath];
                return true;
            }
            else
            {
                requirement = null;
                return false;
            }
        }

        public IEnumerable<ResourceRequirement> GetRequirementsForRegistry(string registryName)
        {
            return Items.Where(r => r.RequiredInRegistry(registryName) && (r.IsUnclaimed || r.IsClaimedBy(registryName))).ToList();
        }

        public void AddOrRequireRegistry(string virtualPath, string registryName, ResourceRequirementReason reason)
        {
            ResourceRequirement requirement;
            if(TryGetRequirement(virtualPath, out requirement))
                requirement.RequireRegistry(registryName, reason);
            else
                Add(new ResourceRequirement(virtualPath, registryName, reason));
        }

        public void DisambiguateMultipleGroupMatches()
        {
            foreach (var requirement in this)
            {
                if (requirement.ChosenGroupUrl == null)
                {
                    //TODO: Make choosing algorithm smarter by looking at other matches
                    requirement.ChosenGroupUrl = requirement.ResolvedGroupUrls.First();
                }
            }
        }
    }

    public enum ResourceRequirementReason
    {
        Explicit,
        Dependency
    }
}