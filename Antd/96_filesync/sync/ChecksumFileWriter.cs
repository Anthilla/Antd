using System;
using System.IO;

namespace Antd.sync {
    /// <summary>
    /// This class writes signature files in the same format as RDiff
    /// </summary>
    internal static class ChecksumFileWriter {
        public static void WriteCommonHeader(Signature signature) {
            // Writing the initial header
            signature.Output.Write(RDiffBinary.SIGNATURE_MAGIC, 0, RDiffBinary.SIGNATURE_MAGIC.Length);
            signature.Output.Write(RDiffBinary.FixEndian(BitConverter.GetBytes(signature.BlockLength)), 0, 4);
            signature.Output.Write(RDiffBinary.FixEndian(BitConverter.GetBytes(signature.StrongLength)), 0, 4);
        }

        /// <summary>
        /// Adds all checksums in an entire stream 
        /// </summary>
        /// <param name="sourceData">The stream to read checksums from</param>
        /// <param name="signature">Holds the signature data parameters</param>
        public static void GenerateSignatureData(Signature signature, Stream sourceData) {
            byte[] buffer;
            int readBytes;

            buffer = new byte[signature.BlockLength];

            while((readBytes = Utility.ForceStreamRead(sourceData, buffer, buffer.Length)) != 0) {
                AddChunk(signature, buffer, 0, readBytes);
            }
        }

        /// <summary>
        /// Adds a chunck of data to checksum list
        /// </summary>
        /// <param name="buffer">The data to add a checksum entry for</param>
        /// <param name="index">The index in the buffer to start reading from</param>
        /// <param name="count">The number of bytes to extract from the array</param>
        private static void AddChunk(Signature signature, byte[] buffer, int index, int count) {
            //Add weak checksum (Adler-32) to the chunk
            signature.Output.Write(RDiffBinary.FixEndian(BitConverter.GetBytes(Adler32Checksum.Calculate(buffer, index, count))), 0, 4);

            //Add strong checksum
            signature.Output.Write(Utility.Hash.ComputeHash(buffer, index, count), 0, signature.StrongLength);
        }
    }
}
