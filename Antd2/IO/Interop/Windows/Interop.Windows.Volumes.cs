//
// Interop.Windows.Volumes.cs
//
// Author:
//       Natalia Portillo <claunia@claunia.com>
//
// Copyright (c) 2015 © Claunia.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
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
using LPCTSTR = System.String;
using LPDWORD = System.UInt32;
using LPTSTR = System.Text.StringBuilder;

#endregion

internal static partial class Interop {
    internal static partial class Windows {
        [Flags]
        public enum FileSystemFlags : LPDWORD {
            /// <summary>
            /// The specified volume supports case-sensitive file names.
            /// </summary>
            FILE_CASE_SENSITIVE_SEARCH = 0x00000001,
            /// <summary>
            /// The specified volume supports preserved case of file names when it places a name on disk.
            /// </summary>
            FILE_CASE_PRESERVED_NAMES = 0x00000002,
            /// <summary>
            /// The specified volume supports Unicode in file names as they appear on disk.
            /// </summary>
            FILE_UNICODE_ON_DISK = 0x00000004,
            /// <summary>
            /// The specified volume preserves and enforces access control lists (ACL).
            /// </summary>
            FILE_PERSISTENT_ACLS = 0x00000008,
            /// <summary>
            /// The specified volume supports file-based compression.
            /// </summary>
            FILE_FILE_COMPRESSION = 0x00000010,
            /// <summary>
            /// The specified volume supports disk quotas.
            /// </summary>
            FILE_VOLUME_QUOTAS = 0x00000020,
            /// <summary>
            /// The specified volume supports sparse files.
            /// </summary>
            FILE_SUPPORTS_SPARSE_FILES = 0x00000040,
            /// <summary>
            /// The specified volume supports reparse points.
            /// </summary>
            FILE_SUPPORTS_REPARSE_POINTS = 0x00000080,

            /// <summary>
            /// Undocumented
            /// </summary>
            FILE_SUPPORTS_REMOTE_STORAGE = 0x00000100,
            /// <summary>
            /// Undocumented
            /// </summary>
            FILE_SUPPORTS_LFN_APIS = 0x00000200,

            /// <summary>
            /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
            /// </summary>
            FILE_VOLUME_IS_COMPRESSED = 0x00008000,
            /// <summary>
            /// The specified volume supports object identifiers.
            /// </summary>
            FILE_SUPPORTS_OBJECT_IDS = 0x00010000,
            /// <summary>
            /// The specified volume supports the Encrypted File System (EFS).
            /// </summary>
            FILE_SUPPORTS_ENCRYPTION = 0x00020000,
            /// <summary>
            /// The specified volume supports named streams.
            /// </summary>
            FILE_NAMED_STREAMS = 0x00040000,
            /// <summary>
            /// The specified volume is read-only.
            /// </summary>
            FILE_READ_ONLY_VOLUME = 0x00080000,
            /// <summary>
            /// The specified volume supports a single sequential write.
            /// </summary>
            FILE_SEQUENTIAL_WRITE_ONCE = 0x00100000,
            /// <summary>
            /// The specified volume supports transactions.
            /// </summary>
            FILE_SUPPORTS_TRANSACTIONS = 0x00200000,

            /// <summary>
            /// The specified volume supports hard links.
            /// </summary>
            FILE_SUPPORTS_HARD_LINKS = 0x00400000,
            /// <summary>
            /// The specified volume supports extended attributes. An extended attribute is a piece of application-specific metadata that an application can associate with a file and is not part of the file's data.
            /// </summary>
            FILE_SUPPORTS_EXTENDED_ATTRIBUTES = 0x00800000,
            /// <summary>
            /// The file system supports open by FileID.
            /// </summary>
            FILE_SUPPORTS_OPEN_BY_FILE_ID = 0x01000000,
            /// <summary>
            /// The specified volume supports update sequence number (USN) journals.
            /// </summary>
            FILE_SUPPORTS_USN_JOURNAL = 0x02000000,

            /// <summary>
            /// Undocumented
            /// </summary>
            FILE_SUPPORTS_INTEGRITY_STREAMS = 0x04000000
        }

        const int MAX_PATH = 260;

        /// <summary>
        /// Retrieves information about the file system and volume associated with the specified root directory.
        /// </summary>
        /// <returns><c>true</c>, if all the requested information is retrieved, <c>false</c> otherwise.</returns>
        /// <param name="lpRootPathName">A string that contains the root directory of the volume to be described.
        /// If this parameter is <c>null</c>, the root of the current directory is used. A trailing backslash is required.</param>
        /// <param name="lpVolumeNameBuffer">A pointer to a buffer that receives the name of a specified volume.</param>
        /// <param name="nVolumeNameSize">Length of <paramref name="lpVolumeNameBuffer"/>, to a maxium of <see cref="MAX_PATH"/>.</param>
        /// <param name="lpVolumeSerialNumber">A variable that receives the volume serial number.
        /// This parameter can be <c>null</c> if the serial number is not required.</param>
        /// <param name="lpMaximumComponentLength">A pointer to a variable that receives the maximum length, of a file name component that a specified file system supports.</param>
        /// <param name="lpFileSystemFlags">A variable that receives flags associated with the specified file system.</param>
        /// <param name="lpFileSystemNameBuffer">A buffer that receives the name of the file system, for example, the FAT file system or the NTFS file system.</param>
        /// <param name="nFileSystemNameSize">Length of <paramref name="lpFileSystemNameBuffer"/>, to a maxium of <see cref="MAX_PATH"/>.</param>
        [DllImport(Libraries.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static BOOL GetVolumeInformation(
            LPCTSTR lpRootPathName,
            LPTSTR lpVolumeNameBuffer,
            DWORD nVolumeNameSize,
            out LPDWORD lpVolumeSerialNumber,
            out LPDWORD lpMaximumComponentLength,
            out FileSystemFlags lpFileSystemFlags,
            LPTSTR lpFileSystemNameBuffer,
            DWORD nFileSystemNameSize);
    }
}