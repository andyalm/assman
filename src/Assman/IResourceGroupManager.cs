using System;
using System.Collections.Generic;

namespace Assman
{
	public interface IResourceGroupManager
	{
		void Add(IResourceGroupTemplate template);

		void Clear();

		bool Any();

		bool Consolidate { get; set; }

		bool MutuallyExclusiveGroups { get; set; }

		string ResolveResourceUrl(string resourceUrl);

		IEnumerable<string> GetResourceUrlsInGroup(string groupUrl, IResourceFinder finder);
		
		bool IsGroupUrlWithConsolidationDisabled(string resourceUrl);

		bool IsConsolidatedUrl(string virtualPath);

		GroupTemplateContext GetGroupTemplateOrDefault(string consolidatedUrl);

		IResourceGroup GetGroupOrDefault(string consolidatedUrl, IResourceFinder finder);

		void EachGroup(IEnumerable<IResource> allResources, Action<IResourceGroup> handler);

		bool IsPartOfGroup(string virtualPath);
		
		IEnumerable<string> GetGlobalDependencies();

		void AddGlobalDependencies(IEnumerable<string> paths);
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