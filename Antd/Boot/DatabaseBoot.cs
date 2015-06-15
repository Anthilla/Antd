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
using System.Threading;
using System.Linq;
using Antd.Common;

namespace Antd.Boot {

    public class DatabaseBoot {

        public static void Start(string[] dbPaths) {
            //v3
            //DeNSo.Configuration.BasePath = dbPaths;
            //DeNSo.Configuration.EnableJournaling = true;
            //DeNSo.Configuration.EnableDataCompression = false;
            //DeNSo.Configuration.DBCheckTimeSpan = new TimeSpan(0, 2, 0);
            //DeNSo.Configuration.SaveInterval = new TimeSpan(0, 2, 0);
            //DeNSo.Session.DefaultDataBase = "Antd_DB";
            //DeNSo.Session.Start();

            DeNSo.Configuration.BasePath = dbPaths;
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.DBCheckTimeSpan = new System.TimeSpan(0, 1, 0);
            DeNSo.Configuration.ReindexCheck = new System.TimeSpan(0, 1, 0);
            DeNSo.Configuration.EnableOperationsLog = false;
            DeNSo.Session.DefaultDataBase = "Antd_DB";
            DeNSo.Session.Start();
        }

        public static void ShutDown() {
            DeNSo.Session.ShutDown();
        }

        public static void Test(bool isActive) {
            if (isActive == true) {
                ConsoleLogger.Log("Test DATABASE");
                var guid = Guid.NewGuid().ToString();
                ConsoleLogger.Log(">> write");
                TestClass write = new TestClass();
                write._Id = guid;
                write.Date = DateTime.Now;
                write.Foo = "foo";
                write.Bar = write.Foo + write.Date.ToString() + write.Foo;
                DeNSo.Session.New.Set(write);
                ConsoleLogger.Success(">> write done");
                Thread.Sleep(1000);
                ConsoleLogger.Log(">> read");
                var read = DeNSo.Session.New.Get<TestClass>(m => m._Id == guid).First();
                ConsoleLogger.Info(">> read done");
                if (read != null) {
                    ConsoleLogger.Success(">> result: " + JsonConvert.SerializeObject(read));
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
                ConsoleLogger.Info(">> read done");
                Thread.Sleep(1000);
                if (edited != null) {
                    ConsoleLogger.Success(">> result: " + JsonConvert.SerializeObject(edited));
                }
                else {
                    ConsoleLogger.Warn(">> read failed");
                }
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
