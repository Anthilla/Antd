using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace antdsh {
    public static class StringCompression {
        public static string CompressString(string uncompressedString) {
            var compressedStream = new MemoryStream();
            var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString));
            using(var compressorStream = new DeflateStream(compressedStream, CompressionMode.Compress, true)) {
                uncompressedStream.CopyTo(compressorStream);
            }
            return Convert.ToBase64String(compressedStream.ToArray());
        }

        public static string DecompressString(string compressedString) {
            var decompressedStream = new MemoryStream();
            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));
            using(var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress)) {
                decompressorStream.CopyTo(decompressedStream);
            }
            return Encoding.UTF8.GetString(decompressedStream.ToArray());
        }
    }
}
