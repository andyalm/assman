using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman
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
			return _filters.Any(filter => filter.IsMatch(resource));
		}

		public CompositeResourceFilter Clone()
		{
			var clone = new CompositeResourceFilter();
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
