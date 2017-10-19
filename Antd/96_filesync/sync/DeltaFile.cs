using System;
using System.IO;

namespace Antd.sync {
    /// <summary>
    /// This class contains operations on a RDiff compatible delta file
    /// </summary>
    internal class DeltaFile {
        /// <summary>
        /// The size of the internal buffer used to read in data
        /// </summary>
        private const int BUFFER_SIZE = 100 * 1024;

        /// <summary>
        /// Creates a new file based on the basefile and the delta information.
        /// The basefile and output stream cannot point to the same resource (ea. file).
        /// The base file MUST be seekable.
        /// </summary>
        /// <param name="basefile">A seekable stream with the baseinformation</param>
        /// <param name="output">The stream to write the patched data to. Must not point to the same resource as the basefile.</param>
        public static void PatchFile(Stream basefile, Stream output, Stream deltaFile) {
            #region Variables

            byte[] readBuffer;
            int command;
            int commandLength;
            byte[] commandBuffer;
            long chunkSize;
            long baseFileOffset;

            #endregion

            readBuffer = new byte[4];

            Utility.ValidateSignature(deltaFile, readBuffer, RDiffBinary.DELTA_MAGIC);

            // Reading the first command
            command = deltaFile.ReadByte();

            //Keep reading until we hit the end command
            while(command != RDiffBinary.EndCommand) {
                // It is an error to omit the end command
                if(command < 0) {
                    throw new Exception(Strings.DeltaFile.EndofstreamWithoutMarkerError);
                }

                if(Enum.IsDefined(typeof(LiteralDeltaCommand), (LiteralDeltaCommand)command)) {
                    // Find out how many bytes of literal data there is
                    commandLength = RDiffBinary.GetLiteralLength((LiteralDeltaCommand)command);
                    commandBuffer = new byte[commandLength];

                    if(Utility.ForceStreamRead(deltaFile, commandBuffer, commandBuffer.Length) != commandBuffer.Length) {
                        throw new Exception(Strings.DeltaFile.UnexpectedEndofstreamError);
                    }

                    chunkSize = RDiffBinary.DecodeLength(commandBuffer);

                    if(chunkSize < 0) {
                        throw new Exception(Strings.DeltaFile.InvalidLitteralSizeError);
                    }

                    // Copy the literal data from the patch to the output
                    Utility.StreamCopy(deltaFile, output, chunkSize);
                }
                else if(Enum.IsDefined(typeof(CopyDeltaCommand), (CopyDeltaCommand)command)) {
                    // Find the offset of the data in the base file
                    commandLength = RDiffBinary.GetCopyOffsetSize((CopyDeltaCommand)command);
                    commandBuffer = new byte[commandLength];

                    if(Utility.ForceStreamRead(deltaFile, commandBuffer, commandBuffer.Length) != commandBuffer.Length) {
                        throw new Exception(Strings.DeltaFile.UnexpectedEndofstreamError);
                    }

                    baseFileOffset = RDiffBinary.DecodeLength(commandBuffer);
                    if(baseFileOffset < 0) {
                        throw new Exception(Strings.DeltaFile.InvalidCopyOffsetError);
                    }

                    //Find the length of the data to copy from the basefile
                    commandLength = RDiffBinary.GetCopyLengthSize((CopyDeltaCommand)command);
                    commandBuffer = new byte[commandLength];
                    if(Utility.ForceStreamRead(deltaFile, commandBuffer, commandBuffer.Length) != commandBuffer.Length) {
                        throw new Exception(Strings.DeltaFile.UnexpectedEndofstreamError);
                    }

                    chunkSize = RDiffBinary.DecodeLength(commandBuffer);
                    if(chunkSize < 0) {
                        throw new Exception(Strings.DeltaFile.InvalidCopyLengthError);
                    }

                    //Seek to the begining, and copy
                    basefile.Position = baseFileOffset;
                    Utility.StreamCopy(basefile, output, chunkSize);
                }
                else if(command <= RDiffBinary.LiteralLimit) {
                    //Literal data less than 64 bytes are found, copy it
                    Utility.StreamCopy(deltaFile, output, command);
                }
                else {
                    throw new Exception(Strings.DeltaFile.UnknownCommandError);
                }

                // Read the next command
                command = deltaFile.ReadByte();
            }

            output.Flush();
        }

        private static void LookupMatch(DeltaBlock<long> matched, DeltaBlock<int> buffer, BlockLookup lookup, Stream output) {
            //First match
            if(matched.Size == 0) {
                // Applying the lookup offset to the match
                matched.Offset = lookup.MatchedIndex * lookup.BlockLength;
            }
            else if(!lookup.DoesSequenceFit) {
                //Subsequent match, but the sequence does not fit
                WriteCopy(matched, output);

                //Pretend this was the first block
                matched.Size = 0;
                matched.Offset = lookup.MatchedIndex * lookup.BlockLength;
            }

            //If the next block matches this signature, we can write larger
            // copy instructions and thus safe space
            lookup.NextMatchKey = lookup.MatchedIndex + 1;

            //Adjust the counters
            matched.Size += lookup.BlockLength;
            buffer.Offset += lookup.BlockLength;
        }

        /// <summary>
        /// Generates a delta file from input, and writes it to output
        /// </summary>
        /// <param name="sourceData">The stream to generate the delta from</param>
        /// <param name="deltaData">The stream to write the delta to</param>
        public static void GenerateDeltaFile(Stream sourceData, Stream deltaData, ChecksumFileReader checksumFile) {
            #region variables

            // The matched data block
            DeltaBlock<long> matchedBufferIndex;

            // The unmatched data block
            DeltaBlock<int> unmatchedBufferIndex;

            // The buffer block
            DeltaBlock<int> bufferIndex;

            // The moving frame that looks for the blocks
            BlockLookup lookup;

            byte[] workingData;
            byte[] tempWork;

            uint weakChecksum;

            int lastPossibleBlock;
            int remainingBytes;

            bool loadBuffer = false;
            bool recalculateWeakChecksum = false;
            bool streamExhausted = false;
            bool doesWeakChecksumExist;
            bool finishedDeltaRun;
            int tempRead;

            #endregion

            #region Initialize the iteration

            matchedBufferIndex = new DeltaBlock<long>(0, 0);
            unmatchedBufferIndex = new DeltaBlock<int>(0, 0);
            bufferIndex = new DeltaBlock<int>(0, 0);
            lookup = new BlockLookup(checksumFile.BlockLength);

            //We use statically allocated buffers, and we need two buffers
            // to prevent Array.Copy from allocating a temp buffer
            workingData = new byte[BUFFER_SIZE];
            tempWork = new byte[BUFFER_SIZE];

            loadBuffer = false;
            recalculateWeakChecksum = false;
            streamExhausted = false;
            finishedDeltaRun = false;

            #endregion

            //Read the initial buffer block
            bufferIndex.Size = Utility.ForceStreamRead(sourceData, workingData);
            lookup.BlockLength = Math.Min(lookup.BlockLength, bufferIndex.Size);

            //Setup the initial checksum 
            //Calculate the Adler checksum of the buffer
            weakChecksum = Adler32Checksum.Calculate(workingData, 0, lookup.BlockLength);

            lookup.ResetMatchIndex();

            while(lookup.BlockLength > 0) {
                //Check if the block matches somewhere, if we have force-reloaded the buffer, 
                //the check has already been made
                if(loadBuffer) {
                    loadBuffer = false;
                }
                else {
                    lookup.MatchedIndex = checksumFile.LookupChunck(
                        weakChecksum,
                        workingData,
                        bufferIndex.Offset,
                        lookup.BlockLength,
                        lookup.NextMatchKey);
                }

                if(lookup.IsMatch) {
                    // We have a match, flush the unmatched chunk into the result
                    // But the match is offset.
                    CommitUnmatchedData(workingData, unmatchedBufferIndex, deltaData);

                    // Matched a stream chunk in the weak checksum. We don't yet know where the actual chunk is. 
                    // Find the chunk
                    LookupMatch(matchedBufferIndex, bufferIndex, lookup, deltaData);

                    if(bufferIndex.Size - bufferIndex.Offset < lookup.BlockLength) {
                        //If this is the last chunck, compare to the last hash
                        if(finishedDeltaRun) {
                            lookup.BlockLength = Math.Min(lookup.BlockLength, bufferIndex.Size - bufferIndex.Offset);
                        }
                        else //We are out of buffer, reload
                        {
                            recalculateWeakChecksum = true;
                        }
                    }

                    //Reset the checksum to fit the new block, but skip it if we are out of data
                    if(!recalculateWeakChecksum) {
                        weakChecksum = Adler32Checksum.Calculate(workingData, bufferIndex.Offset, lookup.BlockLength);
                    }
                }
                else {
                    //At this point we have not advanced the buffer_index, so the weak_checksum matches the data,
                    // even if we arrive here after reloading the buffer

                    //No match, flush accumulated matches, if any
                    if(matchedBufferIndex.Size > 0) {
                        //Send the matching bytes as a copy
                        WriteCopy(matchedBufferIndex, deltaData);
                        matchedBufferIndex.Reset(0, 0);

                        //We do not immediately start tapping the unmatched bytes, 
                        // because the buffer may be nearly empty, and we 
                        // want to gather as many unmatched bytes as possible
                        // to avoid the instruction overhead in the file
                        if(bufferIndex.Offset != 0) {
                            loadBuffer = true;
                        }
                    }
                    else {
                        // No match
                        lastPossibleBlock = bufferIndex.Size - lookup.BlockLength;
                        if(unmatchedBufferIndex.Size == 0) {
                            unmatchedBufferIndex.Offset = bufferIndex.Offset;
                        }

                        doesWeakChecksumExist = false;

                        //Local speedup for long non-matching regions
                        while(bufferIndex.Offset < lastPossibleBlock && !doesWeakChecksumExist) {
                            //Roll the weak checksum buffer by 1 byte until you find a match or you reach the end
                            weakChecksum = Adler32Checksum.Roll(
                                workingData[bufferIndex.Offset],
                                workingData[bufferIndex.Offset + lookup.BlockLength],
                                weakChecksum,
                                lookup.BlockLength);

                            //Check if the new weak checksum roll is a match
                            doesWeakChecksumExist = checksumFile.DoesWeakExist(weakChecksum >> 16);

                            //Update offset for future reference
                            bufferIndex.Offset++;
                        }

                        // Calculating the unmatched chunk size
                        unmatchedBufferIndex.Size = bufferIndex.Offset - unmatchedBufferIndex.Offset;

                        //If this is the last block, claim the remaining bytes as unmatched
                        if(finishedDeltaRun) {
                            //There may be a minor optimization possible here, as the last chunk of the original file may still fit 
                            // and be smaller than the block length

                            unmatchedBufferIndex.Size += lookup.BlockLength;
                            lookup.BlockLength = 0;
                        }
                    }
                }

                //If we are out of buffer, try to load some more
                if(loadBuffer || bufferIndex.Size - bufferIndex.Offset <= checksumFile.BlockLength) {
                    // This section will flush the located unmatched into the delta stream(if any)
                    // And will refill the working buffer with the remainder of the previous buffer 
                    // And fresh data from the source stream

                    //The number of unused bytes the the buffer
                    remainingBytes = bufferIndex.Size - bufferIndex.Offset;

                    //If we have read the last bytes or the buffer is already full, skip this
                    if(!finishedDeltaRun && tempWork.Length - remainingBytes > 0) {
                        // Load the delta amount of data into temp work buffer
                        Buffer.BlockCopy(workingData, bufferIndex.Offset, tempWork, 0, remainingBytes);

                        //Prevent reading the stream after it has been exhausted because some streams break on that
                        if(streamExhausted) {
                            tempRead = 0;
                        }
                        else {
                            // Fill the remainder of tempWork from the source data
                            tempRead = Utility.ForceStreamRead(
                                sourceData,
                                tempWork,
                                remainingBytes,
                                tempWork.Length - remainingBytes);
                        }

                        // If anything was read
                        if(tempRead > 0) {
                            //We are about to discard some data, if it is unmatched, write it to stream
                            CommitUnmatchedData(workingData, unmatchedBufferIndex, deltaData);

                            //Now swap the arrays
                            SwapBuffers(ref workingData, ref tempWork);

                            bufferIndex.Offset = 0;
                            bufferIndex.Size = remainingBytes + tempRead;
                        }
                        else {
                            //Prevent reading the stream after it has been exhausted because some streams break on that
                            streamExhausted = true;

                            if(remainingBytes <= checksumFile.BlockLength) {
                                //Mark as done
                                finishedDeltaRun = true;

                                //The last round has a smaller block length
                                lookup.BlockLength = remainingBytes;
                            }
                        }

                        //If we run out of buffer, we may need to recalculate the checksum
                        if(recalculateWeakChecksum) {
                            weakChecksum = Adler32Checksum.Calculate(workingData, bufferIndex.Offset, lookup.BlockLength);
                            recalculateWeakChecksum = false;
                        }
                    }
                }
            }

            //There cannot be both matched and unmatched bytes written
            if(matchedBufferIndex.Size > 0 && unmatchedBufferIndex.Size > 0) {
                throw new Exception(Strings.DeltaFile.InternalBufferError);
            }

            //Commit all remaining matched content
            if(matchedBufferIndex.Size > 0) {
                WriteCopy(matchedBufferIndex, deltaData);
            }

            //Commit all remaining unmatched content
            CommitUnmatchedData(workingData, unmatchedBufferIndex, deltaData);

            //Write end command
            deltaData.WriteByte((byte)RDiffBinary.EndCommand);
            deltaData.Flush();
        }

        private static void CommitUnmatchedData(byte[] workingData, DeltaBlock<int> unmatched, Stream deltaData) {
            if(unmatched.Size > 0) {
                WriteLiteral(workingData, unmatched, deltaData);
                unmatched.Size = 0;
            }
        }

        private static void SwapBuffers(ref byte[] a, ref byte[] b) {
            byte[] temp;

            temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Writes a literal command to a delta stream
        /// </summary>
        /// <param name="data">The literal data to write</param>
        /// <param name="output">The output delta stream</param>
        private static void WriteLiteral(byte[] data, DeltaBlock<int> block, Stream output) {
            byte[] lengthArray;

            output.WriteByte((byte)RDiffBinary.FindLiteralDeltaCommand(block.Size));
            lengthArray = RDiffBinary.EncodeLength(block.Size);
            output.Write(lengthArray, 0, lengthArray.Length);
            output.Write(data, block.Offset, block.Size);
        }

        /// <summary>
        /// Write a copy command to a delta stream
        /// </summary>
        /// <param name="offset">The offset in the basefile where the data is located</param>
        /// <param name="length">The length of the data to copy</param>
        /// <param name="output">The output delta stream</param>
        private static void WriteCopy(DeltaBlock<long> block, Stream output) {
            byte[] lengthArray;

            output.WriteByte((byte)RDiffBinary.FindCopyDeltaCommand(block));
            lengthArray = RDiffBinary.EncodeLength(block.Offset);
            output.Write(lengthArray, 0, lengthArray.Length);
            lengthArray = RDiffBinary.EncodeLength(block.Size);
            output.Write(lengthArray, 0, lengthArray.Length);
        }
    }
}
