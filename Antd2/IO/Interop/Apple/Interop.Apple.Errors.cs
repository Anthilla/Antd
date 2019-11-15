//
// Interop.Apple.Errors.cs
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

internal static partial class Interop {
    internal static partial class Apple {
        internal enum Errors {
            /// <summary>Operation not permitted</summary>
            EPERM = 1,
            /// <summary>No such file or directory</summary>
            ENOENT = 2,
            /// <summary>No such process</summary>
            ESRCH = 3,
            /// <summary>Interrupted system call</summary>
            EINTR = 4,
            /// <summary>Input/output error</summary>
            EIO = 5,
            /// <summary>Device not configured</summary>
            ENXIO = 6,
            /// <summary>Argument list too long</summary>
            E2BIG = 7,
            /// <summary>Exec format error</summary>
            ENOEXEC = 8,
            /// <summary>Bad file descriptor</summary>
            EBADF = 9,
            /// <summary>No child processes</summary>
            ECHILD = 10,
            /// <summary>Resource deadlock avoided</summary>
            EDEADLK = 11,
            /// <summary>Cannot allocate memory</summary>
            ENOMEM = 12,
            /// <summary>Permission denied</summary>
            EACCES = 13,
            /// <summary>Bad address</summary>
            EFAULT = 14,
            /// <summary>Block device required</summary>
            ENOTBLK = 15,
            /// <summary>Device / Resource busy</summary>
            EBUSY = 16,
            /// <summary>File exists</summary>
            EEXIST = 17,
            /// <summary>Cross-device link</summary>
            EXDEV = 18,
            /// <summary>Operation not supported by device</summary>
            ENODEV = 19,
            /// <summary>Not a directory</summary>
            ENOTDIR = 20,
            /// <summary>Is a directory</summary>
            EISDIR = 21,
            /// <summary>Invalid argument</summary>
            EINVAL = 22,
            /// <summary>Too many open files in system</summary>
            ENFILE = 23,
            /// <summary>Too many open files</summary>
            EMFILE = 24,
            /// <summary>Inappropriate ioctl for device</summary>
            ENOTTY = 25,
            /// <summary>Text file busy</summary>
            ETXTBSY = 26,
            /// <summary>File too large</summary>
            EFBIG = 27,
            /// <summary>No space left on device</summary>
            ENOSPC = 28,
            /// <summary>Illegal seek</summary>
            ESPIPE = 29,
            /// <summary>Read-only file system</summary>
            EROFS = 30,
            /// <summary>Too many links</summary>
            EMLINK = 31,
            /// <summary>Broken pipe</summary>
            EPIPE = 32,
            /// <summary>Numerical argument out of domain</summary>
            EDOM = 33,
            /// <summary>Result too large</summary>
            ERANGE = 34,

            /// <summary>Resource temporarily unavailable</summary>
            EAGAIN = 35,
            /// <summary>Operation would block</summary>
            EWOULDBLOCK = EAGAIN,
            /// <summary>Operation now in progress</summary>
            EINPROGRESS = 36,
            /// <summary>Operation already in progress</summary>
            EALREADY = 37,

            /// <summary>Socket operation on non-socket</summary>
            ENOTSOCK = 38,
            /// <summary>Destination address required</summary>
            EDESTADDRREQ = 39,
            /// <summary>Message too long</summary>
            EMSGSIZE = 40,
            /// <summary>Protocol wrong type for socket</summary>
            EPROTOTYPE = 41,
            /// <summary>Protocol not available</summary>
            ENOPROTOOPT = 42,
            /// <summary>Protocol not supported</summary>
            EPROTONOSUPPORT = 43,
            /// <summary>Socket type not supported</summary>
            ESOCKTNOSUPPORT = 44,
            /// <summary>Operation not supported</summary>
            ENOTSUP = 45,
            /// <summary>Operation not supported on socket</summary>
            EOPNOTSUPP = ENOTSUP,
            /// <summary>Protocol family not supported</summary>
            EPFNOSUPPORT = 46,
            /// <summary>Address family not supported by protocol family</summary>
            EAFNOSUPPORT = 47,
            /// <summary>Address already in use</summary>
            EADDRINUSE = 48,
            /// <summary>Can't assign requested address</summary>
            EADDRNOTAVAIL = 49,

            /// <summary>Network is down</summary>
            ENETDOWN = 50,
            /// <summary>Network is unreachable</summary>
            ENETUNREACH = 51,
            /// <summary>Network dropped connection on reset</summary>
            ENETRESET = 52,
            /// <summary>Software caused connection abort</summary>
            ECONNABORTED = 53,
            /// <summary>Connection reset by peer</summary>
            ECONNRESET = 54,
            /// <summary>No buffer space available</summary>
            ENOBUFS = 55,
            /// <summary>Socket is already connected</summary>
            EISCONN = 56,
            /// <summary>Socket is not connected</summary>
            ENOTCONN = 57,
            /// <summary>Can't send after socket shutdown</summary>
            ESHUTDOWN = 58,
            /// <summary>Too many references: can't splice</summary>
            ETOOMANYREFS = 59,
            /// <summary>Operation timed out</summary>
            ETIMEDOUT = 60,
            /// <summary>Connection refused</summary>
            ECONNREFUSED = 61,

            /// <summary>Too many levels of symbolic links</summary>
            ELOOP = 62,
            /// <summary>File name too long</summary>
            ENAMETOOLONG = 63,

            /// <summary>Host is down</summary>
            EHOSTDOWN = 64,
            /// <summary>No route to host</summary>
            EHOSTUNREACH = 65,
            /// <summary>Directory not empty</summary>
            ENOTEMPTY = 66,

            /// <summary>Too many processes</summary>
            EPROCLIM = 67,
            /// <summary>Too many users</summary>
            EUSERS = 68,
            /// <summary>Disc quota exceeded</summary>
            EDQUOT = 69,

            /// <summary>Stale NFS file handle</summary>
            ESTALE = 70,
            /// <summary>Too many levels of remote in path</summary>
            EREMOTE = 71,
            /// <summary>RPC struct is bad</summary>
            EBADRPC = 72,
            /// <summary>RPC version wrong</summary>
            ERPCMISMATCH = 73,
            /// <summary>RPC prog. not avail</summary>
            EPROGUNAVAIL = 74,
            /// <summary>Program version wrong</summary>
            EPROGMISMATCH = 75,
            /// <summary>Bad procedure for program</summary>
            EPROCUNAVAIL = 76,

            /// <summary>No locks available</summary>
            ENOLCK = 77,
            /// <summary>Function not implemented</summary>
            ENOSYS = 78,

            /// <summary>Inappropriate file type or format</summary>
            EFTYPE = 79,
            /// <summary>Authentication error</summary>
            EAUTH = 80,
            /// <summary>Need authenticator</summary>
            ENEEDAUTH = 81,

            /// <summary>Device power is off</summary>
            EPWROFF = 82,
            /// <summary>Device error, e.g. paper out</summary>
            EDEVERR = 83,

            /// <summary>Value too large to be stored in data type</summary>
            EOVERFLOW = 84,

            /// <summary>Bad executable</summary>
            EBADEXEC = 85,
            /// <summary>Bad CPU type in executable</summary>
            EBADARCH = 86,
            /// <summary>Shared library version mismatch</summary>
            ESHLIBVERS = 87,
            /// <summary>Malformed Macho file</summary>
            EBADMACHO = 88,

            /// <summary>Operation canceled</summary>
            ECANCELED = 89,

            /// <summary>Identifier removed</summary>
            EIDRM = 90,
            /// <summary>No message of desired type</summary>   
            ENOMSG = 91,
            /// <summary>Illegal byte sequence</summary>
            EILSEQ = 92,
            /// <summary>Attribute not found</summary>
            ENOATTR = 93,

            /// <summary>Bad message</summary>
            EBADMSG = 94,
            /// <summary>Reserved</summary>
            EMULTIHOP = 95,
            /// <summary>No message available on STREAM</summary>
            ENODATA = 96,
            /// <summary>Reserved</summary>
            ENOLINK = 97,
            /// <summary>No STREAM resources</summary>
            ENOSR = 98,
            /// <summary>Not a STREAM</summary>
            ENOSTR = 99,
            /// <summary>Protocol error</summary>
            EPROTO = 100,
            /// <summary>STREAM ioctl timeout</summary>
            ETIME = 101,

            /// <summary>Operation not supported on socket</summary>
            EOPNOTSUPPSKT = 102,

            /// <summary>No such policy registered</summary>
            ENOPOLICY = 103,

            /// <summary>State not recoverable</summary>
            ENOTRECOVERABLE = 104,
            /// <summary>Previous owner died</summary>
            EOWNERDEAD = 105,
            /// <summary>Interface output queue is full</summary>
            EQFULL = 106,
        }
    }
}

