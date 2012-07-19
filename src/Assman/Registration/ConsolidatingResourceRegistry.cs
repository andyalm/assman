using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assman.Registration
{
	/// <summary>
	/// Represents a <see cref="IResourceRegistry"/> that consolidates script or css includes that
	/// are registered through it.
	/// </summary>
	/// <remarks>
	/// This is a proxy for an inner <see cref="IResourceRegistry"/> which means it can wrap any implementation of a
	/// <see cref="IResourceRegistry"/>.
	/// </remarks>
	public class ConsolidatingResourceRegistry : IReadableResourceRegistry
	{
		private readonly Dictionary<object, Action<TextWriter>> _inlineBlocks = new Dictionary<object, Action<TextWriter>>();
		private readonly ResourceRequirementCollection _requirements = new ResourceRequirementCollection();
		private readonly string _registryName;
		private readonly IResourcePathResolver _pathResolver;
		private readonly IResourceVersioningStrategy _versioningStrategy;

		internal ConsolidatingResourceRegistry(ResourceRequirementCollection requirements, string registryName, IResourcePathResolver pathResolver, IResourceVersioningStrategy versioningStrategy)
		{
			_requirements = requirements;
			_registryName = registryName;
			_pathResolver = pathResolver;
			_versioningStrategy = versioningStrategy;
		}

		public void Require(string resourcePath)
		{
			var canonicalPath = ToCanonicalUrl(resourcePath);
			RequireDependencies(canonicalPath);
			ResourceRequirement requirement;
			if(_requirements.TryGetRequirement(canonicalPath, out requirement))
			{
				requirement.RequireRegistry(_registryName, ResourceRequirementReason.Explicit);
			}
			else
			{
				_requirements.Add(new ResourceRequirement(canonicalPath, _registryName, ResourceRequirementReason.Explicit));
			}
		}

		private void RequireDependencies(string canonicalPath)
		{
			foreach (var dependencyUrl in _pathResolver.GetDependencyUrls(canonicalPath))
			{
				_requirements.AddOrRequireRegistry(dependencyUrl, _registryName, ResourceRequirementReason.Dependency);
			}
		}

		public void RegisterInlineBlock(Action<TextWriter> block, object key)
		{
			if (key == null)
			{
				_inlineBlocks.Add(block, block);
			}
			else
			{
				if (!_inlineBlocks.ContainsKey(key))
				{
					_inlineBlocks.Add(key, block);
				}
			}
		}

		public bool IsInlineBlockRegistered(object key)
		{
			return _inlineBlocks.ContainsKey(key);
		}

		public IEnumerable<Action<TextWriter>> GetInlineBlocks()
		{
			return _inlineBlocks.Values;
		}

		public IEnumerable<string> GetIncludes()
		{
			GetMatchingGroupUrls();
			DisambiguateMultipleGroupMatches();
			ExpandGroupsIfConsolidationDisabled();
			ClaimRequirementsForThisRegistry();

			return _requirements.Where(r => r.IsClaimedBy(_registryName))
				.SelectMany(r => r.ResolvedIncludeUrls)
				.Distinct(Comparers.VirtualPath)
				.Select(url => _versioningStrategy.ApplyVersion(url))
				.ToList();
		}

		private string ToCanonicalUrl(string url)
		{
			return url.ToAppRelativePath();
		}

		private void GetMatchingGroupUrls()
		{
			foreach (var requirement in _requirements)
			{
				if (requirement.ResolvedGroupUrls == null)
				{
					requirement.ResolvedGroupUrls = _pathResolver.GetMatchingGroupUrls(requirement.VirtualPath).ToArray();
					if (!requirement.ResolvedGroupUrls.Any())
						requirement.ResolvedGroupUrls = new[] {requirement.VirtualPath};
				}
			}
		}

		private void ExpandGroupsIfConsolidationDisabled()
		{
			foreach (var requirement in _requirements)
			{
				if(requirement.ResolvedIncludeUrls == null)
				{
					requirement.ResolvedIncludeUrls = _pathResolver.ResolveGroupUrl(requirement.ChosenGroupUrl).ToArray();
					var embeddedResourceUrl = requirement.ResolvedIncludeUrls.FirstOrDefault(EmbeddedResource.IsVirtualPath);
					if (embeddedResourceUrl != null)
					{
						throw new InvalidOperationException(
							@"Cannot include embedded resource because it has not been configured in the Assman.config to be consolidated anywhere.
					Please add an include rule that matches the path '" + embeddedResourceUrl + "'.");
					}    
				}
			}
		}

		private void DisambiguateMultipleGroupMatches()
		{
			_requirements.DisambiguateMultipleGroupMatches();
		}

		private void ClaimRequirementsForThisRegistry()
		{
			foreach (var requirement in _requirements)
			{
				if (requirement.RequiredInRegistry(_registryName) && requirement.IsUnclaimed)
				{
				    _requirements.ClaimAllWithChosenGroup(requirement.ChosenGroupUrl, _registryName);
				}
			}
		}
	}
}