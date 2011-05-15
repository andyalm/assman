using System;
using System.Web;

namespace Assman
{
	internal static class VirtualPathUtil
	{
		public static string AddQueryParam(this string existingPath, string name, string value)
		{
			string separator = "?";
			if (existingPath.Contains("?"))
				separator = "&";

			return string.Format("{0}{1}{2}={3}", existingPath, separator, name, HttpUtility.UrlEncode(value));
		}

		public static string WithoutQuery(this string path)
		{
			if (path.Contains("?"))
				return path.Substring(0, path.IndexOf("?"));

			return path;
		}

		public static string ToAppPath(this string path, IResource context)
		{
			if (IsAppPath(path))
				return path;
			
			var relativeUri = new Uri(path, UriKind.Relative);
			var contextUri = new Uri("http://www.website.com" + context.VirtualPath.Substring(1));

			var resolvedUri = new Uri(contextUri, relativeUri);

			return "~" + resolvedUri.AbsolutePath;
		}

		private static bool IsAppPath(string path)
		{
			return path.StartsWith("~");
		}
	}
}