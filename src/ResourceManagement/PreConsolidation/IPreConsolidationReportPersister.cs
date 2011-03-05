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
		public bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport)
		{
			preConsolidationReport = null;
			return false;
		}

		public void SavePreConsolidationInfo(PreConsolidationReport preConsolidationReport) {}
	}
}