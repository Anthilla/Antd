//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using antdlib.Log;

namespace antdlib.Boot {
    public class DatabaseBoot {

        public static void Start(string[] dbPaths) {
            DeNSo.Configuration.BasePath = dbPaths;
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.EnableDataCompression = false;
            DeNSo.Configuration.ReindexCheck = new TimeSpan(0, 1, 0);
            DeNSo.Configuration.EnableOperationsLog = false;
            DeNSo.Session.DefaultDataBase = Folder.AntdCfgDatabaseName;
            DeNSo.Session.Start();
            CheckPaths(dbPaths);
        }

        public static void ShutDown() {
            DeNSo.Session.ShutDown();
        }

        private static void CheckPaths(string[] dbPaths) {
            foreach (var path in dbPaths) {
                Directory.CreateDirectory(path);
                Directory.CreateDirectory($"{path}/{Folder.AntdCfgDatabaseName}");
            }
            foreach (var path in dbPaths.Where(path => !Directory.Exists(path))) {
                Directory.CreateDirectory(path);
                ConsoleLogger.Warn("You are trying to write your database in {0}, but this dir does'nt exist!", path);
                ConsoleLogger.Warn("Although Antd has created this folder, please check that you do not miss anything!");
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