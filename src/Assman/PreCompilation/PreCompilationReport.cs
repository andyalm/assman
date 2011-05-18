using System;
using System.Collections.Generic;

namespace Assman.PreCompilation
{
	public class PreCompilationReport
	{
		public PreCompiledResourceReport Scripts { get; set; }
		public PreCompiledResourceReport Stylesheets { get; set; }
		public List<PreCompiledResourceDependencies> Dependencies { get; set; }
		public string Version { get; set; }

		public PreCompilationReport()
		{
			Version = "";
			Scripts = new PreCompiledResourceReport();
			Stylesheets = new PreCompiledResourceReport();
			Dependencies = new List<PreCompiledResourceDependencies>();
		}
	}
}