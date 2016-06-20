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
using System.Linq;
using antdlib.common;
using antdlib.Models;
using Newtonsoft.Json;

namespace antdlib.Info {

    public class Meminfo {

        public static string GetText() {
            return JsonConvert.SerializeObject(FileSystem.ReadFile("/proc/meminfo"));
        }

        public static IEnumerable<MeminfoModel> GetModel() {
            return ConvertMeminfo(FileSystem.ReadFile("/proc/meminfo"));
        }

        public static IEnumerable<MeminfoModel> ConvertMeminfo(string meminfoText) {
            var rowList = meminfoText.Split(new[] { "\n" }, StringSplitOptions.None).ToArray();
            return (from row in rowList
                    where !string.IsNullOrEmpty(row)
                    select row.Split(new[] { ": " }, StringSplitOptions.None).ToArray()
                into cellList
                    select new MeminfoModel {
                        key = cellList[0].Trim(),
                        value = cellList[1].Trim()
                    }).ToList();
        }

        public static MeminfoMappedModel GetMappedModel() {
            var raw = ConvertMeminfo(FileSystem.ReadFile("/proc/meminfo"));
            var meminfoModels = raw as IList<MeminfoModel> ?? raw.ToList();
            var meminfo = new MeminfoMappedModel {
                MemTotal = meminfoModels.Where(_ => _.key == "MemTotal").Select(_ => _.value).FirstOrDefault(),
                MemFree = meminfoModels.Where(_ => _.key == "MemFree").Select(_ => _.value).FirstOrDefault(),
                MemAvailable = meminfoModels.Where(_ => _.key == "MemAvailable").Select(_ => _.value).FirstOrDefault(),
                Buffers = meminfoModels.Where(_ => _.key == "Buffers").Select(_ => _.value).FirstOrDefault(),
                Cached = meminfoModels.Where(_ => _.key == "Cached").Select(_ => _.value).FirstOrDefault(),
                SwapCached = meminfoModels.Where(_ => _.key == "SwapCached").Select(_ => _.value).FirstOrDefault(),
                SwapTotal = meminfoModels.Where(_ => _.key == "SwapTotal").Select(_ => _.value).FirstOrDefault(),
                SwapFree = meminfoModels.Where(_ => _.key == "SwapFree").Select(_ => _.value).FirstOrDefault(),
            };
            return meminfo;
        }
    }

    public class MeminfoMappedModel {
        public string MemTotal { get; set; }
        public string MemFree { get; set; }
        public string MemAvailable { get; set; }
        public string Buffers { get; set; }
        public string Cached { get; set; }
        public string SwapCached { get; set; }
        public string SwapTotal { get; set; }
        public string SwapFree { get; set; }
    }
}
