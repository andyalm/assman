using System;
using System.Collections.Generic;
using System.Linq;

namespace Assman.Spark
{
	public class AttributeBasedActionFinder : ISparkJavascriptActionFinder
	{
		public IEnumerable<SparkJavascriptAction> FindJavascriptActions(Type controllerType)
		{
			return from m in controllerType.GetMethods()
			       where m.HasAttribute<ConsolidateSparkScriptAttribute>()
			       select m.GetAttribute<ConsolidateSparkScriptAttribute>().CreateAction(m.ActionName());
		}
	}
}