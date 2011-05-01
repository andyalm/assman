using System;

namespace Assman.Spark
{
	public class ConsolidateSparkScriptAttribute : Attribute
	{
		public string ViewName { get; set; }

		public string MasterName { get; set; }

		public SparkJavascriptAction CreateAction(string actionName)
		{
			return new SparkJavascriptAction
			{
				ActionName = actionName,
				MasterName = MasterName,
				ViewName = ViewName ?? actionName
			};
		}
	}
}