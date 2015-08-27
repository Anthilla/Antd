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

using antdlib.MountPoint;
using antdlib.ViewBinds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.Svcs.Dhcp {
    public class DhcpConfig {

        private static string serviceGuid = "470782AC-4EB4-4964-8B55-F900DABAB59B";

        private static string dir = "/etc/dhcp";

        private static string DIR = Mount.SetDIRSPath(dir);

        private static string mainFile = "dhcpd.conf";

        private static string antdDhcpFile = "antd.dhcp.conf";

        public static void SetReady() {
            Terminal.Execute($"cp {dir} {DIR}");
            FileSystem.CopyDirectory(dir, DIR);
            Mount.Dir(dir);
        }

        private static bool CheckIsActive() {
            var mount = MountRepository.Get(dir);
            return (mount == null) ? false : true;
        }

        public static bool IsActive { get { return CheckIsActive(); } }

        /// <summary>
        /// todo prendere comando giusto
        /// </summary>
        public static void ReloadConfig() {
            //Terminal.Execute($"smbcontrol all reload-config");
        }

        private static List<KeyValuePair<string, List<string>>> GetServiceStructure() {
            var list = new List<KeyValuePair<string, List<string>>>() { };
            var files = Directory.EnumerateFiles(DIR, "*.conf", SearchOption.AllDirectories).ToArray();
            for (int i = 0; i < files.Length; i++) {
                if (File.ReadLines(files[i]).Any(line => line.Contains("include"))) {
                    var lines = File.ReadLines(files[i]).Where(line => line.Contains("include")).ToList();
                    var dump = new List<string>() { };
                    foreach (var line in lines) {
                        dump.Add(line.Split('=')[1].Trim().Replace(dir, DIR));
                    }
                    list.Add(new KeyValuePair<string, List<string>>(files[i].Replace("\\", "/"), dump));
                }
            }
            if (list.Count() < 1) {
                list.Add(new KeyValuePair<string, List<string>>($"{DIR}/{mainFile}", new List<string>() { }));
            }
            return list;
        }

        public static List<KeyValuePair<string, List<string>>> Structure { get { return GetServiceStructure(); } }

        private static List<string> GetServiceSimpleStructure() {
            var list = new List<string>() { };
            var files = Directory.EnumerateFiles(DIR, "*.conf", SearchOption.AllDirectories).ToArray();
            for (int i = 0; i < files.Length; i++) {
                if (File.ReadLines(files[i]).Any(line => line.Contains("include"))) {
                    var lines = File.ReadLines(files[i]).Where(line => line.Contains("include")).ToList();
                    foreach (var line in lines) {
                        list.Add(line.Split('=')[1].Trim().Replace(dir, DIR));
                    }
                }
            }
            if (list.Count() < 1) {
                list.Add($"{DIR}/{mainFile}");
            }
            return list;
        }

        public static List<string> SimpleStructure { get { return GetServiceSimpleStructure(); } }

        public class MapRules {
            public static char CharCommentConf { get { return '#'; } }

            public static string CharCommentConfAlt { get { return "//"; } }

            public static char CharCommentZone { get { return ';'; } }

            public static string VerbInclude { get { return "include"; } }

            public static string VerbIncludeZone { get { return "zone"; } }

            public static char CharOrigin { get { return '$'; } }

            public static string VerbOrigin { get { return "$ORIGIN"; } }

            public static char CharKevValueSeparator { get { return ' '; } }

            public static char CharValueArraySeparator { get { return ','; } }

            public static char CharEndOfLine { get { return '\n'; } }

            public static char CharSectionOpen { get { return '['; } }

            public static char CharSectionClose { get { return ']'; } }

            public static char CharBlockOpen { get { return '{'; } }

            public static char CharBlockClose { get { return '}'; } }

            public static char CharZoneOpen { get { return '('; } }

            public static char CharZoneClose { get { return ')'; } }

            public static char CharEndOfLineValue { get { return ';'; } }

            public class Statement {
                public static string Key { get { return "key"; } }

                public static string Zone { get { return "zone"; } }

                public static string Subnet { get { return "subnet"; } }

                public static string Host { get { return "host"; } }

                public static string Class { get { return "class"; } }

                public static string Sublass { get { return "subclass"; } }

                public static string Failover { get { return "failover"; } }

                public static string Logging { get { return "logging"; } }

                public static string Include { get { return "include"; } }

                public static string Prefix6 { get { return "prefix6"; } }

                public static string Range6 { get { return "range6"; } }

                public static string Range { get { return "range"; } }

                public static string Subnet6 { get { return "subnet6"; } }

                public static string SharedNetwork { get { return "shared-network"; } }

                public static string Group { get { return "group"; } }
            }
        }

        public class LineModel {
            public string FilePath { get; set; }

            public string Key { get; set; }

            public string Value { get; set; }

            public ServiceDataType Type { get; set; }

            public KeyValuePair<string, string> BooleanVerbs { get; set; }
        }

        public class OptionModel {
            public string FilePath { get; set; } = $"{DIR}/{mainFile}";

            public string Name { get; set; }

            public string StringDefinition { get; set; } = "";

            public List<LineModel> Data { get; set; } = new List<LineModel>() { };
        }

        public class DhcpModel {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public string Timestamp { get; set; }

            public List<LineModel> Global { get; set; } = new List<LineModel>() { };

            public List<LineModel> Include { get; set; } = new List<LineModel>() { };

            public List<OptionModel> Key { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> Subnet { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> Host { get; set; } = new List<OptionModel>() { };

            public List<LineModel> Prefix6 { get; set; } = new List<LineModel>() { };

            public List<LineModel> Range6 { get; set; } = new List<LineModel>() { };

            public List<LineModel> Range { get; set; } = new List<LineModel>() { };

            public List<OptionModel> Subnet6 { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> Failover { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> Logging { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> SharedNetwork { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> Group { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> Class { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> Subclass { get; set; } = new List<OptionModel>() { };
        }

        public class MapFile {

            private static string CleanLine(string line) {
                var removeTab = line.Replace("\t", " ");
                var clean = removeTab;
                if (removeTab.Contains(MapRules.CharCommentConf) && !line.StartsWith(MapRules.CharCommentConf.ToString())) {
                    var splitAtComment = removeTab.Split(MapRules.CharCommentConf);
                    clean = splitAtComment[0].Trim();
                }
                return clean;
            }

            private static ServiceDataType SupposeDataType(string value) {
                if (value == "true" || value == "True" ||
                    value == "false" || value == "False" ||
                    value == "yes" || value == "Yes" ||
                    value == "no" || value == "No") {
                    return ServiceDataType.Boolean;
                }
                //else if (value.Length > 5 && value.Contains(",")) {
                //    return ServiceDataType.StringArray;
                //}
                else {
                    return ServiceDataType.String;
                }
            }

            private static KeyValuePair<string, string> SupposeBooleanVerbs(string value) {
                if (value == "true" || value == "false") {
                    return new KeyValuePair<string, string>("true", "false");
                }
                else if (value == "True" || value == "False") {
                    return new KeyValuePair<string, string>("True", "False");
                }
                else if (value == "yes" || value == "no") {
                    return new KeyValuePair<string, string>("yes", "no");
                }
                else if (value == "Yes" || value == "No") {
                    return new KeyValuePair<string, string>("Yes", "No");
                }
                else {
                    return new KeyValuePair<string, string>("", "");
                }
            }

            public static void Render() {
                var options = new List<OptionModel>() { };
                var data = new List<LineModel>() { };

                var dhcp = new DhcpModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    Global = data,
                    Host = options,
                    Key = options,
                    Subnet = options
                };
                DeNSo.Session.New.Set(dhcp);
            }

            public static DhcpModel Get() {
                var dhcp = DeNSo.Session.New.Get<DhcpModel>(s => s.Guid == serviceGuid).FirstOrDefault();
                return dhcp;
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceDhcp parameter) {
                ServiceDataType type = SupposeDataType(parameter.DataValue);
                var booleanVerbs = SupposeBooleanVerbs(parameter.DataValue);
                var data = new LineModel() {
                    FilePath = parameter.DataFilePath,
                    Key = parameter.DataKey,
                    Value = parameter.DataValue,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                return data;
            }

            private static ServiceDataType SupposeDataType(string value) {
                if (value == "true" || value == "True" ||
                    value == "false" || value == "False" ||
                    value == "yes" || value == "Yes" ||
                    value == "no" || value == "No") {
                    return ServiceDataType.Boolean;
                }
                else {
                    return ServiceDataType.String;
                }
            }

            private static KeyValuePair<string, string> SupposeBooleanVerbs(string value) {
                if (value == "true" || value == "false") {
                    return new KeyValuePair<string, string>("true", "false");
                }
                else if (value == "True" || value == "False") {
                    return new KeyValuePair<string, string>("True", "False");
                }
                else if (value == "yes" || value == "no") {
                    return new KeyValuePair<string, string>("yes", "no");
                }
                else if (value == "Yes" || value == "No") {
                    return new KeyValuePair<string, string>("Yes", "No");
                }
                else {
                    return new KeyValuePair<string, string>("", "");
                }
            }

            public static void SaveGlobalConfig(List<ServiceDhcp> newParameters) {
                var dhcp = new DhcpModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                };
                DeNSo.Session.New.Set(dhcp);
            }

            public static void DumpGlobalConfig() {
  
            }

            private static void CleanFile(string path) {
                File.WriteAllText(path, "");
            }

            private static void AppendLine(string path, string text) {
                File.AppendAllText(path, $"{text}{Environment.NewLine}");
            }

            public static void SaveShareConfig(string fileName, string name, string queryName, List<ServiceDhcp> newParameters) {
            }

            public static void DumpShare(string shareName) {
            }

            public static void AddParameterToGlobal(string key, string value) {
            }
        }
    }
}
