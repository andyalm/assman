using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	public class DependencyManager
	{
		private readonly IResourceFinder _resourceFinder;
		private IDependencyCache _dependencyCache;
		private readonly IDictionary<string, IDependencyProvider> _parsers = new Dictionary<string, IDependencyProvider>(StringComparer.OrdinalIgnoreCase);

		public DependencyManager(IResourceFinder resourceFinder, IDependencyCache dependencyCache)
		{
			_resourceFinder = resourceFinder;
			_dependencyCache = dependencyCache;
		}

		public void MapProvider(string extension, IDependencyProvider dependencyProvider)
		{
			_parsers[extension] = dependencyProvider;
		}

		public IEnumerable<string> GetDependencies(string virtualPath)
		{	
			var dependencyList = new List<IEnumerable<string>>();
			AccumulateDependencies(dependencyList, virtualPath);

			return CollapseDependencies(dependencyList);
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
			var dependencies = provider.GetDependencies(resource);
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
}