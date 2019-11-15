//
// Interop.Linux.stat64.cs
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

#region Linux 64-bit type definitions
using blkcnt_t = System.Int64;
using blksize_t = System.Int64;
using dev_t = System.UInt64;
using gid_t = System.UInt32;
using ino_t = System.UInt64;
using nlink_t = System.UInt64;
using off_t = System.Int64;
using uid_t = System.UInt32;

#endregion

internal static partial class Interop {
    internal static partial class Linux {
        /// <summary>
        /// stat(2) structure when 64 bits
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct Stat64 {
            /// <summary>
            /// ID of device containing file
            /// </summary>
            dev_t st_dev;
            /// <summary>
            /// inode number
            /// </summary>
            ino_t st_ino;
            /// <summary>
            /// number of hard links
            /// </summary>
            nlink_t st_nlink;
            /// <summary>
            /// protection
            /// </summary>
            mode_t st_mode;
            /// <summary>
            /// user ID of owner
            /// </summary>
            uid_t st_uid;
            /// <summary>
            /// group ID of owner
            /// </summary>
            gid_t st_gid;
            /// <summary>
            /// padding
            /// </summary>
            Int32 __pad0;
            /// <summary>
            /// device ID (if special file)
            /// </summary>
            dev_t st_rdev;
            /// <summary>
            /// total size, in bytes
            /// </summary>
            off_t st_size;
            /// <summary>
            /// blocksize for filesystem I/O
            /// </summary>
            blksize_t st_blksize;
            /// <summary>
            /// number of 512B blocks allocated
            /// </summary>
            blkcnt_t st_blocks;
            /// <summary>
            /// time of last access
            /// </summary>
            Int64 st_atime;
            /// <summary>
            /// time of last access nanosecond
            /// </summary>
            UInt64 st_atimensec;
            /// <summary>
            /// time of last modification
            /// </summary>
            Int64 st_mtime;
            /// <summary>
            /// time of last modification naonsecond
            /// </summary>
            UInt64 st_mtimensec;
            /// <summary>
            /// time of last status change
            /// </summary>
            Int64 st_ctime;
            /// <summary>
            /// time of last status change nanosecond
            /// </summary>
            UInt64 st_ctimensec;
            [MarshalAs(UnmanagedType.ByValArray,
                ArraySubType = UnmanagedType.I8, SizeConst = 3)]
            [Obsolete("RESERVED: DO NOT USE")]
            Int64[] __glibc_reserved;
        }

        /// <summary>
        /// Obtains information of the file pointed by <paramref name="path"/>.
        /// Calls to system's stat(2) for 64 bit systems
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="buf"><see cref="Stat64"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true, EntryPoint = "stat")]
        public static extern int stat64(string path, out Stat64 buf);
    }
}

