//
// Interop.Windows.EAs.cs
//
// Author:
//       Natalia Portillo <claunia@claunia.com>
//
// Copyright (c) 2015 © Claunia.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software", to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Runtime.InteropServices;

#region Win32 type definitions
using BOOLEAN = System.Boolean;
using CHAR = System.Byte;
using HANDLE = Microsoft.Win32.SafeHandles.SafeFileHandle;
using PULONG = System.UInt32;
using PVOID = System.IntPtr;
using UCHAR = System.Byte;
using ULONG = System.UInt32;
using USHORT = System.UInt16;

#endregion

internal static partial class Interop {
    internal static partial class Windows {
        /// <summary>
        /// Status of a I/O call
        /// </summary>
        public struct IO_STATUS_BLOCK {
            /// <summary>
            /// This is the completion status, either <see cref="NTSTATUS.STATUS_SUCCESS"/> if the requested operation was completed successfully or an informational, warning, or error STATUS_XXX value. For more information, <see cref="NTSTATUS"/>.
            /// </summary>
            public NTSTATUS status;
            /// <summary>
            /// This is set to a request-dependent value. For example, on successful completion of a transfer request, this is set to the number of bytes transferred. If a transfer request is completed with another STATUS_XXX, this member is set to zero.
            /// </summary>
            public PVOID information;
        }

        const byte FILE_NEED_EA = 0x80;

        /// <summary>
        /// The FILE_FULL_EA_INFORMATION structure provides extended attribute (EA) information.
        /// </summary>
        public struct FILE_FULL_EA_INFORMATION {
            /// <summary>
            /// The offset of the next <see cref="FILE_FULL_EA_INFORMATION"/>-type entry. This member is zero if no other entries follow this one.
            /// </summary>
            public ULONG NextEntryOffset;
            /// <summary>
            /// Can be zero or can be set with <see cref="FILE_NEED_EA"/>, indicating that the file to which the EA belongs cannot be interpreted without understanding the associated extended attributes.
            /// </summary>
            public UCHAR Flags;
            /// <summary>
            /// The length in bytes of the <see cref="EaName"/> array. This value does not include a <c>null</c>-terminator to <see cref="EaName"/>.
            /// </summary>
            public UCHAR EaNameLength;
            /// <summary>
            /// The length in bytes of each EA value in the array.
            /// </summary>
            public USHORT EaValueLength;
            /// <summary>
            /// An array of characters naming the EA for this entry.
            /// </summary>
            public CHAR[] EaName;
        }

        /// <summary>
        /// The FILE_GET_EA_INFORMATION structure is used to query for extended-attribute (EA) information.
        /// </summary>
        struct FILE_GET_EA_INFORMATION {
            /// <summary>
            /// Offset, in bytes, of the next <see cref="FILE_GET_EA_INFORMATION"/>-typed entry. This member is zero if no other entries follow this one.
            /// </summary>
            public ULONG NextEntryOffset;
            /// <summary>
            /// Length, in bytes, of the EaName array. This value does not include a <c>NULL</c> terminator.
            /// </summary>
            public UCHAR EaNameLength;
            /// <summary>
            /// Specifies the first character of the name of the extended attribute to be queried. This is followed in memory by the remainder of the string.
            /// </summary>
            public CHAR[] EaName;
        }

        /// <summary>
        /// Returns information about extended-attribute (EA) values for a file
        /// </summary>
        /// <returns>Returns <see cref="NTSTATUS.STATUS_SUCCESS"/> or an appropriate <see cref="NTSTATUS"/> value such as the following.
        /// <see cref="NTSTATUS.STATUS_EAS_NOT_SUPPORTED"/>: The file system does not support extended attributes.
        /// <see cref="NTSTATUS.STATUS_INSUFFICIENT_RESOURCES"/>: The NtQueryEaFile routine encountered a pool allocation failure.
        /// <see cref="NTSTATUS.STATUS_EA_LIST_INCONSISTENT"/>: The EaList parameter is not formatted correctly.
        /// </returns>
        /// <param name="FileHandle">The handle for the file on which the operation is to be performed.</param>
        /// <param name="IoStatusBlock">A pointer to an <see cref="IO_STATUS_BLOCK"/> structure that receives the final completion status and other information about the requested operation.</param>
        /// <param name="Buffer">A pointer to a caller-supplied <see cref="FILE_FULL_EA_INFORMATION"/>-structured output buffer, where the extended attribute values are to be returned.</param>
        /// <param name="Length">The length, in bytes, of the buffer that the Buffer parameter points to.</param>
        /// <param name="ReturnSingleEntry">Set to <c>true</c> if NtQueryEaFile should return only the first entry that is found.</param>
        /// <param name="EaList">A pointer to a caller-supplied <see cref="FILE_GET_EA_INFORMATION"/>-structured input buffer, which specifies the extended attributes to be queried. This parameter is optional and can be <c>null</c>.</param>
        /// <param name="EaListLength">The length, in bytes, of the buffer that the EaList parameter points to.</param>
        /// <param name="EaIndex">The index of the entry at which scanning the file's extended-attribute list should begin. This parameter is ignored if the EaList parameter points to a nonempty list. This parameter is optional and can be <c>null</c>.</param>
        /// <param name="RestartScan">Set to <c>true</c> if NtQueryEaFile should begin the scan at the first entry in the file's extended-attribute list. If this parameter is set to <c>false</c>, the routine resumes the scan from a previous call to ZwQueryEaFile.</param>
        [DllImport(Libraries.NTDLL)]
        public static extern NTSTATUS NtQueryEaFile(IntPtr FileHandle,
                                                    ref IO_STATUS_BLOCK IoStatusBlock, PVOID Buffer, ULONG Length,
                                                    [MarshalAs(UnmanagedType.Bool)] BOOLEAN ReturnSingleEntry, PVOID EaList, ULONG EaListLength, ref PULONG EaIndex,
                                                    [MarshalAs(UnmanagedType.Bool)] BOOLEAN RestartScan);

        /// <summary>
        /// Sets extended-attribute (EA) values for a file.
        /// </summary>
        /// <returns>Returns STATUS_SUCCESS or an appropriate NTSTATUS value such as the following:
        /// <see cref="NTSTATUS.STATUS_EA_LIST_INCONSISTENT"/>: The EaList parameter is not formatted correctly.
        /// </returns>
        /// <param name="FileHandle">The handle for the file on which the operation is to be performed.</param>
        /// <param name="IoStatusBlock">A pointer to an <see cref="IO_STATUS_BLOCK"/> structure that receives the final completion status and other information about the requested operation.</param>
        /// <param name="Buffer">A pointer to a caller-supplied, <see cref="FILE_FULL_EA_INFORMATION"/>-structured input buffer that contains the extended attribute values to be set.</param>
        /// <param name="Length">Length, in bytes, of the buffer that the Buffer parameter points to.</param>
        [DllImport(Libraries.NTDLL)]
        public static extern NTSTATUS NtSetEaFile(HANDLE FileHandle, ref IO_STATUS_BLOCK IoStatusBlock, PVOID Buffer, ULONG Length);
    }
}

