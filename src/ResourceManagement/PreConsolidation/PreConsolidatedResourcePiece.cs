using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class PreConsolidatedResourcePiece
	{
		public string Path { get; set; }

		public List<string> Dependencies { get; set; }
	}
}