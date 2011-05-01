using System;
using System.IO;
using System.Reflection;

namespace Assman.TestSupport
{
	internal class TempDirectoryManager : IDisposable
	{
		private string _directoryName;

		public string DirectoryName
		{
			get { return _directoryName; }
		}

		public TempDirectoryManager()
		{
			string executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_directoryName = Path.Combine(executingDirectory, "TestDir");
			Directory.CreateDirectory(_directoryName);
		}

		public void CreateFile(string filename)
		{
			CreateFile(null, filename, String.Empty);
		}
		
		public void CreateFile(string filename, string content)
		{
			CreateFile(null, filename, content);
		}
		
		public void CreateFile(string folder, string filename, string content)
		{
			if (!String.IsNullOrEmpty(folder))
				folder = Path.Combine(_directoryName, folder);
			else
				folder = _directoryName;
			Directory.CreateDirectory(folder);
			string fullPath = Path.Combine(folder, filename);
			using (TextWriter writer = new StreamWriter(File.Create(fullPath)))
			{
				writer.Write(content);
			}
		}

		public void Dispose()
		{
			Directory.Delete(_directoryName, true);
		}
	}
}