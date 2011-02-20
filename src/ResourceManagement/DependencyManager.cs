using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmWitt.Web.ResourceManagement
{
	public class DependencyManager
	{
		private readonly IResourceFinder _resourceFinder;
		private readonly IDictionary<string, IDependencyProvider> _parsers = new Dictionary<string, IDependencyProvider>(StringComparer.OrdinalIgnoreCase);

		public DependencyManager(IResourceFinder resourceFinder)
		{
			_resourceFinder = resourceFinder;
		}

		public void MapProvider(string extension, IDependencyProvider dependencyProvider)
		{
			_parsers[extension] = dependencyProvider;
		}

		public IEnumerable<string> GetDependencies(string virtualPath)
		{
			var dependencyList = new List<IEnumerable<string>>();
			AccumulateDependencies(dependencyList, virtualPath);

			return dependencyList.SelectMany(d => d)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		private void AccumulateDependencies(List<IEnumerable<string>> dependencyList, string virtualPath)
		{
			var resource = _resourceFinder.FindResource(virtualPath);
			if(resource == null)
				return;

			IDependencyProvider provider;
			if(!_parsers.TryGetValue(resource.FileExtension, out provider))
				return;

			var dependencies = provider.GetDependencies(resource);
			if(dependencies.Any())
			{
				dependencyList.Insert(0, dependencies);
				foreach (var dependency in dependencies)
				{
					AccumulateDependencies(dependencyList, dependency);
				}
			}
		}
	}
}