using System;
using System.IO;

namespace Antd.sync {
    internal class Signature {
        public const int DEFAULT_BLOCK_SIZE = 2048;

        public const int DEFAULT_STRONG_LENGTH = 8;

        /// <summary>
        /// The default number of bytes generated per input block
        /// </summary>
        public const int DEFAULT_BYTES_PER_BLOCK = DEFAULT_STRONG_LENGTH + 4;

        /// <summary>
        /// The number of bytes used for the rdiff header
        /// </summary>
        public static readonly int HEADER_SIZE = RDiffBinary.SIGNATURE_MAGIC.Length + 4 + 4;

        /// <summary>
        /// The number of bytes to include in each checksum
        /// </summary>
        public int BlockLength { get; private set; }

        /// <summary>
        /// The default number of bytes to use from the strong hash
        /// </summary>
        public int StrongLength { get; private set; }

        /// <summary>
        /// The signature data stream(the one that the signature is written to)
        /// </summary>
        public Stream Output { get; private set; }

        public Signature(Stream output) : this(output, DEFAULT_BLOCK_SIZE, DEFAULT_STRONG_LENGTH) { }

        /// <summary>
        /// Generates the object model to hold the general signature properties
        /// </summary>
        /// <param name="blocklength">The length of a single block</param>
        /// <param name="stronglength">The number of bytes in the MD5 checksum</param>
        /// <param name="outputstream">The stream into which the checksum data is written</param>
        public Signature(Stream output, int blockLength, int strongLength) {
            if(output == null) {
                throw new ArgumentNullException("outputstream");
            }

            BlockLength = blockLength;
            StrongLength = strongLength;
            Output = output;
        }
    }
}
