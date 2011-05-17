using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman.PreConsolidation
{
	public class PreConsolidatedGroupManager : IResourceGroupManager
	{
		private readonly IDictionary<string,string> _resourceUrlMap = new Dictionary<string, string>(Comparers.VirtualPath);
		private readonly IDictionary<string, IEnumerable<string>> _consolidatedUrlMap = new Dictionary<string,IEnumerable<string>>(Comparers.VirtualPath);

		public PreConsolidatedGroupManager(IEnumerable<PreConsolidatedResourceGroup> preConsolidatedGroups)
		{
			PopulateResourceUrlMap(preConsolidatedGroups);
			Consolidate = true;
		}

		public void Add(IResourceGroupTemplate template) {}
		public void Clear() {}
		public bool Any()
		{
			return true;
		}

		public bool Consolidate { get; set; }

		public string ResolveResourceUrl(string resourceUrl)
		{
			string resolvedPath;
			if (_resourceUrlMap.TryGetValue(resourceUrl, out resolvedPath))
				return resolvedPath;
			else
				return resourceUrl;
		}

		public bool IsGroupUrlWithConsolidationDisabled(string resourceUrl)
		{
			return !Consolidate && IsConsolidatedUrl(resourceUrl);
		}

		public IEnumerable<string> GetResourceUrlsInGroup(string consolidatedUrl, ResourceMode mode, IResourceFinder finder)
		{
			IEnumerable<string> resourceUrls;
			if (_consolidatedUrlMap.TryGetValue(consolidatedUrl, out resourceUrls))
				return resourceUrls;
			else
				return Enumerable.Empty<string>();
		}

		public bool IsConsolidatedUrl(string virtualPath)
		{
			return _consolidatedUrlMap.ContainsKey(virtualPath);
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

		public bool IsPartOfGroup(IResource resource)
		{
			throw NotSupported();
		}

		public IEnumerable<string> GetGlobalDependencies()
		{
			throw NotSupported();
		}

		public void AddGlobalDependencies(IEnumerable<string> paths)
		{
			//no implementation needed as the global dependencies should have been included for each resource in the pre-consolidation report
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

			foreach (var @group in groups)
			{
				_resourceUrlMap.Add(@group.ConsolidatedUrl, @group.ConsolidatedUrl);
				_consolidatedUrlMap.Add(@group.ConsolidatedUrl, @group.Resources);
			}
		}
	}
}