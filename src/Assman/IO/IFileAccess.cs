using System.IO;

namespace Assman.IO
{
	internal interface IFileAccess
	{
		TextWriter OpenWriter();
		bool Exists();
		TextReader OpenReader();
	}

	internal class FileAccessWrapper : IFileAccess
	{
		private readonly string _filePath;

		public FileAccessWrapper(string filePath)
		{
			_filePath = filePath;
		}

		public TextWriter OpenWriter()
		{
			return new StreamWriter(_filePath);
		}

		public bool Exists()
		{
			return File.Exists(_filePath);
		}

		public TextReader OpenReader()
		{
			return new StreamReader(File.OpenRead(_filePath));
		}
	}
}