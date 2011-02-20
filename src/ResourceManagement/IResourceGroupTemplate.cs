using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceGroupTemplate : IResourceFilter
	{
		bool MatchesConsolidatedUrl(string consolidatedUrl);
		ResourceType ResourceType { get; }
		bool Compress { get; }
		IEnumerable<IResourceGroup> GetGroups(ResourceCollection allResources);
		bool TryGetConsolidatedUrl(string virtualPath, out string consolidatedUrl);
	}
}