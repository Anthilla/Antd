//
// AppleStructs.cs
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

namespace Antd2.IO {
    /// <summary>
    /// A point as defined by Apple Finder
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4, CharSet = CharSet.Ansi)]
    public struct FinderPoint {
        /// <summary>
        /// Vertical (Y) coordinate
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short v;
        /// <summary>
        /// Horizontal (X) coordinate
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short h;
    }

    /// <summary>
    /// A rectangle as defined by Apple Finder
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4, CharSet = CharSet.Ansi)]
    public struct FinderRectangle {
        /// <summary>
        /// Top coordinate
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short top;
        /// <summary>
        /// Left coordinate
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short left;
        /// <summary>
        /// Bottom coordinate
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short bottom;
        /// <summary>
        /// Right coordinate
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short right;
    }

    /// <summary>
    /// Finder Info for files
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16, CharSet = CharSet.Ansi)]
    public struct AppleFileInfo {
        /// <summary>
        /// OSType of file type
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string fileType;
        /// <summary>
        /// OSType of file creator
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string fileCreator;
        /// <summary>
        /// <see cref="Antd2.IO.FinderFlags"/> 
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public FinderFlags finderFlags;
        /// <summary>
        /// Location of file icon in folder
        /// </summary>
        public FinderPoint location;
        /// <summary>
        /// DEPRECATED: ID of the folder this file belongs to.
        /// </summary>
        /// <value>-3 = Trash</value>
        /// <value>-2 = Desktop/value>
        [MarshalAs(UnmanagedType.I2)]
        public short folderID;
    }

    /// <summary>
    /// Extended Finder Info for files
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16, CharSet = CharSet.Ansi)]
    public struct AppleExtendedFileInfo {
        /// <summary>
        /// DEPRECATED: ID on resource fork to use as file's icon
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short iconID;
        /// <summary>
        /// Reserved
        /// </summary>
        [MarshalAs(UnmanagedType.LPArray,
            ArraySubType = UnmanagedType.I2, SizeConst = 3)]
        public short[] reserved1;
        /// <summary>
        /// <see cref="Antd2.IO.ExtendedFinderFlags"/>
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ExtendedFinderFlags extendedFinderFlags;
        /// <summary>
        /// DEPRECATED: Comment ID if high-bit is set
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short comment;
        /// <summary>
        /// "Put away folder" ID
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int putAwayFolderID;
    }

    /// <summary>
    /// Finder Info for folders
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16, CharSet = CharSet.Ansi)]
    public struct AppleFolderInfo {
        /// <summary>
        /// Rectangle of the Finder window for this folder
        /// </summary>
        public FinderRectangle windowBounds;
        /// <summary>
        /// <see cref="Antd2.IO.FinderFlags"/>
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public FinderFlags finderFlags;
        /// <summary>
        /// Location of this folder's icon on parent folder's view
        /// </summary>
        public FinderPoint location;
        /// <summary>
        /// Code for Finder selected view
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short view;
    }

    /// <summary>
    /// Extended Finder Info for folders
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16, CharSet = CharSet.Ansi)]
    public struct AppleExtendedFolderInfo {
        /// <summary>
        /// Scroll position of this folder's Finder window
        /// </summary>
        public FinderPoint scrollPosition;
        /// <summary>
        /// DEPRECATED: Unknown
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int openChain;
        /// <summary>
        /// <see cref="Antd2.IO.ExtendedFinderFlags"/>
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ExtendedFinderFlags extendedFinderFlags;
        /// <summary>
        /// DEPRECATED: Comment ID if high-bit is set
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short comment;
        /// <summary>
        /// "Put away folder" ID
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int putAwayFolderID;
    }

    /// <summary>
    /// Finder info as returned by newer Mac OS APIs (> 8)
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32, CharSet = CharSet.Ansi)]
    public struct FinderInfo {
        /// <summary>
        /// Finder info for files
        /// </summary>
        [FieldOffset(0)]
        public AppleFileInfo fileInfo;
        /// <summary>
        /// Extended Finder info for files
        /// </summary>
        [FieldOffset(16)]
        public AppleExtendedFileInfo extendedFileInfo;
        /// <summary>
        /// Finder info for folders
        /// </summary>
        [FieldOffset(0)]
        public AppleFolderInfo folderInfo;
        /// <summary>
        /// Extended Finder info for folders
        /// </summary>
        [FieldOffset(16)]
        public AppleExtendedFolderInfo extendedFolderInfo;
    }
}

