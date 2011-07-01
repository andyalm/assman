using System;
using System.Collections.Generic;

namespace Assman
{
	public interface IResourceGroupTemplate : IResourceFilter
	{
		bool Consolidate { get; }
		bool MatchesConsolidatedUrl(string consolidatedUrl);
		bool IsMatch(string virtualPath);
		ResourceType ResourceType { get; }
		bool Minify { get; }
		IEnumerable<IResourceGroup> GetGroups(IEnumerable<IResource> allResources, ResourceMode mode);
		bool TryGetConsolidatedUrl(string virtualPath, out string consolidatedUrl);
	}
}