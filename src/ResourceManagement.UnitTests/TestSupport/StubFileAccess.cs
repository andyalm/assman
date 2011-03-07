using System.IO;

using AlmWitt.Web.ResourceManagement.IO;

namespace AlmWitt.Web.ResourceManagement.TestSupport
{
	internal class StubFileAccess : IFileAccess
	{
		private StringWriter _writer;
		
		public TextWriter OpenWriter()
		{
			_writer = new StringWriter();

			return _writer;
		}

		public bool Exists()
		{
			return true;
		}

		public TextReader OpenReader()
		{
			return new StringReader(_writer.ToString());
		}
	}
}