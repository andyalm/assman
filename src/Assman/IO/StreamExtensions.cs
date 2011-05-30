using System.IO;

namespace Assman.IO
{
    public static class StreamExtensions
    {
         public static void CopyTo(this Stream source, Stream destination)
         {
             const int bufferSize = 4096;
             var buffer = new byte[bufferSize];
             int count;
             while ((count = source.Read(buffer, 0, buffer.Length)) != 0)
                 destination.Write(buffer, 0, count);
         }
    }
}