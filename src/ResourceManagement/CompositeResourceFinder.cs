using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public class CompositeResourceFinder : IResourceFinder
	{
		readonly List<IResourceFinder> _finders = new List<IResourceFinder>();

		public IEnumerable<IResource> FindResources(ResourceType resourceType)
		{
			var combined = new List<IResource>();
			foreach (IResourceFinder finder in _finders)
			{
				var found = finder.FindResources(resourceType);
				combined.AddRange(found);
			}

			return combined;
		}

		public IResource FindResource(string virtualPath)
		{
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
