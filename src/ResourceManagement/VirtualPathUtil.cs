using System.Web;

namespace AlmWitt.Web.ResourceManagement
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
	}
}