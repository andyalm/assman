using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Assman.TestSupport
{
	public class StubRequestContext : IRequestContext
	{
		public StubRequestContext()
		{
			IfModifiedSince = null;
			StatusCode = 200;
			StatusDescription = String.Empty;
			ContentType = String.Empty;
			QueryString = new NameValueCollection();
			OutputStream = new MemoryStream();
		}

		public DateTime? IfModifiedSince { get; set; }

		public int StatusCode { get; set; }

		public NameValueCollection QueryString { get; private set; }

		public string ContentType { get; set; }

		public Stream OutputStream { get; private set; }

		public string StatusDescription { get; set; }

		public DateTime LastModified { get; set; }

		public DateTime Expires { get; set; }

		public HttpCacheability Cacheability { get; set; }
	}
}
