using System.Web.Mvc;

namespace AlmWitt.Web.ResourceManagement.Mvc
{
	public abstract class ResourceIncludeResolver
	{
		public abstract string ResolveUrl(UrlHelper url, string virtualPath);

		private static readonly ResourceIncludeResolver _defaultInstance = new DefaultResourceIncludeResolver();
		private static ResourceIncludeResolver _instance;
		public static ResourceIncludeResolver Instance
		{
			get { return _instance ?? _defaultInstance; }
			set { _instance = value; }
		}
	}

	public class DefaultResourceIncludeResolver : ResourceIncludeResolver
	{
		public override string ResolveUrl(UrlHelper url, string virtualPath)
		{
			return url.Content(virtualPath);
		}
	}
}