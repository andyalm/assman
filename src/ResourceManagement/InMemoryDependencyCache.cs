using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public class InMemoryDependencyCache : IDependencyCache
	{
		private readonly IDictionary<string, IEnumerable<string>> _virtualPathCache = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
		private readonly IDictionary<string, IEnumerable<string>> _resourceCache = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);

		public bool TryGetDependencies(string virtualPath, out IEnumerable<string> dependencies)
		{
			return _virtualPathCache.TryGetValue(virtualPath, out dependencies);
		}

		public bool TryGetDependencies(IResource resource, out IEnumerable<string> dependencies)
		{
			return _resourceCache.TryGetValue(resource.VirtualPath, out dependencies);
		}

		public void StoreDependencies(IResource resource, IEnumerable<string> dependencies)
		{
			_virtualPathCache[resource.VirtualPath] = dependencies;
			_resourceCache[resource.VirtualPath] = dependencies;
		}

        public void Clear()
        {
            _virtualPathCache.Clear();
            _resourceCache.Clear();
        }

		public void StoreDependenciesByResourceOnly(IResource resource, IEnumerable<string> dependencies)
		{
			_resourceCache[resource.VirtualPath] = dependencies;
		}
	}
}