using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceGroupManager
	{
		void Add(IResourceGroupTemplate template);

		void Clear();

		bool Any();

        bool Consolidate { get; set; }

	    string ResolveResourceUrl(string resourceUrl);

	    IEnumerable<string> GetResourceUrlsInGroup(string groupUrl, ResourceMode mode, IResourceFinder finder);
	    
        bool IsGroupUrlWithConsolidationDisabled(string resourceUrl);

	    bool IsConsolidatedUrl(string virtualPath);

	    GroupTemplateContext GetGroupTemplateOrDefault(string consolidatedUrl);

	    IResourceGroup GetGroupOrDefault(string consolidatedUrl, ResourceMode mode, IResourceFinder finder);

	    void EachGroup(IEnumerable<IResource> allResources, ResourceMode mode, Action<IResourceGroup> handler);
	}

	public static class ResourceGroupManagerExtensions
	{
		public static void AddRange(this IResourceGroupManager groupManager, IEnumerable<IResourceGroupTemplate> templates)
		{
			foreach (var template in templates)
			{
				groupManager.Add(template);
			}
		}
	}
}