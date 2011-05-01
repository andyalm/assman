using System;
using System.Collections.Generic;

using Spark.Web.Mvc;

namespace Assman.Spark
{
	public interface ISparkJavascriptActionFinder
	{
		IEnumerable<SparkJavascriptAction> FindJavascriptActions(Type controllerType);
	}
}