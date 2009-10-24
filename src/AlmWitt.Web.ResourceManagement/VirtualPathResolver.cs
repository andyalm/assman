using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	internal class VirtualPathResolver
	{
		private string _appPath;

		public VirtualPathResolver(string appPath)
		{
			_appPath = appPath;
		}

		public string AppPath
		{
			get { return _appPath; }
		}

		public string MapPath(string virtualPath)
		{
			if (virtualPath.StartsWith("~"))
				virtualPath = virtualPath.Substring(1);
			if (virtualPath.StartsWith("/"))
				virtualPath = virtualPath.Substring(1);
			virtualPath = virtualPath.Replace('/', '\\');

			return Path.Combine(AppPath, virtualPath);
		}
	}
}