//
// Interop.Windows.Streams.cs
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
using BOOL = System.Boolean;
using DWORD = System.UInt32;
using HANDLE = Microsoft.Win32.SafeHandles.SafeFileHandle;
using LARGE_INTEGER = System.Int64;
using LPBYTE = System.IntPtr;
using LPDWORD = System.UInt32;
using LPVOID = System.IntPtr;
using WCHAR = System.String;

#endregion

internal static partial class Interop {
    internal static partial class Windows {
        /// <summary>
        /// Type of data on alternate stream.
        /// </summary>
        public enum StreamType : DWORD {
            /// <summary>
            /// Standard data. This corresponds to the NTFS $DATA stream type on the default (unnamed) data stream.
            /// </summary>
            BACKUP_DATA = 0x00000001,
            /// <summary>
            /// Extended attribute data. This corresponds to the NTFS $EA stream type.
            /// </summary>
            BACKUP_EA_DATA = 0x00000002,
            /// <summary>
            /// Security descriptor data.
            /// </summary>
            BACKUP_SECURITY_DATA = 0x00000003,
            /// <summary>
            /// Alternative data streams. This corresponds to the NTFS $DATA stream type on a named data stream.
            /// </summary>
            BACKUP_ALTERNATE_DATA = 0x00000004,
            /// <summary>
            /// Hard link information. This corresponds to the NTFS $FILE_NAME stream type.
            /// </summary>
            BACKUP_LINK = 0x00000005,
            /// <summary>
            /// Property data.
            /// </summary>
            BACKUP_PROPERTY_DATA = 0x00000006,
            /// <summary>
            /// Objects identifiers. This corresponds to the NTFS $OBJECT_ID stream type.
            /// </summary>
            BACKUP_OBJECT_ID = 0x00000007,
            /// <summary>
            /// Reparse points. This corresponds to the NTFS $REPARSE_POINT stream type.
            /// </summary>
            BACKUP_REPARSE_DATA = 0x00000008,
            /// <summary>
            /// Sparse file. This corresponds to the NTFS $DATA stream type for a sparse file.
            /// </summary>
            BACKUP_SPARSE_BLOCK = 0x00000009,
            /// <summary>
            /// Transactional NTFS (TxF) data stream. This corresponds to the NTFS $TXF_DATA stream type.
            /// </summary>
            BACKUP_TXFS_DATA = 0x0000000A
        }

        [Flags]
        public enum StreamAttributes : DWORD {
            /// <summary>
            /// Normal attribute
            /// </summary>
            STREAM_NORMAL_ATTRIBUTE = 0x00000000,
            /// <summary>
            /// Attribute set if the stream contains data that is modified when read. Allows the backup application to know that verification of data will fail.
            /// </summary>
            STREAM_MODIFIED_WHEN_READ = 0x00000001,
            /// <summary>
            /// Stream contains security data (general attributes). Allows the stream to be ignored on cross-operations restore.
            /// </summary>
            STREAM_CONTAINS_SECURITY = 0x00000002,
            STREAM_CONTAINS_PROPERTIES = 0x00000004,
            STREAM_SPARSE_ATTRIBUTE = 0x00000008
        }

        /// <summary>
        /// Contains stream data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct WIN32_STREAM_ID {
            /// <summary>
            /// Type of data.
            /// </summary>
            public StreamType dwStreamId;
            /// <summary>
            /// Attributes of data to facilitate cross-operating system transfer.
            /// </summary>
            public StreamAttributes dwStreamAttributes;
            /// <summary>
            /// Size of data, in bytes.
            /// </summary>
            public LARGE_INTEGER Size;
            /// <summary>
            /// Length of the name of the alternative data stream, in bytes.
            /// </summary>
            public DWORD dwStreamNameSize;
            /// <summary>
            /// Unicode string that specifies the name of the alternative data stream.
            /// </summary>
            public WCHAR cStreamName;
        }

        /// <summary>
        /// The BackupRead function can be used to back up a file or directory, including the security information. The function reads data associated with a specified file or directory into a buffer.
        /// </summary>
        /// <returns><c>true</c> on success, <c>false</c> otherwise.</returns>
        /// <param name="hFile">Handle to the file or directory to be backed up.</param>
        /// <param name="lpBuffer">Pointer to a buffer that receives the data.</param>
        /// <param name="nNumberOfBytesToRead">Length of the buffer, in bytes. The buffer size must be greater than the size of a <see cref="WIN32_STREAM_ID"/> structure.</param>
        /// <param name="lpNumberOfBytesRead">Pointer to a variable that receives the number of bytes read.</param>
        /// <param name="bAbort">Indicates whether you have finished using BackupRead on the handle. While you are backing up the file, specify this parameter as <c>false</c>. Once you are done using BackupRead, you must call BackupRead one more time specifying <c>true</c> for this parameter and passing the appropriate <paramref name="lpContext"/>. <paramref name="lpContext"/> must be passed when <paramref name="bAbort"/> is <c>true</c>; all other parameters are ignored.</param>
        /// <param name="bProcessSecurity">Indicates whether the function will restore the access-control list (ACL) data for the file or directory.
        /// If bProcessSecurity is <c>true</c>, the ACL data will be backed up.</param>
        /// <param name="lpContext">Pointer to a variable that receives a pointer to an internal data structure used by BackupRead to maintain context information during a backup operation.
        /// You must set the variable pointed to by  <paramref name="lpContext"/> to <c>null</c> before the first call to BackupRead for the specified file or directory. The function allocates memory for the data structure, and then sets the variable to point to that structure. You must not change <paramref name="lpContext"/> or the variable that it points to between calls to BackupRead.
        /// To release the memory used by the data structure, call BackupRead with the <paramref name="bAbort"/> parameter set to <c>true</c> when the backup operation is complete.</param>
        [DllImport(Libraries.Kernel32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BackupRead(HANDLE hFile, LPBYTE lpBuffer,
                                             DWORD nNumberOfBytesToRead, out LPDWORD lpNumberOfBytesRead,
                                             [MarshalAs(UnmanagedType.Bool)] BOOL bAbort,
                                             [MarshalAs(UnmanagedType.Bool)] BOOL bProcessSecurity,
                                             ref LPVOID lpContext);

        /// <summary>
        /// The BackupSeek function seeks forward in a data stream initially accessed by using the <see cref="BackupRead"/> or <see cref="BackupWrite"/> function.
        /// </summary>
        /// <returns><c>true</c>, if seek was backuped, <c>false</c> otherwise.</returns>
        /// <param name="hFile">Handle to the file or directory. This handle is created by using the <see cref="CreateFile"/> function.</param>
        /// <param name="dwLowBytesToSeek">Low-order part of the number of bytes to seek.</param>
        /// <param name="dwHighBytesToSeek">High-order part of the number of bytes to seek.</param>
        /// <param name="lpdwLowByteSeeked">Pointer to a variable that receives the low-order bits of the number of bytes the function actually seeks.</param>
        /// <param name="lpdwHighByteSeeked">Pointer to a variable that receives the high-order bits of the number of bytes the function actually seeks.</param>
        /// <param name="lpContext">Pointer to an internal data structure used by the function. This structure must be the same structure that was initialized by the <see cref="BackupRead"/> or <see cref="BackupWrite"/> function. An application must not touch the contents of this structure.</param>
        [DllImport(Libraries.Kernel32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BackupSeek(HANDLE hFile,
                                             DWORD dwLowBytesToSeek, DWORD dwHighBytesToSeek, out LPDWORD lpdwLowByteSeeked,
                                             out LPDWORD lpdwHighByteSeeked, ref LPVOID lpContext);
    }
}