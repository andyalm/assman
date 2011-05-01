using System.Collections.Generic;

namespace Assman.PreConsolidation
{
	public class PreConsolidationReport
	{
		public List<PreConsolidatedResourceGroup> ScriptGroups { get; set; }
		public List<PreConsolidatedResourceGroup> StyleGroups { get; set; }
		public List<PreConsolidatedResourceDependencies> Dependencies { get; set; }
		public string Version { get; set; }

		public PreConsolidationReport()
		{
			Version = "";
			ScriptGroups = new List<PreConsolidatedResourceGroup>();
			StyleGroups = new List<PreConsolidatedResourceGroup>();
			Dependencies = new List<PreConsolidatedResourceDependencies>();
		}
	}
}