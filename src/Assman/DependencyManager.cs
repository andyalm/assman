using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
{
	public class DependencyManager
	{
		private readonly IResourceFinder _resourceFinder;
		private IDependencyCache _dependencyCache;
		private readonly IResourceGroupManager _scriptGroups;
		private readonly IResourceGroupManager _styleGroups;
		private readonly IDictionary<string, IDependencyProvider> _parsers = new Dictionary<string, IDependencyProvider>(Comparers.VirtualPath);

		public DependencyManager(IResourceFinder resourceFinder, IDependencyCache dependencyCache, IResourceGroupManager scriptGroups, IResourceGroupManager styleGroups)
		{
			_resourceFinder = resourceFinder;
			_dependencyCache = dependencyCache;
			_scriptGroups = scriptGroups;
			_styleGroups = styleGroups;
		}

		public void MapProvider(string extension, IDependencyProvider dependencyProvider)
		{
			_parsers[extension] = dependencyProvider;
		}

		public IEnumerable<string> GetDependencies(string virtualPath)
		{	
			IEnumerable<string> cachedDependencies;
			if (_dependencyCache.TryGetDependencies(virtualPath, out cachedDependencies))
				return cachedDependencies;

			var dependencyList = new List<IEnumerable<string>>();
			IEnumerable<IResource> resourcesInGroup;
			if(IsConsolidatedUrl(virtualPath, out resourcesInGroup))
			{
				foreach (var resource in resourcesInGroup)
				{
					AccumulateDependencies(dependencyList, resource);
				}

				//filter out dependencies within the group
				return CollapseDependencies(dependencyList).Where(
						d => !resourcesInGroup.Any(r => r.VirtualPath.Equals(d, StringComparison.OrdinalIgnoreCase)));
			}
			else
			{
				AccumulateDependencies(dependencyList, virtualPath);
				return CollapseDependencies(dependencyList);
			}	
		}

		internal int Comparer(IResource x, IResource y)
		{
			var xDepends = GetDependencies(x);
			var yDepends = GetDependencies(y);

			if (xDepends.Contains(y.VirtualPath, Comparers.VirtualPath))
				return 1;
			if (yDepends.Contains(x.VirtualPath, Comparers.VirtualPath))
				return -1;

			return 0;
		}

        internal int Comparer(string virtualPath1, string virtualPath2)
        {
            var xDepends = GetDependencies(virtualPath1);
            var yDepends = GetDependencies(virtualPath2);

            if (xDepends.Contains(virtualPath2, Comparers.VirtualPath))
                return 1;
            if (yDepends.Contains(virtualPath1, Comparers.VirtualPath))
                return -1;

            return 0;
        }

		private bool IsConsolidatedUrl(string virtualPath, out IEnumerable<IResource> resourcesInGroup)
		{
			if (IsConsolidatedUrl(virtualPath, _scriptGroups, out resourcesInGroup))
				return true;
			if (IsConsolidatedUrl(virtualPath, _styleGroups, out resourcesInGroup))
				return true;

			return false;
		}

		private bool IsConsolidatedUrl(string virtualPath, IResourceGroupManager groupTemplates, out IEnumerable<IResource> resourcesInGroup)
		{
			var group = groupTemplates.GetGroupOrDefault(virtualPath, ResourceMode.Debug, _resourceFinder); //the ResourceMode value shouldn't matter here, we'll use Debug because this code will only be executed when you haven't pre-consolidated.
			
			if (group == null)
			{
				resourcesInGroup = null;
				return false;
			}
			
			resourcesInGroup = group.GetResources().SortByDependencies(this);
			return true;
		}

		public IEnumerable<string> GetDependencies(IResource resource)
		{
			IEnumerable<string> cachedDependencies;
			if (_dependencyCache.TryGetDependencies(resource.VirtualPath, out cachedDependencies))
				return cachedDependencies;

			var dependencyList = new List<IEnumerable<string>>();
			AccumulateDependencies(dependencyList, resource);

			return CollapseDependencies(dependencyList);
		}

		public void SetCache(IDependencyCache cache)
		{
			_dependencyCache = cache;
		}

		private void AccumulateDependencies(List<IEnumerable<string>> dependencyList, string virtualPath)
		{
			IEnumerable<string> cachedDependencies;
			if (_dependencyCache.TryGetDependencies(virtualPath, out cachedDependencies))
			{
				dependencyList.Insert(0, cachedDependencies);
				return;
			}
			
			var resource = _resourceFinder.FindResource(virtualPath);
			if(resource == null)
				return;

			AccumulateDependencies(dependencyList, resource);
		}

		private void AccumulateDependencies(List<IEnumerable<string>> dependencyList, IResource resource)
		{
			IEnumerable<string> cachedDependencies;
			if (_dependencyCache.TryGetDependencies(resource, out cachedDependencies))
			{
				dependencyList.Insert(0, cachedDependencies);
				//store in cache so that it will also be indexed by virtual path
				_dependencyCache.StoreDependencies(resource, cachedDependencies);
				return;
			}

			IDependencyProvider provider;
			if (!_parsers.TryGetValue(resource.FileExtension, out provider))
				return;

			var dependencyListEntrySize = dependencyList.Count;
			var dependencies = provider.GetDependencies(resource).ToList();
			if (dependencies.Any())
			{
				dependencyList.Insert(0, dependencies);
				foreach (var dependency in dependencies)
				{
					AccumulateDependencies(dependencyList, dependency);
				}
			}
			var dependenciesForCurrentResource = CollapseDependencies(dependencyList.Take(dependencyList.Count - dependencyListEntrySize));
			_dependencyCache.StoreDependencies(resource, dependenciesForCurrentResource);
		}

		private IEnumerable<string> CollapseDependencies(IEnumerable<IEnumerable<string>> dependencyList)
		{
			return dependencyList.SelectMany(d => d)
				.Distinct(Comparers.VirtualPath)
				.ToList();
		}
	}

	public static class DependencyManagerExtensions
	{
		//we use a stable sort here because the resources are already sorted within their groups (by the order in which they are included in the config), so we should try to preserve that order unless
        //the dependencies instruct otherwise
        
        public static IEnumerable<IResource> SortByDependencies(this IEnumerable<IResource> resources, DependencyManager dependencyManager)
		{
			return resources.StableSort(dependencyManager.Comparer);
		}

        public static IEnumerable<string> SortByDependencies(this IEnumerable<string> resourcePaths, DependencyManager dependencyManager)
        {
            return resourcePaths.StableSort(dependencyManager.Comparer);
        }
	}
}