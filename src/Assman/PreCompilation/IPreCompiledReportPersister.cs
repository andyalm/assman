using System;

namespace Assman.PreCompilation
{
	public interface IPreCompiledReportPersister
	{
		bool TryGetPreConsolidationInfo(out PreCompilationReport preCompilationReport);
		void SavePreConsolidationInfo(PreCompilationReport preCompilationReport);
	}

	internal class NullPreCompiledPersister : IPreCompiledReportPersister
	{
		private static readonly IPreCompiledReportPersister _instance = new NullPreCompiledPersister();

		public static IPreCompiledReportPersister Instance
		{
			get { return _instance; }
		}

		private NullPreCompiledPersister() {}

		public bool TryGetPreConsolidationInfo(out PreCompilationReport preCompilationReport)
		{
			preCompilationReport = null;
			return false;
		}

		public void SavePreConsolidationInfo(PreCompilationReport preCompilationReport) {}
	}
}