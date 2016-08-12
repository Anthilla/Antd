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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace antdlib.common {
    public class ConsoleLogger {

        public static string Method() => new StackTrace().GetFrame(1).GetMethod().Name;

        public static IEnumerable<string> GetAll() {
            var logs = File.ReadAllLines("/cfg/antd/antd.log");
            return logs;
        }

        public static void Log(dynamic message, string source = "") {
            var src = source.Length > 0 ? $" src={source}" : "";
            var logEntry = $"app=antd lvl=0 msg={message}{src}";
            Console.WriteLine(logEntry);
            Write(logEntry);
        }

        public static void Warn(dynamic message, string source = "") {
            var src = source.Length > 0 ? $" src={source}" : "";
            var logEntry = $"app=antd lvl=1 msg={message}{src}";
            Console.WriteLine(logEntry);
            Write(logEntry);
        }

        public static void Error(dynamic message, string source = "") {
            var src = source.Length > 0 ? $" src={source}" : "";
            var logEntry = $"app=antd lvl=2 msg={message}{src}";
            Console.WriteLine(logEntry);
            Write(logEntry);
        }

        public static void Point(dynamic message) {
            Console.WriteLine($"→→ {message}");
        }

        private static void Write(string message) {
            if (!File.Exists("/cfg/antd/antd.log")) {
                Directory.CreateDirectory("/cfg");
                Directory.CreateDirectory("/cfg/antd");
                File.WriteAllText("/cfg/antd/antd.log", "");
                return;
            }
            using (var writer = new StreamWriter("/cfg/antd/antd.log", true)) {
                writer.WriteLine(message);
            }
        }
    }
}