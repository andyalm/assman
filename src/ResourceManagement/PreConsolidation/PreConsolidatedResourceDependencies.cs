using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class PreConsolidatedResourceDependencies
	{
		public string ResourcePath { get; set; }

		public List<string> Dependencies { get; set; }

		public PreConsolidatedResourceDependencies()
		{
			Dependencies = new List<string>();
		}

		public override string ToString()
		{
			return ResourcePath;
		}
	}
}