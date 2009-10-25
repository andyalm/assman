using System;
using System.Collections.Generic;

using Spark.Web.Mvc;

namespace AlmWitt.Web.ResourceManagement.Spark
{
	public interface ISparkJavascriptActionFinder
	{
		IEnumerable<SparkJavascriptAction> FindJavascriptActions(Type controllerType);
	}
}