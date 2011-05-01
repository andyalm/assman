using System;

namespace Assman.Spark
{
	public interface ISparkResourceContentFetcher
	{
		string GetContent(string controllerName, string viewName, string masterName);
	}
}