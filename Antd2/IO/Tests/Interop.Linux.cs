////
//// Interop.Linux.cs
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

//namespace Tests {
//    [TestFixture]
//    public class InteropLinux {
//        [Test]
//        public void TestUname() {
//            if (Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.Linux || Interop.DetectOS.GetRealPlatformID() == Interop.PlatformID.Android) {
//                Interop.Linux.utsname utsName;
//                int errno = Interop.Linux.uname(out utsName);

//                Assert.AreEqual(0, errno, "uname returned error {0}", (Interop.Linux.Errors)Marshal.GetLastWin32Error());
//                Assert.AreEqual("Linux", utsName.sysname);
//                Assert.AreEqual("hades.claunia.com", utsName.nodename);
//                Assert.AreEqual("3.15.0-sabayon", utsName.release);
//                Assert.AreEqual("#1 SMP Mon Aug 18 02:55:48 UTC 2014", utsName.version);
//                Assert.AreEqual("x86_64", utsName.machine);
//                Assert.AreEqual("(none)", utsName.domainname);
//            }
//        }
//    }
//}
//#endif