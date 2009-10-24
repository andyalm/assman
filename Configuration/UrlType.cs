using System;

namespace AlmWitt.Web.ResourceManagement.Configuration
{
	internal abstract class UrlType
	{
		private static readonly UrlType _static = new StaticUrlType();
		private static readonly UrlType _dynamic = new DynamicUrlType();

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
                if (url.EndsWith("x", StringComparison.OrdinalIgnoreCase))
                    url = url.Substring(0, url.Length - 1);

                return url;
            }
        }

	    private class DynamicUrlType : UrlType
        {
            public override string ConvertUrl(string url)
            {
                if (!url.EndsWith("x", StringComparison.OrdinalIgnoreCase))
                    url += "x";

                return url;
            }
        }
	}

	
}
