///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

//using DeNSo.P2P;
//using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace antdlib.Boot {
    public class DatabaseBoot {

        public static void Start(string[] dbPaths, bool doTest) {
            DeNSo.Configuration.BasePath = dbPaths;
            DeNSo.Configuration.EnableJournaling = true;
            //DeNSo.Configuration.EnableDataCompression = false; /*DensoV3*/
            //DeNSo.Configuration.DBCheckTimeSpan = new TimeSpan(0, 1, 0);
            DeNSo.Configuration.ReindexCheck = new TimeSpan(0, 1, 0);
            //DeNSo.Configuration.SaveInterval = new TimeSpan(0, 1, 0); /*DensoV3*/
            DeNSo.Configuration.EnableOperationsLog = false;

            //EventP2PDispatcher.EnableP2PEventMesh();
            //EventP2PDispatcher.MakeNodeAvaiableToPNRP(Cloud.Available);

            DeNSo.Session.DefaultDataBase = "antd_db_0";
            DeNSo.Session.Start();

            if (doTest == true) {
                Test();
            }
            Log.Logger.TraceMethod("Boot", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        public static void ShutDown() {
            //EventP2PDispatcher.StopP2PEventMesh();
            DeNSo.Session.ShutDown();
            Log.Logger.TraceMethod("Boot", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void CheckPaths(string[] dbPaths) {
            foreach (string path in dbPaths) {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                    ConsoleLogger.Warn("You are trying to write your database in {0}, but this dir does'nt exist!", path);
                    ConsoleLogger.Warn("Although john has created this folder, please check that you do not miss anything!");
                }
            }
            Log.Logger.TraceMethod("Boot", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        private static void Test() {
            ConsoleLogger.Warn("     dbtest -> start");
            var guid = Guid.NewGuid().ToString();
            TestClass write = new TestClass {
                _Id = guid,
                Date = DateTime.Now,
                Foo = "foo"
            };
            write.Bar = write.Foo + write.Date.ToString() + write.Foo;
            DeNSo.Session.New.Set(write);
            Thread.Sleep(1000);
            var read = DeNSo.Session.New.Get<TestClass>(m => m._Id == guid).First();
            if (read != null) {
            }
            else {
                ConsoleLogger.Warn("     dbtest -> error while reading");
            }
            Thread.Sleep(1000);
            read.Date = DateTime.Now;
            read.Foo = "foo_edit";
            read.Bar = read.Foo + read.Date.ToString() + read.Foo;
            DeNSo.Session.New.Set(read);
            var edited = DeNSo.Session.New.Get<TestClass>(m => m._Id == guid).First();
            Thread.Sleep(1000);
            if (edited != null) {
            }
            else {
                ConsoleLogger.Warn("     dbtest -> error while reading");
            }
        }
    }

    public class TestClass {

        public string _Id { get; set; }

        public DateTime Date { get; set; }

        public string Foo { get; set; }

        public string Bar { get; set; }
    }
}