using System;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public interface IPreConsolidationInfoPersister
	{
		bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport);
		void SavePreConsolidationInfo(PreConsolidationReport preConsolidationReport);
	}

	internal class NullPreConsolidationPersister : IPreConsolidationInfoPersister
	{
		public bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport)
		{
			preConsolidationReport = null;
			return false;
		}

		public void SavePreConsolidationInfo(PreConsolidationReport preConsolidationReport) {}
	}
}