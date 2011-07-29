using System;
using System.IO;

using Assman.Configuration;

using dotless.Core.Input;

namespace Assman.dotLess
{
	public class VirtualPathFileReader : IFileReader
	{
		private readonly IPathResolver _pathResolver;

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