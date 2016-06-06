
using antdlib.Common;
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
using antdlib.MountPoint;
using antdlib.ViewBinds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.Svcs.Systemd.Networkd {
    public class NetworkdConfig {

        private static string serviceGuid = "61733230-B5E2-47CA-AA86-A371B53F76C1";

        private static string dir = "/etc/networkd";

        private static string DIR = Mount.SetDirsPath(dir);

        private static string mainFile = "smb.conf";

        private static string antdNetworkdFile = "antd.networkd.conf";

        public static void SetReady() {
            Terminal.Terminal.Execute($"cp {dir} {DIR}");
            FileSystem.CopyDirectory(dir, DIR);
            Mount.Dir(dir);
        }

        private static bool CheckIsActive() {
            var mount = MountRepository.Get(dir);
            return mount == null ? false : true;
        }

        public static bool IsActive { get { return CheckIsActive(); } }

        public static void ReloadConfig() {
            Terminal.Terminal.Execute($"smbcontrol all reload-config");
        }

        private static List<KeyValuePair<string, List<string>>> GetServiceStructure() {
            var list = new List<KeyValuePair<string, List<string>>>() { };
            var files = Directory.EnumerateFiles(DIR, "*.conf", SearchOption.AllDirectories).ToArray();
            for (var i = 0; i < files.Length; i++) {
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
            for (var i = 0; i < files.Length; i++) {
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
            public static char CharComment { get { return ';'; } }

            public static string VerbInclude { get { return "include"; } }

            public static char CharKevValueSeparator { get { return '='; } }

            public static char CharValueArraySeparator { get { return ','; } }

            public static char CharEndOfLine { get { return '\n'; } }

            public static char CharSectionOpen { get { return '['; } }

            public static char CharSectionClose { get { return ']'; } }
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

            public List<LineModel> Data { get; set; } = new List<LineModel>() { };
        }

        public class NetworkdModel {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public string Timestamp { get; set; }

            public List<LineModel> Data { get; set; } = new List<LineModel>() { };

            public List<ShareModel> Share { get; set; } = new List<ShareModel>() { };
        }

        public class MapFile {

            private static string CleanLine(string line) {
                var removeTab = line.Replace("\t", " ");
                var clean = removeTab;
                if (removeTab.Contains(MapRules.CharComment) && !line.StartsWith(MapRules.CharComment.ToString())) {
                    var splitAtComment = removeTab.Split(MapRules.CharComment);
                    clean = splitAtComment[0].Trim();
                }
                return clean;
            }

            private static IEnumerable<LineModel> ReadFile(string path) {
                var text = FileSystem.ReadFile(path);
                var lines = text.Split(MapRules.CharEndOfLine);
                var list = new List<LineModel>() { };
                foreach (var line in lines) {
                    if (line != "" && !line.StartsWith("include")) {
                        var cleanLine = CleanLine(line);
                        list.Add(ReadLine(path, cleanLine));
                    }
                }
                return list;
            }

            private static ShareModel ReadFileShare(string path) {
                var shareName = GetShareName(path) == null ? "" : GetShareName(path);
                var model = new ShareModel() {
                    FilePath = path,
                    Name = shareName
                };
                foreach (var data in ReadFile(path)) {
                    model.Data.Add(data);
                }
                return model;
            }

            private static string GetShareName(string path) {
                var text = FileSystem.ReadFile(path);
                return text.SplitAndGetTextBetween(MapRules.CharSectionOpen, MapRules.CharSectionClose).FirstOrDefault();
            }

            private static LineModel ReadLine(string path, string line) {
                var keyValuePair = line.Split(new String[] { MapRules.CharKevValueSeparator.ToString() }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                ServiceDataType type;
                var key = keyValuePair.Length > 0 ? keyValuePair[0] : "";
                var value = "";
                if (line.StartsWith(MapRules.CharComment.ToString())) {
                    type = ServiceDataType.Disabled;
                }
                else if (line.StartsWith(MapRules.CharSectionOpen.ToString())) {
                    type = ServiceDataType.Disabled;
                }
                else {
                    value = keyValuePair.Length > 1 ? keyValuePair[1] : "";
                    type = Helper.ServiceData.SupposeDataType(value.Trim());
                }
                KeyValuePair<string, string> booleanVerbs;
                if (type == ServiceDataType.Boolean) {
                    booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value.Trim());
                }
                else {
                    booleanVerbs = new KeyValuePair<string, string>("", "");
                }
                var model = new LineModel() {
                    FilePath = path,
                    Key = key.Trim(),
                    Value = value.Trim(),
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                return model;
            }

            public static void Render() {
                var shares = new List<ShareModel>() { };
                var data = new List<LineModel>() { };
                foreach (var file in SimpleStructure) {
                    if (file.Contains("/share/")) {
                        shares.Add(ReadFileShare(file));
                    }
                    else {
                        var lines = ReadFile(file);
                        foreach (var line in lines) {
                            data.Add(line);
                        }
                    }
                }
                var networkd = new NetworkdModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(networkd);
            }

            public static NetworkdModel Get() {
                var networkd = DeNSo.Session.New.Get<NetworkdModel>(s => s.Guid == serviceGuid).FirstOrDefault();
                return networkd;
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceNetworkd parameter) {
                var type = Helper.ServiceData.SupposeDataType(parameter.DataValue);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(parameter.DataValue);
                var data = new LineModel() {
                    FilePath = parameter.DataFilePath,
                    Key = parameter.DataKey,
                    Value = parameter.DataValue,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                return data;
            }

            public static void SaveGlobalConfig(List<ServiceNetworkd> newParameters) {
                var shares = MapFile.Get().Share;
                var data = new List<LineModel>() { };
                foreach (var parameter in newParameters) {
                    data.Add(ConvertData(parameter));
                }
                var networkd = new NetworkdModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(networkd);
            }

            public static void DumpGlobalConfig() {
                var parameters = MapFile.Get().Data.ToArray();
                var filesToClean = parameters.Select(p => p.FilePath).ToDynamicHashSet();
                foreach (var file in filesToClean) {
                    CleanFile(file);
                }
                for (var i = 0; i < parameters.Length; i++) {
                    var line = $"{parameters[i].Key} {MapRules.CharKevValueSeparator} {parameters[i].Value}";
                    AppendLine(parameters[i].FilePath, line);
                }
            }

            private static void CleanFile(string path) {
                File.WriteAllText(path, "");
            }

            private static void AppendLine(string path, string text) {
                File.AppendAllText(path, $"{text}{Environment.NewLine}");
            }

            public static void SaveShareConfig(string fileName, string name, string queryName, List<ServiceNetworkd> newParameters) {
                var data = MapFile.Get().Data;
                var shares = MapFile.Get().Share;
                var oldShare = shares.Where(o => o.Name == queryName).FirstOrDefault();
                shares.Remove(oldShare);
                var shareData = new List<LineModel>() { };
                foreach (var parameter in newParameters) {
                    shareData.Add(ConvertData(parameter));
                }
                var newShare = new ShareModel() {
                    FilePath = fileName,
                    Name = name,
                    Data = shareData
                };
                shares.Add(newShare);
                var networkd = new NetworkdModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(networkd);
            }

            public static void DumpShare(string shareName) {
                var share = MapFile.Get().Share.Where(s => s.Name == shareName).FirstOrDefault();
                var parameters = share.Data.ToArray();
                var file = share.FilePath;
                CleanFile(file);
                AppendLine(file, $"{MapRules.CharSectionOpen}{share.Name}{MapRules.CharSectionClose}");
                for (var i = 0; i < parameters.Length; i++) {
                    var line = $"{parameters[i].Key} {MapRules.CharKevValueSeparator} {parameters[i].Value}";
                    AppendLine(parameters[i].FilePath, line);
                }
            }

            public static void AddParameterToGlobal(string key, string value) {
                SetCustomFile();
                var type = Helper.ServiceData.SupposeDataType(value);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value);
                var line = new LineModel() {
                    FilePath = $"{DIR}/{antdNetworkdFile}",
                    Key = key,
                    Value = value,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                var shares = MapFile.Get().Share;
                var data = MapFile.Get().Data;
                data.Add(line);
                var networkd = new NetworkdModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(networkd);
            }

            private static void SetCustomFile() {
                var path = $"{DIR}/{antdNetworkdFile}";
                if (!File.Exists(path)) {
                    File.Create(path);
                }
            }

            public static void RewriteSMBCONF() {
                var file = $"{DIR}/{mainFile}";
                CleanFile(file);
                AppendLine(file, "[global]");
                AppendLine(file, "");
                AppendLine(file, $"{MapRules.CharComment}GLOBAL START");
                foreach (var path in GetGlobalPaths()) {
                    AppendLine(file, $"{MapRules.VerbInclude} {MapRules.CharKevValueSeparator} {path}");
                }
                AppendLine(file, $"{MapRules.CharComment}GLOBAL END");
                AppendLine(file, "");
                AppendLine(file, $"{MapRules.CharComment}SHARE START");
                foreach (var path in GetSharePaths()) {
                    AppendLine(file, $"{MapRules.VerbInclude} {MapRules.CharKevValueSeparator} {path}");
                }
                AppendLine(file, $"{MapRules.CharComment}SHARE END");
            }

            private static HashSet<dynamic> GetGlobalPaths() {
                var share = MapFile.Get().Data.Select(s => s.FilePath).ToDynamicHashSet();
                return share;
            }

            private static HashSet<dynamic> GetSharePaths() {
                var share = MapFile.Get().Share.Select(s => s.FilePath).ToDynamicHashSet();
                return share;
            }

            public static void AddShare(string name, string directory) {
                SetShareFile(name);
                var shareData = new List<LineModel>() { };
                var defaultParameter00 = new LineModel() {
                    FilePath = $"{DIR}/share/{name.Replace($"", "_")}.conf",
                    Key = "path",
                    Value = directory,
                    Type = ServiceDataType.String,
                    BooleanVerbs = new KeyValuePair<string, string>("", "") 
                };
                shareData.Add(defaultParameter00);
                var defaultParameter01 = new LineModel() {
                    FilePath = $"{DIR}/share/{name.Replace($"", "_")}.conf",
                    Key = "browseable",
                    Value = "yes",
                    Type = ServiceDataType.Boolean,
                    BooleanVerbs = new KeyValuePair<string, string>("yes", "no")
                };
                shareData.Add(defaultParameter01);
                var sh = new ShareModel() {
                    FilePath = $"{DIR}/share/{name.Replace($"", "_")}.conf",
                    Name = name,
                    Data = shareData
                };
                var shares = MapFile.Get().Share;
                var data = MapFile.Get().Data;
                shares.Add(sh);
                var networkd = new NetworkdModel() {
                    _Id = serviceGuid,
                    Guid = serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(networkd);
            }

            private static void SetShareFile(string shareName) {
                var sharePath = $"{DIR}/share/{shareName.Replace($"", "_")}.conf";
                if (!File.Exists(sharePath)) {
                    File.Create(sharePath);
                }
            }
        }
    }
}
