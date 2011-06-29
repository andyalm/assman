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

		public static string ToAppRelativePath(this string path, IResource context)
		{
			return path.ToAppRelativePath(context.VirtualPath);
		}

		public static string ToAppRelativePath(this string path, string contextVirtualPath)
		{
			if (IsAppPath(path))
				return path;

			var relativeUri = new Uri(path, UriKind.Relative);
			var contextUri = new Uri("http://www.website.com" + contextVirtualPath.Substring(1));

			var resolvedUri = new Uri(contextUri, relativeUri);

			return "~" + resolvedUri.AbsolutePath;
		}

		public static string ToAppRelativePath(this string virtualPath)
		{
			return ResolveVirtualPath(virtualPath, VirtualPathUtility.ToAppRelative);
		}

		public static string ToAbsolutePath(this string virtualPath)
		{
			return ResolveVirtualPath(virtualPath, VirtualPathUtility.ToAbsolute);
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

		private static string ResolveVirtualPath(string pathToResolve, Func<string,string,string> virtualPathUtilMethod)
		{
			//if its a full url, no need to do anything more, just return it.
			if (Uri.IsWellFormedUriString(pathToResolve, UriKind.Absolute))
				return pathToResolve;

			var applicationPath = HttpRuntime.AppDomainAppVirtualPath ?? "/";
			
			if (!pathToResolve.Contains("?"))
			{
				return virtualPathUtilMethod(pathToResolve, applicationPath);
			}
			else
			{
				//VirtualPathUtility throws if a query exists in the url.  Strip it
				//off before calling and then append it back on afterwards.
				string[] urlParts = pathToResolve.Split('?');
				string appRelativeUrl = virtualPathUtilMethod(urlParts[0], applicationPath);

				return appRelativeUrl + "?" + urlParts[1];
			}
		}
	}
}