using System;

namespace AlmWitt.Web.ResourceManagement.Spark
{
	public class SparkResource : IResource
	{
		public static string GetVirtualPath(string controllerName, string actionName)
		{
			return "sparkjs://" + controllerName + "/" + actionName;
		}

		private readonly SparkJavascriptAction _action;
		private readonly string _controllerName;
		private readonly string _actionName;
		private DateTime _lastModified;
		private ISparkResourceContentFetcher _contentFetcher;
		
		public SparkResource(string controllerName, SparkJavascriptAction action, ISparkResourceContentFetcher contentFetcher)
		{
			_controllerName = controllerName;
			_action = action;
			_actionName = _action.ActionName;
			_contentFetcher = contentFetcher;
			_lastModified = DateTime.Now; //don't know how to determine this right now
		}

		public string Name
		{
			get { return _controllerName + "/" + _actionName; }
		}

		public string VirtualPath
		{
			get { return GetVirtualPath(_controllerName, _actionName); }
		}

		public DateTime LastModified
		{
			get
			{
				return _lastModified;
			}
		}

		public string GetContent()
		{
			return _contentFetcher.GetContent(_controllerName, _action.ViewName, _action.MasterName);
		}

		public override string ToString()
		{
			return VirtualPath;
		}
	}
}