namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public class CompiledFilePersister : IPreConsolidationInfoPersister
	{
		private readonly string _rootWebPath;

		public CompiledFilePersister(string rootWebPath)
		{
			_rootWebPath = rootWebPath;
		}

		public bool TryGetPreConsolidationInfo(out PreConsolidationReport preConsolidationReport)
		{
			preConsolidationReport = null;
			return false;
		}

		public void SavePreConsolidationInfo(PreConsolidationReport preConsolidationReport) {}
	}
}