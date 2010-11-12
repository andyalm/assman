using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
	internal class FileResourceFinder : IResourceFinder
	{
		private readonly string _directory;

		/// <summary>
		/// Constructs a new <see cref="FileResourceFinder"/>.
		/// </summary>
		/// <param name="directory">The directory to recursively find resources.</param>
		public FileResourceFinder(string directory)
		{
			this._directory = directory;
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
	}
}
