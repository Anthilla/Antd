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
using antdlib.Models;
using System.Collections.Generic;
using System.Linq;
using antdlib.Common;
using Newtonsoft.Json;

namespace antdlib.Status {
    public class Cpuinfo {
        public static IEnumerable<string> Get() {
            var r1 = Terminal.Terminal.Execute("numactl -s");
            var list = r1.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(row => row).ToList();
            var r2 = Terminal.Terminal.Execute("numactl -H");
            list.AddRange(r2.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(row => row));
            return list;
        }

        public static string GetText() {
            return JsonConvert.SerializeObject(FileSystem.ReadFile("/proc/cpuinfo"));
        }

        public static List<CpuinfoModel> GetModel() {
            return ConvertCpuinfo(FileSystem.ReadFile("/proc/cpuinfo"));
        }

        private static List<CpuinfoModel> ConvertCpuinfo(string cpuinfoText) {
            var rowList = cpuinfoText.Split(new[] { "\n" }, StringSplitOptions.None).ToArray();
            return (from row in rowList
                    where !string.IsNullOrEmpty(row)
                    select row.Split(new[] { ": " }, StringSplitOptions.None).ToArray()
                into cellList
                    select new CpuinfoModel {
                        Key = cellList[0],
                        Value = (cellList.Length > 1) ? cellList[1] : ""
                    }).ToList();
        }
    }
}
