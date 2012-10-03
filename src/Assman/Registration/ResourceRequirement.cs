using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;

namespace Assman.Registration
{
    internal class ResourceRequirement
    {
        public ResourceRequirement(string virtualPath, string registryName, ResourceRequirementReason reason)
        {
            VirtualPath = virtualPath;
            _registryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {registryName};
            Reasons = new HashSet<ResourceRequirementReason> { reason };
        }
        
        public string VirtualPath { get; private set; }

        public string ClaimingRegistry { get; private set; }

        public string[] ResolvedGroupUrls { get; set; }

        public string ChosenGroupUrl { get; set; }
        
        public string[] ResolvedIncludeUrls { get; set; }

        private readonly HashSet<string> _registryNames;
        public IEnumerable<string> RegistryNames
        {
            get { return _registryNames; }
        } 

        public HashSet<ResourceRequirementReason> Reasons { get; private set; }

        public void RequireRegistry(string registryName, ResourceRequirementReason reason)
        {
            if (!_registryNames.Contains(registryName))
                _registryNames.Add(registryName);
            
            if (!Reasons.Contains(reason))
                Reasons.Add(reason);
        }

        public bool RequiredInRegistry(string registryName)
        {
            return _registryNames.Contains(registryName);
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

    internal class ResourceRequirementCollection : KeyedCollection<string,ResourceRequirement>
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
                    var explicitlyRequiredGroups = requirement.ResolvedGroupUrls
                        .Where(url => this.Contains(url) && this[url].Reasons.Contains(ResourceRequirementReason.Explicit))
                        .ToList();
                    if(explicitlyRequiredGroups.Count == 0)
                    {
                        requirement.ChosenGroupUrl = requirement.ResolvedGroupUrls.First();
                    }
                    else if(explicitlyRequiredGroups.Count == 1)
                    {
                        requirement.ChosenGroupUrl = explicitlyRequiredGroups[0];
                    }
                    else
                    {
                        //TODO: Improved this error message
                        throw new NotSupportedException("The resource '" + requirement.VirtualPath + "' resolves to multiple groups and more than one of those groups was explicitly required on the page.  This is not allowed.");
                    }
                }
            }
        }

        public void ClaimAllWithChosenGroup(string chosenGroupUrl, string registryName)
        {
            var requirementsToClaim = this.Where(r => r.ChosenGroupUrl.EqualsVirtualPath(chosenGroupUrl));
            foreach (var requirement in requirementsToClaim)
            {
                requirement.Claim(registryName);
            }
        }

        public void RetroactivelyClaimRequirementsForClaimedGroups()
        {
            foreach (var requirement in this)
            {
                string claimingRegistry;
                if(requirement.IsUnclaimed && IsGroupClaimed(requirement.ChosenGroupUrl, out claimingRegistry))
                {
                    requirement.Claim(claimingRegistry);
                }
            }
        }

        private bool IsGroupClaimed(string chosenGroupUrl, out string claimingRegistry)
        {
            var claimedRequirementInGroup = this.FirstOrDefault(r => r.IsClaimed && r.ChosenGroupUrl.EqualsVirtualPath(chosenGroupUrl));
            if (claimedRequirementInGroup != null)
            {
                claimingRegistry = claimedRequirementInGroup.ClaimingRegistry;
                return true;
            }

            claimingRegistry = null;
            return false;
        }
    }

    public enum ResourceRequirementReason
    {
        Explicit,
        Dependency
    }
}