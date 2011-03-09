using System;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public interface IPreConsolidationReportPersister
	{
		bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport);
		void SavePreConsolidationInfo(PreConsolidationReport preConsolidationReport);
	}

	internal class NullPreConsolidationPersister : IPreConsolidationReportPersister
	{
		private static readonly IPreConsolidationReportPersister _instance = new NullPreConsolidationPersister();

		public static IPreConsolidationReportPersister Instance
		{
			get { return _instance; }
		}

		private NullPreConsolidationPersister() {}

		public bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport)
		{
			preConsolidationReport = null;
			return false;
		}

		public void SavePreConsolidationInfo(PreConsolidationReport preConsolidationReport) {}
	}
}