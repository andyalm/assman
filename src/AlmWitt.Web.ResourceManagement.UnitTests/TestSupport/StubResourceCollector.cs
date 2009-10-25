using System;

namespace AlmWitt.Web.ResourceManagement.UnitTests.TestSupport
{
	internal class StubResourceCollector : IResourceCollector
	{
		private ConsolidatedResource _resource;
		private int _getCount = 0;

		public ConsolidatedResource Resource
		{
			get { return _resource; }
			set { _resource = value; }
		}

		public int GetCount
		{
			get { return _getCount; }
		}

		public StubResourceCollector(ConsolidatedResource resource)
		{
			_resource = resource;
		}

		public ConsolidatedResource GetResource(IResourceFinder finder, string extension, IResourceFilter exclude)
		{
			_getCount++;
			return _resource;
		}
	}
}