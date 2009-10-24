using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;

namespace AlmWitt.Web.ResourceManagement
{
	internal static class Util
	{
		public static bool IsDebugMode
		{
			get
			{
				bool debugMode = false;
				CompilationSection configSection = WebConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
				if (configSection != null)
					debugMode = configSection.Debug;

				return debugMode;
			}
		}
	}
}
