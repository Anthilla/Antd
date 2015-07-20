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

using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Antd.Boot {

    public class DatabaseBoot {

        public static void Start(string[] dbPaths, bool doTest) {
            DeNSo.Configuration.BasePath = dbPaths;
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.EnableDataCompression = false;
            DeNSo.Configuration.DBCheckTimeSpan = new TimeSpan(0, 2, 0);
            DeNSo.Configuration.SaveInterval = new TimeSpan(0, 2, 0);

            DeNSo.Session.DefaultDataBase = "antd_db_0";
            DeNSo.Session.Start();

            if (doTest == true) {
                Test();
            }
        }

        public static void ShutDown() {
            DeNSo.Session.ShutDown();
        }

        private static void CheckPaths(string[] dbPaths) {
            foreach (string path in dbPaths) {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                    ConsoleLogger.Warn("You are trying to write your database in {0}, but this dir does'nt exist!", path);
                    ConsoleLogger.Warn("Although john has created this folder, please check that you do not miss anything!");
                }
            }
        }

        private static void Test() {
            ConsoleLogger.Log("Test DATABASE");
            var guid = Guid.NewGuid().ToString();
            ConsoleLogger.Log(">> write");
            TestClass write = new TestClass {
                _Id = guid,
                Date = DateTime.Now,
                Foo = "foo"
            };
            write.Bar = write.Foo + write.Date.ToString() + write.Foo;
            DeNSo.Session.New.Set(write);
            ConsoleLogger.Log(">> write done");
            Thread.Sleep(1000);
            ConsoleLogger.Log(">> read");
            var read = DeNSo.Session.New.Get<TestClass>(m => m._Id == guid).First();
            ConsoleLogger.Log(">> read done");
            if (read != null) {
                ConsoleLogger.Log(">> result: " + JsonConvert.SerializeObject(read));
            }
            else {
                ConsoleLogger.Warn(">> read failed");
            }
            Thread.Sleep(1000);
            ConsoleLogger.Log(">> edit");
            read.Date = DateTime.Now;
            read.Foo = "foo_edit";
            read.Bar = read.Foo + read.Date.ToString() + read.Foo;
            DeNSo.Session.New.Set(read);
            var edited = DeNSo.Session.New.Get<TestClass>(m => m._Id == guid).First();
            ConsoleLogger.Log(">> read done");
            Thread.Sleep(1000);
            if (edited != null) {
                ConsoleLogger.Log(">> result: " + JsonConvert.SerializeObject(edited));
            }
            else {
                ConsoleLogger.Warn(">> read failed");
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