using System;

namespace Antd.sync {
    /// <summary>
    /// This class contains various values and methods used for reading and writing
    /// RDiff compatible binary files
    /// </summary>
    internal class RDiffBinary {
        /// <summary>
        /// A small helper to determine the endianness of the machine
        /// </summary>
        private static readonly byte[] ENDIAN = BitConverter.GetBytes((short)1);
        /// <summary>
        /// The magic header value in an RDiff signature file
        /// </summary>
        public static readonly byte[] SIGNATURE_MAGIC = { (byte)'r', (byte)'s', 0x1, (byte)'6' };
        /// <summary>
        /// The magic header value in an RDiff delta file
        /// </summary>
        public static readonly byte[] DELTA_MAGIC = { (byte)'r', (byte)'s', 0x2, (byte)'6' };

        /// <summary>
        /// Denotes the end of a delta stream.
        /// </summary>
        public const byte EndCommand = 0x0;
        /// <summary>
        /// This is the max number of literal bytes that can be encoded without having the size present.
        /// The delta files created from this library does not contain these, but they can be read.
        /// </summary>
        public static byte LiteralLimit = 0x40;

        /// <summary>
        /// This is a lookup table to find a copy command.
        /// The first index is size of the offset in the original file,
        /// the second index is the size of the length of the block to copy.
        /// </summary>
        public static readonly CopyDeltaCommand[][] CopyCommand =
        {
            new CopyDeltaCommand[] {CopyDeltaCommand.Copy_Byte_Byte,  CopyDeltaCommand.Copy_Short_Byte,  CopyDeltaCommand.Copy_Int_Byte,  CopyDeltaCommand.Copy_Long_Byte },
            new CopyDeltaCommand[] {CopyDeltaCommand.Copy_Byte_Short, CopyDeltaCommand.Copy_Short_Short, CopyDeltaCommand.Copy_Int_Short, CopyDeltaCommand.Copy_Long_Short},
            new CopyDeltaCommand[] {CopyDeltaCommand.Copy_Byte_Int,   CopyDeltaCommand.Copy_Short_Int,   CopyDeltaCommand.Copy_Int_Int,   CopyDeltaCommand.Copy_Long_Int  },
            new CopyDeltaCommand[] {CopyDeltaCommand.Copy_Byte_Long,  CopyDeltaCommand.Copy_Short_Long,  CopyDeltaCommand.Copy_Int_Long,  CopyDeltaCommand.Copy_Long_Long },
        };

        /// <summary>
        /// Determines the number of bytes required to encode the given size
        /// </summary>
        /// <param name="size">The length to encode</param>
        /// <returns>The number of bytes required</returns>
        public static int FindLength(long size) {
            int count = 1;
            if(size > byte.MaxValue) {
                count++;
            }
            if(size > ushort.MaxValue) {
                count += 2;
            }
            if(size > uint.MaxValue) {
                count += 4;
            }
            if(size > long.MaxValue) {
                throw new Exception(string.Format(Strings.RDiffBinary.ValueTooLargeError, long.MaxValue));
            }

            return count;
        }

        /// <summary>
        /// Decodes the size parameter.
        /// </summary>
        /// <param name="data">The data to decode</param>
        /// <returns>The decoded size value</returns>
        public static long DecodeLength(byte[] data) {
            long response;

            switch(data.Length) {
                case (1): {
                        response = (long)data[0];

                        break;
                    }
                case (2): {
                        response = (long)BitConverter.ToUInt16(FixEndian(data), 0);

                        break;
                    }
                case (4): {
                        response = (long)BitConverter.ToUInt32(FixEndian(data), 0);

                        break;
                    }
                case (8): {
                        response = BitConverter.ToInt64(FixEndian(data), 0);

                        // Double check
                        if(response < 0) {
                            throw new Exception(string.Format(Strings.RDiffBinary.SizeTooLargeError, long.MaxValue));
                        }

                        break;
                    }
                default: {
                        throw new Exception(Strings.RDiffBinary.InvalidDataLengthError);
                    }
            }

            return response;
        }

        /// <summary>
        /// Returns an encoded array with the given size value, 
        /// using the least possible number of bytes.
        /// The value is fixed with regards to endianness.
        /// </summary>
        /// <param name="size">The value to write</param>
        /// <returns>The written bytes</returns>
        public static byte[] EncodeLength(long size) {
            byte[] response;
            int length;

            length = FindLength(size);

            switch(length) {
                case (1): {
                        response = new byte[] { (byte)size };

                        break;
                    }
                case (2): {
                        response = FixEndian(BitConverter.GetBytes((short)size));

                        break;
                    }
                case (4): {
                        response = FixEndian(BitConverter.GetBytes((int)size));

                        break;
                    }
                case (8): {
                        response = FixEndian(BitConverter.GetBytes((long)size));

                        break;
                    }
                default: {
                        throw new Exception(Strings.RDiffBinary.EncodedLengthError);
                    }
            }

            return response;
        }

        private static int NormalizeLength(int length) {
            int response;

            switch(length) {
                case (4): {
                        response = 3;

                        break;
                    }
                case (8): {
                        response = 4;

                        break;
                    }
                default: {
                        response = length;

                        break;
                    }
            }

            return response;
        }

        /// <summary>
        /// Returns the correct delta copy command, given the offset and size.
        /// </summary>
        /// <param name="offset">The offset in the file where the copy begins</param>
        /// <param name="size">The size of the copy</param>
        /// <returns>The delta copy command</returns>
        public static CopyDeltaCommand FindCopyDeltaCommand(DeltaBlock<long> copyBlock) {
            int offsetLength;
            int sizeLength;

            offsetLength = FindLength(copyBlock.Offset);
            offsetLength = NormalizeLength(offsetLength);

            sizeLength = FindLength(copyBlock.Size);
            sizeLength = NormalizeLength(sizeLength);

            return CopyCommand[sizeLength - 1][offsetLength - 1];
        }

        /// <summary>
        /// Returns the literal delta command for the given size
        /// </summary>
        /// <param name="size">The number that is to be encoded</param>
        /// <returns>The literal delta command</returns>
        public static LiteralDeltaCommand FindLiteralDeltaCommand(long size) {
            LiteralDeltaCommand response;

            switch(FindLength(size)) {
                case 1: {
                        response = LiteralDeltaCommand.LiteralSizeByte;

                        break;
                    }
                case 2: {
                        response = LiteralDeltaCommand.LiteralSizeShort;

                        break;
                    }
                case 4: {
                        response = LiteralDeltaCommand.LiteralSizeInt;

                        break;
                    }
                case 8: {
                        response = LiteralDeltaCommand.LiteralSizeLong;

                        break;
                    }
                default: {
                        throw new Exception("Invalid size for encoded value!");
                    }
            }

            return response;
        }

        /// <summary>
        /// Returns the number of bytes the copy commands offset argument occupies.
        /// </summary>
        /// <param name="command">The copy command to evaluate</param>
        /// <returns>The number of bytes the copy commands offset argument occupies.</returns>
        public static int GetCopyOffsetSize(CopyDeltaCommand command) {
            int response;

            switch(command) {
                case CopyDeltaCommand.Copy_Byte_Byte:
                case CopyDeltaCommand.Copy_Byte_Short:
                case CopyDeltaCommand.Copy_Byte_Int:
                case CopyDeltaCommand.Copy_Byte_Long: {
                        response = 1;

                        break;
                    }
                case CopyDeltaCommand.Copy_Short_Byte:
                case CopyDeltaCommand.Copy_Short_Short:
                case CopyDeltaCommand.Copy_Short_Int:
                case CopyDeltaCommand.Copy_Short_Long: {
                        response = 2;

                        break;
                    }
                case CopyDeltaCommand.Copy_Int_Byte:
                case CopyDeltaCommand.Copy_Int_Short:
                case CopyDeltaCommand.Copy_Int_Int:
                case CopyDeltaCommand.Copy_Int_Long: {
                        response = 4;

                        break;
                    }
                case CopyDeltaCommand.Copy_Long_Byte:
                case CopyDeltaCommand.Copy_Long_Short:
                case CopyDeltaCommand.Copy_Long_Int:
                case CopyDeltaCommand.Copy_Long_Long: {
                        response = 8;

                        break;
                    }
                default: {
                        throw new Exception(string.Format(Strings.RDiffBinary.InvalidDeltaCopyCommandError, command));
                    }
            }

            return response;
        }

        /// <summary>
        /// Returns the number of bytes the copy commands length argument occupies.
        /// </summary>
        /// <param name="command">The copy command to evaluate</param>
        /// <returns>The number of bytes the copy commands length argument occupies.</returns>
        public static int GetCopyLengthSize(CopyDeltaCommand command) {
            int response;

            switch(command) {
                case CopyDeltaCommand.Copy_Byte_Byte:
                case CopyDeltaCommand.Copy_Short_Byte:
                case CopyDeltaCommand.Copy_Int_Byte:
                case CopyDeltaCommand.Copy_Long_Byte: {
                        response = 1;

                        break;
                    }

                case CopyDeltaCommand.Copy_Byte_Short:
                case CopyDeltaCommand.Copy_Short_Short:
                case CopyDeltaCommand.Copy_Int_Short:
                case CopyDeltaCommand.Copy_Long_Short: {
                        response = 2;

                        break;
                    }
                case CopyDeltaCommand.Copy_Byte_Int:
                case CopyDeltaCommand.Copy_Short_Int:
                case CopyDeltaCommand.Copy_Int_Int:
                case CopyDeltaCommand.Copy_Long_Int: {
                        response = 4;

                        break;
                    }
                case CopyDeltaCommand.Copy_Byte_Long:
                case CopyDeltaCommand.Copy_Short_Long:
                case CopyDeltaCommand.Copy_Int_Long:
                case CopyDeltaCommand.Copy_Long_Long: {
                        response = 8;

                        break;
                    }
                default: {
                        throw new Exception(string.Format(Strings.RDiffBinary.InvalidDeltaCopyCommandError, command));
                    }
            }

            return response;
        }

        /// <summary>
        /// Returns the number of bytes the literal command's size argument fills
        /// </summary>
        /// <param name="command">The command to find the size for</param>
        /// <returns>The number of bytes the literal command's size argument fills</returns>
        public static int GetLiteralLength(LiteralDeltaCommand command) {
            int response;

            switch(command) {
                case LiteralDeltaCommand.LiteralSizeByte: {
                        response = 1;

                        break;
                    }
                case LiteralDeltaCommand.LiteralSizeShort: {
                        response = 2;

                        break;
                    }
                case LiteralDeltaCommand.LiteralSizeInt: {
                        response = 4;

                        break;
                    }
                case LiteralDeltaCommand.LiteralSizeLong: {
                        response = 8;

                        break;
                    }
                default: {
                        throw new Exception(string.Format(Strings.RDiffBinary.InvalidLiteralCommand, command));
                    }
            }

            return response;
        }

        /// <summary>
        /// Reverses endian order, if required
        /// </summary>
        /// <param name="data">The data to reverse</param>
        /// <returns>The reversed data</returns>
        public static byte[] FixEndian(byte[] data) {
            if(ENDIAN[0] != 0) {
                Array.Reverse(data);
            }

            return data;
        }
    }
}
