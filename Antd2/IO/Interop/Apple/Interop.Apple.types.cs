//
// Interop.Apple.types.cs
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
    internal static partial class Apple {
        /// <summary>
        /// File mode and permissions
        /// </summary>
        [Flags]
        internal enum mode_t : ushort {
            // File type

            /// <summary>
            /// File type mask
            /// </summary>
            S_IFMT = 0xF000,
            /// <summary>
            /// Named pipe (FIFO)
            /// </summary>
            S_IFIFO = 0x1000,
            /// <summary>
            /// Character device
            /// </summary>
            S_IFCHR = 0x2000,
            /// <summary>
            /// Directory
            /// </summary>
            S_IFDIR = 0x4000,
            /// <summary>
            /// Block device
            /// </summary>
            S_IFBLK = 0x6000,
            /// <summary>
            /// Regular file
            /// </summary>
            S_IFREG = 0x8000,
            /// <summary>
            /// Symbolic link
            /// </summary>
            S_IFLNK = 0xA000,
            /// <summary>
            /// Socket file
            /// </summary>
            S_IFSOCK = 0xC000,
            /// <summary>
            /// OBSOLETE: whiteout
            /// </summary>
            [Obsolete]
            S_IFWHT = 0xE000,

            // POSIX Permissions

            /// <summary>
            /// Owner permissions mask
            /// </summary>
            S_IRWXU = 0x01C0,
            /// <summary>
            /// Readable by owner
            /// </summary>
            S_IRUSR = 0x0100,
            /// <summary>
            /// Writable by owner
            /// </summary>
            S_IWUSR = 0x0080,
            /// <summary>
            /// Executable by owner
            /// </summary>
            S_IXUSR = 0x0040,

            /// <summary>
            /// Group permissions mask
            /// </summary>
            S_IRWXG = 0x0038,
            /// <summary>
            /// Readable by group
            /// </summary>
            S_IRGRP = 0x0020,
            /// <summary>
            /// Writable by group
            /// </summary>
            S_IWGRP = 0x0010,
            /// <summary>
            /// Executable by group
            /// </summary>
            S_IXGRP = 0x0008,

            /// <summary>
            /// Others permissions mask
            /// </summary>
            S_IRWXO = 0x0007,
            /// <summary>
            /// Readable by others
            /// </summary>
            S_IROTH = 0x0004,
            /// <summary>
            /// Writable by others
            /// </summary>
            S_IWOTH = 0x0002,
            /// <summary>
            /// Executable by others
            /// </summary>
            S_IXOTH = 0x0001,

            /// <summary>
            /// Set UID on execution
            /// </summary>
            S_ISUID = 0x0800,
            /// <summary>
            /// Set GID on execution
            /// </summary>
            S_ISGID = 0x0400,
            /// <summary>
            /// Only file/directory owners (or suser) can removed files from directory
            /// </summary>
            S_ISVTX = 0x0200,

            /// <summary>
            /// Sticky bit, not supported by OS X
            /// </summary>
            [Obsolete("Not supported under OS X")]
            S_ISTXT = S_ISVTX,
            /// <summary>
            /// For backwards compatibility
            /// </summary>
            S_IREAD = S_IRUSR,
            /// <summary>
            /// For backwards compatibility
            /// </summary>
            S_IWRITE = S_IWUSR,
            /// <summary>
            /// For backwards compatibility
            /// </summary>
            S_IEXEC = S_IXUSR
        }

        /// <summary>
        /// User-set flags
        /// </summary>
        [Flags]
        internal enum flags_t : uint {
            /// <summary>
            /// Mask of flags changeable by owner
            /// </summary>
            UF_SETTABLE = 0x0000FFFF,
            /// <summary>
            /// Do not dump file
            /// </summary>
            UF_NODUMP = 0x00000001,
            /// <summary>
            /// File is immutable (read-only)
            /// </summary>
            UF_IMMUTABLE = 0x00000002,
            /// <summary>
            /// File can only be appended
            /// </summary>
            UF_APPEND = 0x00000004,
            /// <summary>
            /// The directory is opaque when viewed through a union stack.
            /// </summary>
            UF_OPAQUE = 0x00000008,
            /// <summary>
            /// INCOMPATIBLE: Used in FreeBSD, unimplemented in OS X.
            /// File cannot be removed or renamed.
            /// </summary>
            [Obsolete("Unimplemented in OS X")]
            UF_NOUNLINK = 0x00000010,
            /// <summary>
            /// File is compressed in HFS+ (>=10.6)
            /// </summary>
            UF_COMPRESSED = 0x00000020,
            /// <summary>
            /// OBSOLETE: No longer used.
            /// Issue notifications for deletes or renames of files with this flag set
            /// </summary>
            [Obsolete("No longer used")]
            UF_TRACKED = 0x00000040,
            /// <summary>
            /// Hide the file in Finder
            /// </summary>
            UF_HIDDEN = 0x00008000,

            /// <summary>
            /// Mask of flags changeable by the superuser
            /// </summary>
            SF_SETTABLE = 0xffff0000,
            /// <summary>
            /// File has been archived
            /// </summary>
            SF_ARCHIVED = 0x00010000,
            /// <summary>
            /// File is immutable (read-only)
            /// </summary>
            SF_IMMUTABLE = 0x00020000,
            /// <summary>
            /// File can only be appended
            /// </summary>
            SF_APPEND = 0x00040000,
            /// <summary>
            /// Restricted access
            /// </summary>
            SF_RESTRICTED = 0x00080000,
            /// <summary>
            /// INCOMPATIBLE: Used in FreeBSD, unimplemented in OS X.
            /// File cannot be removed or renamed.
            /// </summary>
            [Obsolete("Unimplemented in OS X")]
            SF_NOUNLINK = 0x00100000,
            /// <summary>
            /// INCOMPATIBLE: Used in FreeBSD, unimplemented in OS X.
            /// Snapshot inode
            /// </summary>
            [Obsolete("Unimplemented in OS X")]
            SF_SNAPSHOT = 0x00200000
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct Timespec {
            // TODO: Mono is 32-bit only on Mac OS X, but when it becomes 64-bit this will become int64
            /// <summary>
            /// Seconds
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int tv_sec;
            /// <summary>
            /// Nanoseconds
            /// </summary>
            [MarshalAs(UnmanagedType.I4)]
            public int tv_nsec;
        }

        enum vtype {
            VNON = 0,
            VREG = 1,
            VDIR = 2,
            VBLK = 3,
            VCHR = 4,
            VLNK = 5,
            VSOCK = 6,
            VFIFO = 7,
            VBAD = 8,
            VSTR = 9,
            VCPLX = 10
        }

        enum vtagtype {
            VT_NON = 0,
            VT_UFS = 1,
            VT_NFS = 2,
            VT_MFS = 3,
            VT_MSDOSFS = 4,
            VT_LFS = 5,
            VT_LOFS = 6,
            VT_FDESC = 7,
            VT_PORTAL = 8,
            VT_NULL = 9,
            VT_UMAP = 10,
            VT_KERNFS = 11,
            VT_PROCFS = 12,
            VT_AFS = 13,
            VT_ISOFS = 14,
            VT_MOCKFS = 15,
            VT_HFS = 16,
            VT_ZFS = 17,
            VT_DEVFS = 18,
            VT_WEBDAV = 19,
            VT_UDF = 20,
            VT_AFP = 21,
            VT_CDDA = 22,
            VT_CIFS = 23,
            VT_OTHER = 24
        }

        enum text_encoding_t : uint {
            kTextEncodingMacRoman = 0,
            kTextEncodingMacJapanese = 1,
            kTextEncodingMacChineseTrad = 2,
            kTextEncodingMacKorean = 3,
            kTextEncodingMacArabic = 4,
            kTextEncodingMacHebrew = 5,
            kTextEncodingMacGreek = 6,
            kTextEncodingMacCyrillic = 7,
            kTextEncodingMacDevanagari = 9,
            kTextEncodingMacGurmukhi = 10,
            kTextEncodingMacGujarati = 11,
            kTextEncodingMacOriya = 12,
            kTextEncodingMacBengali = 13,
            kTextEncodingMacTamil = 14,
            kTextEncodingMacTelugu = 15,
            kTextEncodingMacKannada = 16,
            kTextEncodingMacMalayalam = 17,
            kTextEncodingMacSinhalese = 18,
            kTextEncodingMacBurmese = 19,
            kTextEncodingMacKhmer = 20,
            kTextEncodingMacThai = 21,
            kTextEncodingMacLaotian = 22,
            kTextEncodingMacGeorgian = 23,
            kTextEncodingMacArmenian = 24,
            kTextEncodingMacChineseSimp = 25,
            kTextEncodingMacTibetan = 26,
            kTextEncodingMacMongolian = 27,
            kTextEncodingMacEthiopic = 28,
            kTextEncodingMacCentralEurRoman = 29,
            kTextEncodingMacVietnamese = 30,
            kTextEncodingMacExtArabic = 31,
            kTextEncodingMacSymbol = 33,
            kTextEncodingMacDingbats = 34,
            kTextEncodingMacTurkish = 35,
            kTextEncodingMacCroatian = 36,
            kTextEncodingMacIcelandic = 37,
            kTextEncodingMacRomanian = 38,
            kTextEncodingMacCeltic = 39,
            kTextEncodingMacGaelic = 40,
            kTextEncodingMacKeyboardGlyphs = 41,

            // Older names
            kTextEncodingMacTradChinese = kTextEncodingMacChineseTrad,
            kTextEncodingMacRSymbol = 8,
            kTextEncodingMacSimpChinese = kTextEncodingMacChineseSimp,
            kTextEncodingMacGeez = kTextEncodingMacEthiopic,
            kTextEncodingMacEastEurRoman = kTextEncodingMacCentralEurRoman,
            kTextEncodingMacUninterp = 32,


            /// <summary>
            /// Meta-value, Unicode as a Mac encoding
            /// </summary>
            kTextEncodingMacUnicode = 0x7E,

            // Variant Mac OS encodings that use script codes other than 0
            // The following use script code 4, smArabic
            /// <summary>
            /// Like MacArabic but uses Farsi digits
            /// </summary>
            kTextEncodingMacFarsi = 0x8C,
            // The following use script code 7, smCyrillic
            /// <summary>
            /// Meta-value in TEC 1.5 & later; maps to kTextEncodingMacCyrillic variant    
            /// </summary>
            kTextEncodingMacUkrainian = 0x98,
            // The following use script code 28, smEthiopic
            /// <summary>
            /// The following use script code 32, smUnimplemented
            /// </summary>
            kTextEncodingMacInuit = 0xEC,
            /// <summary>
            /// VT100/102 font from Comm Toolbox: Latin-1 repertoire + box drawing etc
            /// </summary>
            kTextEncodingMacVT100 = 0xFC,

            /// <summary>
            /// Meta-value, should never appear in a table.
            /// </summary>
            kTextEncodingMacHFS = 0xFF,

            /// <summary>
            /// Meta-value, should never appear in a table.
            /// </summary>
            kTextEncodingUnicodeDefault = 0x0100,
            kTextEncodingUnicodeV1_1 = 0x0101,
            /// <summary>
            /// Code points identical to Unicode 1.1
            /// </summary>
            kTextEncodingISO10646_1993 = 0x0101,
            /// <summary>
            /// New location for Korean Hangul
            /// </summary>
            kTextEncodingUnicodeV2_0 = 0x0103,
            /// <summary>
            /// We treat both Unicode 2.0 and Unicode 2.1 as 2.1
            /// </summary>
            kTextEncodingUnicodeV2_1 = 0x0103,
            kTextEncodingUnicodeV3_0 = 0x0104,
            /// <summary>
            /// Adds characters requiring surrogate pairs in UTF-16
            /// </summary>
            kTextEncodingUnicodeV3_1 = 0x0105,
            kTextEncodingUnicodeV3_2 = 0x0106,
            kTextEncodingUnicodeV4_0 = 0x0108,
            kTextEncodingUnicodeV5_0 = 0x010A,

            // ISO 8-bit and 7-bit encodings begin at 0x200
            /// <summary>
            /// ISO 8859-1, Western European
            /// </summary>
            kTextEncodingISOLatin1 = 0x0201,
            /// <summary>
            /// ISO 8859-2, Central European
            /// </summary>
            kTextEncodingISOLatin2 = 0x0202,
            /// <summary>
            /// ISO 8859-3, South European (Maltese...)
            /// </summary>
            kTextEncodingISOLatin3 = 0x0203,
            /// <summary>
            /// ISO 8859-4, North European & some Baltic
            /// </summary>
            kTextEncodingISOLatin4 = 0x0204,
            /// <summary>
            /// ISO 8859-5
            /// </summary>
            kTextEncodingISOLatinCyrillic = 0x0205,
            /// <summary>
            /// ISO 8859-6, = ASMO 708, =DOS CP 708
            /// </summary>
            kTextEncodingISOLatinArabic = 0x0206,
            /// <summary>
            /// ISO 8859-7
            /// </summary>
            kTextEncodingISOLatinGreek = 0x0207,
            /// <summary>
            /// ISO 8859-8
            /// </summary>
            kTextEncodingISOLatinHebrew = 0x0208,
            /// <summary>
            /// ISO 8859-9, Turkish
            /// </summary>
            kTextEncodingISOLatin5 = 0x0209,
            /// <summary>
            /// ISO 8859-10, Nordic                    
            /// </summary>
            kTextEncodingISOLatin6 = 0x020A,
            /// <summary>
            /// ISO 8859-13, Baltic Rim                   
            /// </summary>
            kTextEncodingISOLatin7 = 0x020D,
            /// <summary>
            /// ISO 8859-14, Celtic                    
            /// </summary>
            kTextEncodingISOLatin8 = 0x020E,
            /// <summary>
            /// ISO 8859-15, 8859-1 changed for EURO &amp; CP1252 letters  
            /// </summary>
            kTextEncodingISOLatin9 = 0x020F,
            /// <summary>
            /// ISO 8859-16, Romanian
            /// </summary>
            kTextEncodingISOLatin10 = 0x0210,

            // MS-DOS & Windows encodings begin at 0x400
            /// <summary>
            /// code page 437
            /// </summary>
            kTextEncodingDOSLatinUS = 0x0400,
            /// <summary>
            /// code page 737 (formerly code page 437G)
            /// </summary>
            kTextEncodingDOSGreek = 0x0405,
            /// <summary>
            /// code page 775
            /// </summary>
            kTextEncodingDOSBalticRim = 0x0406,
            /// <summary>
            /// code page 850, "Multilingual"
            /// </summary>
            kTextEncodingDOSLatin1 = 0x0410,
            /// <summary>
            /// code page 851
            /// </summary>
            kTextEncodingDOSGreek1 = 0x0411,
            /// <summary>
            /// code page 852, Slavic
            /// </summary>
            kTextEncodingDOSLatin2 = 0x0412,
            /// <summary>
            /// code page 855, IBM Cyrillic
            /// </summary>
            kTextEncodingDOSCyrillic = 0x0413,
            /// <summary>
            /// code page 857, IBM Turkish
            /// </summary>
            kTextEncodingDOSTurkish = 0x0414,
            /// <summary>
            /// code page 860
            /// </summary>
            kTextEncodingDOSPortuguese = 0x0415,
            /// <summary>
            /// code page 861
            /// </summary>
            kTextEncodingDOSIcelandic = 0x0416,
            /// <summary>
            /// code page 862
            /// </summary>
            kTextEncodingDOSHebrew = 0x0417,
            /// <summary>
            /// code page 863
            /// </summary>
            kTextEncodingDOSCanadianFrench = 0x0418,
            /// <summary>
            /// code page 864
            /// </summary>
            kTextEncodingDOSArabic = 0x0419,
            /// <summary>
            /// code page 865
            /// </summary>
            kTextEncodingDOSNordic = 0x041A,
            /// <summary>
            /// code page 866
            /// </summary>
            kTextEncodingDOSRussian = 0x041B,
            /// <summary>
            /// code page 869, IBM Modern Greek
            /// </summary>
            kTextEncodingDOSGreek2 = 0x041C,
            /// <summary>
            /// code page 874, also for Windows
            /// </summary>
            kTextEncodingDOSThai = 0x041D,
            /// <summary>
            /// code page 932, also for Windows; Shift-JIS with additions
            /// </summary>
            kTextEncodingDOSJapanese = 0x0420,
            /// <summary>
            /// code page 936, also for Windows; was EUC-CN, now GBK (EUC-CN extended)
            /// </summary>
            kTextEncodingDOSChineseSimplif = 0x0421,
            /// <summary>
            /// code page 949, also for Windows; Unified Hangul Code (EUC-KR extended)
            /// </summary>
            kTextEncodingDOSKorean = 0x0422,
            /// <summary>
            /// code page 950, also for Windows; Big-5
            /// </summary>
            kTextEncodingDOSChineseTrad = 0x0423,
            /// <summary>
            /// code page 1252
            /// </summary>
            kTextEncodingWindowsLatin1 = 0x0500,
            /// <summary>
            /// code page 1252 (alternate name)
            /// </summary>
            kTextEncodingWindowsANSI = 0x0500,
            /// <summary>
            /// code page 1250, Central Europe
            /// </summary>
            kTextEncodingWindowsLatin2 = 0x0501,
            /// <summary>
            /// code page 1251, Slavic Cyrillic
            /// </summary>
            kTextEncodingWindowsCyrillic = 0x0502,
            /// <summary>
            /// code page 1253
            /// </summary>
            kTextEncodingWindowsGreek = 0x0503,
            /// <summary>
            /// code page 1254, Turkish
            /// </summary>
            kTextEncodingWindowsLatin5 = 0x0504,
            /// <summary>
            /// code page 1255
            /// </summary>
            kTextEncodingWindowsHebrew = 0x0505,
            /// <summary>
            /// code page 1256
            /// </summary>
            kTextEncodingWindowsArabic = 0x0506,
            /// <summary>
            /// code page 1257
            /// </summary>
            kTextEncodingWindowsBalticRim = 0x0507,
            /// <summary>
            /// code page 1258
            /// </summary>
            kTextEncodingWindowsVietnamese = 0x0508,
            /// <summary>
            /// code page 1361, for Windows NT
            /// </summary>
            kTextEncodingWindowsKoreanJohab = 0x0510,

            // Various national standards begin at 0x600
            kTextEncodingUS_ASCII = 0x0600,
            /// <summary>
            /// ANSEL (ANSI Z39.47) for library use
            /// </summary>
            kTextEncodingANSEL = 0x0601,
            /// <summary>
            /// JIS Roman and 1-byte katakana (halfwidth)
            /// </summary>
            kTextEncodingJIS_X0201_76 = 0x0620,
            kTextEncodingJIS_X0208_83 = 0x0621,
            kTextEncodingJIS_X0208_90 = 0x0622,
            kTextEncodingJIS_X0212_90 = 0x0623,
            kTextEncodingJIS_C6226_78 = 0x0624,
            /// <summary>
            /// Shift-JIS format encoding of JIS X0213 planes 1 and 2
            /// </summary>
            kTextEncodingShiftJIS_X0213 = 0x0628,
            /// <summary>
            /// JIS X0213 in plane-row-column notation (3 bytes)
            /// </summary>
            kTextEncodingJIS_X0213_MenKuTen = 0x0629,
            kTextEncodingGB_2312_80 = 0x0630,
            /// <summary>
            /// annex to GB 13000-93; for Windows 95; EUC-CN extended
            /// </summary>
            kTextEncodingGBK_95 = 0x0631,
            kTextEncodingGB_18030_2000 = 0x0632,
            /// <summary>
            /// same as KSC 5601-92 without Johab annex
            /// </summary>
            kTextEncodingKSC_5601_87 = 0x0640,
            /// <summary>
            /// KSC 5601-92 Johab annex
            /// </summary>
            kTextEncodingKSC_5601_92_Johab = 0x0641,
            /// <summary>
            /// CNS 11643-1992 plane 1
            /// </summary>
            kTextEncodingCNS_11643_92_P1 = 0x0651,
            /// <summary>
            /// CNS 11643-1992 plane 2
            /// </summary>
            kTextEncodingCNS_11643_92_P2 = 0x0652,
            /// <summary>
            /// CNS 11643-1992 plane 3 (was plane 14 in 1986 version)
            /// </summary>
            kTextEncodingCNS_11643_92_P3 = 0x0653,

            // ISO 2022 collections begin at 0x800
            /// <summary>
            /// RFC 1468
            /// </summary>
            kTextEncodingISO_2022_JP = 0x0820,
            /// <summary>
            /// RFC 1554
            /// </summary>
            kTextEncodingISO_2022_JP_2 = 0x0821,
            /// <summary>
            /// RFC 2237
            /// </summary>
            kTextEncodingISO_2022_JP_1 = 0x0822,
            /// <summary>
            /// JIS X0213
            /// </summary>
            kTextEncodingISO_2022_JP_3 = 0x0823,
            /// <summary>
            /// RFC 1922
            /// </summary>
            kTextEncodingISO_2022_CN = 0x0830,
            /// <summary>
            /// RFC 1922
            /// </summary>
            kTextEncodingISO_2022_CN_EXT = 0x0831,
            /// <summary>
            /// RFC 1557
            /// </summary>
            kTextEncodingISO_2022_KR = 0x0840,

            // EUC collections begin at 0x900
            /// <summary>
            /// ISO 646, 1-byte katakana, JIS 208, JIS 212
            /// </summary>
            kTextEncodingEUC_JP = 0x0920,
            /// <summary>
            /// ISO 646, GB 2312-80
            /// </summary>
            kTextEncodingEUC_CN = 0x0930,
            /// <summary>
            /// ISO 646, CNS 11643-1992 Planes 1-16
            /// </summary>
            kTextEncodingEUC_TW = 0x0931,
            /// <summary>
            /// RFC 1557: ISO 646, KS C 5601-1987
            /// </summary>
            kTextEncodingEUC_KR = 0x0940,

            // Misc standards begin at 0xA00
            /// <summary>
            /// plain Shift-JIS
            /// </summary>
            kTextEncodingShiftJIS = 0x0A01,
            /// <summary>
            /// RFC 1489, Russian internet standard
            /// </summary>
            kTextEncodingKOI8_R = 0x0A02,
            /// <summary>
            /// Big-5 (has variants)
            /// </summary>
            kTextEncodingBig5 = 0x0A03,
            /// <summary>
            /// Mac OS Roman permuted to align with ISO Latin-1
            /// </summary>
            kTextEncodingMacRomanLatin1 = 0x0A04,
            /// <summary>
            /// HZ (RFC 1842, for Chinese mail & news)
            /// </summary>
            kTextEncodingHZ_GB_2312 = 0x0A05,
            /// <summary>
            /// Big-5 with Hong Kong special char set supplement
            /// </summary>
            kTextEncodingBig5_HKSCS_1999 = 0x0A06,
            /// <summary>
            /// RFC 1456, Vietnamese
            /// </summary>
            kTextEncodingVISCII = 0x0A07,
            /// <summary>
            /// RFC 2319, Ukrainian
            /// </summary>
            kTextEncodingKOI8_U = 0x0A08,
            /// <summary>
            /// Taiwan Big-5E standard
            /// </summary>
            kTextEncodingBig5_E = 0x0A09,

            // Other platform encodings
            /// <summary>
            /// NextStep Latin encoding
            /// </summary>
            kTextEncodingNextStepLatin = 0x0B01,
            /// <summary>
            /// NextStep Japanese encoding (variant of EUC-JP)
            /// </summary>
            kTextEncodingNextStepJapanese = 0x0B02,

            // EBCDIC & IBM host encodings begin at 0xC00
            /// <summary>
            /// basic EBCDIC-US
            /// </summary>
            kTextEncodingEBCDIC_US = 0x0C01,
            /// <summary>
            /// code page 037, extended EBCDIC (Latin-1 set) for US,Canada...
            /// </summary>
            kTextEncodingEBCDIC_CP037 = 0x0C02,

            // Special values
            /// <summary>
            /// Multi-encoding text with external run info
            /// </summary>
            kTextEncodingMultiRun = 0x0FFF,
            /// <summary>
            /// Unknown or unspecified                  
            /// </summary>
            kTextEncodingUnknown = 0xFFFF
        }

        /// <summary>
        /// File system ID type
        /// </summary>
        struct fsid_t {
            public Int32 val1;
            public Int32 val2;
        }
    }
}