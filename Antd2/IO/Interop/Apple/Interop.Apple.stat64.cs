//
// Interop.Apple.stat64.cs
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

#region Mac OS X 64-bit type definitions

#endregion

internal static partial class Interop {
    internal static partial class Apple {
        /// <summary>
        /// stat(2) structure when __DARWIN_64_BIT_INO_T is defined
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct Stat64 {
            /// <summary>
            /// ID of device containing file
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int st_dev;
            /// <summary>
            /// Mode of file
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public mode_t st_mode;
            /// <summary>
            /// Number of hard links
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public ushort st_nlink;
            /// <summary>
            /// File serial number
            /// </summary>
            [MarshalAs(UnmanagedType.U8)]
            public ulong st_ino;
            /// <summary>
            /// User ID of the file
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uint st_uid;
            /// <summary>
            /// Group ID of the file
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uint st_gid;
            /// <summary>
            /// Device ID
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int st_rdev;
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
            /// time of file creation(birth)
            /// </summary>
            public Timespec st_birthtimespec;
            /// <summary>
            /// file size, in bytes
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public long st_size;
            /// <summary>
            /// blocks allocated for file
            /// </summary>
            [MarshalAs(UnmanagedType.I8)]
            public long st_blocks;
            /// <summary>
            /// optimal blocksize for I/O
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int st_blksize;
            /// <summary>
            /// user defined flags for file
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public flags_t st_flags;
            /// <summary>
            /// file generation number
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uint st_gen;
            /// <summary>
            /// Reserved: DO NOT USE
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            [Obsolete("RESERVED: DO NOT USE")]
            public uint st_lspare;
            /// <summary>
            /// Reserved: DO NOT USE
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray,
                ArraySubType = UnmanagedType.U8, SizeConst = 2)]
            [Obsolete("RESERVED: DO NOT USE")]
            public ulong[] st_qspare;
        }

        /// <summary>
        /// Obtains information of the file pointed by <paramref name="path"/>.
        /// Calls to system's stat64(2)
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="buf"><see cref="Stat64"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int stat64(string path, out Stat64 buf);
    }
}

