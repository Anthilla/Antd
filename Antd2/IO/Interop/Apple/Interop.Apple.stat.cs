//
// Interop.Apple.stat.cs
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
using System.Runtime.InteropServices;

#region Mac OS X 32-bit type definitions
using dev_t = System.Int32;
using gid_t = System.UInt32;
using ino_t = System.UInt32;
using nlink_t = System.UInt16;
using off_t = System.Int64;
using quad_t = System.Int64;
using uid_t = System.UInt32;
using u_long = System.UInt32;

#endregion

internal static partial class Interop {
    internal static partial class Apple {
        /// <summary>
        /// stat(2) structure when __DARWIN_64_BIT_INO_T is NOT defined
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct Stat {
            /// <summary>
            /// ID of device containing file
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public dev_t st_dev;
            /// <summary>
            /// File serial number
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public ino_t st_ino;
            /// <summary>
            /// Mode of file
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public mode_t st_mode;
            /// <summary>
            /// Number of hard links
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public nlink_t st_nlink;
            /// <summary>
            /// User ID of the file
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uid_t st_uid;
            /// <summary>
            /// Group ID of the file
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public gid_t st_gid;
            /// <summary>
            /// Device ID
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public dev_t st_rdev;
            /// <summary>
            /// time of last access
            /// </summary>
            public Timespec st_atimespec;
            /// <summary>
            /// time of last data modification
            /// </summary>
            public Timespec st_mtimespec;
            /// <summary>
            /// time of last status change
            /// </summary>
            public Timespec st_ctimespec;
            /// <summary>
            /// file size, in bytes
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public off_t st_size;
            /// <summary>
            /// blocks allocated for file
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public quad_t st_blocks;
            /// <summary>
            /// optimal blocksize for I/O
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public u_long st_blksize;
            /// <summary>
            /// user defined flags for file
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public flags_t st_flags;
            /// <summary>
            /// file generation number
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public u_long st_gen;
        }

        /// <summary>
        /// Obtains information of the file pointed by <paramref name="path"/>.
        /// Calls to system's stat(2)
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="buf"><see cref="Stat"/> on 32 bit systems and <see cref="Stat64"/> on 64 bit systems.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int stat(string path, out Stat buf);
    }
}

