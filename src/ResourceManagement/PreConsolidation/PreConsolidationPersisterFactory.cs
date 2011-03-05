using System;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public static class PreConsolidationPersisterFactory
	{
		private static readonly IPreConsolidationReportPersister _null = new NullPreConsolidationPersister();

		public static IPreConsolidationReportPersister Null
		{
			get { return _null; }
		}
	}
}