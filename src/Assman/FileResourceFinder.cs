using System;
using System.Collections.Generic;
using System.IO;

namespace Assman
{
	internal class FileResourceFinder : IResourceFinder
	{
		private readonly string _directory;
		private readonly IPathResolver _pathResolver;

		/// <summary>
		/// Constructs a new <see cref="FileResourceFinder"/>.
		/// </summary>
		/// <param name="directory">The directory to recursively find resources.</param>
		public FileResourceFinder(string directory)
		{
			_directory = directory;
			_pathResolver = VirtualPathResolver.GetInstance(_directory);
		}

		/// <summary>
		/// Recursively finds resources in the given directory with the given extension.
		/// </summary>
		/// <param name="resourceType">The resource type to be found.</param>
		/// <returns></returns>
		public IEnumerable<IResource> FindResources(ResourceType resourceType)
		{
			foreach (var extension in resourceType.FileExtensions)
			{
				string[] files = Directory.GetFiles(_directory, "*" + extension, SearchOption.AllDirectories);
				foreach (string filePath in files)
				{
					yield return new FileResource(filePath, _directory);
				}
			}
		}

		public IResource FindResource(string virtualPath)
		{
			if (!virtualPath.StartsWith("~/"))
				return null;

			var filePath = _pathResolver.MapPath(virtualPath);

			if(File.Exists(filePath))
			{
				return new FileResource(filePath, _directory);
			}

			return null;
		}
	}
}
