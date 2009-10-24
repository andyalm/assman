using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	internal interface IRequestContext
	{
		DateTime? IfModifiedSince { get; }

		int StatusCode { get; set; }

		string ContentType { get; set; }

		Stream OutputStream { get; }

		string StatusDescription { get; set; }

		void SetLastModified(DateTime lastModified);
	}
}