namespace Assman.Registration
{
	public abstract class ResourceIncludeResolver
	{
		public abstract string ResolveScriptUrl(string virtualPath);
		public abstract string ResolveStylesheetUrl(string virtualPath);

		protected string AppRelativeToAbsolute(string appRelativePath)
		{
		    return appRelativePath.ToAbsolutePath();
		}

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
		public override string ResolveScriptUrl(string virtualPath)
		{
			return AppRelativeToAbsolute(virtualPath);
		}

		public override string ResolveStylesheetUrl(string virtualPath)
		{
			return AppRelativeToAbsolute(virtualPath);
		}
	}
}