//
// Interop.Apple.statfs64.cs
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

#region Mac OS X type definitions
using int32_t = System.Int32;
using uid_t = System.UInt32;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

#endregion

internal static partial class Interop {
    internal static partial class Apple {
        /// <summary>
        /// statfs(2) structure when _DARWIN_FEATURE_64_BIT_INODE is NOT defined
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct StatFS64 {
            /// <summary>
            /// Fundamental file system block size
            /// </summary>
            uint32_t f_bsize;
            /// <summary>
            /// Optimal transfer block size
            /// </summary>
            int32_t f_iosize;
            /// <summary>
            /// Total data blocks in file system
            /// </summary>
            uint64_t f_blocks;
            /// <summary>
            /// Free blocks in file system
            /// </summary>
            uint64_t f_bfree;
            /// <summary>
            /// Free blocks avail to non-superuser
            /// </summary>
            uint64_t f_bavail;
            /// <summary>
            /// Total file nodes in file system
            /// </summary>
            uint64_t f_files;
            /// <summary>
            /// Free file nodes in file system
            /// </summary>
            uint64_t f_ffree;
            /// <summary>
            /// File system id
            /// </summary>
            fsid_t f_fsid;
            /// <summary>
            /// User that mounted the filesystem
            /// </summary>
            uid_t f_owner;
            /// <summary>
            /// Type of filesystem
            /// </summary>
            UInt32 f_type;
            /// <summary>
            /// Copy of mount exported flags
            /// </summary>
            UInt32 f_flags;
            /// <summary>
            /// File system sub-type (flavor)
            /// </summary>
            UInt32 f_fssubtype;
            /// <summary>
            /// File system type name
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            string f_fstypename;
            /// <summary>
            /// Directory on which mounted
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            string f_mntonname;
            /// <summary>
            /// Mounted file system
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            string f_mntfromname;
            /// <summary>
            /// For future use
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray,
                ArraySubType = UnmanagedType.I8, SizeConst = 8)]
            [Obsolete("RESERVED: DO NOT USE")]
            uint32_t[] f_reserved;
        }

        /// <summary>
        /// Obtains information of the file system mounted at <paramref name="path"/>.
        /// Calls to system's statfs64(2)
        /// </summary>
        /// <param name="path">Path to the filesystem mount point.</param>
        /// <param name="buf"><see cref="Stat64"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int statfs64(string path, out StatFS64 buf);
    }
}

