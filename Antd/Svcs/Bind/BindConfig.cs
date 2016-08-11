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
using antdlib.ViewBinds;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;
using Antd.Database;
using Antd.MountPoint;
using Newtonsoft.Json;

namespace antdlib.Svcs.Bind {
    public class BindConfig {
        private const string ServiceGuid = "B9E929A8-D2AA-41EC-BDDC-0E5B05DF1D2F";
        private const string Dir = "/etc/bind";
        private static readonly string MntDir = Mounts.SetDirsPath(Dir);
        private const string MainFile = "named.conf";

        public static void SetReady() {
            Terminal.Execute($"cp {Dir} {MntDir}");
            FileSystem.CopyDirectory(Dir, MntDir);
            Mount.Dir(Dir);
        }

        private static bool CheckIsActive() {
            var mount = new MountRepository().GetByPath(Dir);
            return mount != null;
        }

        public static bool IsActive => CheckIsActive();

        public static void ReloadConfig() {
        }

        public class MapRules {
            public static char CharCommentConf => '#';
            public static string CharCommentConfAlt => "//";
            public static char CharCommentZone => ';';
            public static string VerbInclude => "include";
            public static string VerbIncludeZone => "zone";
            public static char CharOrigin => '$';
            public static string VerbOrigin => "$ORIGIN";
            public static char CharKevValueSeparator => ' ';
            public static char CharValueArraySeparator => ',';
            public static char CharEndOfLine => '\n';
            public static char CharSectionOpen => '[';
            public static char CharSectionClose => ']';
            public static char CharBlockOpen => '{';
            public static char CharBlockClose => '}';
            public static char CharZoneOpen => '(';
            public static char CharZoneClose => ')';
            public static char CharEndOfLineValue => ';';

            public class Statement {
                public static string Acl => "acl";
                public static string Controls => "controls";
                public static string Include => "include";
                public static string Key => "key";
                public static string Logging => "logging";
                public static string Lwres => "lwres";
                public static string Masters => "masters";
                public static string Options => "options";
                public static string Server => "server";
                public static string StatisticsChannels => "statistics-channels";
                public static string TrustedKeys => "trusted-keys";
                public static string ManagedKeys => "managed-keys";
                public static string View => "view";
                public static string Zone => "zone";
            }
        }

        private static readonly BindConfigRepository Repository = new BindConfigRepository();

        public class LineModel {
            public string FilePath { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public ServiceDataType Type { get; set; }
            public KeyValuePair<string, string> BooleanVerbs { get; set; }
        }

        public class OptionModel {
            public string FilePath { get; set; } = $"{MntDir}/{MainFile}";
            public string Name { get; set; }
            public string StringDefinition { get; set; } = "";
            public List<LineModel> Data { get; set; } = new List<LineModel>();
        }

        public class BindModel {
            public List<LineModel> BindAcl { get; set; } = new List<LineModel>();
            public List<OptionModel> BindControls { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindInclude { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindKey { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindLogging { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindLwres { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindMasters { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindOptions { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindServer { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindStatisticsChannels { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindTrustedKeys { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindManagedKeys { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindView { get; set; } = new List<OptionModel>();
            public List<OptionModel> BindZone { get; set; } = new List<OptionModel>();
        }

        public class MapFile {

            public static void Render() {
                var path = $"{MntDir}/{MainFile}";
                //var includes = new List<string>();
                var namedFileText = File.ReadAllText(path);

                var acl = BindStatement.AssignAcl(namedFileText).ToList();
                var controls = BindStatement.AssignControls(namedFileText).ToList();
                var include = BindStatement.AssignInclude(namedFileText).ToList();
                var key = BindStatement.AssignKey(namedFileText).ToList();
                var logging = BindStatement.AssignLogging(namedFileText).ToList();
                var lwres = BindStatement.AssignLwres(namedFileText).ToList();
                var masters = BindStatement.AssignMasters(namedFileText).ToList();
                var options = BindStatement.AssignOptions(namedFileText).ToList();
                var server = BindStatement.AssignServer(namedFileText).ToList();
                var statisticsChannels = BindStatement.AssignStatisticsChannels(namedFileText).ToList();
                var trustedKeys = BindStatement.AssignTrustedKeys(namedFileText).ToList();
                var managedKeys = BindStatement.AssignManagedKeys(namedFileText).ToList();
                var view = BindStatement.AssignView(namedFileText).ToList();
                var zones = BindStatement.AssignZone(namedFileText).ToList();

                var bind = new BindModel {
                    BindAcl = acl,
                    BindControls = controls,
                    BindInclude = include,
                    BindKey = key,
                    BindLogging = logging,
                    BindLwres = lwres,
                    BindMasters = masters,
                    BindOptions = options,
                    BindServer = server,
                    BindStatisticsChannels = statisticsChannels,
                    BindTrustedKeys = trustedKeys,
                    BindManagedKeys = managedKeys,
                    BindView = view,
                    BindZone = zones
                };
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static BindModel Get() {
                var dhcpCgf = Repository.GetByGuid(ServiceGuid);
                if (string.IsNullOrEmpty(dhcpCgf.Config)) {
                    return null;
                }
                var bind = JsonConvert.DeserializeObject<BindModel>(dhcpCgf.Config);
                return bind;
            }

            public static void AddAcl(string key, string value) {
                var acl = new LineModel {
                    Key = key,
                    Value = value
                };
                var bind = Get();
                bind.BindAcl.Add(acl);
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void AddKey(string name) {
                var key = new OptionModel { Name = name };
                var bind = Get();
                bind.BindKey.Add(key);
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void AddMasters(string name) {
                var masters = new OptionModel { Name = name };
                var bind = Get();
                bind.BindMasters.Add(masters);
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void AddServer(string name) {
                var server = new OptionModel { Name = name };
                var bind = Get();
                bind.BindServer.Add(server);
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void AddView(string name) {
                var view = new OptionModel { Name = name };
                var bind = Get();
                bind.BindView.Add(view);
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void AddZone(string name) {
                var zone = new OptionModel { Name = name };
                var bind = Get();
                bind.BindZone.Add(zone);
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceBind parameter) {
                ServiceDataType type = Helper.ServiceData.SupposeDataType(parameter.DataValue);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(parameter.DataValue);
                var data = new LineModel {
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
                var data = (from parameter in newParameters where parameter.DataKey.Length > 0 select ConvertData(parameter)).ToList();
                var options = new List<OptionModel>();
                var option = new OptionModel {
                    Name = section,
                    Data = data
                };
                options.Add(option);
                if (section == "controls") { bind.BindControls = options; }
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
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void SaveAcls(List<ServiceBind> newParameters) {
                var bind = MapFile.Get();
                var data = (from parameter in newParameters where parameter.DataKey.Length > 0 select ConvertData(parameter)).ToList();
                bind.BindAcl = data;
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void SaveZoneConfig(string zoneName, List<ServiceBind> newParameters) {
                var bind = MapFile.Get();
                var data = (from parameter in newParameters where parameter.DataKey.Length > 0 select ConvertData(parameter)).ToList();
                var options = bind.BindZone;
                var oldOption = options.FirstOrDefault(o => o.Name == zoneName);
                options.Remove(oldOption);
                var newOption = new OptionModel {
                    Name = zoneName,
                    Data = data
                };
                options.Add(newOption);
                bind.BindZone = options;
                Repository.Dump(ServiceGuid, JsonConvert.SerializeObject(bind));
            }

            public static void DumpGlobalConfig() {
                var filePath = $"{MntDir}/{MainFile}";
                var bind = MapFile.Get();
                CleanFile(filePath);

                foreach (var section in bind.BindOptions) {
                    WriteSimpleSection("options", filePath, section.Data);
                }
                WriteAcl(filePath, bind.BindAcl);
                foreach (var section in bind.BindControls) {
                    WriteMutipleSection("controls", section.Name, filePath, section.Data);
                }
                foreach (var section in bind.BindInclude) {
                    WriteMutipleSection("include", section.Name, filePath, section.Data);
                }
                foreach (var section in bind.BindKey) {
                    WriteMutipleSection("key", section.Name, filePath, section.Data);
                }
                foreach (var section in bind.BindLogging) {
                    WriteSimpleSection("logging", filePath, section.Data);
                }
                foreach (var section in bind.BindLwres) {
                    WriteSimpleSection("lwres", filePath, section.Data);
                }
                foreach (var section in bind.BindMasters) {
                    WriteMutipleSection("masters", section.Name, filePath, section.Data);
                }
                foreach (var section in bind.BindServer) {
                    WriteMutipleSection("server", section.Name, filePath, section.Data);
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
                    WriteMutipleSection("view", section.Name, filePath, section.Data);
                }
                foreach (var section in bind.BindZone) {
                    WriteMutipleSection("zone", section.Name, filePath, section.Data);
                }
            }

            private static void CleanFile(string path) {
                File.WriteAllText(path, "");
            }

            private static void WriteAcl(string filePath, IEnumerable<LineModel> lines) {
                var linesToAppend = new List<string>();
                foreach (var line in lines) {
                    linesToAppend.Add($"acl {line.Key} {{");
                    linesToAppend.Add($"{line.Value}");
                    linesToAppend.Add("};\n");
                }
                File.AppendAllLines(filePath, linesToAppend);
            }

            private static void WriteSimpleSection(string section, string filePath, IEnumerable<LineModel> lines) {
                var linesToAppend = new List<string> { $"{section} {{" };
                foreach (var line in lines) {
                    if (line.Type == ServiceDataType.StringArray) {
                        linesToAppend.Add($"{line.Key} {{ {line.Value} }};");
                    }
                    else {
                        linesToAppend.Add(line.Value.Contains("/")
                            ? $"{line.Key} {line.Value};"
                            : $"{line.Key} \"{line.Value}\";");
                    }
                }
                linesToAppend.Add("};\n");
                File.AppendAllLines(filePath, linesToAppend);
            }

            /// <summary>
            /// todo: include ha una sintassi a parte
            /// todo: includere nella stringa il NOME della sezione, e vedere se ci vogliono gli apici o no
            /// </summary>
            /// <param name="section"></param>
            /// <param name="name"></param>
            /// <param name="filePath"></param>
            /// <param name="lines"></param>
            private static void WriteMutipleSection(string section, string name, string filePath, IEnumerable<LineModel> lines) {
                var linesToAppend = new List<string>();
                var nametowrite = section == "zone" ? $" \"{name}\" " : $" {name} ";
                linesToAppend.Add($"{section}{nametowrite}{{");
                foreach (var line in lines) {
                    if (line.Type == ServiceDataType.StringArray) {
                        linesToAppend.Add($"{line.Key} {{ {line.Value} }};");
                    }
                    else if (line.Type == ServiceDataType.DataArray) {
                        linesToAppend.Add($"{line.Key} {{ {line.Value.Replace(",", ";")} }};");
                    }
                    else {
                        linesToAppend.Add(line.Value.Contains("/")
                            ? $"{line.Key} {line.Value};"
                            : $"{line.Key} \"{line.Value}\";");
                    }
                }
                linesToAppend.Add("};\n");
                File.AppendAllLines(filePath, linesToAppend);
            }
        }
    }
}