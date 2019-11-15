//
// Interop.FreeBSD.types.cs
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

internal static partial class Interop {
    internal static partial class FreeBSD {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Timespec {
            /// <summary>
            /// Seconds
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public Int32 tv_sec;
            /// <summary>
            /// Nanoseconds
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public Int32 tv_nsec;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Timespec64 {
            /// <summary>
            /// Seconds
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public Int64 tv_sec;
            /// <summary>
            /// Nanoseconds
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public Int64 tv_nsec;
        }

        /// <summary>
        /// File system ID type
        /// </summary>
        struct fsid_t {
            public Int32 val1;
            public Int32 val2;
        }

        [Flags]
        enum mntflags_t : UInt64 {
            /// <summary>read only filesystem</summary>
            MNT_RDONLY = 0x0000000000000001,
            /// <summary>fs written synchronously</summary>
            MNT_SYNCHRONOUS = 0x0000000000000002,
            /// <summary>can't exec from filesystem</summary>
            MNT_NOEXEC = 0x0000000000000004,
            /// <summary>don't honor setuid fs bits</summary>
            MNT_NOSUID = 0x0000000000000008,
            /// <summary>enable NFS version 4 ACLs</summary>
            MNT_NFS4ACLS = 0x0000000000000010,
            /// <summary>union with underlying fs</summary>
            MNT_UNION = 0x0000000000000020,
            /// <summary>fs written asynchronously</summary>
            MNT_ASYNC = 0x0000000000000040,
            /// <summary>special SUID dir handling</summary>
            MNT_SUIDDIR = 0x0000000000100000,
            /// <summary>using soft updates</summary>
            MNT_SOFTDEP = 0x0000000000200000,
            /// <summary>do not follow symlinks</summary>
            MNT_NOSYMFOLLOW = 0x0000000000400000,
            /// <summary>GEOM journal support enabled</summary>
            MNT_GJOURNAL = 0x0000000002000000,
            /// <summary>MAC support for objects</summary>
            MNT_MULTILABEL = 0x0000000004000000,
            /// <summary>ACL support enabled</summary>
            MNT_ACLS = 0x0000000008000000,
            /// <summary>dont update file access time</summary>
            MNT_NOATIME = 0x0000000010000000,
            /// <summary>disable cluster read</summary>
            MNT_NOCLUSTERR = 0x0000000040000000,
            /// <summary>disable cluster write</summary>
            MNT_NOCLUSTERW = 0x0000000080000000,
            /// <summary>using journaled soft updates</summary>
            MNT_SUJ = 0x0000000100000000,
            /// <summary>mounted by automountd(8)</summary>
            MNT_AUTOMOUNTED = 0x0000000200000000,

            /*
              * NFS export related mount flags.
              */
            /// <summary>exported read only</summary>
            MNT_EXRDONLY = 0x0000000000000080,
            /// <summary>filesystem is exported</summary>
            MNT_EXPORTED = 0x0000000000000100,
            /// <summary>exported to the world</summary>
            MNT_DEFEXPORTED = 0x0000000000000200,
            /// <summary>anon uid mapping for all</summary>
            MNT_EXPORTANON = 0x0000000000000400,
            /// <summary>exported with Kerberos</summary>
            MNT_EXKERB = 0x0000000000000800,
            /// <summary>public export (WebNFS)</summary>
            MNT_EXPUBLIC = 0x0000000020000000,

            /*
              * Flags set by internal operations,
              * but visible to the user.
              * XXX some of these are not quite right.. (I've never seen the root flag set)
              */
            /// <summary>filesystem is stored locally</summary>
            MNT_LOCAL = 0x0000000000001000,
            /// <summary>quotas are enabled on fs</summary>
            MNT_QUOTA = 0x0000000000002000,
            /// <summary>identifies the root fs</summary>
            MNT_ROOTFS = 0x0000000000004000,
            /// <summary>mounted by a user</summary>
            MNT_USER = 0x0000000000008000,
            /// <summary>do not show entry in df</summary>
            MNT_IGNORE = 0x0000000000800000
        }

        public enum attrNamespace {
            /// <summary>
            /// Empty namespace
            /// </summary>
            EXTATTR_NAMESPACE_EMPTY = 0x00000000,
            /// <summary>
            /// User namespace
            /// </summary>
            EXTATTR_NAMESPACE_USER = 0x00000001,
            /// <summary>
            /// System namespace
            /// </summary>
            EXTATTR_NAMESPACE_SYSTEM = 0x00000002
        }

        /// <summary>
        /// File mode and permissions
        /// </summary>
        [Flags]
        internal enum mode_t : ushort {
            /// <summary>type of file mask</summary>
            S_IFMT = 0xF000,
            /// <summary>named  pipe (fifo)</summary>
            S_IFIFO = 0x1000,
            /// <summary>character special</summary>
            S_IFCHR = 0x2000,
            /// <summary>directory</summary>
            S_IFDIR = 0x4000,
            /// <summary>block  special</summary>
            S_IFBLK = 0x6000,
            /// <summary>regular</summary>
            S_IFREG = 0x8000,
            /// <summary>symbolic link </summary>
            S_IFLNK = 0xA000,
            /// <summary>socket</summary>
            S_IFSOCK = 0xC000,
            /// <summary>whiteout</summary>
            S_IFWHT = 0xE000,
            /// <summary>set user id on execution</summary>
            S_ISUID = 0x0800,
            /// <summary>set group id on execution</summary>
            S_ISGID = 0x0400,
            /// <summary>save swapped text even after use</summary>
            S_ISVTX = 0x0200,
            /// <summary>RWX mask for owner</summary>
            S_IRWXU = 0x01C0,
            /// <summary>read permission, owner</summary>
            S_IRUSR = 0x0100,
            /// <summary>write  permission, owner</summary>
            S_IWUSR = 0x0080,
            /// <summary>execute/search permission, owner</summary>
            S_IXUSR = 0x0040,
            /// <summary>RWX mask for group</summary>
            S_IRWXG = 0x0038,
            /// <summary>read permission, group</summary>
            S_IRGRP = 0x0020,
            /// <summary>write  permission, group</summary>
            S_IWGRP = 0x0010,
            /// <summary>execute/search permission, group</summary>
            S_IXGRP = 0x0008,
            /// <summary>RWX mask for other</summary>
            S_IRWXO = 0x0007,
            /// <summary>read permission, other</summary>
            S_IROTH = 0x0004,
            /// <summary>write  permission, other</summary>
            S_IWOTH = 0x0002,
            /// <summary>execute/search permission, other</summary>
            S_IXOTH = 0x0001
        }

        /// <summary>
        /// User-set flags
        /// </summary>
        [Flags]
        internal enum flags_t : uint {
            /// <summary>
            /// Mask of owner changeable flags
            /// </summary>
            UF_SETTABLE = 0x0000FFFF,
            /// <summary>
            /// Do not dump file
            /// </summary>
            UF_NODUMP = 0x00000001,
            /// <summary>
            /// File may not be changed
            /// </summary>
            UF_IMMUTABLE = 0x00000002,
            /// <summary>
            /// Writes to file may only append
            /// </summary>
            UF_APPEND = 0x00000004,
            /// <summary>
            /// The directory is opaque when viewed through a union stack.
            /// </summary>
            UF_OPAQUE = 0x00000008,
            /// <summary>
            /// File may not be removed or renamed.
            /// </summary>
            UF_NOUNLINK = 0x00000010,
            /// <summary>
            /// File is compressed in HFS+ (>=10.6)
            /// </summary>
            [Obsolete("Unimplemented in FreeBSD")]
            UF_COMPRESSED = 0x00000020,
            /// <summary>
            /// OBSOLETE: No longer used.
            /// Issue notifications for deletes or renames of files with this flag set
            /// </summary>
            [Obsolete("Unimplemented in FreeBSD")]
            UF_TRACKED = 0x00000040,
            /// <summary>
            /// Windows system file bit
            /// </summary>
            UF_SYSTEM = 0x00000080,
            /// <summary>
            /// Sparse file
            /// </summary>
            UF_SPARSE = 0x00000100,
            /// <summary>
            /// File is offline
            /// </summary>
            UF_OFFLINE = 0x00000200,
            /// <summary>
            /// Windows reparse point file bit
            /// </summary>
            UF_REPARSE = 0x00000400,
            /// <summary>
            /// File needs to be archived
            /// </summary>
            UF_ARCHIVE = 0x00000800,
            /// <summary>
            /// Windows readonly file bit
            /// </summary>
            UF_READONLY = 0x00001000,

            /// <summary>
            /// File is hidden
            /// </summary>
            UF_HIDDEN = 0x00008000,

            /// <summary>
            /// Mask of superuser changeable flags
            /// </summary>
            SF_SETTABLE = 0xffff0000,
            /// <summary>
            /// File is archived
            /// </summary>
            SF_ARCHIVED = 0x00010000,
            /// <summary>
            /// File may not be changed
            /// </summary>
            SF_IMMUTABLE = 0x00020000,
            /// <summary>
            /// Writes to file may only append
            /// </summary>
            SF_APPEND = 0x00040000,
            /// <summary>
            /// File may not be removed or renamed
            /// </summary>
            SF_NOUNLINK = 0x00100000,
            /// <summary>
            /// Snapshot inode
            /// </summary>
            SF_SNAPSHOT = 0x00200000
        }
    }
}