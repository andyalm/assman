using System;
using System.Collections.Generic;

namespace Assman.PreCompilation
{
	public class PreCompiledResourceGroup
	{
		public string ConsolidatedUrl { get; set; }

		public List<string> Resources { get; set; }

		public PreCompiledResourceGroup()
		{
			Resources = new List<string>();
		}

		public override string ToString()
		{
			return ConsolidatedUrl;
		}
	}
}