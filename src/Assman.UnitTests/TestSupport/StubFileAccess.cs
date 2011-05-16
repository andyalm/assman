using System.IO;

using Assman.IO;

namespace Assman.TestSupport
{
	internal class StubFileAccess : IFileAccess
	{
		private StringWriter _writer;
		
		public TextWriter OpenWriter(string filePath)
		{
			_writer = new StringWriter();

			return _writer;
		}

		public bool Exists(string filePath)
		{
			return true;
		}

		public TextReader OpenReader(string filePath)
		{
			return new StringReader(_writer.ToString());
		}
	}
}