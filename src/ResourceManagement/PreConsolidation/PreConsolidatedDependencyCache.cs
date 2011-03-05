using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class PreConsolidatedDependencyCache : IDependencyCache
	{
		private static readonly string[] _empty = new string[0];
		private readonly IDictionary<string,IEnumerable<string>> _dependencyMap = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
		
		public void SetDependencies(string path, IEnumerable<string> dependencies)
		{
			_dependencyMap[path] = dependencies;
		}
		
		public bool TryGetDependencies(string virtualPath, out IEnumerable<string> dependencies)
		{
			if (!_dependencyMap.TryGetValue(virtualPath, out dependencies))
				dependencies = _empty;

			return true;
		}

		public bool TryGetDependencies(IResource resource, out IEnumerable<string> dependencies)
		{
			dependencies = _empty;
			return true;
		}

		public void StoreDependencies(IResource resource, IEnumerable<string> dependencies)
		{
			//no-op
		}
	}
}