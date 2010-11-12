using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Spark.Web.Mvc;

namespace AlmWitt.Web.ResourceManagement.Spark
{
	public class StaticMethodActionFinder : ISparkJavascriptActionFinder
	{
		public IEnumerable<SparkJavascriptAction> FindJavascriptActions(Type controllerType)
		{
			return from m in controllerType.GetMethods()
			       where m.IsPublic && m.IsStatic && m.ReturnType.Is<JavascriptViewResult>()
			       select CreateJavascriptAction(m);
		
		}

		private SparkJavascriptAction CreateJavascriptAction(MethodInfo methodInfo)
		{
			var javascriptViewResult = (JavascriptViewResult) methodInfo.Invoke(null, new object[0]);

			return new SparkJavascriptAction
			{
				ActionName = methodInfo.ActionName(),
				ViewName = javascriptViewResult.ViewName,
				MasterName = javascriptViewResult.MasterName
			};
		}
	}
}