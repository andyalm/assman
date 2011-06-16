using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Linq;

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

		public NameValueCollection QueryString
		{
			get { return _httpContext.Request.QueryString; }
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

		public DateTime LastModified
		{
			set
			{
				//not sure why, but it appears that if you pass a very recent UTC date into SetLastModified,
				//it can throw on you.  Converting back to local time and ensuring its not in the future
				//should safeguard this from happening.
				DateTime valueLocalTime = value.ToLocalTime();
				if (valueLocalTime > DateTime.Now)
					valueLocalTime = DateTime.Now;
				_httpContext.Response.Cache.SetLastModified(valueLocalTime);
			}
		}

		public DateTime Expires
		{
			set { _httpContext.Response.Cache.SetExpires(value); }
		}

		public HttpCacheability Cacheability
		{
			set { _httpContext.Response.Cache.SetCacheability(value); }
		}

		public bool AcceptsGZip
		{
			get
			{
				var acceptEncoding = _httpContext.Request.Headers["Accept-Encoding"];
				return acceptEncoding != null && acceptEncoding.Contains("gzip");
			}
		}

		public string Vary
		{
			set { _httpContext.Response.Cache.SetVaryByCustom(value); }
		}

		public string ContentEncoding
		{
			set { _httpContext.Response.AddHeader("Content-Encoding", value); }
		}
	}
}
