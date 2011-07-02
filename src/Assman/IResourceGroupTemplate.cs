using System;
using System.Collections.Generic;

using Assman.Configuration;

namespace Assman
{
	public interface IResourceGroupTemplate : IResourceFilter
	{
		ResourceModeCondition Consolidate { get; }
		bool MatchesConsolidatedUrl(string consolidatedUrl);
		bool IsMatch(string virtualPath);
		ResourceType ResourceType { get; }
		ResourceModeCondition Minify { get; }
		IEnumerable<IResourceGroup> GetGroups(IEnumerable<IResource> allResources, ResourceMode mode);
		bool TryGetConsolidatedUrl(string virtualPath, ResourceMode resourceMode, out string consolidatedUrl);
	}
}