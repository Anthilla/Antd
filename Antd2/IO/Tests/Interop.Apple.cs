////
//// Interop.Apple.cs
////
//// Author:
////       Natalia Portillo <claunia@claunia.com>
////
//// Copyright (c) 2015 © Claunia.com
////
//// Permission is hereby granted, free of charge, to any person obtaining a copy
//// of this software and associated documentation files (the "Software"), to deal
//// in the Software without restriction, including without limitation the rights
//// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//// copies of the Software, and to permit persons to whom the Software is
//// furnished to do so, subject to the following conditions:
////
//// The above copyright notice and this permission notice shall be included in
//// all copies or substantial portions of the Software.
////
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//// THE SOFTWARE.
//#if DEBUG
//using System;
//using System.Runtime.InteropServices;
//using NUnit.Framework;

//namespace Tests
//{
//    [TestFixture]
//    public class InteropApple
//    {
//        [Test]
//        public void TestStat()
//        {
//            if (Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.MacOSX || Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.iOS)
//            {
//                string testFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/.localized";

//                bool testFileExists = System.IO.File.Exists(testFile);

//                Assert.AreEqual(true, testFileExists);

//                Interop.Apple.Stat64 stat;
//                int errno = Interop.Apple.stat64(testFile, out stat);

//                Assert.AreEqual(0, errno, "stat returned error {0}", (Interop.Apple.Errors)Marshal.GetLastWin32Error());
////                Assert.AreEqual(16777219, stat.st_dev);
//                Assert.AreEqual((Interop.Apple.mode_t)33279, stat.st_mode);
//                Assert.AreEqual(1, stat.st_nlink);
//                Assert.AreEqual(2986697, stat.st_ino);
//                Assert.AreEqual(502, stat.st_uid);
//                Assert.AreEqual(20, stat.st_gid);
//                Assert.AreEqual(0, stat.st_rdev);

////                Assert.AreEqual(1423425231, stat.st_atimespec.tv_sec);
//                Assert.AreEqual(0, stat.st_atimespec.tv_nsec);
//                Assert.AreEqual(1253706844, stat.st_mtimespec.tv_sec);
//                Assert.AreEqual(0, stat.st_mtimespec.tv_nsec);
//                Assert.AreEqual(1318498077, stat.st_ctimespec.tv_sec);
//                Assert.AreEqual(0, stat.st_ctimespec.tv_nsec);
//                Assert.AreEqual(0, stat.st_size);
//                Assert.AreEqual(8, stat.st_blocks);
//                Assert.AreEqual(4096, stat.st_blksize);
//                Assert.AreEqual((Interop.Apple.flags_t)32768, stat.st_flags);
//                Assert.AreEqual(0, stat.st_gen);
//#pragma warning disable 618
//                Assert.AreEqual(0, stat.st_lspare);
//                Assert.AreEqual(0, stat.st_qspare[0]);
//                Assert.AreEqual(0, stat.st_qspare[1]);
//#pragma warning restore 618
//            }
//        }

//        [Test]
//        public void TestStat64()
//        {
//            if (Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.MacOSX || Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.iOS)
//            {
//                string testFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/.localized";

//                bool testFileExists = System.IO.File.Exists(testFile);

//                Assert.AreEqual(true, testFileExists);

//                Interop.Apple.Stat64 stat;
//                int errno = Interop.Apple.stat64(testFile, out stat);

//                Assert.AreEqual(0, errno, "stat64 returned error {0}", (Interop.Apple.Errors)Marshal.GetLastWin32Error());
////                Assert.AreEqual(16777219, stat.st_dev);
//                Assert.AreEqual((Interop.Apple.mode_t)33279, stat.st_mode);
//                Assert.AreEqual(1, stat.st_nlink);
//                Assert.AreEqual(2986697, stat.st_ino);
//                Assert.AreEqual(502, stat.st_uid);
//                Assert.AreEqual(20, stat.st_gid);
//                Assert.AreEqual(0, stat.st_rdev);

////                Assert.AreEqual(1423425231, stat.st_atimespec.tv_sec);
//                Assert.AreEqual(0, stat.st_atimespec.tv_nsec);
//                Assert.AreEqual(1253706844, stat.st_mtimespec.tv_sec);
//                Assert.AreEqual(0, stat.st_mtimespec.tv_nsec);
//                Assert.AreEqual(1318498077, stat.st_ctimespec.tv_sec);
//                Assert.AreEqual(0, stat.st_ctimespec.tv_nsec);
//                Assert.AreEqual(1185301066, stat.st_birthtimespec.tv_sec);
//                Assert.AreEqual(0, stat.st_birthtimespec.tv_nsec);
//                Assert.AreEqual(0, stat.st_size);
//                Assert.AreEqual(8, stat.st_blocks);
//                Assert.AreEqual(4096, stat.st_blksize);
//                Assert.AreEqual((Interop.Apple.flags_t)32768, stat.st_flags);
//                Assert.AreEqual(0, stat.st_gen);
//#pragma warning disable 618
//                Assert.AreEqual(0, stat.st_lspare);
//                Assert.AreEqual(0, stat.st_qspare[0]);
//                Assert.AreEqual(0, stat.st_qspare[1]);
//#pragma warning restore 618
//            }
//        }

//        [Test]
//        public void TestUname()
//        {
//            if (Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.MacOSX || Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.iOS)
//            {
//                Interop.Apple.utsname utsName;
//                int errno = Interop.Apple.uname(out utsName);

//                Assert.AreEqual(0, errno, "uname returned error {0}", (Interop.Apple.Errors)Marshal.GetLastWin32Error());
//                Assert.AreEqual("Darwin", utsName.sysname);
//                Assert.AreEqual("zeus.claunia.com", utsName.nodename);
//                Assert.AreEqual("14.0.0", utsName.release);
//                Assert.AreEqual("Darwin Kernel Version 14.0.0: Fri Sep 19 00:26:44 PDT 2014; root:xnu-2782.1.97~2/RELEASE_X86_64", utsName.version);
//                Assert.AreEqual("x86_64", utsName.machine);
//            }
//        }
//    }
//}
//#endif
