using System;

namespace AlmWitt.Web.ResourceManagement.Spark
{
	public interface ISparkResourceContentFetcher
	{
		string GetContent(string controllerName, string viewName, string masterName);
	}
}