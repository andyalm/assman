using System.IO;

using Assman.Configuration;

using dotless.Core.Input;

namespace Assman.Less
{
	public class VirtualPathFileReader : IFileReader
	{
		private readonly VirtualPathResolver _pathResolver;

		public VirtualPathFileReader()
		{
			var config = AssmanConfiguration.Current;
			_pathResolver = VirtualPathResolver.GetInstance(config.RootFilePath);
		}

		public string GetFileContents(string fileName)
		{
			var resolvedPath = _pathResolver.MapPath(fileName);

			return File.ReadAllText(resolvedPath);
		}
	}
}