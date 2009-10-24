using System;

namespace AlmWitt.Web.ResourceManagement
{
	internal class CachingResourceCollector : IResourceCollector
	{
		private readonly IResourceCollector _collector;
		private ConsolidatedResource _cachedResource;

		public CachingResourceCollector(IResourceCollector collector)
		{
			_collector = collector;
		}

		public ConsolidatedResource GetResource(IResourceFinder finder, string extension, IResourceFilter exclude)
		{
		    if(_cachedResource == null)
		        _cachedResource = _collector.GetResource(finder, extension, exclude);

		    return _cachedResource;
		}
	}
}