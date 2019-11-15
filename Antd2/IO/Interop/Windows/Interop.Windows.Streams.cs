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
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

#region Win32 type definitions
using BOOL = System.Boolean;
using DWORD = System.UInt32;
using LPCWSTR = System.String;

#endregion

internal static partial class Interop {
    internal static partial class Windows {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid {
            SafeFindHandle() : base(true) {
            }

            protected override bool ReleaseHandle() {
                return FindClose(this.handle);
            }

            [DllImport(Libraries.Kernel32)]
            [return: MarshalAs(UnmanagedType.Bool)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            static extern bool FindClose(IntPtr handle);
        }

        public enum STREAM_INFO_LEVELS {
            /// <summary>
            /// The data is returned in a <see cref="WIN32_FIND_STREAM_DATA"/> structure.
            /// </summary>
            FindStreamInfoStandard = 0
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_STREAM_DATA {
            /// <summary>
            /// Sspecifies the size of the stream, in bytes.
            /// </summary>
            public long StreamSize;
            /// <summary>
            /// The name of the stream. The string name format is ":streamname:$streamtype".
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 296)]
            public string cStreamName;
        }

        /// <summary>
        /// Enumerates the first stream with a ::$DATA stream type in the specified file or directory.
        /// </summary>
        /// <returns>
        /// If the function succeeds, the return value is a search handle that can be used in subsequent calls to the FindNextStreamW function.
        /// If the function fails, <see cref="SafeFindHandle.IsInvalid"/> is set.
        /// </returns>
        /// <param name="lpFileName">The fully qualified file name.</param>
        /// <param name="InfoLevel">The information level of the returned data. This parameter is one of the values in the <see cref="STREAM_INFO_LEVELS"/> enumeration type.</param>
        /// <param name="lpFindStreamData">A pointer to a buffer that receives the file stream data. The format of this data depends on the value of the <paramref name="InfoLevel"/> parameter.</param>
        /// <param name="dwFlags">Reserved for future use. This parameter must be zero.</param>
        [DllImport(Libraries.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeFindHandle FindFirstStreamW(LPCWSTR lpFileName,
                                                             STREAM_INFO_LEVELS InfoLevel,
                                                             [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_STREAM_DATA lpFindStreamData,
                                                             DWORD dwFlags);


        /// <summary>
        /// Continues a stream search started by a previous call to the FindFirstStreamW function.
        /// </summary>
        /// <returns><c>true</c>, if there are more streams to find, <c>false</c> otherwise or on error.</returns>
        /// <param name="hndFindFile">The search handle returned by a previous call to the <see cref="FindFirstStreamW"/> function.</param>
        /// <param name="lpFindStreamData">A pointer to the <see cref="WIN32_FIND_STREAM_DATA"/> structure that receives information about the stream.</param>
        [DllImport(Libraries.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern BOOL FindNextStreamW(SafeFindHandle hndFindFile,
                                                  [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_STREAM_DATA lpFindStreamData);
    }
}