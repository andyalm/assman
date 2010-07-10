using System;
using System.Collections.Generic;
using System.IO;

namespace AlmWitt.Web.ResourceManagement.IO
{
    public interface IFileFinder
    {
        IEnumerable<string> FindFilesRecursively(string directory, string fileExtensionBeginningWithDot);
    }

    public class FileFinder : IFileFinder
    {
        public static IFileFinder GetInstance()
        {
            return new FileFinder();
        }
        
        public IEnumerable<string> FindFilesRecursively(string directory, string fileExtensionBeginningWithDot)
        {
            return Directory.GetFiles(directory, "*" + fileExtensionBeginningWithDot, SearchOption.AllDirectories);
        }
    }
}