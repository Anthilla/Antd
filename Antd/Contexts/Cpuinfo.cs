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

using Antd.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd {

    public class Cpuinfo {

        public static string GetText() {
            string meminfoContent = "";
            meminfoContent = FileSystem.ReadFile("/proc/cpuinfo");
            string meminfoJson = JsonConvert.SerializeObject(meminfoContent);
            return meminfoJson;
        }

        public static List<CpuinfoModel> GetModel() {
            string meminfoContent = "";
            meminfoContent = FileSystem.ReadFile("/proc/cpuinfo");
            var meminfo = ConvertCpuinfo(meminfoContent);
            return meminfo;
        }

        private static List<CpuinfoModel> ConvertCpuinfo(string cpuinfoText) {
            List<CpuinfoModel> cpuinfoList = new List<CpuinfoModel>();
            string[] rowDivider = new String[] { "\n" };
            string[] cellDivider = new String[] { ": " };
            string[] rowList = cpuinfoText.Split(rowDivider, StringSplitOptions.None).ToArray();
            foreach (string row in rowList) {
                if (!string.IsNullOrEmpty(row)) {
                    string[] cellList = row.Split(cellDivider, StringSplitOptions.None).ToArray();
                    CpuinfoModel cpuinfo = new CpuinfoModel();
                    cpuinfo.key = cellList[0];
                    cpuinfo.value = (cellList.Length > 1) ? cellList[1] : "";
                    cpuinfoList.Add(cpuinfo);
                }
            }
            return cpuinfoList;
        }
    }
}
