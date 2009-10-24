using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement
{
	internal class CompositeResourceFilter : IResourceFilter, ICloneable
	{
		private readonly List<IResourceFilter> _filters = new List<IResourceFilter>();

		public void AddFilter(IResourceFilter filter)
		{
			_filters.Add(filter);
		}
		
		public bool IsMatch(IResource resource)
		{
			foreach (IResourceFilter filter in _filters)
			{
				if (filter.IsMatch(resource))
					return true;
			}

			return false;
		}

		public CompositeResourceFilter Clone()
		{
			CompositeResourceFilter clone = new CompositeResourceFilter();
			foreach (IResourceFilter filter in _filters)
			{
				clone.AddFilter(filter);
			}

			return clone;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}
