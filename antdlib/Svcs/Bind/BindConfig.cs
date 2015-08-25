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
using System.Text.RegularExpressions;

namespace antdlib.Svcs.Bind {
    public class BindConfig {

        private static string serviceGuid = "B9E929A8-D2AA-41EC-BDDC-0E5B05DF1D2F";

        private static string dir = "/etc/bind";

        private static string DIR = Mount.SetDIRSPath(dir);

        private static string dyn = "/etc/bind/dyn";

        private static string DYN = Mount.SetDIRSPath(dyn);

        private static string master = "/etc/bind/master";

        private static string MASTER = Mount.SetDIRSPath(master);

        private static string mainFile = "named.conf";

        private static string mainFileRndc = "rndc.conf";

        private static string antdBindFile = "antd.bind.conf";

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
        /// todo cambiare comando
        /// </summary>
        public static void ReloadConfig() {
            Terminal.Execute($"");
        }

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
                public static string Acl { get { return "acl"; } }

                public static string Controls { get { return "controls"; } }

                public static string Include { get { return "include"; } }

                public static string Key { get { return "key"; } }

                public static string Logging { get { return "logging"; } }

                public static string Lwres { get { return "lwres"; } }

                public static string Masters { get { return "masters"; } }

                public static string Options { get { return "options"; } }

                public static string Server { get { return "server"; } }

                public static string StatisticsChannels { get { return "statistics-channels"; } }

                public static string TrustedKeys { get { return "trusted-keys"; } }

                public static string ManagedKeys { get { return "managed-keys"; } }

                public static string View { get { return "view"; } }

                public static string Zone { get { return "zone"; } }
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

        public class BindModel {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public string Timestamp { get; set; }

            public List<OptionModel> BindAcl { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindControls { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindInclude { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindKey { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindLogging { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindLwres { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindMasters { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindOptions { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindServer { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindStatisticsChannels { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindTrustedKeys { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindManagedKeys { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindView { get; set; } = new List<OptionModel>() { };

            public List<OptionModel> BindZone { get; set; } = new List<OptionModel>() { };

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

            private static void ReformatFile(string path) {
                var text = File.ReadAllText(path).Replace("\t", " ").Replace("\r", "").Replace("\n", "").Replace("  ", " ").Replace("{", "{\n").Replace(";", ";\n").Replace("};", "};\n").Replace("} ", "}\n");
                File.WriteAllText(path, "");
                File.WriteAllText(path, text);
            }

            //private static IEnumerable<LineModel> ReadLines(string path, string text) {
            //    var list = new List<LineModel>();
            //    var splitLines = text.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            //    for (int i = 0; i < splitLines.Length; i++) {
            //        var keyValuePair = splitLines[i].Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            //        ServiceDataType type;
            //        var key = (keyValuePair.Length > 0) ? keyValuePair[0] : "";
            //        var value = "";
            //        if (splitLines[i].StartsWith(MapRules.CharCommentConf.ToString())) {
            //            type = ServiceDataType.Disabled;
            //        }
            //        else if (splitLines[i].StartsWith(MapRules.CharSectionOpen.ToString())) {
            //            type = ServiceDataType.Disabled;
            //        }
            //        else {
            //            value = (keyValuePair.Length > 1) ? keyValuePair[1] : "";
            //            type = SupposeDataType(value.Trim());
            //        }
            //        KeyValuePair<string, string> booleanVerbs;
            //        if (type == ServiceDataType.Boolean) {
            //            booleanVerbs = SupposeBooleanVerbs(value.Trim());
            //        }
            //        else {
            //            booleanVerbs = new KeyValuePair<string, string>("", "");
            //        }
            //        var model = new LineModel() {
            //            FilePath = path,
            //            Key = key.Trim(),
            //            Value = value.Trim(),
            //            Type = type,
            //            BooleanVerbs = booleanVerbs
            //        };
            //        list.Add(model);
            //    }
            //    return list;
            ////}

            public static void Render() {
                var path = $"{DIR}/{mainFile}";
                var includes = new List<string>() { };
                //ReformatFile(path);
                var namedFileText = File.ReadAllText(path);

                //var acl = BindStatement.AssignAcl(namedFileText).ToList();
                var controls = BindStatement.AssignControls(namedFileText).ToList();
                //var include = BindStatement.AssignInclude(namedFileText).ToList();
                //var key = BindStatement.AssignKey(namedFileText).ToList();
                var logging = BindStatement.AssignLogging(namedFileText).ToList();
                var lwres = BindStatement.AssignLwres(namedFileText).ToList();
                //var masters = BindStatement.AssignMasters(namedFileText).ToList();
                var options = BindStatement.AssignOptions(namedFileText).ToList();
                //var server = BindStatement.AssignServer(namedFileText).ToList();
                var statisticsChannels = BindStatement.AssignStatisticsChannels(namedFileText).ToList();
                var trustedKeys = BindStatement.AssignTrustedKeys(namedFileText).ToList();
                var managedKeys = BindStatement.AssignManagedKeys(namedFileText).ToList();
                //var view = BindStatement.AssignView(namedFileText).ToList();
                //var zones = BindStatement.AssignZone(namedFileText).ToList();

                var bind = new BindModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    //BindAcl = acl,
                    BindControls = controls,
                    //BindInclude = include,
                    //BindKey = key,
                    BindLogging = logging,
                    BindLwres = lwres,
                    //BindMasters = masters,
                    BindOptions = options,
                    //BindServer = server,
                    BindStatisticsChannels = statisticsChannels,
                    BindTrustedKeys = trustedKeys,
                    BindManagedKeys = managedKeys,
                    //BindView = view,
                    //BindZone = zones
                };
                DeNSo.Session.New.Set(bind);
            }

            public static BindModel Get() {
                var bind = DeNSo.Session.New.Get<BindModel>(s => s.Guid == serviceGuid).FirstOrDefault();
                return bind;
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceBind parameter) {
                ServiceDataType type = BindStatement.SupposeDataType(parameter.DataValue);
                var booleanVerbs = BindStatement.SupposeBooleanVerbs(parameter.DataValue);
                var data = new LineModel() {
                    FilePath = parameter.DataFilePath,
                    Key = parameter.DataKey,
                    Value = parameter.DataValue,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                return data;
            }

            public static void SaveGlobalConfig(string section, List<ServiceBind> newParameters) {
                var bind = MapFile.Get();
                bind.Timestamp = Timestamp.Now;
                var data = new List<LineModel>() { };
                foreach (var parameter in newParameters) {
                    data.Add(ConvertData(parameter));
                }
                var options = new List<OptionModel>() { };
                var option = new OptionModel() {
                    Name = section,
                    Data = data
                };
                options.Add(option);
                if (section == "acl") { bind.BindAcl = options; }
                else if (section == "controls") { bind.BindControls = options; }
                else if (section == "include") { bind.BindInclude = options; }
                else if (section == "key") { bind.BindKey = options; }
                else if (section == "logging") { bind.BindLogging = options; }
                else if (section == "lwres") { bind.BindLwres = options; }
                else if (section == "masters") { bind.BindMasters = options; }
                else if (section == "options") { bind.BindOptions = options; }
                else if (section == "server") { bind.BindServer = options; }
                else if (section == "statistics-channels") { bind.BindStatisticsChannels = options; }
                else if (section == "trusted-keys") { bind.BindTrustedKeys = options; }
                else if (section == "managed-leys") { bind.BindManagedKeys = options; }
                else if (section == "view") { bind.BindView = options; }
                else if (section == "zones") { bind.BindZone = options; }
                DeNSo.Session.New.Set(bind);
            }

            /// <summary>
            /// todo, write file from model :D
            /// </summary>
            public static void DumpGlobalConfig() {
                var filePath = $"{DIR}/{mainFile}";
                var bind = MapFile.Get();
                CleanFile(filePath);

                foreach (var section in bind.BindOptions) {
                    WriteSimpleSection("options", filePath, section.Data);
                }
                foreach (var section in bind.BindAcl) {
                    WriteMutipleSection("acl", filePath, section.Data);
                }
                foreach (var section in bind.BindControls) {
                    WriteMutipleSection("controls", filePath, section.Data);
                }
                foreach (var section in bind.BindInclude) {
                    WriteMutipleSection("include", filePath, section.Data);
                }
                foreach (var section in bind.BindKey) {
                    WriteMutipleSection("key", filePath, section.Data);
                }
                foreach (var section in bind.BindLogging) {
                    WriteSimpleSection("logging", filePath, section.Data);
                }
                foreach (var section in bind.BindLwres) {
                    WriteSimpleSection("lwres", filePath, section.Data);
                }
                foreach (var section in bind.BindMasters) {
                    WriteMutipleSection("masters", filePath, section.Data);
                }
                foreach (var section in bind.BindServer) {
                    WriteMutipleSection("server", filePath, section.Data);
                }
                foreach (var section in bind.BindStatisticsChannels) {
                    WriteSimpleSection("statistics-channel", filePath, section.Data);
                }
                foreach (var section in bind.BindTrustedKeys) {
                    WriteSimpleSection("trusted-keys", filePath, section.Data);
                }
                foreach (var section in bind.BindManagedKeys) {
                    WriteSimpleSection("managed-keys", filePath, section.Data);
                }
                foreach (var section in bind.BindView) {
                    WriteMutipleSection("view", filePath, section.Data);
                }
                foreach (var section in bind.BindZone) {
                    WriteMutipleSection("zone", filePath, section.Data);
                }
            }

            private static void CleanFile(string path) {
                File.WriteAllText(path, "");
            }

            private static void WriteSimpleSection(string sectionName, string filePath, List<LineModel> lines) {
                var linesToAppend = new List<string>() { };
                linesToAppend.Add($"{sectionName} {{");
                foreach (var line in lines) {
                    if (line.Type == ServiceDataType.StringArray) {
                        linesToAppend.Add($"{line.Key} {{ {line.Value} }};");
                    }
                    else {
                        if (line.Value.Contains("/")) {
                            linesToAppend.Add($"{line.Key} {line.Value};");
                        }
                        else {
                            linesToAppend.Add($"{line.Key} \"{line.Value}\";");
                        }
                    }
                }
                linesToAppend.Add("};\n");
                File.AppendAllLines(filePath, linesToAppend);
            }

            /// <summary>
            /// todo: include ha una sintassi a parte
            /// todo: includere nella stringa il NOME della sezione, e vedere se ci vogliono gli apici o no
            /// </summary>
            /// <param name="sectionName"></param>
            /// <param name="filePath"></param>
            /// <param name="lines"></param>
            private static void WriteMutipleSection(string sectionName, string filePath, List<LineModel> lines) {
                var linesToAppend = new List<string>() { };
                linesToAppend.Add($"{sectionName} {{");

                File.AppendAllLines(filePath, linesToAppend);
            }
        }
    }
}
