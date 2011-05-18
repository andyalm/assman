using System;
using System.Collections.Generic;
using System.IO;

namespace Assman
{
    public interface ICompiledResource
    {
        string CompiledPath { get; }
        DateTime LastModified { get; }
        IEnumerable<IResource> Resources { get; }
        void WriteTo(Stream outputStream);
    }

    public static class CompiledResourceExtensions
    {
        public static void WriteToFile(this ICompiledResource resource, string path)
        {
            //ensure the destination directory exists
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            using (Stream outputStream = new FileStream(path, FileMode.Create))
            {
                resource.WriteTo(outputStream);
            }
        }
    }
}