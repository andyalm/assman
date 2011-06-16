using System.IO;
using System.IO.Compression;

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

        public static Stream Decompress(this Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }

        public static Stream Compress(this Stream stream, bool leaveOpen)
        {
            return new GZipStream(stream, CompressionMode.Compress, leaveOpen);
        }

        public static string ReadToEnd(this Stream stream)
        {
            return new StreamReader(stream).ReadToEnd();
        }
    }
}