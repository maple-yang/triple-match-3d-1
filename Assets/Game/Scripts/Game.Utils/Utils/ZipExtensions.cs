using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Game.Utils.Utils
{
    public static class ZipExtensions
    {
        public static bool IsCompressedToBase64(this string data)
        {
            return data.StartsWith("H4sI");
        }
        public static string CompressToBase64(this string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data).Compress());
        }

        public static string DecompressFromBase64(this string data)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(data).Decompress());
        }

        private static byte[] Compress(this byte[] data)
        {
            using var sourceStream = new MemoryStream(data);
            using var destinationStream = new MemoryStream();
            sourceStream.CompressTo(destinationStream);
            return destinationStream.ToArray();
        }

        private static byte[] Decompress(this byte[] data)
        {
            using var sourceStream = new MemoryStream(data);
            using var destinationStream = new MemoryStream();
            sourceStream.DecompressTo(destinationStream);
            return destinationStream.ToArray();
        }

        private static void CompressTo(this Stream stream, Stream outputStream)
        {
            using var gZipStream = new GZipStream(outputStream, CompressionLevel.Optimal);
            stream.CopyTo(gZipStream);
            gZipStream.Flush();
        }

        private static void DecompressTo(this Stream stream, Stream outputStream)
        {
            using var gZipStream = new GZipStream(stream, CompressionMode.Decompress);
            gZipStream.CopyTo(outputStream);
        }
    }
}