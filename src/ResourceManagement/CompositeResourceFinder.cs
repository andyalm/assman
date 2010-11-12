using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	public class CompositeResourceFinder : IResourceFinder
	{
		List<IResourceFinder> _finders = new List<IResourceFinder>();

		public ResourceCollection FindResources(ResourceType resourceType)
		{
			ResourceCollection combined = new ResourceCollection();
			foreach (IResourceFinder finder in _finders)
			{
				ResourceCollection found = finder.FindResources(resourceType);
				combined.AddRange(found);
			}

			return combined;
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
