using System;
using System.IO;

namespace Assman.IO
{
	internal interface IFileAccess
	{
		TextWriter OpenWriter(string filePath);
		bool Exists(string filePath);
		TextReader OpenReader(string filePath);
		DateTime LastModified(string filePath);
		Stream OpenStream(string filePath);
	}

	internal class FileAccessWrapper : IFileAccess
	{
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
		
		public Stream OpenStream(string filePath)
		{
			return File.OpenRead(filePath);
		}

		public DateTime LastModified(string filePath)
		{
			return new FileInfo(filePath).LastWriteTime;
		}
	}
}