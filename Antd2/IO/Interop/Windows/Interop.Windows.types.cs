//
// Interop.Windows.types.cs
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

#region Win32 type definitions
using BOOL = System.Boolean;
using BOOLEAN = System.Boolean;
using CCHAR = System.SByte;
using DWORD = System.UInt32;
using FILE_ID_128 = System.Guid;
using HANDLE = Microsoft.Win32.SafeHandles.SafeFileHandle;
using LARGE_INTEGER = System.Int64;
using UCHAR = System.Byte;
using ULONG = System.UInt32;
using ULONGLONG = System.UInt64;
using WCHAR = System.String;
using WORD = System.UInt16;

#endregion

internal static partial class Interop {
    internal static partial class Windows {
        [Flags]
        public enum ACCESS_MASK : DWORD {
            /// <summary>Right to read data from the file. (FILE)</summary>
            FILE_READ_DATA = 0x00000001,
            /// <summary>Right to list contents of a directory. (DIRECTORY)</summary>
            FILE_LIST_DIRECTORY = 0x00000001,

            /// <summary>Right to write data to the file. (FILE)</summary>
            FILE_WRITE_DATA = 0x00000002,
            /// <summary>Right to create a file in the directory. (DIRECTORY)</summary>
            FILE_ADD_FILE = 0x00000002,

            /// <summary>Right to append data to the file. (FILE)</summary>
            FILE_APPEND_DATA = 0x00000004,
            /// <summary>Right to create a subdirectory. (DIRECTORY)</summary>
            FILE_ADD_SUBDIRECTORY = 0x00000004,

            /// <summary>Right to read extended attributes. (FILE/DIRECTORY)</summary>
            FILE_READ_EA = 0x00000008,

            /// <summary>Right to write extended attributes. (FILE/DIRECTORY)</summary>
            FILE_WRITE_EA = 0x00000010,

            /// <summary>Right to execute a file. (FILE)</summary>
            FILE_EXECUTE = 0x00000020,
            /// <summary>Right to traverse the directory. (DIRECTORY)</summary>
            FILE_TRAVERSE = 0x00000020,

            /// <summary>
            /// Right to delete a directory and all the files it contains (its
            /// children, even if the files are read-only. (DIRECTORY)
            /// </summary>
            FILE_DELETE_CHILD = 0x00000040,

            /// <summary>Right to read file attributes. (FILE/DIRECTORY)</summary>
            FILE_READ_ATTRIBUTES = 0x00000080,

            /// <summary>Right to change file attributes. (FILE/DIRECTORY)</summary>
            FILE_WRITE_ATTRIBUTES = 0x00000100,

            /// <summary>
            /// The standard rights (bits 16 to 23). Are independent of the type of
            /// object being secured.
            /// </summary>

            /// <summary>Right to delete the object.</summary>
            DELETE = 0x00010000,

            /// <summary>
            /// Right to read the information in the object's security descriptor,
            /// not including the information in the SACL. I.e. right to read the
            /// security descriptor and owner.
            /// </summary>
            READ_CONTROL = 0x00020000,

            /// <summary>Right to modify the DACL in the object's security descriptor.</summary>
            WRITE_DAC = 0x00040000,

            /// <summary>Right to change the owner in the object's security descriptor.</summary>
            WRITE_OWNER = 0x00080000,

            /// <summary>
            /// Right to use the object for synchronization. Enables a process to
            /// wait until the object is in the signalled state. Some object types
            /// do not support this access right.
            /// </summary>
            SYNCHRONIZE = 0x00100000,

            /// <summary>
            /// The following STANDARD_RIGHTS_* are combinations of the above for
            /// convenience and are defined by the Win32 API.
            /// </summary>

            /// <summary>These are currently defined to READ_CONTROL.</summary>
            STANDARD_RIGHTS_READ = 0x00020000,
            STANDARD_RIGHTS_WRITE = 0x00020000,
            STANDARD_RIGHTS_EXECUTE = 0x00020000,

            /// <summary>Combines DELETE, READ_CONTROL, WRITE_DAC, and WRITE_OWNER access.</summary>
            STANDARD_RIGHTS_REQUIRED = 0x000f0000,

            /// <summary>
            /// Combines DELETE, READ_CONTROL, WRITE_DAC, WRITE_OWNER, and
            /// SYNCHRONIZE access.
            /// </summary>
            STANDARD_RIGHTS_ALL = 0x001f0000,

            /// <summary>
            /// The access system ACL and maximum allowed access types (bits 24 to
            /// 25, bits 26 to 27 are reserved).
            /// </summary>
            ACCESS_SYSTEM_SECURITY = 0x01000000,
            MAXIMUM_ALLOWED = 0x02000000,

            /// <summary>
            /// The generic rights (bits 28 to 31). These map onto the standard and
            /// specific rights.
            /// </summary>

            /// <summary>Read, write, and execute access.</summary>
            GENERIC_ALL = 0x10000000,

            /// <summary>Execute access.</summary>
            GENERIC_EXECUTE = 0x20000000,

            /// <summary>
            /// Write access. For files, this maps onto:
            ///  FILE_APPEND_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_DATA |
            ///  FILE_WRITE_EA | STANDARD_RIGHTS_WRITE | SYNCHRONIZE
            /// For directories, the mapping has the same numerical value. See
            /// above for the descriptions of the rights granted.
            /// </summary>
            GENERIC_WRITE = 0x40000000,

            /// <summary>
            /// Read access. For files, this maps onto:
            ///  FILE_READ_ATTRIBUTES | FILE_READ_DATA | FILE_READ_EA |
            ///  STANDARD_RIGHTS_READ | SYNCHRONIZE
            /// For directories, the mapping has the same numerical value. See
            /// above for the descriptions of the rights granted.
            /// </summary>
            GENERIC_READ = 0x80000000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING : IDisposable {
            public ushort Length;
            public ushort MaximumLength;
            private IntPtr buffer;

            public UNICODE_STRING(string s) {
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length + 2);
                buffer = Marshal.StringToHGlobalUni(s);
            }

            public void Dispose() {
                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;
            }

            public override string ToString() {
                return Marshal.PtrToStringUni(buffer);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_ATTRIBUTES : IDisposable {
            public int Length;
            public IntPtr RootDirectory;
            private IntPtr objectName;
            public uint Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;

            public OBJECT_ATTRIBUTES(string name, uint attrs) {
                Length = 0;
                RootDirectory = IntPtr.Zero;
                objectName = IntPtr.Zero;
                Attributes = attrs;
                SecurityDescriptor = IntPtr.Zero;
                SecurityQualityOfService = IntPtr.Zero;

                Length = Marshal.SizeOf(this);
                ObjectName = new UNICODE_STRING(name);
            }

            public UNICODE_STRING ObjectName {
                get {
                    return (UNICODE_STRING)Marshal.PtrToStructure(
                        objectName, typeof(UNICODE_STRING));
                }

                set {
                    bool fDeleteOld = objectName != IntPtr.Zero;
                    if (!fDeleteOld)
                        objectName = Marshal.AllocHGlobal(Marshal.SizeOf(value));
                    Marshal.StructureToPtr(value, objectName, fDeleteOld);
                }
            }

            public void Dispose() {
                if (objectName != IntPtr.Zero) {
                    Marshal.DestroyStructure(objectName, typeof(UNICODE_STRING));
                    Marshal.FreeHGlobal(objectName);
                    objectName = IntPtr.Zero;
                }
            }
        }

        public enum FILE_ATTRIBUTES : DWORD {
            /// <summary>
            /// A file that is read-only. Applications can read the file,
            /// but cannot write to it or delete it. This attribute is not
            /// honored on directories.
            /// </summary>
            FILE_ATTRIBUTE_READONLY = 0x0000001,
            /// <summary>
            /// The file or directory is hidden. It is not included in an ordinary directory listing.
            /// </summary>
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            /// <summary>
            /// A file or directory that the operating system uses a part of, or uses exclusively.
            /// </summary>
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            /// <summary>
            /// Old DOS attribute that indicates directory entry is the volume name
            /// </summary>
            FILE_ATTRIBUTE_VOLUME_NAME = 0x00000008,
            /// <summary>
            /// The handle that identifies a directory.
            /// </summary>
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            /// <summary>
            /// A file or directory that is an archive file or directory. Applications typically use this attribute to mark files for backup or removal.
            /// </summary>
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            /// <summary>
            /// This value is reserved for system use.
            /// </summary>
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            /// <summary>
            /// A file that does not have other attributes set. This attribute is valid only when used alone.
            /// </summary>
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            /// <summary>
            /// A file that is being used for temporary storage.
            /// File systems avoid writing data back to mass storage if sufficient
            /// cache memory is available, because typically, an application deletes a
            /// temporary file after the handle is closed. In that scenario, the system
            /// can entirely avoid writing the data. Otherwise, the data is written
            /// after the handle is closed.
            /// </summary>
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            /// <summary>
            /// A file that is a sparse file.
            /// </summary>
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            /// <summary>
            /// A file or directory that has an associated reparse point, or a file that is a symbolic link.
            /// </summary>
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
            /// <summary>
            /// A file or directory that is compressed. For a file, all of the data in the file is compressed. For a directory, compression is the default for newly created files and subdirectories.
            /// </summary>
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
            /// <summary>
            /// The data of a file is not available immediately. This attribute indicates that the file data is physically moved to offline storage.
            /// </summary>
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,
            /// <summary>
            /// The file or directory is not to be indexed by the content indexing service.
            /// </summary>
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
            /// <summary>
            /// A file or directory that is encrypted. For a file, all data streams in the file are encrypted. For a directory, encryption is the default for newly created files and subdirectories.
            /// </summary>
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000,
            /// <summary>
            /// The directory or user data stream is configured with integrity (only supported on ReFS volumes).
            /// This flag is not supported until Windows Server 2012.
            /// </summary>
            FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x00008000,
            /// <summary>
            /// This value is reserved for system use.
            /// </summary>
            FILE_ATTRIBUTE_VIRTUAL = 0x00010000,
            /// <summary>
            /// The user data stream not to be read by the background data integrity scanner (AKA scrubber). When set on a directory it only provides inheritance. This flag is only supported on Storage Spaces and ReFS volumes. It is not included in an ordinary directory listing.
            /// This flag is not supported until Windows 8 and Windows Server 2012.
            /// </summary>
            FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x00020000
        }

        /// <summary>
        /// Contains the basic information for a file.
        /// </summary>
        public struct FILE_BASIC_INFO {
            /// <summary>
            /// he time the file was created in FILETIME format, which is a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC).
            /// </summary>
            public LARGE_INTEGER CreationTime;
            /// <summary>
            /// The time the file was last accessed in FILETIME format.
            /// </summary>
            public LARGE_INTEGER LastAccessTime;
            /// <summary>
            /// The time the file was last written to in FILETIME format.
            /// </summary>
            public LARGE_INTEGER LastWriteTime;
            /// <summary>
            /// The time the file was changed in FILETIME format.
            /// </summary>
            public LARGE_INTEGER ChangeTime;
            /// <summary>
            /// The file attributes. <see cref="FILE_ATTRIBUTES"/> 
            /// </summary>
            public FILE_ATTRIBUTES FileAttributes;
        }

        /// <summary>
        /// Receives extended information for the file.
        /// </summary>
        public struct FILE_STANDARD_INFO {
            /// <summary>
            /// The amount of space that is allocated for the file.
            /// </summary>
            public LARGE_INTEGER AllocationSize;
            /// <summary>
            /// The end of the file.
            /// </summary>
            public LARGE_INTEGER EndOfFile;
            /// <summary>
            /// The number of links to the file.
            /// </summary>
            public DWORD NumberOfLinks;
            /// <summary>
            /// <c>true</c> if the file in the delete queue; otherwise, <c>false</c>.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public BOOLEAN DeletePending;
            /// <summary>
            /// <c>true</c> if the file is a directory; otherwise, <c>false</c>.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public BOOLEAN Directory;
        }

        /// <summary>
        /// Receives the file name.
        /// </summary>
        public struct FILE_NAME_INFO {
            /// <summary>
            /// The size of the <see cref="FILE_NAME_INFO.FileName"/> string, in bytes.
            /// </summary>
            public DWORD FileNameLength;
            /// <summary>
            /// The file name that is returned.
            /// </summary>
            public WCHAR FileName;
        }

        /// <summary>
        /// Contains the name to which the file should be renamed.
        /// </summary>
        public struct FILE_RENAME_INFO {
            /// <summary>
            /// TRUE to replace the file; otherwise, FALSE.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public BOOL ReplaceIfExists;
            /// <summary>
            /// A handle to the root directory in which the file to be renamed is located.
            /// </summary>
            public HANDLE RootDirectory;
            /// <summary>
            /// The size of <see cref="FILE_RENAME_INFO.FileName"/> in bytes.
            /// </summary>
            public DWORD FileNameLength;
            /// <summary>
            /// The new file name.
            /// </summary>
            public WCHAR FileName;
        }

        /// <summary>
        /// Indicates whether a file should be deleted.
        /// </summary>
        public struct FILE_DISPOSITION_INFO {
            /// <summary>
            /// Indicates whether the file should be deleted. Set to TRUE to delete the file.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public BOOL DeleteFile;
        }

        /// <summary>
        /// Contains the total number of bytes that should be allocated for a file.
        /// </summary>
        public struct FILE_ALLOCATION_INFO {
            /// <summary>
            /// The new file allocation size, in bytes. This value is typically a multiple of the sector or cluster size for the underlying physical device.
            /// </summary>
            public LARGE_INTEGER AllocationSize;
        }

        /// <summary>
        /// Contains the specified value to which the end of the file should be set.
        /// </summary>
        public struct FILE_END_OF_FILE_INFO {
            /// <summary>
            /// The specified value for the new end of the file.
            /// </summary>
            public LARGE_INTEGER EndOfFile;
        }

        /// <summary>
        /// Receives file stream information for the specified file.
        /// The FILE_STREAM_INFO structure must be aligned on a LONGLONG (8-byte) boundary. If a buffer contains two or more of these structures, the NextEntryOffset value in each entry, except the last, falls on an 8-byte boundary.
        /// </summary>
        public struct FILE_STREAM_INFO {
            /// <summary>
            /// The offset for the next <see cref="FILE_STREAM_INFO"/> entry that is returned. This member is zero if no other entries follow this one.
            /// </summary>
            public DWORD NextEntryOffset;
            /// <summary>
            /// The length, in bytes, of <see cref="FILE_STREAM_INFO.StreamName"/>.
            /// </summary>
            public DWORD StreamNameLength;
            /// <summary>
            /// The size, in bytes, of the data stream.
            /// </summary>
            public LARGE_INTEGER StreamSize;
            /// <summary>
            /// The amount of space that is allocated for the stream, in bytes. This value is usually a multiple of the sector or cluster size of the underlying physical device.
            /// </summary>
            public LARGE_INTEGER StreamAllocationSize;
            /// <summary>
            /// The stream name.
            /// </summary>
            public WCHAR StreamName;
        }

        /// <summary>
        /// Receives file compression information.
        /// </summary>
        public struct FILE_COMPRESSION_INFO {
            /// <summary>
            /// The file size of the compressed file.
            /// </summary>
            public LARGE_INTEGER CompressedFileSize;
            /// <summary>
            /// The compression format that is used to compress the file.
            /// </summary>
            public WORD CompressionFormat;
            /// <summary>
            /// The factor that the compression uses.
            /// </summary>
            public UCHAR CompressionUnitShift;
            /// <summary>
            /// The number of chunks that are shifted by compression.
            /// </summary>
            public UCHAR ChunkShift;
            /// <summary>
            /// The number of clusters that are shifted by compression.
            /// </summary>
            public UCHAR ClusterShift;
            /// <summary>
            /// Reserved.
            /// 3 bytes
            /// </summary>
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]
            public UCHAR[] Reserved;
        }

        /// <summary>
        /// Contains information about files in the specified directory.
        /// </summary>
        public struct FILE_ID_BOTH_DIR_INFO {
            /// <summary>
            /// The offset for the next <see cref="FILE_ID_BOTH_DIR_INFO"/> structure that is returned. Contains zero (0) if no other entries follow this one.
            /// </summary>
            public DWORD NextEntryOffset;
            /// <summary>
            /// The byte offset of the file within the parent directory. This member is undefined for file systems, such as NTFS, in which the position of a file within the parent directory is not fixed and can be changed at any time to maintain sort order.
            /// </summary>
            public DWORD FileIndex;
            /// <summary>
            /// The time that the file was created.
            /// </summary>
            public LARGE_INTEGER CreationTime;
            /// <summary>
            /// The time that the file was last accessed.
            /// </summary>
            public LARGE_INTEGER LastAccessTime;
            /// <summary>
            /// The time that the file was last written to.
            /// </summary>
            public LARGE_INTEGER LastWriteTime;
            /// <summary>
            /// The time that the file was last changed.
            /// </summary>
            public LARGE_INTEGER ChangeTime;
            /// <summary>
            /// The absolute new end-of-file position as a byte offset from the start of the file to the end of the file. Because this value is zero-based, it actually refers to the first free byte in the file. In other words, EndOfFile is the offset to the byte that immediately follows the last valid byte in the file.
            /// </summary>
            public LARGE_INTEGER EndOfFile;
            /// <summary>
            /// The number of bytes that are allocated for the file. This value is usually a multiple of the sector or cluster size of the underlying physical device.
            /// </summary>
            public LARGE_INTEGER AllocationSize;
            /// <summary>
            /// The file attributes. <see cref="FILE_ATTRIBUTES"/>.
            /// </summary>
            public FILE_ATTRIBUTES FileAttributes;
            /// <summary>
            /// The length of the file name.
            /// </summary>
            public DWORD FileNameLength;
            /// <summary>
            /// The size of the extended attributes for the file.
            /// </summary>
            public DWORD EaSize;
            /// <summary>
            /// The length of ShortName.
            /// </summary>
            public CCHAR ShortNameLength;
            /// <summary>
            /// The short 8.3 file naming convention (for example, "FILENAME.TXT") name of the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public WCHAR ShortName;
            /// <summary>
            /// The file ID.
            /// </summary>
            public LARGE_INTEGER FileId;
            /// <summary>
            /// The first character of the file name string. This is followed in memory by the remainder of the string.
            /// </summary>
            public WCHAR FileName;
        }

        /// <summary>
        /// Receives the requested file attribute information.
        /// </summary>
        public struct FILE_ATTRIBUTE_TAG_INFO {
            /// <summary>
            /// The file attribute information.
            /// </summary>
            public FILE_ATTRIBUTES FileAttributes;
            /// <summary>
            /// The reparse tag.
            /// </summary>
            public DWORD ReparseTag;
        }

        public enum PRIORITY_HINT {
            /// <summary>
            /// The lowest possible priority hint level. The system uses this value for background I/O operations.
            /// </summary>
            IoPriorityHintVeryLow = 0,
            /// <summary>
            /// A low-priority hint level.
            /// </summary>
            IoPriorityHintLow,
            /// <summary>
            /// A normal-priority hint level. This value is the default setting for an I/O operation.
            /// </summary>
            IoPriorityHintNormal,
            /// <summary>
            /// This value is used for validation. Supported values are less than this value.
            /// </summary>
            MaximumIoPriorityHintType
        }

        /// <summary>
        /// Specifies the priority hint for a file I/O operation.
        /// </summary>
        public struct FILE_IO_PRIORITY_HINT_INFO {
            /// <summary>
            /// The priority hint. This member is a value from the <see cref="PRIORITY_HINT"/> enumeration.
            /// </summary>
            public PRIORITY_HINT PriorityHint;
        }

        /// <summary>
        /// Contains directory information for a file.
        /// </summary>
        public struct FILE_FULL_DIR_INFO {
            /// <summary>
            /// The offset for the next <see cref="FILE_FULL_DIR_INFO"/> structure that is returned. Contains zero (0) if no other entries follow this one.
            /// </summary>
            public ULONG NextEntryOffset;
            /// <summary>
            /// The byte offset of the file within the parent directory. This member is undefined for file systems, such as NTFS, in which the position of a file within the parent directory is not fixed and can be changed at any time to maintain sort order.
            /// </summary>
            public ULONG FileIndex;
            /// <summary>
            /// The time that the file was created.
            /// </summary>
            public LARGE_INTEGER CreationTime;
            /// <summary>
            /// The time that the file was last accessed.
            /// </summary>
            public LARGE_INTEGER LastAccessTime;
            /// <summary>
            /// The time that the file was last written to.
            /// </summary>
            public LARGE_INTEGER LastWriteTime;
            /// <summary>
            /// The time that the file was last changed.
            /// </summary>
            public LARGE_INTEGER ChangeTime;
            /// <summary>
            /// The absolute new end-of-file position as a byte offset from the start of the file to the end of the default data stream of the file. Because this value is zero-based, it actually refers to the first free byte in the file. In other words, EndOfFile is the offset to the byte that immediately follows the last valid byte in the file.
            /// </summary>
            public LARGE_INTEGER EndOfFile;
            /// <summary>
            /// The number of bytes that are allocated for the file. This value is usually a multiple of the sector or cluster size of the underlying physical device.
            /// </summary>
            public LARGE_INTEGER AllocationSize;
            /// <summary>
            /// The file attributes. <see cref="FILE_ATTRIBUTES"/>
            /// </summary>
            public FILE_ATTRIBUTES FileAttributes;
            /// <summary>
            /// The length of the file name.
            /// </summary>
            public ULONG FileNameLength;
            /// <summary>
            /// The size of the extended attributes for the file.
            /// </summary>
            public ULONG EaSize;
            /// <summary>
            /// The first character of the file name string. This is followed in memory by the remainder of the string.
            /// </summary>
            public WCHAR FileName;
        }

        /// <summary>
        /// Flags specifying information about the alignment of the storage.
        /// </summary>
        [Flags]
        public enum FILE_STORAGE_INFO_FLAGS : ULONG {
            /// <summary>
            /// When set, this flag indicates that the logical sectors of the storage device are aligned to physical sector boundaries.
            /// </summary>
            STORAGE_INFO_FLAGS_ALIGNED_DEVICE = 0x00000001,
            /// <summary>
            /// When set, this flag indicates that the partition is aligned to physical sector boundaries on the storage device.
            /// </summary>
            STORAGE_INFO_FLAGS_PARTITION_ALIGNED_ON_DEVICE = 0x00000002
        }

        const ULONG STORAGE_INFO_OFFSET_UNKNOWN = 0xFFFFFFFF;

        /// <summary>
        /// Contains directory information for a file.
        /// </summary>
        public struct FILE_STORAGE_INFO {
            /// <summary>
            /// Logical bytes per sector reported by physical storage. This is the smallest size for which uncached I/O is supported.
            /// </summary>
            public ULONG LogicalBytesPerSector;
            /// <summary>
            /// Bytes per sector for atomic writes. Writes smaller than this may require a read before the entire block can be written atomically.
            /// </summary>
            public ULONG PhysicalBytesPerSectorForAtomicity;
            /// <summary>
            /// Bytes per sector for optimal performance for writes.
            /// </summary>
            public ULONG PhysicalBytesPerSectorForPerformance;
            /// <summary>
            /// This is the size of the block used for atomicity by the file system. This may be a trade-off between the optimal size of the physical media and one that is easier to adapt existing code and structures.
            /// </summary>
            public ULONG FileSystemEffectivePhysicalBytesPerSectorForAtomicity;
            /// <summary>
            /// This member can contain combinations of flags specifying information about the alignment of the storage. <see cref="FILE_STORAGE_INFO_FLAGS"/> 
            /// </summary>
            public FILE_STORAGE_INFO_FLAGS Flags;
            /// <summary>
            /// Logical sector offset within the first physical sector where the first logical sector is placed, in bytes. If this value is set to <see cref="STORAGE_INFO_OFFSET_UNKNOWN"/>, there was insufficient information to compute this field.
            /// </summary>
            public ULONG ByteOffsetForSectorAlignment;
            /// <summary>
            /// Offset used to align the partition to a physical sector boundary on the storage device, in bytes. If this value is set to <see cref="STORAGE_INFO_OFFSET_UNKNOWN"/>, there was insufficient information to compute this field.
            /// </summary>
            public ULONG ByteOffsetForPartitionAlignment;
        }

        /// <summary>
        /// Contains alignment information for a file.
        /// </summary>
        public struct FILE_ALIGNMENT_INFO {
            /// <summary>
            /// Minimum alignment requirement, in bytes.
            /// </summary>
            public ULONG AlignmentRequirement;
        }

        /// <summary>
        /// Contains identification information for a file.
        /// </summary>
        public struct FILE_ID_INFO {
            /// <summary>
            /// The serial number of the volume that contains a file.
            /// </summary>
            public ULONGLONG VolumeSerialNumber;
            /// <summary>
            /// The 128-bit file identifier for the file. The file identifier and the volume serial number uniquely identify a file on a single computer. To determine whether two open handles represent the same file, combine the identifier and the volume serial number for each file and compare them.
            /// </summary>
            public FILE_ID_128 FileId;
        }

        public enum REPARSE_POINT_TAGS : ULONG {
            IO_REPARSE_TAG_CSV = 0x80000009,
            IO_REPARSE_TAG_DEDUP = 0x80000013,
            IO_REPARSE_TAG_DFS = 0x8000000A,
            IO_REPARSE_TAG_DFSR = 0x80000012,
            IO_REPARSE_TAG_HSM = 0xC0000004,
            IO_REPARSE_TAG_HSM2 = 0x80000006,
            IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003,
            IO_REPARSE_TAG_NFS = 0x80000014,
            IO_REPARSE_TAG_SIS = 0x80000007,
            IO_REPARSE_TAG_SYMLINK = 0xA000000C,
            IO_REPARSE_TAG_WIM = 0x80000008
        }

        /// <summary>
        /// Contains identification information for a file.
        /// </summary>
        public struct FILE_ID_EXTD_DIR_INFO {
            /// <summary>
            /// The offset for the next <see cref="FILE_ID_EXTD_DIR_INFO"/> structure that is returned. Contains zero (0) if no other entries follow this one.
            /// </summary>
            public ULONG NextEntryOffset;
            /// <summary>
            /// The byte offset of the file within the parent directory. This member is undefined for file systems, such as NTFS, in which the position of a file within the parent directory is not fixed and can be changed at any time to maintain sort order.
            /// </summary>
            public ULONG FileIndex;
            /// <summary>
            /// The time that the file was created.
            /// </summary>
            public LARGE_INTEGER CreationTime;
            /// <summary>
            /// The time that the file was last accessed.
            /// </summary>
            public LARGE_INTEGER LastAccessTime;
            /// <summary>
            /// The time that the file was last written to.
            /// </summary>
            public LARGE_INTEGER LastWriteTime;
            /// <summary>
            /// The time that the file was last changed.
            /// </summary>
            public LARGE_INTEGER ChangeTime;
            /// <summary>
            /// The absolute new end-of-file position as a byte offset from the start of the file to the end of the file. Because this value is zero-based, it actually refers to the first free byte in the file. In other words, EndOfFile is the offset to the byte that immediately follows the last valid byte in the file.
            /// </summary>
            public LARGE_INTEGER EndOfFile;
            /// <summary>
            /// The number of bytes that are allocated for the file. This value is usually a multiple of the sector or cluster size of the underlying physical device.
            /// </summary>
            public LARGE_INTEGER AllocationSize;
            /// <summary>
            /// The file attributes. <see cref="FILE_ATTRIBUTES"/> 
            /// </summary>
            public FILE_ATTRIBUTES FileAttributes;
            /// <summary>
            /// The length of the file name.
            /// </summary>
            public ULONG FileNameLength;
            /// <summary>
            /// The size of the extended attributes for the file.
            /// </summary>
            public ULONG EaSize;
            /// <summary>
            /// If the FileAttributes member includes the FILE_ATTRIBUTE_REPARSE_POINT attribute, this member specifies the reparse point tag.
            /// Otherwise, this value is undefined and should not be used.
            /// For more information <see cref="REPARSE_POINT_TAGS"/> 
            /// </summary>
            public REPARSE_POINT_TAGS ReparsePointTag;
            /// <summary>
            /// The file ID.
            /// </summary>
            public FILE_ID_128 FileId;
            /// <summary>
            /// The first character of the file name string. This is followed in memory by the remainder of the string.
            /// </summary>
            public WCHAR FileName;
        }

        /// <summary>
        /// Contains a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601 (UTC).
        /// </summary>
        public struct FILETIME {
            /// <summary>
            /// The low-order part of the file time.
            /// </summary>
            public DWORD dwLowDateTime;
            /// <summary>
            /// The high-order part of the file time.
            /// </summary>
            public DWORD dwHighDateTime;
        }

        /// <summary>
        /// Contains information that the <see cref="GetFileInformationByHandle"/> function retrieves.
        /// </summary>
        public struct BY_HANDLE_FILE_INFORMATION {
            /// <summary>
            /// The file attributes. <see cref="FILE_ATTRIBUTES"/> 
            /// </summary>
            public FILE_ATTRIBUTES dwFileAttributes;
            /// <summary>
            /// A FILETIME structure that specifies when a file or directory is created. If the underlying file system does not support creation time, this member is zero (0).
            /// </summary>
            public FILETIME ftCreationTime;
            /// <summary>
            /// A FILETIME structure. For a file, the structure specifies the last time that a file is read from or written to. For a directory, the structure specifies when the directory is created. For both files and directories, the specified date is correct, but the time of day is always set to midnight. If the underlying file system does not support the last access time, this member is zero (0).
            /// </summary>
            public FILETIME ftLastAccessTime;
            /// <summary>
            /// A FILETIME structure. For a file, the structure specifies the last time that a file is written to. For a directory, the structure specifies when the directory is created. If the underlying file system does not support the last write time, this member is zero (0).
            /// </summary>
            public FILETIME ftLastWriteTime;
            /// <summary>
            /// The serial number of the volume that contains a file.
            /// </summary>
            public DWORD dwVolumeSerialNumber;
            /// <summary>
            /// The high-order part of the file size.
            /// </summary>
            public DWORD nFileSizeHigh;
            /// <summary>
            /// The low-order part of the file size.
            /// </summary>
            public DWORD nFileSizeLow;
            /// <summary>
            /// The number of links to this file. For the FAT file system this member is always 1. For the NTFS file system, it can be more than 1.
            /// </summary>
            public DWORD nNumberOfLinks;
            /// <summary>
            /// The high-order part of a unique identifier that is associated with a file. For more information, see <see cref="nFileIndexLow"/>.
            /// </summary>
            public DWORD nFileIndexHigh;
            /// <summary>
            /// The low-order part of a unique identifier that is associated with a file.
            /// The identifier (low and high parts) and the volume serial number uniquely
            /// identify a file on a single computer. To determine whether two open handles
            /// represent the same file, combine the identifier and the volume serial number
            /// for each file and compare them.
            /// The ReFS file system, introduced with Windows Server 2012, includes 128-bit
            /// file identifiers. To retrieve the 128-bit file identifier use the
            /// <see cref="GetFileInformationByHandleEx"/> function with
            /// <see cref="FILE_INFO_BY_HANDLE_CLASS.FileIdInfo"/> to retrieve the
            /// <see cref="FILE_ID_INFO"/> structure. The 64-bit identifier in this structure
            /// is not guaranteed to be unique on ReFS.
            /// </summary>
            public DWORD nFileIndexLow;
        }
    }
}