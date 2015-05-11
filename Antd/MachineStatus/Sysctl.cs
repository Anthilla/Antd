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

using Antd.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Antd.Status {
    public class Sysctl {

        private static List<SysctlModel> GetAllSysctls() {
            CommandModel command = Command.Launch("sysctl", "--all");
            var output = JsonConvert.SerializeObject(command.output);
            List<SysctlModel> sysctls = MapSysctlJson(output);
            return sysctls;
        }

        public static List<SysctlModel> Running { get { return GetAllSysctls(); } }

        private static List<SysctlModel> ReadSysctlCustomFile() {
            string text = FileSystem.ReadFile("/cfg", "antd.sysctl.conf");
            var output = JsonConvert.SerializeObject(text);
            List<SysctlModel> sysctls = MapSysctlJson(output);
            return sysctls;
        }

        public static List<SysctlModel> Antd { get { return ReadSysctlCustomFile(); } }

        private static List<SysctlModel> ReadSysctlStockFile() {
            string text = FileSystem.ReadFile("/etc", "sysctl.conf");
            var output = JsonConvert.SerializeObject(text);
            List<SysctlModel> sysctls = MapSysctlJson(output);
            return sysctls;
        }

        public static List<SysctlModel> Stock { get { return ReadSysctlStockFile(); } }

        private static List<SysctlModel> MapSysctlJson(string _sysctlJson) {
            string sysctlJson2 = _sysctlJson;
            sysctlJson2 = Regex.Replace(_sysctlJson, @"\s{2,}", " ").Replace("\"", "").Replace("\\n", "\n");
            string sysctlJson = sysctlJson2;
            sysctlJson = Regex.Replace(sysctlJson2, @"\\t", " ");
            string[] rowDivider = new String[] { "\n" };
            string[] sysctlJsonRow = new string[] { };
            sysctlJsonRow = sysctlJson.Split(rowDivider, StringSplitOptions.None).ToArray();
            List<SysctlModel> sysctls = new List<SysctlModel>() { };
            foreach (string rowJson in sysctlJsonRow) {
                if (rowJson != null && rowJson != "") {
                    var fCh = rowJson.ToArray()[0];
                    if (fCh != '#') {
                        string[] sysctlJsonCell = new string[] { };
                        string[] cellDivider = new String[] { " = " };
                        sysctlJsonCell = rowJson.Split(cellDivider, StringSplitOptions.None).ToArray();
                        SysctlModel sysctl = MapSysctl(sysctlJsonCell);
                        sysctls.Add(sysctl);
                    }
                }
            }
            return sysctls;
        }

        private static SysctlModel MapSysctl(string[] _sysctlJsonCell) {
            string[] sysctlJsonCell = _sysctlJsonCell;
            SysctlModel sysctl = new SysctlModel();
            if (sysctlJsonCell.Length > 1) {
                sysctl.param = sysctlJsonCell[0];
                sysctl.value = sysctlJsonCell[1];
            }
            return sysctl;
        }

        public static string Config(string param, string value) {
            CommandModel command = Command.Launch("sysctl", "-w " + param + "=\"" + value + "\"");
            var output = JsonConvert.SerializeObject(command.output);
            WriteConfig();
            LoadConfig();
            return output;
        }

        public static void WriteConfig() {
            var parameters = Stock;
            Directory.CreateDirectory("/cfg");
            string path = Path.Combine("/cfg", "antd.sysctl.conf");
            if (File.Exists(path)) {
                File.Delete(path);
            }
            using (StreamWriter sw = File.CreateText(path)) {
                sw.WriteLine("# " + path);
                sw.WriteLine("# Custom Configuration for Antd");
                foreach (SysctlModel p in parameters) {
                    sw.WriteLine(p.param + " = " + p.value);
                }
                sw.WriteLine("vm.swappiness = 61");
                sw.WriteLine("");
            }
        }

        public static void LoadConfig() {
            string path = Path.Combine("/cfg", "antd.sysctl.conf");
            Command.Launch("sysctl", "-p " + path);
        }
    }
}
