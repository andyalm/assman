using System;
using System.IO;

namespace Assman
{
	public class VirtualPathResolver
	{
		public static VirtualPathResolver GetInstance(string appPath)
		{
			return new VirtualPathResolver(appPath);
		}

		private readonly string _appPath;

		internal VirtualPathResolver(string appPath)
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