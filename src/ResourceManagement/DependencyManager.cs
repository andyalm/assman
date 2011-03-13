using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	public class DependencyManager
	{
		private readonly IResourceFinder _resourceFinder;
		private IDependencyCache _dependencyCache;
		private readonly ResourceGroupTemplateCollection _scriptGroups;
		private readonly ResourceGroupTemplateCollection _styleGroups;
		private readonly IDictionary<string, IDependencyProvider> _parsers = new Dictionary<string, IDependencyProvider>(StringComparer.OrdinalIgnoreCase);

		public DependencyManager(IResourceFinder resourceFinder, IDependencyCache dependencyCache, ResourceGroupTemplateCollection scriptGroups, ResourceGroupTemplateCollection styleGroups)
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

		public IEnumerable<IResource> SortByDependencies(IEnumerable<IResource> resources)
		{
			var resourceList = resources.ToList();
			resourceList.Sort(Comparer);

			return resourceList;
		}

		public int Comparer(IResource x, IResource y)
		{
			var xDepends = GetDependencies(x);
			var yDepends = GetDependencies(y);

			if (xDepends.Contains(y.VirtualPath, StringComparer.OrdinalIgnoreCase))
				return 1;
			if (yDepends.Contains(x.VirtualPath, StringComparer.OrdinalIgnoreCase))
				return -1;

			return 0;
		}

		private bool IsConsolidatedUrl(string virtualPath, out IEnumerable<IResource> resourcesInGroup)
		{
			//TODO: Figure out how to cache the IsConsolidatedUrl question as it could potentially be run for every unique script that is included
			//in a dev environment, which could be slow
			if (IsConsolidatedUrl(virtualPath, _scriptGroups, out resourcesInGroup))
				return true;
			if (IsConsolidatedUrl(virtualPath, _styleGroups, out resourcesInGroup))
				return true;

			return false;
		}

		private bool IsConsolidatedUrl(string virtualPath, ResourceGroupTemplateCollection groupTemplates, out IEnumerable<IResource> resourcesInGroup)
		{
			resourcesInGroup = null;
			var groupTemplateContext = groupTemplates.FindGroupTemplate(virtualPath);
			if (groupTemplateContext == null)
				return false;

			var group = groupTemplateContext.FindGroupOrDefault(_resourceFinder, virtualPath, ResourceMode.Debug);
			if (group == null) //could be null group is defined by convention, but there were no resources that matched the group pattern
				return false;
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
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();
		}
	}

	public static class DependencyManagerExtensions
	{
		public static IEnumerable<IResource> SortByDependencies(this IEnumerable<IResource> resources, DependencyManager dependencyManager)
		{
			return resources.Sort(dependencyManager.Comparer);
		}
	}
}