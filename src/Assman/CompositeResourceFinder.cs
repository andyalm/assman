using System;
using System.Collections;
using System.Collections.Generic;

namespace Assman
{
	public class CompositeResourceFinder : IResourceFinder
	{
		private readonly List<IResourceFinder> _finders = new List<IResourceFinder>();
		private readonly IResourceCache _resourceCache;

		public CompositeResourceFinder() : this(ResourceCacheFactory.GetCache())
		{
			
		}

		public CompositeResourceFinder(IResourceCache resourceCache)
		{
			_resourceCache = resourceCache;
		}

		public IEnumerable<IResource> FindResources(ResourceType resourceType)
		{
			return _resourceCache.GetOrAddResources(resourceType, () =>
			{
				var combined = new List<IResource>();
				foreach (IResourceFinder finder in _finders)
				{
					var found = finder.FindResources(resourceType);
					combined.AddRange(found);
				}

				return combined;
			});
		}

		public IResource FindResource(string virtualPath)
		{
			//don't need to cache the results of this method currently
			foreach (var finder in _finders)
			{
				var resource = finder.FindResource(virtualPath);
				if (resource != null)
					return resource;
			}

			return null;
		}

		public void AddFinder(IResourceFinder finder)
		{
			_finders.Add(finder);
		}

		public void AddFinders(IEnumerable<IResourceFinder> finders)
		{
			_finders.AddRange(finders);
		}
	}
}
