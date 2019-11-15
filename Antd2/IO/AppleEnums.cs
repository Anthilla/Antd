//
// FinderFlags.cs
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

namespace Antd2.IO {
    /// <summary>
    /// FinderFlags from http://dubeiko.com/development/FileSystems/HFSPLUS/tn1150.html#FinderInfo
    /// More info from http://www.opensource.apple.com/source/CarbonHeaders/CarbonHeaders-9A581/Finder.h
    /// </summary>
    [Serializable]
    [Flags]
    public enum FinderFlags : ushort {
        /// <summary>
        /// File or folder should appear on Desktop
        /// </summary>
        IsOnDesktop = 0x0001,
        /// <summary>
        /// Has a color applied
        /// </summary>
        HasColor = 0x000E,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        RequireSwitchLaunch = 0x0020,
        /// <summary>
        /// If clear the application needs to write to its own resource fork and therefor, cannot be shared
        /// </summary>
        IsShared = 0x0040,
        /// <summary>
        /// Has no INIT resource
        /// </summary>
        HasNoINITs = 0x0080,
        /// <summary>
        /// Contained desktop database resources has already been added to desktop database
        /// </summary>
        HasBeenInited = 0x0100,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        ACOE = 0x0200,
        /// <summary>
        /// Has a custom icon resource
        /// </summary>
        HasCustomIcon = 0x0400,
        /// <summary>
        /// Is stationery
        /// </summary>
        IsStationery = 0x0800,
        /// <summary>
        /// Name cannot be changed
        /// </summary>
        NameLocked = 0x1000,
        /// <summary>
        /// Has a bundle resource (file)
        /// Has to be shown as a package (folder)
        /// </summary>
        HasBundle = 0x2000,
        /// <summary>
        /// Is invisible
        /// </summary>
        IsInvisible = 0x4000,
        /// <summary>
        /// Is an HFS Alias
        /// </summary>
        IsAlias = 0x8000
    }

    /// <summary>
    /// ExtendedFinderFlags from http://dubeiko.com/development/FileSystems/HFSPLUS/tn1150.html#FinderInfo
    /// </summary>
    [Serializable]
    [Flags]
    public enum ExtendedFinderFlags : ushort {
        /// <summary>
        /// This flags are invalid if this flag is set
        /// </summary>
        ExtendedFlagsAreInvalid = 0x8000,
        /// <summary>
        /// The file or folder has a badge resource
        /// </summary>
        HasCustomBadge = 0x0100,
        /// <summary>
        /// Object is busy or incomplete
        /// </summary>
        ObjectIsBusy = 0x0080,
        /// <summary>
        /// The file contains a routing info resource
        /// </summary>
        HasRoutingInfo = 0x0004
    }
}

