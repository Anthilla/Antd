using System;
using System.Collections.Generic;
using System.IO;

namespace Antd.sync {
    internal class ChecksumGenerator {
        /// <summary>
        /// The number of bytes to include in each checksum
        /// </summary>
        private const int DEFAULT_BLOCK_SIZE = 2048;
        /// <summary>
        /// The default number of bytes to use from the strong hash
        /// </summary>
        private const int DEFAULT_STRONG_LEN = 8;
        /// <summary>
        /// The default number of bytes generated per input block
        /// </summary>
        private const int DEFAULT_BYTES_PER_BLOCK = DEFAULT_STRONG_LEN + 4;
        /// <summary>
        /// The number of bytes used for the rdiff header
        /// </summary>
        private static readonly int HEADER_SIZE = RDiffBinary.SIGNATURE_MAGIC.Length + 4 + 4;

        /// <summary>
        /// The final signature buffer
        /// </summary>
        public byte[] _signature { get; private set; }

        /// <summary>
        /// Byte array used to construct signature
        /// </summary>
        private List<byte> _signatureByteList;

        public ChecksumGenerator(Stream inputFileStream) {
            byte[] tempBuffer;
            int byteCounter;

            _signatureByteList = new List<byte>();
            _signatureByteList.AddRange(RDiffBinary.SIGNATURE_MAGIC);
            _signatureByteList.AddRange(RDiffBinary.FixEndian(BitConverter.GetBytes(DEFAULT_BLOCK_SIZE)));
            _signatureByteList.AddRange(RDiffBinary.FixEndian(BitConverter.GetBytes(DEFAULT_STRONG_LEN)));

            tempBuffer = new byte[DEFAULT_BLOCK_SIZE];

            while((byteCounter = inputFileStream.Read(tempBuffer, 0, tempBuffer.Length)) != 0) {
                AddSignatureChunk(tempBuffer, 0, byteCounter);
            }

            _signature = _signatureByteList.ToArray();
        }

        /// <summary>
        /// Adds a chunck of data to checksum list
        /// </summary>
        /// <param name="buffer">The data to add a checksum entry for</param>
        /// <param name="index">The index in the buffer to start reading from</param>
        /// <param name="count">The number of bytes to extract from the array</param>
        private void AddSignatureChunk(byte[] buffer, int index, int count) {
            byte[] tempBuffer;

            //if (!hashAlgorithm.CanReuseTransform)
            //{
            //    hashAlgorithm = MD5.Create();
            //}

            _signatureByteList.AddRange(
                RDiffBinary.FixEndian(
                    BitConverter.GetBytes(
                        Adler32Checksum.Calculate(buffer, index, count))));

            //Add first half of the computed hash
            tempBuffer = Utility.Hash.ComputeHash(buffer, index, count);

            for(int i = 0; i < DEFAULT_STRONG_LEN; i++) {
                _signatureByteList.Add(tempBuffer[i]);
            }
        }
    }
}
