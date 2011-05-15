using System.IO;

namespace Assman.IO
{
	internal interface IFileAccess
	{
		TextWriter OpenWriter(string filePath);
		bool Exists(string filePath);
		TextReader OpenReader(string filePath);
	}

	internal class FileAccessWrapper : IFileAccess
	{
		public FileAccessWrapper()
		{
			
		}

		public TextWriter OpenWriter(string filePath)
		{
			return new StreamWriter(filePath);
		}

		public bool Exists(string filePath)
		{
			return File.Exists(filePath);
		}

		public TextReader OpenReader(string filePath)
		{
			return new StreamReader(File.OpenRead(filePath));
		}
	}
}