//
// Interop.FreeBSD.extattr64.cs
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

#region FreeBSD 64-bit type definitions
using size_t = System.Int64;
using ssize_t = System.Int64;

#endregion

internal static partial class Interop {
    internal static partial class FreeBSD {
        /// <summary>
        /// Gets an extended attribute value
        /// Calls to system's extattr_get_file(2)
        /// </summary>
        /// <returns>Number of bytes read. If <paramref name="data"/> is <c>null</c>, then the size for the buffer to store the data.</returns>
        /// <param name="path">Path to the file.</param>
        /// <param name="attrnamespace">Extended attribute namespace, <see cref="attrNamespace"/>.</param>
        /// <param name="attrname">Extended attribute name.</param>
        /// <param name="data">Pointer to buffer where to store the data.</param>
        /// <param name="nbytes">Size of the buffer.</param>
        [DllImport(Libraries.Libc, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern ssize_t extattr_get_file(string path, attrNamespace attrnamespace, string attrname, IntPtr data, size_t nbytes);

        /// <summary>
        /// Sets an extended attribute value
        /// Calls to system's extattr_set_file(2)
        /// </summary>
        /// <returns>Number of bytes written.</returns>
        /// <param name="path">Path to the file.</param>
        /// <param name="attrnamespace">Extended attribute namespace, <see cref="attrNamespace"/>.</param>
        /// <param name="attrname">Extended attribute name.</param>
        /// <param name="data">Pointer where the data is stored.</param>
        /// <param name="nbytes">Size of the data.</param>
        [DllImport(Libraries.Libc, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern ssize_t extattr_set_file(string path, attrNamespace attrnamespace, string attrname, IntPtr data, size_t nbytes);

        /// <summary>
        /// Deletes an extended attribute value
        /// Calls to system's extattr_delete_file(2)
        /// </summary>
        /// <returns>0 if successful, -1 if failure.</returns>
        /// <param name="path">Path to the file.</param>
        /// <param name="attrnamespace">Extended attribute namespace, <see cref="attrNamespace"/>.</param>
        /// <param name="attrname">Extended attribute name.</param>
        [DllImport(Libraries.Libc, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern ssize_t extattr_delete_file(string path, attrNamespace attrnamespace, string attrname);

        /// <summary>
        /// Gets a list of extended attributes that the file has in that namespace
        /// The list is stored in the buffer as an undetermined length of Pascal strings,
        /// 1 byte tells the size of the extended attribute name in bytes, and is followed by the name.
        /// Calls to system's extattr_list_file(2)
        /// </summary>
        /// <returns>Size of the list in bytes. If <paramref name="data"/> is <c>null</c>, then the size for the buffer to store the list.</returns>
        /// <param name="path">Path to the file.</param>
        /// <param name="attrnamespace">Extended attribute namespace, <see cref="attrNamespace"/>.</param>
        /// <param name="data">Pointer to buffer where to store the list.</param>
        /// <param name="nbytes">Size of the buffer.</param>
        [DllImport(Libraries.Libc, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern ssize_t extattr_list_file(string path, attrNamespace attrnamespace, IntPtr data, size_t nbytes);
    }
}

