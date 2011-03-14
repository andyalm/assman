using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public interface IResourceGroupManager
	{
		void Add(IResourceGroupTemplate template);

		void Clear();

		bool Any();

		bool TryGetConsolidatedUrl(string virtualPath, out string consolidatedUrl);

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