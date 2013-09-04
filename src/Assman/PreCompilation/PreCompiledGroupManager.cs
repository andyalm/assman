using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman.PreCompilation
{
	public class PreCompiledGroupManager : IResourceGroupManager
	{
		private readonly IResourceGroupManager _inner;
		private readonly IDictionary<string,List<string>> _unconsolidatedUrlMap = new Dictionary<string, List<string>>(Comparers.VirtualPath);
		private readonly IDictionary<string, IEnumerable<string>> _consolidatedUrlMap = new Dictionary<string,IEnumerable<string>>(Comparers.VirtualPath);

		public PreCompiledGroupManager(PreCompiledResourceReport resourceReport, IResourceGroupManager inner)
		{
			_inner = inner;
			PopulateResourceUrlMap(resourceReport);
			Consolidate = true;
			MutuallyExclusiveGroups = inner.MutuallyExclusiveGroups;
		}

		public void Add(IResourceGroupTemplate template) {}
		public void Clear() {}
		public bool Any()
		{
			return true;
		}

		public bool Consolidate { get; set; }

		public bool MutuallyExclusiveGroups { get; set; }

		public bool IsGroupUrlWithConsolidationDisabled(string resourceUrl)
		{
			return !Consolidate && IsConsolidatedUrl(resourceUrl);
		}

		public IEnumerable<string> GetResourceUrlsInGroup(string consolidatedUrl, IResourceFinder finder)
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

		public IEnumerable<string> GetMatchingConsolidatedUrls(string resourceUrl)
		{
			List<string> consolidatedUrls;
			if (_unconsolidatedUrlMap.TryGetValue(resourceUrl, out consolidatedUrls))
			{
				return consolidatedUrls;
			}
			else
			{
				//if the url was not in the prepopulated resource url map, then it is still possible that it matches a pattern
				//of a group.  Thus, we need to check the inner group manager.  This is an edge case that should really only happen
				//if you are trying to include a resource path that does not actually exist.  However, we need to handle it
				//because otherwise things will work differently when running in a precompiled state, and that would not be good.
				consolidatedUrls = _inner.GetMatchingConsolidatedUrls(resourceUrl).ToList();
				_unconsolidatedUrlMap[resourceUrl] = consolidatedUrls;
				return consolidatedUrls;
			}
		}

		public GroupTemplateContext GetGroupTemplateOrDefault(string consolidatedUrl)
		{
			throw NotSupported();
		}

		public IResourceGroup GetGroupOrDefault(string consolidatedUrl, IResourceFinder finder)
		{
			throw NotSupported();
		}

		public void EachGroup(IEnumerable<IResource> allResources, Action<IResourceGroup> handler)
		{
			throw NotSupported();
		}

		public bool IsPartOfGroup(string virtualPath)
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

		private void PopulateResourceUrlMap(PreCompiledResourceReport resourceReport)
		{
			var resourceUrlMap = from @group in resourceReport.Groups
								 from resourcePath in @group.Resources
								 select new KeyValuePair<string, string>(resourcePath, @group.ConsolidatedUrl);

			foreach (var map in resourceUrlMap)
			{
				List<string> consolidatedUrls;
				if(_unconsolidatedUrlMap.TryGetValue(map.Key, out consolidatedUrls))
					consolidatedUrls.Add(map.Value);
				else
					_unconsolidatedUrlMap.Add(map.Key, new List<string> { map.Value });
			}

			foreach (var @group in resourceReport.Groups)
			{
                if (!_unconsolidatedUrlMap.ContainsKey(@group.ConsolidatedUrl))
				_unconsolidatedUrlMap.Add(@group.ConsolidatedUrl, new List<string> { @group.ConsolidatedUrl});
                if (!_consolidatedUrlMap.ContainsKey(@group.ConsolidatedUrl))
				_consolidatedUrlMap.Add(@group.ConsolidatedUrl, @group.Resources);
			}
		}
	}
}