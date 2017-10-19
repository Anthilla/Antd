using System;
using System.Collections.Generic;
using System.IO;

namespace Antd.sync {
    /// <summary>
    /// This class reads signature files in the same format as RDiff
    /// </summary>
    internal class ChecksumFileReader {
        /// <summary>
        /// The number of bytes in a long
        /// </summary>
        private const int BYTES_PER_LONG = 8;

        private const long INDEX_NOT_FOUND = -1;

        /// <summary>
        /// The length of a datablock
        /// </summary>
        private int _checksumBlockLenght;

        /// <summary>
        /// This is a lookup table for a hash of the weak checksum,
        /// used to quickly determine if a weak checksum exists
        /// </summary>
        private HashSet<uint> _weakLookup;

        private Dictionary<uint, List<long>> _weakKeysIndex;

        /// <summary>
        /// This list contains all strong hashes, encoded as unsigned int64,
        /// which compares much faster than a byte[]. If the strong hash does not fill
        /// an equal number of longs, the remaining bytes are set to zero
        /// before generating the long value.
        /// </summary>
        private ulong[] _strongIndex;

        /// <summary>
        /// The number of long values each strong hash occupies
        /// </summary>
        private int _longsPerStrong;

        private int ReadBlockLength(Stream input, byte[] buffer) {
            int response;

            // Reading block lenght (stram must be in the right position
            Utility.ReadChunk(input, buffer);

            // Getting the block length
            response = BitConverter.ToInt32(RDiffBinary.FixEndian(buffer), 0);

            // Validating the block length
            if(response < 1 || response > int.MaxValue / 2) {
                throw new Exception(string.Format(Strings.ChecksumFile.InvalidBlocksizeError, response));
            }

            return response;
        }

        private int ReadStrongLength(Stream input, byte[] buffer) {
            int response;

            Utility.ReadChunk(input, buffer);

            response = BitConverter.ToInt32(RDiffBinary.FixEndian(buffer), 0);

            if(response < 1 || response > (Utility.Hash.HashSize / 8)) {
                throw new Exception(string.Format(Strings.ChecksumFile.InvalidStrongsizeError, response));
            }

            return response;
        }

        /// <summary>
        /// Reads a ChecksumFile from a stream
        /// </summary>
        /// <param name="input">The stream to read from</param>
        public ChecksumFileReader(Stream input) {
            #region Variables

            byte[] readBuffer;
            int strongLength;
            long chunkCount;
            uint weak;
            byte[] strongBuffer;
            List<ulong> tempStrongIndex;

            #endregion


            readBuffer = new byte[4];

            Utility.ValidateSignature(input, readBuffer, RDiffBinary.SIGNATURE_MAGIC);

            // Saving the block length for future reference
            _checksumBlockLenght = ReadBlockLength(input, readBuffer);

            /// The number of bytes used for storing a single strong signature
            strongLength = ReadBlockLength(input, readBuffer);

            //Prepare the data structures
            _weakLookup = new HashSet<uint>(); //new bool[ushort.MaxValue + 1]; //bool[0x10000];
            _longsPerStrong = (strongLength + (BYTES_PER_LONG - 1)) / BYTES_PER_LONG;
            strongBuffer = new byte[_longsPerStrong * BYTES_PER_LONG];

            //We would like to use static allocation for these lists, but unfortunately
            // the zip stream does not report the correct length
            _weakKeysIndex = new Dictionary<uint, List<long>>();
            tempStrongIndex = new List<ulong>();

            chunkCount = 0;

            //Repeat until the stream is exhausted
            while(Utility.ForceStreamRead(input, readBuffer, 4) == 4) {
                weak = BitConverter.ToUInt32(RDiffBinary.FixEndian(readBuffer), 0);

                if(Utility.ForceStreamRead(input, strongBuffer, strongLength) != strongLength) {
                    throw new Exception(Strings.ChecksumFile.EndofstreamInStrongSignatureError);
                }

                // Ensure key existance
                if(!_weakKeysIndex.ContainsKey(weak)) {
                    _weakKeysIndex.Add(weak, new List<long>());
                }

                //Record the entries
                _weakKeysIndex[weak].Add(chunkCount);

                for(int i = 0; i < _longsPerStrong; i++) {
                    tempStrongIndex.Add(BitConverter.ToUInt64(strongBuffer, i * BYTES_PER_LONG));
                }

                chunkCount++;
            }

            // Storing the indexes in permanent arrays
            _strongIndex = tempStrongIndex.ToArray();
        }

        /// <summary>
        /// Gets the block index of data the equals the given data
        /// </summary>
        /// <param name="weakChecksum">The weak checksum for the data</param>
        /// <param name="data">The data buffer</param>
        /// <param name="offset">The offset into the data buffer</param>
        /// <param name="length">The number of bytes to read from the buffer</param>
        /// <param name="preferedIndex">The prefered index if multiple entries are found</param>
        /// <returns>The index of the matching element</returns>
        public long LookupChunck(uint weakChecksum, byte[] data, int offset, int length, long preferedIndex) {
            long responseValue;
            ulong[] strongDataHash;
            byte[] dataHash;

            // Assigning default value
            responseValue = INDEX_NOT_FOUND;

            // Entry level check
            if(_weakKeysIndex.ContainsKey(weakChecksum)) {
                //At this point we know that the weak hash matches something, so we have to calculate the strong hash
                /* if (!Utility.Hash.CanReuseTransform)
                 {
                     Utility.Hash = MD5.Create();
                 }*/

                dataHash = Utility.Hash.ComputeHash(data, offset, length);

                //Pad with zeros if needed
                if(dataHash.Length % BYTES_PER_LONG != 0) {
                    Array.Resize<byte>(ref dataHash, dataHash.Length + (BYTES_PER_LONG - dataHash.Length % BYTES_PER_LONG));
                }

                //Convert the hash to a list of long's so we can compare at max speed
                //Even though we allocate this buffer each time, it is optimized away,
                // so it is faster than having a pre-allocated buffer
                strongDataHash = new ulong[_longsPerStrong];
                for(int i = 0; i < strongDataHash.Length; i++) {
                    strongDataHash[i] = BitConverter.ToUInt64(dataHash, (i * BYTES_PER_LONG));
                }

                //Test first with prefered index as that gives the smallest possible file
                // we should test with the weak first, but we do not have a direct lookup into
                // the list of weak checksums, and this check should be rather fast,
                // the slow part is generating the MD4 hash, and that  has already been done
                if(preferedIndex >= 0 && preferedIndex < (_strongIndex.Length / _longsPerStrong)) {
                    // If hash matches the data
                    if(CompareHash(strongDataHash, preferedIndex)) {
                        // Updating response
                        responseValue = preferedIndex;
                    }
                }

                // If the preferred index does not match
                if(responseValue == INDEX_NOT_FOUND) {
                    // Search the strong values in the weak key bracket
                    foreach(long strongKey in _weakKeysIndex[weakChecksum]) {
                        // If hash matches the data
                        if(CompareHash(strongDataHash, strongKey)) {
                            responseValue = strongKey;
                        }
                    }
                }
            }

            return responseValue;
        }

        private bool CompareHash(ulong[] dataHash, long strongKey) {
            bool hashMatch;
            int dataHashIndex;

            // Initializing the iteration
            hashMatch = false;
            dataHashIndex = 0;

            while(dataHashIndex < dataHash.Length &&
                            (dataHash[dataHashIndex] == _strongIndex[(strongKey * _longsPerStrong) + dataHashIndex])) {
                dataHashIndex++;
            }

            if(dataHashIndex == dataHash.Length) {
                hashMatch = true;
            }

            return hashMatch;
        }

        /// <summary>
        /// Gets the number of bytes in each hashed block
        /// </summary>
        public int BlockLength { get { return _checksumBlockLenght; } }

        public bool DoesWeakExist(uint key) {
            return _weakLookup.Contains(key);
        }
    }
}
