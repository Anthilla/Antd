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

namespace antdlib.config.Antd_SVCS.Samba {
    public class SambaConfig {

        public class SmartcardConfig {
            public class SambaDomainConfig {
                public string _Id => ServiceGuid;
                public string Domain { get; set; }
                public string HostName { get; set; }
                public string HostIp { get; set; }
                public string AdminPassword { get; set; }
                public string Realm { get; set; }
            }

            public static void ConfigDomain(string domain, string hostName, string hostIp, string adminPassword, string realm) {
                var configModel = new SambaDomainConfig {
                    Domain = domain,
                    Realm = realm,
                    HostName = hostName,
                    HostIp = hostIp,
                    AdminPassword = adminPassword
                };
                //DeNSo.Session.New.Set(configModel);
                //var model = DeNSo.Session.New.Get<SambaDomainConfig>(_ => _._Id == ServiceGuid).FirstOrDefault();
                //if(model != null) {
                    //Bash.Execute($"samba-tool domain provision --domain={model.Domain} --realm={model.Realm} --host-name={model.HostName} --host-ip={model.HostIp} --adminpass={model.AdminPassword} --dns-backend=SAMBA_INTERNAL --server-role=dc");
                //}
            }
        }

        private const string ServiceGuid = "61ED9A36-0D50-4BAE-9434-96E3078657FF";
        private const string Dir = "/etc/samba";
        private static readonly string MntDir = MountHelper.SetDirsPath(Dir);
        private const string MainFile = "smb.conf";
        private const string AntdSambaFile = "antd.samba.conf";

        public static void SetReady() {
            Bash.Execute($"cp {Dir} {MntDir}");
            FileSystem.CopyDirectory(Dir, MntDir);
            MountManagement.Dir(Dir);
        }

        private static bool CheckIsActive() => MountManagement.GetAll().FirstOrDefault(_ => _.RepoDirsPath == MntDir) != null;

        public static bool IsActive => CheckIsActive();

        public static void ReloadConfig() {
            Bash.Execute("smbcontrol all reload-config");
        }

        private static List<string> GetServiceSimpleStructure() {
            if(!Parameter.IsUnix)
                return new List<string>();
            var list = new List<string>();
            var files = Directory.EnumerateFiles(MntDir, "*.conf", SearchOption.AllDirectories).Where(file => File.ReadAllText(file).Contains("include"));
            foreach(var lines in files.Select(file => File.ReadLines(file).Where(line => line.Contains("include")).ToList())) {
                list.AddRange(lines.Select(line => line.Split('=')[1].Trim().Replace(Dir, MntDir)));
            }
            if(!list.Any()) {
                list.Add($"{MntDir}/{MainFile}");
            }
            return list;
        }

        public static List<string> SimpleStructure => GetServiceSimpleStructure();

        public class MapRules {
            public static char CharComment => ';';
            public static string VerbInclude => "include";
            public static char CharKevValueSeparator => '=';
            public static char CharValueArraySeparator => ',';
            public static char CharEndOfLine => '\n';
            public static char CharSectionOpen => '[';
            public static char CharSectionClose => ']';
        }

        public class LineModel {
            public string FilePath { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public ServiceDataType Type { get; set; }
            public KeyValuePair<string, string> BooleanVerbs { get; set; }
        }

        public class ShareModel {
            public string FilePath { get; set; }
            public string Name { get; set; }
            public List<LineModel> Data { get; set; } = new List<LineModel>();
        }

        public class SambaModel {
            public string _Id { get; set; }
            public string Guid { get; set; }
            public string Timestamp { get; set; }
            public List<LineModel> Data { get; set; } = new List<LineModel>();
            public List<ShareModel> Share { get; set; } = new List<ShareModel>();
        }

        public class MapFile {

            private static string CleanLine(string line) {
                var removeTab = line.Replace("\t", " ");
                var clean = removeTab;
                if(!removeTab.Contains(MapRules.CharComment) || line.StartsWith(MapRules.CharComment.ToString()))
                    return clean;
                var splitAtComment = removeTab.Split(MapRules.CharComment);
                clean = splitAtComment[0].Trim();
                return clean;
            }

            private static IEnumerable<LineModel> ReadFile(string path) {
                var text = FileSystem.ReadFile(path);
                var lines = text.Split(MapRules.CharEndOfLine).Where(_ => _ != "" && !_.StartsWith("include"));
                return lines.Select(CleanLine).Select(cleanLine => ReadLine(path, cleanLine)).ToList();
            }

            private static ShareModel ReadFileShare(string path) {
                var shareName = GetShareName(path) ?? "";
                var model = new ShareModel {
                    FilePath = path,
                    Name = shareName
                };
                foreach(var data in ReadFile(path)) {
                    model.Data.Add(data);
                }
                return model;
            }

            private static string GetShareName(string path) {
                var text = FileSystem.ReadFile(path);
                return text.SplitAndGetTextBetween(MapRules.CharSectionOpen, MapRules.CharSectionClose).FirstOrDefault();
            }

            private static LineModel ReadLine(string path, string line) {
                var keyValuePair = line.Split(new[] { MapRules.CharKevValueSeparator.ToString() }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                ServiceDataType type;
                var key = keyValuePair.Length > 0 ? keyValuePair[0] : "";
                var value = "";
                if(line.StartsWith(MapRules.CharComment.ToString())) {
                    type = ServiceDataType.Disabled;
                }
                else if(line.StartsWith(MapRules.CharSectionOpen.ToString())) {
                    type = ServiceDataType.Disabled;
                }
                else {
                    value = keyValuePair.Length > 1 ? keyValuePair[1] : "";
                    type = Helper.ServiceData.SupposeDataType(value.Trim());
                }
                var booleanVerbs = type == ServiceDataType.Boolean ? Helper.ServiceData.SupposeBooleanVerbs(value.Trim()) : new KeyValuePair<string, string>("", "");
                var model = new LineModel {
                    FilePath = path,
                    Key = key.Trim(),
                    Value = value.Trim(),
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                return model;
            }

            public static void Render() {
                var shares = new List<ShareModel>();
                var data = new List<LineModel>();
                foreach(var file in SimpleStructure) {
                    if(file.Contains("/share/")) {
                        shares.Add(ReadFileShare(file));
                    }
                    else {
                        var lines = ReadFile(file);
                        data.AddRange(lines);
                    }
                }
                var samba = new SambaModel {
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                //DeNSo.Session.New.Set(samba);
            }

            public static SambaModel Get() {
                //var samba = DeNSo.Session.New.Get<SambaModel>(s => s.Guid == ServiceGuid).FirstOrDefault();
                //return samba;
                return null;
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceSamba parameter) {
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

            public static void SaveGlobalConfig(List<ServiceSamba> newParameters) {
                var shares = MapFile.Get().Share;
                var data = newParameters.Select(ConvertData).ToList();
                var samba = new SambaModel {
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                //DeNSo.Session.New.Set(samba);
            }

            public static void DumpGlobalConfig() {
                var parameters = MapFile.Get().Data.ToArray();
                var filesToClean = parameters.Select(p => p.FilePath)/*.ToDynamicHashSet()*/;
                foreach(var file in filesToClean) {
                    CleanFile(file);
                }
                foreach(var t in parameters) {
                    var line = $"{t.Key} {MapRules.CharKevValueSeparator} {t.Value}";
                    AppendLine(t.FilePath, line);
                }
            }

            private static void CleanFile(string path) {
                File.WriteAllText(path, "");
            }

            private static void AppendLine(string path, string text) {
                File.AppendAllText(path, $"{text}{Environment.NewLine}");
            }

            public static void SaveShareConfig(string fileName, string name, string queryName, List<ServiceSamba> newParameters) {
                var data = MapFile.Get().Data;
                var shares = MapFile.Get().Share;
                var oldShare = shares.FirstOrDefault(o => o.Name == queryName);
                shares.Remove(oldShare);
                var shareData = newParameters.Select(ConvertData).ToList();
                var newShare = new ShareModel {
                    FilePath = fileName,
                    Name = name,
                    Data = shareData
                };
                shares.Add(newShare);
                var samba = new SambaModel {
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                //DeNSo.Session.New.Set(samba);
            }

            public static void DumpShare(string shareName) {
                var share = MapFile.Get().Share.FirstOrDefault(s => s.Name == shareName);
                if(share == null)
                    return;
                var parameters = share.Data.ToArray();
                var file = share.FilePath;
                CleanFile(file);
                AppendLine(file, $"{MapRules.CharSectionOpen}{share.Name}{MapRules.CharSectionClose}");
                foreach(var t in parameters) {
                    var line = $"{t.Key} {MapRules.CharKevValueSeparator} {t.Value}";
                    AppendLine(t.FilePath, line);
                }
            }

            public static void AddParameterToGlobal(string key, string value) {
                SetCustomFile();
                var type = Helper.ServiceData.SupposeDataType(value);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value);
                var line = new LineModel {
                    FilePath = $"{MntDir}/{AntdSambaFile}",
                    Key = key,
                    Value = value,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                var shares = MapFile.Get().Share;
                var data = MapFile.Get().Data;
                data.Add(line);
                var samba = new SambaModel {
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                //DeNSo.Session.New.Set(samba);
            }

            private static void SetCustomFile() {
                var path = $"{MntDir}/{AntdSambaFile}";
                if(!File.Exists(path)) {
                    File.Create(path);
                }
            }

            public static void RewriteSmbconf() {
                var file = $"{MntDir}/{MainFile}";
                CleanFile(file);
                AppendLine(file, "[global]");
                AppendLine(file, "");
                AppendLine(file, $"{MapRules.CharComment}GLOBAL START");
                foreach(var path in GetGlobalPaths()) {
                    AppendLine(file, $"{MapRules.VerbInclude} {MapRules.CharKevValueSeparator} {path}");
                }
                AppendLine(file, $"{MapRules.CharComment}GLOBAL END");
                AppendLine(file, "");
                AppendLine(file, $"{MapRules.CharComment}SHARE START");
                foreach(var path in GetSharePaths()) {
                    AppendLine(file, $"{MapRules.VerbInclude} {MapRules.CharKevValueSeparator} {path}");
                }
                AppendLine(file, $"{MapRules.CharComment}SHARE END");
            }

            private static IEnumerable<dynamic> GetGlobalPaths() {
                var share = MapFile.Get().Data.Select(s => s.FilePath)/*.ToDynamicHashSet()*/;
                return share;
            }

            private static IEnumerable<dynamic> GetSharePaths() {
                var share = MapFile.Get().Share.Select(s => s.FilePath)/*.ToDynamicHashSet()*/;
                return share;
            }

            public static void AddShare(string name, string directory) {
                SetShareFile(name);
                var shareData = new List<LineModel>();
                var defaultParameter00 = new LineModel {
                    FilePath = $"{MntDir}/share/{name.Replace("", "_")}.conf",
                    Key = "path",
                    Value = directory,
                    Type = ServiceDataType.String,
                    BooleanVerbs = new KeyValuePair<string, string>("", "")
                };
                shareData.Add(defaultParameter00);
                var defaultParameter01 = new LineModel {
                    FilePath = $"{MntDir}/share/{name.Replace("", "_")}.conf",
                    Key = "browseable",
                    Value = "yes",
                    //Type = ServiceDataType.Boolean,
                    BooleanVerbs = new KeyValuePair<string, string>("yes", "no")
                };
                shareData.Add(defaultParameter01);
                var sh = new ShareModel {
                    FilePath = $"{MntDir}/share/{name.Replace("", "_")}.conf",
                    Name = name,
                    Data = shareData
                };
                var shares = MapFile.Get().Share;
                var data = MapFile.Get().Data;
                shares.Add(sh);
                var samba = new SambaModel {
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                //DeNSo.Session.New.Set(samba);
            }

            private static void SetShareFile(string shareName) {
                var sharePath = $"{MntDir}/share/{shareName.Replace("", "_")}.conf";
                if(!File.Exists(sharePath)) {
                    File.Create(sharePath);
                }
            }
        }
    }
}
