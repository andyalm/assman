using System;
using System.IO;

namespace Assman.TestSupport
{
	public class StubRequestContext : IRequestContext
	{
		private DateTime? _ifModifiedSince = null;
		private int _statusCode = 200;
		private string _contentType = String.Empty;
		private Stream _outputStream = new MemoryStream();
		private int _setLastModifiedCalled = 0;
		private string m_statusDescription = String.Empty;

		public DateTime? IfModifiedSince
		{
			get { return _ifModifiedSince; }
			set { _ifModifiedSince = value; }
		}

		public int StatusCode
		{
			get { return _statusCode; }
			set { _statusCode = value; }
		}

		public string ContentType
		{
			get { return _contentType; }
			set { _contentType = value; }
		}

		public Stream OutputStream
		{
			get { return _outputStream; }
		}

		public string StatusDescription
		{
			get { return m_statusDescription; }
			set { m_statusDescription = value; }
		}

		public int SetLastModifiedCalled
		{
			get { return _setLastModifiedCalled; }
		}

		public void SetLastModified(DateTime lastModified)
		{
			_setLastModifiedCalled++;
		}
	}
}
