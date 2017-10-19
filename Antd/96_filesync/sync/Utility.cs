using System;
using System.IO;
using System.Security.Cryptography;

namespace Antd.sync {
    internal static class Utility {
        private static MD5 _hashAlgorithm = MD5.Create();

        internal static HashAlgorithm Hash {
            get {
                return _hashAlgorithm;
            }
        }

        /// <summary>
        /// Size of buffers for copying stream
        /// </summary>
        internal static long DEFAULT_BUFFER_SIZE = 64 * 1024;

        /// <summary>
        /// Some streams can return a number that is less than the requested number of bytes.
        /// This is usually due to fragmentation, and is solved by issuing a new read.
        /// This function wraps that functionality.
        /// </summary>
        /// <param name="stream">The stream to read</param>
        /// <param name="buffer">The buffer to read into</param>
        /// <returns>The actual number of bytes read</returns>
        internal static int ForceStreamRead(Stream stream, byte[] buffer) {
            return ForceStreamRead(stream, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Some streams can return a number that is less than the requested number of bytes.
        /// This is usually due to fragmentation, and is solved by issuing a new read.
        /// This function wraps that functionality.
        /// </summary>
        /// <param name="stream">The stream to read</param>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="count">The amout of bytes to read</param>
        /// <returns>The actual number of bytes read</returns>
        internal static int ForceStreamRead(Stream stream, byte[] buffer, int count) {
            return ForceStreamRead(stream, buffer, 0, count);
        }

        /// <summary>
        /// Some streams can return a number that is less than the requested number of bytes.
        /// This is usually due to fragmentation, and is solved by issuing a new read.
        /// This function wraps that functionality.
        /// </summary>
        /// <param name="stream">The stream to read</param>
        /// <param name="buffer">The buffer to read into</param>
        /// <param name="index">The index into which the writing starts</param>
        /// <param name="count">The amout of bytes to read</param>
        /// <returns>The actual number of bytes read</returns>
        internal static int ForceStreamRead(Stream stream, byte[] buffer, int index, int count) {
            int currentRead;
            int response;

            //Non fragmenting streams skip here
            if(stream is FileStream || stream is MemoryStream) {
                response = stream.Read(buffer, index, count);
            }
            else {
                // Read manually
                response = 0;

                do {
                    currentRead = stream.Read(buffer, index, count);
                    index += currentRead;
                    response += currentRead;
                    count -= currentRead;
                }
                while(currentRead != 0 && count > 0);
            }

            return response;
        }

        /// <summary>
        /// Copies a number of bytes from one stream into another.
        /// </summary>
        /// <param name="input">The stream to read from</param>
        /// <param name="output">The stream to write to</param>
        /// <param name="count">The number of bytes to copy</param>
        internal static void StreamCopy(Stream input, Stream output, long count) {
            byte[] buffer;
            int bytesRead;

            buffer = new byte[DEFAULT_BUFFER_SIZE];

            while(count > 0) {
                bytesRead = input.Read(buffer, 0, (int)Math.Min(count, buffer.Length));

                if(bytesRead <= 0) {
                    throw new Exception(Strings.Utility.UnexpectedEndOfStreamError);
                }

                // Copy
                output.Write(buffer, 0, bytesRead);

                // Substracting the read from the overall
                count -= bytesRead;
            }
        }

        public static void ReadChunk(Stream from, byte[] to) {
            if(Utility.ForceStreamRead(from, to, 4) != 4) {
                throw new Exception(Strings.ChecksumFile.EndofstreamInStronglenError);
            }
        }

        public static void ValidateSignature(Stream input, byte[] buffer, byte[] signature) {
            ReadChunk(input, buffer);

            // Verify that the file header is valid
            for(int i = 0; i < buffer.Length; i++) {
                if(signature[i] != buffer[i]) {
                    throw new Exception(Strings.ChecksumFile.InvalidSignatureHeaderError);
                }
            }
        }
    }
}
