using System;

namespace Assman
{
	internal static class ResourceFilters
	{
		private static readonly FalseFilter _false = new FalseFilter();
		private static readonly TrueFilter _true = new TrueFilter();

		public static IResourceFilter False
		{
			get { return _false; }
		}

		public static IResourceFilter True
		{
			get { return _true; }
		}

		public static IResourceFilter Not(IResourceFilter filter)
		{
			return new NotResourceFilter(filter);
		}

		public static IResourceFilter Predicate(Predicate<IResource> match)
		{
			return new PredicateResourceFilter(match);
		}

		#region Nested Types

		private class FalseFilter : IResourceFilter
		{
			public bool IsMatch(IResource resource)
			{
				return false;
			}
		}

		private class TrueFilter : IResourceFilter
		{
			public bool IsMatch(IResource resource)
			{
				return true;
			}
		}

		private class NotResourceFilter : IResourceFilter
		{
			private readonly IResourceFilter _filter;

			public NotResourceFilter(IResourceFilter filter)
			{
				_filter = filter;
			}

			public bool IsMatch(IResource resource)
			{
				return !_filter.IsMatch(resource);
			}
		}

		private class PredicateResourceFilter : IResourceFilter
		{
			private readonly Predicate<IResource> _match;

			public PredicateResourceFilter(Predicate<IResource> match)
			{
				_match = match;
			}

			public bool IsMatch(IResource resource)
			{
				return _match(resource);
			}
		}

		#endregion

	}
}
