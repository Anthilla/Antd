//
// Interop.Apple.statfs.cs
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
using uid_t = System.UInt32;

#endregion

internal static partial class Interop {
    internal static partial class Apple {
        /// <summary>
        /// statfs(2) structure when _DARWIN_FEATURE_64_BIT_INODE is defined
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct StatFS {
            /// <summary>
            /// Type of file system (reserved: zero)
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            Int16 f_otype;
            /// <summary>
            /// Copy of mount flags (reserved: zero)
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            Int16 f_oflags;
            /// <summary>
            /// Fundamental file system block size
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_bsize;
            /// <summary>
            /// Optimal transfer block size
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_iosize;
            /// <summary>
            /// Total data blocks in file system
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_blocks;
            /// <summary>
            /// Free blocks in file system
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_bfree;
            /// <summary>
            /// Free blocks avail to non-superuser
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_bavail;
            /// <summary>
            /// Total file nodes in file system
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_files;
            /// <summary>
            /// Free file nodes in file system
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_ffree;
            /// <summary>
            /// File system id
            /// </summary>
            fsid_t f_fsid;
            /// <summary>
            /// User that mounted the file system
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            uid_t f_owner;
            /// <summary>
            /// Reserved for future use
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            [Obsolete("RESERVED: DO NOT USE")]
            Int16 f_reserved1;
            /// <summary>
            /// Type of file system (reserved)
            /// </summary>
            [MarshalAs(UnmanagedType.I2)]
            Int16 f_type;
            /// <summary>
            /// Copy of mount flags (reserved)
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            Int64 f_flags;
            /// <summary>
            /// Reserved for future use
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray,
                ArraySubType = UnmanagedType.I8, SizeConst = 2)]
            [Obsolete("RESERVED: DO NOT USE")]
            Int64[] f_reserved2;
            /// <summary>
            /// File system type name
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
            string f_fstypename;
            /// <summary>
            /// Directory on which mounted
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 90)]
            string f_mntonname;
            /// <summary>
            /// Mounted file system
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 90)]
            string f_mntfromname;
            /// <summary>
            /// Reserved for future use
            /// </summary>
            [Obsolete("RESERVED: DO NOT USE")]
            sbyte f_reserved3;
            /// <summary>
            /// Reserved for future use
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray,
                ArraySubType = UnmanagedType.I8, SizeConst = 4)]
            [Obsolete("RESERVED: DO NOT USE")]
            Int64[] f_reserved4;
        }

        /// <summary>
        /// Obtains information of the file system mounted at <paramref name="path"/>.
        /// Calls to system's statfs(2)
        /// </summary>
        /// <param name="path">Path to the filesystem mount point.</param>
        /// <param name="buf"><see cref="Stat"/> on 32 bit systems and <see cref="Stat64"/> on 64 bit systems.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int statfs(string path, out StatFS buf);
    }
}

