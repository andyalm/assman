using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class PreConsolidatedResourcePiece
	{
		public PreConsolidatedResourcePiece()
		{
			Dependencies = new List<string>();
		}
		
		public string Path { get; set; }

		public List<string> Dependencies { get; set; }

		public override string ToString()
		{
			return Path;
		}
	}
}