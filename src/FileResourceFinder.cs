using System;
using System.IO;

namespace AlmWitt.Web.ResourceManagement
{
    internal class FileResourceFinder : IResourceFinder
    {
        private string _directory;

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
        /// <param name="extension">The file extension (beginning with a '.')</param>
        /// <returns></returns>
        public ResourceCollection FindResources(string extension)
        {
            ResourceCollection resources = new ResourceCollection();
            string[] files = Directory.GetFiles(_directory, "*" + extension, SearchOption.AllDirectories);
            foreach (string filePath in files)
            {
                resources.Add(new FileResource(filePath, _directory));
            }


            return resources;
        }
    }
}