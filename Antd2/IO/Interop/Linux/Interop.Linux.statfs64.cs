//
// Interop.Linux.statfs64.cs
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

#region Linux 64-bit type definitions
using fsblkcnt_t = System.UInt64;
using fsfilcnt_t = System.UInt64;
using __fsword_t = System.Int64;

#endregion

internal static partial class Interop {
    internal static partial class Linux {
        /// <summary>
        /// statfs(2) structure when __WORDSIZE is 64
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct StatFS64 {
            /// <summary>
            /// Type of filesystem (see below)
            /// </summary>
            __fsword_t f_type;
            /// <summary>
            /// Optimal transfer block size
            /// </summary>
            __fsword_t f_bsize;
            /// <summary>
            /// Total data blocks in filesystem
            /// </summary>
            fsblkcnt_t f_blocks;
            /// <summary>
            /// Free blocks in filesystem
            /// </summary>
            fsblkcnt_t f_bfree;
            /// <summary>
            /// Free blocks available to unprivileged user
            /// </summary>
            fsblkcnt_t f_bavail;
            /// <summary>
            /// Total file nodes in filesystem
            /// </summary>
            fsfilcnt_t f_files;
            /// <summary>
            /// Free file nodes in filesystem
            /// </summary>
            fsfilcnt_t f_ffree;
            /// <summary>
            /// Filesystem ID
            /// </summary>
            fsid_t f_fsid;
            /// <summary>
            /// Maximum length of filenames
            /// </summary>
            __fsword_t f_namelen;
            /// <summary>
            /// Fragment size (since Linux 2.6)
            /// </summary>
            __fsword_t f_frsize;
            /// <summary>
            /// Mount flags of filesystem (since Linux 2.6.36)
            /// </summary>
            f_flags64_t f_flags;
            /// <summary>
            /// Padding bytes reserved for future use
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray,
                ArraySubType = UnmanagedType.I8, SizeConst = 4)]
            __fsword_t[] f_spare;
        }

        /// <summary>
        /// Obtains information of the file system mounted at <paramref name="path"/>.
        /// Calls to system's statfs(2)
        /// Only call if __WORDSIZE == 64
        /// </summary>
        /// <param name="path">Path to the filesystem mount point.</param>
        /// <param name="buf"><see cref="StatFS64"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true, EntryPoint = "statfs")]
        public static extern int statfs64(string path, out StatFS64 buf);
    }
}

