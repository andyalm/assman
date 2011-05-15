using System;
using System.IO;
using System.Web;

namespace Assman.Handlers
{
	internal class HttpRequestContext : IRequestContext
	{
		private readonly HttpContextBase _httpContext;

		public HttpRequestContext(HttpContextBase httpContext)
		{
			_httpContext = httpContext;
			var resourceCacheKey = httpContext.Request.QueryString[AspNetShortLivedResourceCache.QueryStringKey];
			if (!String.IsNullOrEmpty(resourceCacheKey))
				_httpContext.SetResourceCacheKey(resourceCacheKey);
		}

		public DateTime? IfModifiedSince
		{
			get
			{
				DateTime ifModifiedSince;
				if (DateTime.TryParse(_httpContext.Request.Headers["If-Modified-Since"], out ifModifiedSince))
					return ifModifiedSince;
				else
					return null;
			}
		}

		public int StatusCode
		{
			get { return _httpContext.Response.StatusCode; }
			set { _httpContext.Response.StatusCode = value; }
		}

		public string ContentType
		{
			get { return _httpContext.Response.ContentType; }
			set { _httpContext.Response.ContentType = value; }
		}

		public Stream OutputStream
		{
			get { return _httpContext.Response.OutputStream; }
		}

		public string StatusDescription
		{
			get { return _httpContext.Response.StatusDescription; }
			set { _httpContext.Response.StatusDescription = value; }
		}

		public void SetLastModified(DateTime lastModified)
		{
			_httpContext.Response.AddHeader("Last-Modified", lastModified.ToString("r"));
		}
	}
}
