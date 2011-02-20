using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	internal class FileResourceFinder : IResourceFinder
	{
		private readonly string _directory;
		private VirtualPathResolver _pathResolver;

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
		public ResourceCollection FindResources(ResourceType resourceType)
		{
			var resources = new ResourceCollection();
			foreach (var extension in resourceType.FileExtensions)
			{
				string[] files = Directory.GetFiles(_directory, "*" + extension, SearchOption.AllDirectories);
				foreach (string filePath in files)
				{
					resources.Add(new FileResource(filePath, _directory));
				}
			}
			
			return resources;
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
