using System;

namespace Antd.sync {
    /// <summary>
    /// This is an implementation of the Adler-32 rolling checksum.
    /// This implementation is converted to C# from C code in librsync.
    /// </summary>
    internal class Adler32Checksum {
        /// <summary>
        /// The charated offset used in the checksum
        /// </summary>
        private const ushort CHAR_OFFSET = 31;

        /// <summary>
        /// Calculates the Adler32 checksum for the given data
        /// </summary>
        /// <param name="data">The data to calculate the checksum for</param>
        /// <param name="offset">The offset into the data to start reading</param>
        /// <param name="count">The number of bytes to process</param>
        /// <returns>The adler32 checksum for the data</returns>
        public static uint Calculate(byte[] data, int offset, int count) {
            // TODO : Review
            int i;

            ushort s1 = 0;
            ushort s2 = 0;

            int leftoverdata = (count % 4);
            long rounds = count - leftoverdata;

            //First do an optimized processing in chunks of 4 bytes
            for(i = 0; i < rounds; i += 4) {
                s2 += (ushort)(
                    4 * (s1 + data[offset + 0]) +
                    3 * data[offset + 1] +
                    2 * data[offset + 2] +
                    data[offset + 3] +
                    10 * CHAR_OFFSET);

                s1 += (ushort)(
                    data[offset + 0] +
                    data[offset + 1] +
                    data[offset + 2] +
                    data[offset + 3] +
                        4 * CHAR_OFFSET);

                offset += 4;
            }

            //Do a single step update of the remaining bytes
            for(i = 0; i < leftoverdata; i++) {
                s1 += (ushort)(data[offset + i] + CHAR_OFFSET);
                s2 += s1;
            }

            return (uint)((s1 & 0xffff) + (s2 << 16));
        }

        public bool Calculate(object workingData, int v, object blockLength) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an adler32 cheksum by excluding a byte, and including another
        /// </summary>
        /// <param name="out_byte">The byte that is no longer in the checksum</param>
        /// <param name="in_byte">The byte that is added to the checksum</param>
        /// <param name="checksum">The checksum including the out_byte</param>
        /// <param name="bytecount">The number of bytes the checksum contains</param>
        /// <returns>An updated checksum</returns>
        public static uint Roll(byte out_byte, byte in_byte, uint checksum, long bytecount) {
            ushort s1 = (ushort)(checksum & 0xffff);
            ushort s2 = (ushort)(checksum >> 16);

            s1 += (ushort)(in_byte - out_byte);
            s2 += (ushort)(s1 - (bytecount * (out_byte + CHAR_OFFSET)));

            return (uint)((s1 & 0xffff) + (s2 << 16));
        }
    }
}
