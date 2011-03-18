using System;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	public abstract class UrlType
	{
		private static readonly UrlType _static = new StaticUrlType();
		private static readonly UrlType _dynamic = new DynamicUrlType();

		public static bool ArePathsEqual(string path1, string path2)
		{
			if (path1 == null && path2 == null)
				return true;
			if (path1 == null)
				return false;
			if (path2 == null)
				return false;

			string normalizedPath1 = Static.ConvertUrl(path1);
			string normalizedPath2 = Static.ConvertUrl(path2);

			return StringComparer.OrdinalIgnoreCase.Equals(normalizedPath1, normalizedPath2);
		}

		public static UrlType Static
		{
			get { return _static; }
		}

		public static UrlType Dynamic
		{
			get { return _dynamic; }
		}

		public abstract string ConvertUrl(string url);

	    private class StaticUrlType : UrlType
        {
            public override string ConvertUrl(string url)
            {
            	var urlParts = GetUrlParts(url);
				
				if (urlParts.Path.EndsWith("x", StringComparison.OrdinalIgnoreCase))
                    urlParts.Path = urlParts.Path.Substring(0, url.Length - 1);

                return urlParts.ToString();
            }
        }

	    private class DynamicUrlType : UrlType
        {
            public override string ConvertUrl(string url)
            {
            	var urlParts = GetUrlParts(url);

				if (!urlParts.Path.EndsWith("x", StringComparison.OrdinalIgnoreCase))
					urlParts.Path += "x";

				return urlParts.ToString();
            }
        }

		private static UrlParts GetUrlParts(string url)
		{
			var parts = url.Split('?');

			return new UrlParts
			{
				Path = parts[0],
				Query = (parts.Length > 1) ? parts[1] : String.Empty
			};
		}

		private class UrlParts
		{
			public string Path { get; set; }
			public string Query { get; set; }

			public override string ToString()
			{
				var url = Path;
				if(!String.IsNullOrEmpty(Query))
					url += "?" + Query;

				return url;
			}
		}
	}

	
}
