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

using antdlib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.Common;

namespace antdlib.Status {

    public class Sysctl {

        private static List<SysctlModel> GetAllSysctls() {
            var output = JsonConvert.SerializeObject(Terminal.Terminal.Execute("sysctl --all").ConvertCommandToModel().output);
            return MapSysctlJson(output);
        }

        public static List<SysctlModel> Running => GetAllSysctls();

        private static List<SysctlModel> ReadSysctlCustomFile() {
            var output = JsonConvert.SerializeObject(FileSystem.ReadFile(Folder.Root, "antd.sysctl.conf"));
            return MapSysctlJson(output);
        }

        public static List<SysctlModel> Antd => ReadSysctlCustomFile();

        private static List<SysctlModel> ReadSysctlStockFile() {
            var output = JsonConvert.SerializeObject(FileSystem.ReadFile("/etc", "sysctl.conf"));
            return MapSysctlJson(output);
        }

        public static List<SysctlModel> Stock => ReadSysctlStockFile();

        private static List<SysctlModel> MapSysctlJson(string _sysctlJson) {
            var sysctlJson2 = Regex.Replace(_sysctlJson, @"\s{2,}", " ").Replace("\"", "").Replace("\\n", "\n");
            var sysctlJson = Regex.Replace(sysctlJson2, @"\\t", " ");
            var rowDivider = new[] { "\n" };
            var sysctlJsonRow = sysctlJson.Split(rowDivider, StringSplitOptions.None).ToArray();
            return (from rowJson in sysctlJsonRow where !string.IsNullOrEmpty(rowJson) let fCh = rowJson.ToArray()[0] where fCh != '#' let sysctlJsonCell = new string[] { } let cellDivider = new[] { " = " } select rowJson.Split(cellDivider, StringSplitOptions.None).ToArray() into sysctlJsonCell select MapSysctl(sysctlJsonCell)).ToList();
        }

        private static SysctlModel MapSysctl(string[] sysctlJsonCell) {
            var sysctl = new SysctlModel();
            if (sysctlJsonCell.Length <= 1)
                return sysctl;
            sysctl.param = sysctlJsonCell[0];
            sysctl.value = sysctlJsonCell[1];
            return sysctl;
        }

        public static string Config(string param, string value) {
            WriteConfig();
            LoadConfig();
            return JsonConvert.SerializeObject(Terminal.Terminal.Execute("sysctl -w " + param + "=\"" + value + "\"").ConvertCommandToModel().output);
        }

        public static void WriteConfig() {
            Directory.CreateDirectory(Folder.Root);
            var path = Path.Combine(Folder.Root, "antd.sysctl.conf");
            if (File.Exists(path)) {
                File.Delete(path);
            }
            using (var sw = File.CreateText(path)) {
                sw.WriteLine("# " + path);
                sw.WriteLine("# Custom Configuration for Antd");
                foreach (var p in Stock) {
                    sw.WriteLine(p.param + " = " + p.value);
                }
                sw.WriteLine("");
            }
        }

        public static void LoadConfig() {
            Terminal.Terminal.Execute("sysctl -p " + Path.Combine(Folder.Root, "antd.sysctl.conf"));
        }
    }
}