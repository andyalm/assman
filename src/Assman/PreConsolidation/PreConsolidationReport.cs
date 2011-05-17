using System.Collections.Generic;

namespace Assman.PreConsolidation
{
	public class PreConsolidationReport
	{
		public PreConsolidatedResourceReport Scripts { get; set; }
		public PreConsolidatedResourceReport Stylesheets { get; set; }
		public List<PreConsolidatedResourceDependencies> Dependencies { get; set; }
		public string Version { get; set; }

		public PreConsolidationReport()
		{
			Version = "";
			Scripts = new PreConsolidatedResourceReport();
			Stylesheets = new PreConsolidatedResourceReport();
			Dependencies = new List<PreConsolidatedResourceDependencies>();
		}
	}
}