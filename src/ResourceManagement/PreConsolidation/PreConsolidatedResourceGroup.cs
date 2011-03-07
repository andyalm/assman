using System;
using System.Collections.Generic;

namespace AlmWitt.Web.ResourceManagement.PreConsolidation
{
	public class PreConsolidatedResourceGroup
	{
		public string ConsolidatedUrl { get; set; }

		public List<PreConsolidatedResourcePiece> Resources { get; set; }

		public PreConsolidatedResourceGroup()
		{
			Resources = new List<PreConsolidatedResourcePiece>();
		}

		public override string ToString()
		{
			return ConsolidatedUrl;
		}
	}
}