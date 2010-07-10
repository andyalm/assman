using System;
using System.Linq;

using AlmWitt.Web.ResourceManagement.IO;

namespace AlmWitt.Web.ResourceManagement.Less
{
    public class LessResourceFinder : IResourceFinder
    {
        private readonly string _directory;
        private readonly IFileFinder _fileFinder;

        public LessResourceFinder(string directory, IFileFinder fileFinder)
        {
            _directory = directory;
            _fileFinder = fileFinder;
        }

        public ResourceCollection FindResources(string extension)
        {
            if(extension != ".css")
            {
                return new ResourceCollection();
            }

            return (from filePath in _fileFinder.FindFilesRecursively(_directory, ".less")
                    select new LessResource(filePath, _directory)).ToResourceCollection();
        }
    }
}