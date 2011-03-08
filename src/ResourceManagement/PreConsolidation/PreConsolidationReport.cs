using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class PreConsolidationReport
	{
		public List<PreConsolidatedResourceGroup> ClientScriptGroups { get; set; }
		public List<PreConsolidatedResourceGroup> CssGroups { get; set; }
		public string Version { get; set; }

		public PreConsolidationReport()
		{
			Version = "";
			ClientScriptGroups = new List<PreConsolidatedResourceGroup>();
			CssGroups = new List<PreConsolidatedResourceGroup>();
		}
	}
}