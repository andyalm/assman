using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class PreConsolidatedGroupManager : IResourceGroupManager
	{
		private readonly IDictionary<string,string> _resourceUrlMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private readonly HashSet<string> _consolidatedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public PreConsolidatedGroupManager(IEnumerable<PreConsolidatedResourceGroup> preConsolidatedGroups)
		{
			PopulateResourceUrlMap(preConsolidatedGroups);
		}

		public void Add(IResourceGroupTemplate template) {}
		public void Clear() {}
		public bool Any()
		{
			return true;
		}

		public string GetResourceUrl(string virtualPath)
		{
			string resolvedPath;
			if (_resourceUrlMap.TryGetValue(virtualPath, out resolvedPath))
				return resolvedPath;
			else
				return virtualPath;
		}

		public bool IsConsolidatedUrl(string virtualPath)
		{
			return _consolidatedUrls.Contains(virtualPath);
		}

		public GroupTemplateContext GetGroupTemplateOrDefault(string consolidatedUrl)
		{
			throw NotSupported();
		}

		public IResourceGroup GetGroupOrDefault(string consolidatedUrl, ResourceMode mode, IResourceFinder finder)
		{
			throw NotSupported();
		}

		public void EachGroup(IEnumerable<IResource> allResources, ResourceMode mode, Action<IResourceGroup> handler)
		{
			throw NotSupported();
		}

		private Exception NotSupported()
		{
			return new NotSupportedException("This method is not supported when resources have been pre-consolidated");
		}

		private void PopulateResourceUrlMap(IEnumerable<PreConsolidatedResourceGroup> groups)
		{
			var resourceUrlMap = from @group in groups
								 from resourcePath in @group.Resources
								 select new KeyValuePair<string, string>(resourcePath, @group.ConsolidatedUrl);

			_resourceUrlMap.AddRange(resourceUrlMap);

			var consolidatedUrls = from @group in groups
								   select new KeyValuePair<string, string>(@group.ConsolidatedUrl, @group.ConsolidatedUrl);

			_resourceUrlMap.AddRange(consolidatedUrls);
			_consolidatedUrls.AddRange(consolidatedUrls.Select(p => p.Key));
		}
	}
}