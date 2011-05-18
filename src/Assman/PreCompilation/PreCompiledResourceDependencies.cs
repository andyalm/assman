using System;
using System.Collections.Generic;

namespace Assman.PreCompilation
{
	public class PreCompiledResourceDependencies
	{
		public string ResourcePath { get; set; }

		public List<string> Dependencies { get; set; }

		public PreCompiledResourceDependencies()
		{
			Dependencies = new List<string>();
		}

		public override string ToString()
		{
			return ResourcePath;
		}
	}
}