using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Assman
{
	internal interface IRequestContext
	{
		DateTime? IfModifiedSince { get; }

		int StatusCode { get; set; }

		NameValueCollection QueryString { get; }

		string ContentType { get; set; }

		bool AcceptsGZip { get; }
		
		string Vary { set; }
		
		string ContentEncoding { set; }

		Stream OutputStream { get; }

		string StatusDescription { get; set; }

		DateTime LastModified { set; }

		DateTime Expires { set; }

		HttpCacheability Cacheability { set; }
	}
}