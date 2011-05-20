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
		    return path.ToAppPath(context.VirtualPath);
		}

        public static string ToAppPath(this string path, string contextVirtualPath)
        {
            if (IsAppPath(path))
                return path;

            var relativeUri = new Uri(path, UriKind.Relative);
            var contextUri = new Uri("http://www.website.com" + contextVirtualPath.Substring(1));

            var resolvedUri = new Uri(contextUri, relativeUri);

            return "~" + resolvedUri.AbsolutePath;
        }

		public static string ChangeExtension(this string path, string newExtension)
		{
			var lastDotIndex = path.LastIndexOf(".");
			var baseName = path.Substring(0, lastDotIndex);

			return baseName + newExtension;
		}

		private static bool IsAppPath(string path)
		{
			return path.StartsWith("~");
		}
	}
}