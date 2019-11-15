//
// Interop.Apple.xattr.cs
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

#region Mac OS X 32-bit type definitions
using size_t = System.UInt32;
using u_int32_t = System.UInt32;

#endregion


internal static partial class Interop {
    internal static partial class Apple {
        public enum xattrOptions : int {
            /// <summary>
            /// Don't follow symbolic links
            /// </summary>
            XATTR_NOFOLLOW = 0x0001,
            /// <summary>
            /// Set the value and fail if the xattr already exists
            /// </summary>
            XATTR_CREATE = 0x0002,
            /// <summary>
            /// Set the value and fail if the xattr does not exist
            /// </summary>
            XATTR_REPLACE = 0x0004,
            /// <summary>
            /// Bypass authorization checking
            /// </summary>
            XATTR_NOSECURITY = 0x0008,
            /// <summary>
            /// Bypass default extended attribute file (._file)
            /// </summary>
            XATTR_NODEFAULT = 0x0010,
            /// <summary>
            /// Expose the HFS+ compression extended attributes
            /// </summary>
            XATTR_SHOWCOMPRESSION = 0x0020
        }

        /// <summary>
        /// Maximum length of xattr name
        /// </summary>
        const int XATTR_MAXNAMELEN = 127;

        /// <summary>
        /// Name for <see cref="Claunia.IO.FinderInfo"/> as a xattr
        /// </summary>
        const string XATTR_FINDERINFO_NAME = "com.apple.FinderInfo";
        /// <summary>
        /// Name for resource fork as a xattr
        /// </summary>
        const string XATTR_RESOURCEFORK_NAME = "com.apple.ResourceFork";

        /// <summary>
        /// Gets an extended attribute value
        /// Calls to system's getxattr(2)
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="name">Name of the extended attribute.</param>
        /// <param name="value">Pointer to a buffer where to store the extended attribute.</param>
        /// <param name="size">Size of the allocated buffer.</param>
        /// <param name="position">Offset of the extended attribute where to start reading, valid only for resource forks, 0 for rest.</param>
        /// <param name="options"><see cref="xattrOptions"/>.</param>
        /// <returns>Size of the extended attribute. On failure, -1, and errno is set</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int getxattr(string path, string name, IntPtr value, size_t size, u_int32_t position, xattrOptions options);

        /// <summary>
        /// Sets an extended attribute value
        /// Calls to system's setxattr(2)
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="name">Name of the extended attribute.</param>
        /// <param name="value">Pointer to a buffer where the extended attribute is stored.</param>
        /// <param name="size">Size of the allocated buffer.</param>
        /// <param name="position">Offset of the extended attribute where to start writing, valid only for resource forks, 0 for rest.</param>
        /// <param name="options"><see cref="xattrOptions"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int setxattr(string path, string name, IntPtr value, size_t size, u_int32_t position, xattrOptions options);

        /// <summary>
        /// Removes an extended attribute
        /// Calls to system's removexattr(2)
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="name">Name of the extended attribute.</param>
        /// <param name="options"><see cref="xattrOptions"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int removexattr(string path, string name, xattrOptions options);

        /// <summary>
        /// Lists the extended attributes from a file
        /// Calls to system's listxattr(2)
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="namebuf">Pointer to a buffer where an unordered list of null terminated UTF-8 strings wth the extended attributes names is to be stored.</param>
        /// <param name="size">Size of the allocated buffer.</param>
        /// <param name="options"><see cref="xattrOptions"/>.</param>
        /// <returns>If <paramref name="namebuf"/> is set to null, the needed size to store the whole list.
        /// On success, the size of the list.
        /// If the file has no extended attributes, 0.
        /// On failure, -1, and errno is set</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int listxattr(string path, IntPtr namebuf, size_t size, xattrOptions options);
    }
}

