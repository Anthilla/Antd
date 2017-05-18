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
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;

namespace antdlib.config.Antd_SVCS.Dhcp {
    public class DhcpConfig {
        private const string ServiceGuid = "470782AC-4EB4-4964-8B55-F900DABAB59B";
        private const string Dir = "/etc/dhcp";
        private static readonly string MntDir = MountHelper.SetDirsPath(Dir);
        private const string MainFile = "dhcpd.conf";

        public static void SetReady() {
            Bash.Execute($"cp {Dir} {MntDir}");
            FileSystem.CopyDirectory(Dir, MntDir);
            MountManagement.Dir(Dir);
        }

        private static bool CheckIsActive() {
            var mount = MountManagement.GetAll().FirstOrDefault(_ => _.RepoDirsPath == MntDir);
            return mount != null;
        }

        public static bool IsActive => CheckIsActive();

        public static void ReloadConfig() {
            Bash.Execute("systemctl restart dhcpd4");
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
                public static string Key => "key";
                public static string Zone => "zone";
                public static string Subnet => "subnet";
                public static string Host => "host";
                public static string Class => "class";
                public static string Sublass => "subclass";
                public static string Failover => "failover";
                public static string Logging => "logging";
                public static string Include => "include";
                public static string Prefix6 => "prefix6";
                public static string Range6 => "range6";
                public static string Range => "range";
                public static string Subnet6 => "subnet6";
                public static string SharedNetwork => "shared-network";
                public static string Group => "group";
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
            public string FilePath { get; set; } = $"{MntDir}/{MainFile}";
            public string Name { get; set; }
            public string StringDefinition { get; set; } = "";
            public List<LineModel> Data { get; set; } = new List<LineModel>();
        }

        public class DhcpModel {
            public string Id { get; set; }
            public string Guid { get; set; }
            public string Timestamp { get; set; }
            public List<LineModel> DhcpGlobal { get; set; } = new List<LineModel>();
            public List<LineModel> DhcpInclude { get; set; } = new List<LineModel>();
            public List<OptionModel> DhcpKey { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpSubnet { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpHost { get; set; } = new List<OptionModel>();
            public List<LineModel> DhcpPrefix6 { get; set; } = new List<LineModel>();
            public List<LineModel> DhcpRange6 { get; set; } = new List<LineModel>();
            public List<LineModel> DhcpRange { get; set; } = new List<LineModel>();
            public List<OptionModel> DhcpSubnet6 { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpFailover { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpLogging { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpSharedNetwork { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpGroup { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpClass { get; set; } = new List<OptionModel>();
            public List<OptionModel> DhcpSubclass { get; set; } = new List<OptionModel>();
        }

        public class MapFile {
            public static void Render() {
                var path = $"{MntDir}/{MainFile}";
                var input = File.ReadAllText(path);
                var global = DhcpParser.AssignGlobal(input).ToList();
                var include = DhcpParser.AssignInclude(input).ToList();
                var key = DhcpParser.AssignKey(input).ToList();
                var subnet = DhcpParser.AssignSubnet(input).ToList();
                var host = DhcpParser.AssignHost(input).ToList();
                var prefix6 = DhcpParser.AssignPrefix6(input).ToList();
                var range6 = DhcpParser.AssignRange6(input).ToList();
                var range = DhcpParser.AssignRange(input).ToList();
                var subnet6 = DhcpParser.AssignClass(input).ToList();
                var failover = DhcpParser.AssignFailover(input).ToList();
                var logging = DhcpParser.AssignLogging(input).ToList();
                var sharedNetwork = DhcpParser.AssignSharedNetwork(input).ToList();
                var group = DhcpParser.AssignGroup(input).ToList();
                var @class = DhcpParser.AssignClass(input).ToList();
                var subclass = DhcpParser.AssignSubclass(input).ToList();
                var dhcp = new DhcpModel {
                    Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    DhcpGlobal = global,
                    DhcpInclude = include,
                    DhcpKey = key,
                    DhcpSubnet = subnet,
                    DhcpHost = host,
                    DhcpPrefix6 = prefix6,
                    DhcpRange6 = range6,
                    DhcpRange = range,
                    DhcpSubnet6 = subnet6,
                    DhcpFailover = failover,
                    DhcpLogging = logging,
                    DhcpSharedNetwork = sharedNetwork,
                    DhcpGroup = group,
                    DhcpClass = @class,
                    DhcpSubclass = subclass
                };
                throw new NotImplementedException();
            }

            public static DhcpModel Get() {
                throw new NotImplementedException();
                //var dhcp = DeNSo.Session.New.Get<DhcpModel>(s => s.Guid == ServiceGuid).FirstOrDefault();
                //return dhcp;
            }

            public static void AddGlobal(string key, string value) {
                var ob = new LineModel {
                    FilePath = $"{MntDir}/{MainFile}",
                    Key = key,
                    Value = value,
                    Type = Helper.ServiceData.SupposeDataType(value),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value)
                };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpGlobal.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddPrefix6(string key, string value) {
                var ob = new LineModel {
                    FilePath = $"{MntDir}/{MainFile}",
                    Key = key,
                    Value = value,
                    Type = Helper.ServiceData.SupposeDataType(value),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value)
                };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpPrefix6.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddRange6(string key, string value) {
                var ob = new LineModel {
                    FilePath = $"{MntDir}/{MainFile}",
                    Key = key,
                    Value = value,
                    Type = Helper.ServiceData.SupposeDataType(value),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value)
                };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpRange6.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddRange(string key, string value) {
                var ob = new LineModel {
                    FilePath = $"{MntDir}/{MainFile}",
                    Key = key,
                    Value = value,
                    Type = Helper.ServiceData.SupposeDataType(value),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value)
                };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpRange.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddKey(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpKey.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddSubnet(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpSubnet.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddClass(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpClass.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddHost(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpHost.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddSubclass(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpSubclass.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddSubnet6(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpSubnet6.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddFailover(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpFailover.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddLogging(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpLogging.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddGroup(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpGroup.Add(ob);
                throw new NotImplementedException();
            }

            public static void AddSharedNetwork(string name) {
                var ob = new OptionModel { Name = name };
                var dhcp = Get();
                dhcp.Timestamp = Timestamp.Now;
                dhcp.DhcpSharedNetwork.Add(ob);
                throw new NotImplementedException();
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceDhcp parameter) {
                var type = Helper.ServiceData.SupposeDataType(parameter.DataValue);
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

            public static void SaveConfigFor(string section, List<ServiceDhcp> newParameters) {
                var dhcp = MapFile.Get();
                dhcp.Timestamp = Timestamp.Now;
                var data = (from parameter in newParameters where parameter.DataKey.Length > 0 select ConvertData(parameter)).ToList();
                var options = new List<OptionModel>();
                var option = new OptionModel {
                    Name = section,
                    Data = data
                };
                options.Add(option);
                if(section == "key") { dhcp.DhcpKey = options; }
                else if(section == "subnet6") { dhcp.DhcpSubnet6 = options; }
                else if(section == "subnet") { dhcp.DhcpSubnet = options; }
                else if(section == "host") { dhcp.DhcpHost = options; }
                else if(section == "class") { dhcp.DhcpClass = options; }
                else if(section == "subclass") { dhcp.DhcpSubclass = options; }
                else if(section == "failover") { dhcp.DhcpFailover = options; }
                else if(section == "logging") { dhcp.DhcpLogging = options; }
                else if(section == "group") { dhcp.DhcpGroup = options; }
                else if(section == "shared-network") { dhcp.DhcpSharedNetwork = options; }
                throw new NotImplementedException();
            }

            public static void SaveGlobal(List<ServiceDhcp> newParameters) {
                var dhcp = MapFile.Get();
                dhcp.Timestamp = Timestamp.Now;
                var data = (from parameter in newParameters
                            where parameter.DataKey.Length > 0
                            select ConvertData(parameter)).ToList();
                dhcp.DhcpGlobal = data;
                throw new NotImplementedException();
            }

            public static void SavePrefix6(List<ServiceDhcp> newParameters) {
                var dhcp = MapFile.Get();
                dhcp.Timestamp = Timestamp.Now;
                var data = (from parameter in newParameters where parameter.DataKey.Length > 0 select ConvertData(parameter)).ToList();
                dhcp.DhcpPrefix6 = data;
                throw new NotImplementedException();
            }

            public static void SaveRange6(List<ServiceDhcp> newParameters) {
                var dhcp = MapFile.Get();
                dhcp.Timestamp = Timestamp.Now;
                var data = (from parameter in newParameters where parameter.DataKey.Length > 0 select ConvertData(parameter)).ToList();
                dhcp.DhcpRange6 = data;
                throw new NotImplementedException();
            }

            public static void SaveRange(List<ServiceDhcp> newParameters) {
                var dhcp = MapFile.Get();
                dhcp.Timestamp = Timestamp.Now;
                var data = (from parameter in newParameters where parameter.DataKey.Length > 0 select ConvertData(parameter)).ToList();
                dhcp.DhcpRange = data;
                throw new NotImplementedException();
            }

            public static void DumpGlobalConfig() {
                var filePath = $"{MntDir}/{MainFile}";
                var dhcp = MapFile.Get();
                CleanFile(filePath);

                WriteSimpleSection(filePath, dhcp.DhcpGlobal);
                WriteSimpleSection(filePath, dhcp.DhcpPrefix6);
                WriteSimpleSection(filePath, dhcp.DhcpRange);
                WriteSimpleSection(filePath, dhcp.DhcpRange6);

                foreach(var section in dhcp.DhcpKey) {
                    WriteMutipleSection("key", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpSubnet) {
                    WriteMutipleSection("subnet", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpSubnet6) {
                    WriteMutipleSection("subnet6", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpHost) {
                    WriteMutipleSection("host", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpClass) {
                    WriteMutipleSection("class", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpSubclass) {
                    WriteMutipleSection("subclass", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpFailover) {
                    WriteMutipleSection("failover", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpLogging) {
                    WriteMutipleSection("logging", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpGroup) {
                    WriteMutipleSection("group", section.Name, filePath, section.Data);
                }
                foreach(var section in dhcp.DhcpSharedNetwork) {
                    WriteMutipleSection("shared-network", section.Name, filePath, section.Data);
                }
            }

            private static void CleanFile(string path) {
                File.WriteAllText(path, "");
            }

            private static void WriteSimpleSection(string filePath, List<LineModel> lines) {
                var linesToAppend = new List<string>();
                foreach(var line in lines) {
                    if(line.Type == ServiceDataType.StringArray) {
                        linesToAppend.Add($"{line.Key} {{ {line.Value} }};");
                    }
                    else {
                        linesToAppend.Add(line.Value.Contains("/")
                            ? $"{line.Key} {line.Value};"
                            : $"{line.Key} \"{line.Value}\";");
                    }
                }
                File.AppendAllLines(filePath, linesToAppend);
            }

            private static void WriteMutipleSection(string section, string name, string filePath, IEnumerable<LineModel> lines) {
                var linesToAppend = new List<string>();
                var nametowrite = section == "zone" ? $" \"{name}\" " : $" {name} ";
                linesToAppend.Add($"{section}{nametowrite}{{");
                foreach(var line in lines) {
                    if(line.Type == ServiceDataType.StringArray) {
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
        }
    }
}
