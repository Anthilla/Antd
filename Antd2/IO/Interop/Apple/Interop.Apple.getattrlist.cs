//
// Interop.Apple.getattrlist.cs
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

#region Mac OS X type definitions
using int32_t = System.Int32;
using u_int16_t = System.UInt16;
using u_int32_t = System.UInt32;
using u_short = System.UInt16;

// Only applicable to Mac OS X 32-bit ABI
using size_t = System.UInt32;

#endregion

internal static partial class Interop {
    internal static partial class Apple {
        /// <summary>
        /// Determines what attributes are returned by the function
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AttrList {
            /// <summary>
            /// Number of attr bit sets in list, ATTR_BIT_MAP_COUNT
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public u_short bitmapCount;
            /// <summary>
            /// To maintain 4-byte alignment
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public u_int16_t reserved;
            /// <summary>
            /// Common attribut group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public cmnAttrGroup_t commonAttr;
            /// <summary>
            /// Volume attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public volAttrGroup_t volAttr;
            /// <summary>
            /// Directory attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public dirAttrGroup_t dirAttr;
            /// <summary>
            /// File attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public fileAttrGroup_t fileAttr;
            /// <summary>
            /// Fork attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public forkAttrGroup_t forkAttr;
        }

        internal const UInt16 ATTR_BIT_MAP_COUNT = 5;
        internal const UInt32 DIR_MNTSTATUS_MNTPOINT = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        internal struct AttrReference {
            /// <summary>
            /// Offset in bytes from this structure to the structure data
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            int32_t dataOffset;
            /// <summary>
            /// Length of the attribute data in bytes, always alligned to 4 bytes.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            u_int32_t Length;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct VolCapabilitiesSet {
            /// <summary>
            /// Contains information about the volume format
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public volCapabilitiesFormat_t volCapabilitiesFormat;
            /// <summary>
            /// Contains information about optional functions supported by the volume format implementation
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public volCapabilitiesInterfaces_t volCapabilitiesInterfaces;
            /// <summary>
            /// Reserved, should be 0.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public u_int32_t volCapabilitiesReserved1;
            /// <summary>
            /// Reserved, should be 0.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public u_int32_t volCapabilitiesReserved2;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct VolCapabilitiesAttr {
            /// <summary>
            /// Indicates whether a particular feature is implemented.
            /// </summary>
            VolCapabilitiesSet capabilities;
            /// <summary>
            /// Indicates which flags are known to the volume format implementation.
            /// </summary>
            VolCapabilitiesSet valid;
        }

        /// <summary>
        /// Determines what attributes are returned by the function
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct AttributeSet {
            /// <summary>
            /// Common attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public cmnAttrGroup_t commonAttr;
            /// <summary>
            /// Volume attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public volAttrGroup_t volAttr;
            /// <summary>
            /// Directory attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public dirAttrGroup_t dirAttr;
            /// <summary>
            /// File attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public fileAttrGroup_t fileAttr;
            /// <summary>
            /// Fork attribute group
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public forkAttrGroup_t forkAttr;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct VolAttributesAttr {
            /// <summary>
            /// Indicates whether a particular attribute is implemented.
            /// </summary>
            AttributeSet capabilities;
            /// <summary>
            /// Indicates which flags are known to the volume format implementation.
            /// </summary>
            AttributeSet valid;
        }

        /// <summary>
        /// "options" parameter for getattrlist(2)
        /// </summary>
        [Flags]
        internal enum getAttrListOptions : uint {
            /// <summary>
            /// Do not follow symlinks
            /// </summary>
            FSOPT_NOFOLLOW = 0x00000001,
            /// <summary>
            /// The size of attributes reported will be the size needed to hold all the requested attributes
            /// </summary>
            FSOPT_REPORT_FULLSIZE = 0x00000002,
            /// <summary>
            /// Return all requested attributes, even ones not supported by the object or filesystem.
            /// Fills default values for invalid ones.
            /// </summary>
            FSOPT_PACK_INVAL_ATTRS = 0x00000004,
            /// <summary>
            /// If set, then ATTR_CMN_GEN_COUNT and ATTR_CMN_DOCUMENT_ID can be requested.
            /// </summary>
            FSOPT_ATTR_CMN_EXTENDED = 0x00000008
        }

        /// <summary>
        /// Common attributes to all types of file system objects.
        /// </summary>
        [Flags]
        internal enum cmnAttrGroup_t : uint {
            /// <summary>
            /// An attribute_set_t structure reporting which of the requested attributes were actually returned.
            /// It will always be the first attribute returned.
            /// </summary>
            ATTR_CMN_RETURNED_ATTRS = 0x80000000,
            /// <summary>
            /// An <see cref="AttrReference"/> structure containing the name of the file system object as an UTF-8 C string.
            /// </summary>
            ATTR_CMN_NAME = 0x00000001,
            /// <summary>
            /// A dev_t containing the device number of the device on which this file system object's volume is mounted.
            /// <see cref="Stat.st_dev"/> 
            /// </summary>
            ATTR_CMN_DEVID = 0x00000002,
            /// <summary>
            /// An fsid_t containing the file system identifier for the volume on which the file system object resides.
            /// <see cref="StatFS.f_fsid"/> 
            /// </summary>
            ATTR_CMN_FSID = 0x00000004,
            /// <summary>
            /// An fsobj_type_t that identifies the type of file system object.
            /// <see cref="vtype"/> 
            /// </summary>
            ATTR_CMN_OBJTYPE = 0x00000008,
            /// <summary>
            /// An fsobj_tag_t that identifies the type of file system containing the object.
            /// <see cref="vtagtype"/> 
            /// </summary>
            ATTR_CMN_OBJTAG = 0x00000010,
            /// <summary>
            /// An fsobj_id_t that uniquely identifies the file system object within a mounted volume for the duration of it's mount.
            /// It is not guaranteed to be persistent.
            /// </summary>
            ATTR_CMN_OBJID = 0x00000020,
            /// <summary>
            /// An fsobj_id_t that uniquely identifies fhe file system object, guaranteed to be persistent between mounts.
            /// </summary>
            ATTR_CMN_OBJPERMANENTID = 0x00000040,
            /// <summary>
            /// An fsobj_id_t that uniquely identifies the parent directory of the file system object for the duration of it's mount.
            /// It is not guaranteed to be persistent.
            /// If the file system object is hard linked from multiple directories, the parent returned for this attribute is non deterministic.
            /// </summary>
            ATTR_CMN_PAROBJID = 0x00000080,
            /// <summary>
            /// A <see cref="text_encoding_t"/> containing a text encoding hint for the file system objects name.
            /// File systems without an appropiate text encoding value should return <see cref="text_encoding_t.kTextEncodingMacUnicode"/>  
            /// </summary>
            ATTR_CMN_SCRIPT = 0x00000100,
            /// <summary>
            /// A <see cref="Timespec"/> containint the time that the file system object was created.
            /// </summary>
            ATTR_CMN_CRTIME = 0x00000200,
            /// <summary>
            /// A <see cref="Timespec"/> containint the time that the file system object was last modified.
            /// <see cref="Stat.st_mtimespec"/> 
            /// </summary>
            ATTR_CMN_MODTIME = 0x00000400,
            /// <summary>
            /// A <see cref="Timespec"/> containint the time that the file system object's attributes were last modified.
            /// <see cref="Stat.st_ctimespec"/> 
            /// </summary>
            ATTR_CMN_CHGTIME = 0x00000800,
            /// <summary>
            /// A <see cref="Timespec"/> containint the time that the file system object was last accessed.
            /// <see cref="Stat.st_atimespec"/> 
            /// </summary>
            ATTR_CMN_ACCTIME = 0x00001000,
            /// <summary>
            /// A <see cref="Timespec"/> containint the time that the file system object was last backed up.
            /// </summary>
            ATTR_CMN_BKUPTIME = 0x00002000,
            /// <summary>
            /// <see cref="Claunia.IO.FinderInfo"/>.
            /// Multibyte fields are always returned big endian.
            /// </summary>
            ATTR_CMN_FNDRINFO = 0x00004000,
            /// <summary>
            /// A uid_t containing the owner of the file system object.
            /// <see cref="Stat.st_uid"/> 
            /// </summary>
            ATTR_CMN_OWNERID = 0x00008000,
            /// <summary>
            /// A gid_t containing the group of the file system object.
            /// <see cref="Stat.st_gid"/> 
            /// </summary>
            ATTR_CMN_GRPID = 0x00010000,
            /// <summary>
            /// A <see cref="UInt32"/> containing a the access permissions of the file system object.
            /// <see cref="Stat.st_mode"/> 
            /// </summary>
            ATTR_CMN_ACCESSMASK = 0x00020000,
            /// <summary>
            /// A <see cref="UInt32"/> containing file flags of the file system object.
            /// <see cref="Stat.st_flags"/> 
            /// </summary>
            ATTR_CMN_FLAGS = 0x00040000,
            /// <summary>
            /// Not implemented
            /// </summary>
            [Obsolete("Never implemented")]
            ATTR_CMN_NAMEDATTRCOUNT = 0x00080000,
            /// <summary>
            /// Not implemented
            /// </summary>
            [Obsolete("Never implemented")]
            ATTR_CMN_NAMEDATTRLIST = 0x00100000,
            /// <summary>
            /// A <see cref="UInt32"/> containing a non zero monotonically increasing generation count for this file system object.
            /// Tracks the number of times the data in the object has been modified.
            /// Requires <see cref="getAttrListOptions.FSOPT_ATTR_CMN_EXTENDED"/> to be set.
            /// </summary>
            ATTR_CMN_GEN_COUNT = 0x00080000,
            /// <summary>
            /// A <see cref="UInt32"/> containing the document ID. This value is assigned by the kernel to an object to track
            /// it regardless of where it gets moved. 
            /// Requires <see cref="getAttrListOptions.FSOPT_ATTR_CMN_EXTENDED"/> to be set.
            /// </summary>
            ATTR_CMN_DOCUMENT_ID = 0x00100000,
            /// <summary>
            /// A <see cref="UInt32"/> containing the effective permissions of the current user (the process calling). 
            /// </summary>
            ATTR_CMN_USERACCESS = 0x00200000,
            /// <summary>
            /// An <see cref="AttrReference"/> containing a kauth_filesec with the ACL entry only.
            /// </summary>
            ATTR_CMN_EXTENDED_SECURITY = 0x00400000,
            /// <summary>
            /// A guid_t of the owner of the file system object.
            /// </summary>
            ATTR_CMN_UUID = 0x00800000,
            /// <summary>
            /// A guid_t of the group to which the file system object belongs.
            /// </summary>
            ATTR_CMN_GRPUUID = 0x01000000,
            /// <summary>
            /// A <see cref="UInt64"/> that uniquely identifies the file system object within it's mounted volume.
            /// <see cref="Stat.st_ino"/>  
            /// </summary>
            ATTR_CMN_FILEID = 0x02000000,
            /// <summary>
            /// A <see cref="UInt64"/> that uniquely identifies the parent directory of the file system object.
            /// </summary>
            ATTR_CMN_PARENTID = 0x04000000,
            /// <summary>
            /// An <see cref="AttrReference"/> containing the full path (resolving all symlinks) to the filesystem object as an UTF-8 C string.
            /// Inconsistent behaviour may be observed on hard linked items, particularly if the file system does not support <see cref="ATTR_CMN_PARENTID"/>
            /// </summary>
            ATTR_CMN_FULLPATH = 0x08000000,
            /// <summary>
            /// A <see cref="Timespec"/> that contains the time that the file system object was created OR renamed into its containing directory.
            /// Inconsistent behaviour may be observed on hard linked items.
            /// </summary>
            ATTR_CMN_ADDEDTIME = 0x10000000,
            /// <summary>
            /// A <see cref="UInt32"/> that contains the file or directory's data protection class.
            /// </summary>
            ATTR_CMN_DATA_PROTECT_FLAGS = 0x00000000
        }

        /// <summary>
        /// Attributes that relate to volumes (mounted file systems)
        /// </summary>
        [Flags]
        internal enum volAttrGroup_t : uint {
            /// <summary>
            /// This MUST ALWAYS BE SET
            /// </summary>
            ATTR_VOL_INFO = 0x80000000,
            /// <summary>
            /// A <see cref="UInt32"/> containing the file system type.
            /// <see cref="StatFS.f_type"/> 
            /// </summary>
            ATTR_VOL_FSTYPE = 0x00000001,
            /// <summary>
            /// A <see cref="UInt32"/> containing the volume signature word.
            /// This value is unique within a given file system type and lets distinguish between volume formats handled by same file system.
            /// </summary>
            ATTR_VOL_SIGNATURE = 0x00000002,
            /// <summary>
            /// An off_t containing the total size of the volume in bytes.
            /// </summary>
            ATTR_VOL_SIZE = 0x00000004,
            /// <summary>
            /// An off_t containing the free space of the volume in bytes.
            /// </summary>
            ATTR_VOL_SPACEFREE = 0x00000008,
            /// <summary>
            /// An off_t containing the free space of the volume in bytes that is available to non-privileged resources.
            /// </summary>
            ATTR_VOL_SPACEAVAIL = 0x00000010,
            /// <summary>
            /// An off_t containing the minimum allocation size on the volume in bytes.
            /// </summary>
            ATTR_VOL_MINALLOCATION = 0x00000020,
            /// <summary>
            /// An off_t conaining the allocation clump of the volume in btes.
            /// </summary>
            ATTR_VOL_ALLOCATIONCLUMP = 0x00000040,
            /// <summary>
            /// A <see cref="UInt32"/> containing the optimal block size when reading or writing.
            /// <see cref="StatFS.f_iosize"/> 
            /// </summary>
            ATTR_VOL_IOBLOCKSIZE = 0x00000080,
            /// <summary>
            /// A <see cref="UInt32"/> containing the number of file system objects on the volume. 
            /// </summary>
            ATTR_VOL_OBJCOUNT = 0x00000100,
            /// <summary>
            /// A <see cref="UInt32"/> containing the number of files on the volume. 
            /// </summary>
            ATTR_VOL_FILECOUNT = 0x00000200,
            /// <summary>
            /// A <see cref="UInt32"/> containing the number of directories on the volume. 
            /// </summary>
            ATTR_VOL_DIRCOUNT = 0x00000400,
            /// <summary>
            /// A <see cref="UInt32"/> containing the maximum number of file system objects that can be stored on the volume. 
            /// </summary>
            ATTR_VOL_MAXOBJCOUNT = 0x00000800,
            /// <summary>
            /// An <see cref="AttrReference"/> containing the path to the volume's mount point as an UTF-8 C string.
            /// </summary>
            ATTR_VOL_MOUNTPOINT = 0x00001000,
            /// <summary>
            /// An <see cref="AttrReference"/> containing the volume name as an UTF-8 C string.
            /// </summary>
            ATTR_VOL_NAME = 0x00002000,
            /// <summary>
            /// A <see cref="UInt32"/> containing the volume mount flags.
            /// <see cref="StatFS.f_flags"/> 
            /// </summary>
            ATTR_VOL_MOUNTFLAGS = 0x00004000,
            /// <summary>
            /// An <see cref="AttrReference"/> with the same value as <see cref="StatFS.f_mntfromname"/>.
            /// For local volumes this is the path to the device on which the volume is mounted in an UTF-8 C string.
            /// For network volumes this is a unique string identifying the mount.
            /// </summary>
            ATTR_VOL_MOUNTEDDEVICE = 0x00008000,
            /// <summary>
            /// A <see cref="UInt64"/> bitmap of text encodings used in the volume, corresponding to encodingsBitmap as DTS Technote 1150 
            /// </summary>
            ATTR_VOL_ENCODINGSUSED = 0x00010000,
            /// <summary>
            /// A <see cref="VolCapabilitiesAttr"/> describing the optional features supported by this volume.
            /// </summary>
            ATTR_VOL_CAPABILITIES = 0x00020000,
            /// <summary>
            /// A guid containing the file system UUID.
            /// </summary>
            ATTR_VOL_UUID = 0x00040000,
            /// <summary>
            /// A <see cref="VolAttributesAttr"/> describing the attributes supported by this volume.
            /// </summary>
            ATTR_VOL_ATTRIBUTES = 0x40000000
        }

        /// <summary>
        /// Attributes corresponding to directories
        /// </summary>
        [Flags]
        internal enum dirAttrGroup_t : uint {
            /// <summary>
            /// A <see cref="UInt32"/> containing the number of hard links to this directory.
            /// </summary>
            ATTR_DIR_LINKCOUNT = 0x00000001,
            /// <summary>
            /// A <see cref="UInt32"/> containing the number of file system objects in the directory.
            /// </summary>
            ATTR_DIR_ENTRYCOUNT = 0x00000002,
            /// <summary>
            /// A <see cref="UInt32"/> containing flags describing what's mounted in the directory.
            /// Currently only <see cref="DIR_MNTSTATUS_MNTPOINT"/> is defined, to indicate a file system is mounted on this directory.
            /// </summary>
            ATTR_DIR_MOUNTSTATUS = 0x00000004
        }

        /// <summary>
        /// Attributes corresponding to files
        /// </summary>
        [Flags]
        internal enum fileAttrGroup_t : uint {
            /// <summary>
            /// A <see cref="UInt32"/> containing the number of hard links to this file.
            /// <see cref="Stat.st_nlink"/> 
            /// </summary>
            ATTR_FILE_LINKCOUNT = 0x00000001,
            /// <summary>
            /// An off_t containing the total number of bytes in all forks (logical size).
            /// </summary>
            ATTR_FILE_TOTALSIZE = 0x00000002,
            /// <summary>
            /// An off_t containing the total number of bytes allocated for all forks (physical size).
            /// </summary>
            ATTR_FILE_ALLOCSIZE = 0x00000004,
            /// <summary>
            /// A <see cref="UInt32"/> containing the optimal block size for reading/writing this file.
            /// </summary>
            ATTR_FILE_IOBLOCKSIZE = 0x00000008,
            /// <summary>
            /// A <see cref="UInt32"/> containing the allocation clump size in bytes for this file.
            /// </summary>
            ATTR_FILE_CLUMPSIZE = 0x00000020,
            /// <summary>
            /// A <see cref="UInt32"/> containing the device type for a special device file.
            /// <see cref="Stat.st_rdev"/> 
            /// </summary>
            ATTR_FILE_DEVTYPE = 0x00000020,
            /// <summary>
            /// A <see cref="UInt32"/> whose value is reserved.
            /// </summary>
            ATTR_FILE_FILETYPE = 0x00000040,
            /// <summary>
            /// A <see cref="UInt32"/> containing the number of forks in the file.
            /// No file systems implement more than two forks (data and resource).
            /// </summary>
            ATTR_FILE_FORKCOUNT = 0x00000080,
            /// <summary>
            /// A <see cref="AttrReference"/> containing a list of the named forks in the file.
            /// Because Mac OS X only supports data and resource fork, the content of this attribute is not yet defined.
            /// </summary>
            ATTR_FILE_FORKLIST = 0x00000100,
            /// <summary>
            /// An off_t containing the logical size of the data fork in bytes
            /// </summary>
            ATTR_FILE_DATALENGTH = 0x00000200,
            /// <summary>
            /// An off_t containing the physical size of the data fork in bytes
            /// </summary>
            ATTR_FILE_DATAALLOCSIZE = 0x00000400,
            /// <summary>
            /// A diskextent structure representing the first eight extents of the data fork
            /// This attribute should not be used, and may not be accurate.
            /// </summary>
            ATTR_FILE_DATAEXTENTS = 0x00000800,
            /// <summary>
            /// An off_t containing the logical size of the resource fork in bytes
            /// </summary>
            ATTR_FILE_RSRCLENGTH = 0x00001000,
            /// <summary>
            /// An off_t containing the physical size of the resource fork in bytes
            /// </summary>
            ATTR_FILE_RSRCALLOCSIZE = 0x00002000,
            /// <summary>
            /// A diskextent structure representing the first eight extents of the resource fork
            /// This attribute should not be used, and may not be accurate.
            /// </summary>
            ATTR_FILE_RSRCEXTENTS = 0x00004000
        }

        /// <summary>
        /// Attributes related to the actual data in the file.
        /// They are not properly implemented and should not be requested.
        /// </summary>
        [Flags]
        internal enum forkAttrGroup_t : uint {
            /// <summary>
            /// An off_t containing the logical size of the fork in bytes
            /// </summary>
            ATTR_FORK_TOTALSIZE = 0x00000001,
            /// <summary>
            /// An off_t containing the physical size of the fork in bytes
            /// </summary>
            ATTR_FORK_ALLOCSIZE = 0x00000002
        }

        [Flags]
        internal enum volCapabilitiesFormat_t : u_int32_t {
            /// <summary>
            /// The volume supports persistent object identifiers
            /// </summary>
            VOL_CAP_FMT_PERSISTENTOBJECTIDS = 0x00000001,
            /// <summary>
            /// The volume supports symbolic links
            /// </summary>
            VOL_CAP_FMT_SYMBOLICLINKS = 0x00000002,
            /// <summary>
            /// The volume supports hard links
            /// </summary>
            VOL_CAP_FMT_HARDLINKS = 0x00000004,
            /// <summary>
            /// The volume supports journaling
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_JOURNAL = 0x00000008,
            /// <summary>
            /// The volume uses journaling
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_JOURNAL_ACTIVE = 0x00000010,
            /// <summary>
            /// The volume does not store reliable times for the root directory.
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_NO_ROOT_TIMES = 0x00000020,
            /// <summary>
            /// The volume supports sparse files.
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_SPARSE_FILES = 0x00000040,
            /// <summary>
            /// The volume keeps track of allocated but unwritten runs of a file.
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_ZERO_RUNS = 0x00000080,
            /// <summary>
            /// The volume treats upper and lower characters in file and directory names as different.
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_CASE_SENSITIVE = 0x00000100,
            /// <summary>
            /// The volume preserves the case of file and directory names.
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_CASE_PRESERVING = 0x00000200,
            /// <summary>
            /// Indicates that statfs(2) on this volume is fast enough as to not need to be cached by callers.
            /// Introduced in Mac OS X 10.3
            /// </summary>
            VOL_CAP_FMT_FAST_STATFS = 0x00000400,
            /// <summary>
            /// The volume supports bigger than 4GiB files.
            /// Introduced in Mac OS X 10.4
            /// </summary>
            VOL_CAP_FMT_2TB_FILESIZE = 0x00000800,
            /// <summary>
            /// The volume supports open deny modes
            /// </summary>
            VOL_CAP_FMT_OPENDENYMODES = 0x00001000,
            /// <summary>
            /// The volume supports <see cref="flags_t.UF_HIDDEN"/> and maps it to the volume's native "hidden" or "invisible" bit.
            /// </summary>
            VOL_CAP_FMT_HIDDEN_FILES = 0x00002000,
            /// <summary>
            /// The volume is able to get an absolute path from a persistent object ID.
            /// </summary>
            VOL_CAP_FMT_PATH_FROM_ID = 0x00004000,
            /// <summary>
            /// The volume does not support determining values for total, available or free blocks.
            /// Introduced in Mac OS X 10.6
            /// </summary>
            VOL_CAP_FMT_NO_VOLUME_SIZES = 0x00008000,
            /// <summary>
            /// The volume uses 64-bit object IDs.
            /// </summary>
            VOL_CAP_FMT_64BIT_OBJECT_IDS = 0x00020000,
            /// <summary>
            /// The volume supports transparent decompression of compressed files using decmpfs.
            /// </summary>
            VOL_CAP_FMT_DECMPFS_COMPRESSION = 0x00010000
        }

        [Flags]
        internal enum volCapabilitiesInterfaces_t : u_int32_t {
            /// <summary>
            /// The volume implementation supports searchfs(2)
            /// </summary>
            VOL_CAP_INT_SEARCHFS = 0x00000001,
            /// <summary>
            /// The volume implementation supports getattrlist(2) and setattrlist(2)
            /// </summary>
            VOL_CAP_INT_ATTRLIST = 0x00000002,
            /// <summary>
            /// The volume implementation supports being exported via NFS
            /// </summary>
            VOL_CAP_INT_NFSEXPORT = 0x00000004,
            /// <summary>
            /// The volume implementation supports getdirentriesattr(2)
            /// </summary>
            VOL_CAP_INT_READDIRATTR = 0x00000008,
            /// <summary>
            /// The volume implementation supports exchangedata(2)
            /// </summary>
            VOL_CAP_INT_EXCHANGEDATA = 0x00000010,
            /// <summary>
            /// The volume implementation supports the private and undocumented copyfile()
            /// </summary>
            VOL_CAP_INT_COPYFILE = 0x00000020,
            /// <summary>
            /// The volume implementation supports F_PREALLOCATE in fcntl(2)
            /// </summary>
            VOL_CAP_INT_ALLOCATE = 0x00000040,
            /// <summary>
            /// The volume implementation allows you to modify the volume name using setattrlist(2)
            /// </summary>
            VOL_CAP_INT_VOL_RENAME = 0x00000080,
            /// <summary>
            /// The volume implementation supports advisory locking
            /// </summary>
            VOL_CAP_INT_ADVLOCK = 0x00000100,
            /// <summary>
            /// The volume implementation supports whole file locks
            /// </summary>
            VOL_CAP_INT_FLOCK = 0x00000200,
            /// <summary>
            /// The volume implementation supports extended security controls (ACLs)
            /// </summary>
            VOL_CAP_INT_EXTENDED_SECURITY = 0x00000400,
            /// <summary>
            /// The volume implementation supports <see cref="cmnAttrGroup_t.ATTR_CMN_USERACCESS"/> 
            /// </summary>
            VOL_CAP_INT_USERACCESS = 0x00000800,
            /// <summary>
            /// The volume implementation supports AFP-stlye mandatory byte range locks via ioctl(2)
            /// </summary>
            VOL_CAP_INT_MANLOCK = 0x00001000,
            /// <summary>
            /// The volume implementation supports native named streams
            /// </summary>
            VOL_CAP_INT_NAMEDSTREAMS = 0x00002000,
            /// <summary>
            /// The volume implementation supports native extended attributes <see cref="setxattr"/> 
            /// </summary>
            VOL_CAP_INT_EXTENDED_ATTR = 0x00004000,
            /// <summary>
            /// The volume implementation supports kqueue notifications for remote events
            /// </summary>
            VOL_CAP_INT_REMOTE_EVENT = 0x00008000
        }

        /// <summary>
        /// Gets the specified lists of attributes for the file system object.
        /// Calls to system's getattrlist(2)
        /// </summary>
        /// <param name="path">Path to the file system object.</param>
        /// <param name="attrList">List of attributes to get.</param>
        /// <param name="attrBuf">Pointer to a buffer to store the attributes on it.</param>
        /// <param name="attrBufSize">Allocated size of <paramref name="attrBuf"/>.</param>
        /// <param name="options"><see cref="getAttrListOptions"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int getattrlist(string path, AttrList attrList, IntPtr attrBuf, size_t attrBufSize, UInt32 options);

        /// <summary>
        /// Sets the specified lists of attributes for the file system object.
        /// Calls to system's setattrlist(2)
        /// </summary>
        /// <param name="path">Path to the file system object.</param>
        /// <param name="attrList">List of attributes to set.</param>
        /// <param name="attrBuf">Pointer to a buffer that store the attributes on it.</param>
        /// <param name="attrBufSize">Allocated size of <paramref name="attrBuf"/>.</param>
        /// <param name="options"><see cref="getAttrListOptions"/>.</param>
        /// <returns>On success, 0. On failure, -1, and errno is set.</returns>
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern int setattrlist(string path, AttrList attrList, IntPtr attrBuf, size_t attrBufSize, UInt32 options);
    }
}

