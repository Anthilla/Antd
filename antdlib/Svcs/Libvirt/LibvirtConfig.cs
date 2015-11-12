
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

namespace antdlib.Svcs.Libvirt {
    public class LibvirtConfig {

        private static string _serviceGuid = "0F7E663B-7445-492D-B674-B50ACDC1D5B3";

        private static string _dir = "/etc/libvirt";

        private static string _mntDir = Mount.SetDirsPath(_dir);

        private static string _mainFile = "smb.conf";

        private static string _antdLibvirtFile = "antd.libvirt.conf";

        public static void SetReady() {
            Terminal.Terminal.Background.Execute($"cp {_dir} {_mntDir}");
            FileSystem.CopyDirectory(_dir, _mntDir);
            Mount.Dir(_dir);
        }

        private static bool CheckIsActive() {
            var mount = MountRepository.Get(_dir);
            return (mount != null);
        }

        public static bool IsActive => CheckIsActive();

        public static void ReloadConfig() {
            Terminal.Terminal.Background.Execute("smbcontrol all reload-config");
        }

        private static List<KeyValuePair<string, List<string>>> GetServiceStructure() {
            var list = new List<KeyValuePair<string, List<string>>>();
            var files = Directory.EnumerateFiles(_mntDir, "*.conf", SearchOption.AllDirectories).ToArray();
            foreach (string t in files) {
                if (!File.ReadLines(t).Any(line => line.Contains("include")))
                    continue;
                var lines = File.ReadLines(t).Where(line => line.Contains("include")).ToList();
                var dump = new List<string>();
                dump.AddRange(lines.Select(line => line.Split('=')[1].Trim().Replace(_dir, _mntDir)));
                list.Add(new KeyValuePair<string, List<string>>(t.Replace("\\", "/"), dump));
            }
            if (list.Count < 1) {
                list.Add(new KeyValuePair<string, List<string>>($"{_mntDir}/{_mainFile}", new List<string>()));
            }
            return list;
        }

        public static List<KeyValuePair<string, List<string>>> Structure => GetServiceStructure();

        private static List<string> GetServiceSimpleStructure() {
            var list = new List<string>();
            var files = Directory.EnumerateFiles(_mntDir, "*.conf", SearchOption.AllDirectories).ToArray();
            foreach (string t in files) {
                if (!File.ReadLines(t).Any(line => line.Contains("include")))
                    continue;
                var lines = File.ReadLines(t).Where(line => line.Contains("include")).ToList();
                list.AddRange(lines.Select(line => line.Split('=')[1].Trim().Replace(_dir, _mntDir)));
            }
            if (list.Count < 1) {
                list.Add($"{_mntDir}/{_mainFile}");
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

        public class LibvirtModel {
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
                if (!removeTab.Contains(MapRules.CharComment) || line.StartsWith(MapRules.CharComment.ToString()))
                    return clean;
                var splitAtComment = removeTab.Split(MapRules.CharComment);
                clean = splitAtComment[0].Trim();
                return clean;
            }

            private static IEnumerable<LineModel> ReadFile(string path) {
                var text = FileSystem.ReadFile(path);
                var lines = text.Split(MapRules.CharEndOfLine);
                return (from line in lines where line != "" && !line.StartsWith("include") select CleanLine(line) into cleanLine select ReadLine(path, cleanLine)).ToList();
            }

            private static ShareModel ReadFileShare(string path) {
                var shareName = GetShareName(path) ?? "";
                var model = new ShareModel {
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
                var keyValuePair = line.Split(new[] { MapRules.CharKevValueSeparator.ToString() }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                ServiceDataType type;
                var key = (keyValuePair.Length > 0) ? keyValuePair[0] : "";
                var value = "";
                if (line.StartsWith(MapRules.CharComment.ToString())) {
                    type = ServiceDataType.Disabled;
                }
                else if (line.StartsWith(MapRules.CharSectionOpen.ToString())) {
                    type = ServiceDataType.Disabled;
                }
                else {
                    value = (keyValuePair.Length > 1) ? keyValuePair[1] : "";
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
                var libvirt = new LibvirtModel() {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(libvirt);
            }

            public static LibvirtModel Get() {
                var libvirt = DeNSo.Session.New.Get<LibvirtModel>(s => s.Guid == _serviceGuid).FirstOrDefault();
                return libvirt;
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceLibvirt parameter) {
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

            public static void SaveGlobalConfig(List<ServiceLibvirt> newParameters) {
                var shares = MapFile.Get().Share;
                var data = new List<LineModel>();
                data.AddRange(newParameters.Select(ConvertData));
                var libvirt = new LibvirtModel {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(libvirt);
            }

            public static void DumpGlobalConfig() {
                var parameters = MapFile.Get().Data.ToArray();
                var filesToClean = parameters.Select(p => p.FilePath).ToDynamicHashSet();
                foreach (var file in filesToClean) {
                    CleanFile(file);
                }
                foreach (var t in parameters) {
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

            public static void SaveShareConfig(string fileName, string name, string queryName, List<ServiceLibvirt> newParameters) {
                var data = MapFile.Get().Data;
                var shares = MapFile.Get().Share;
                var oldShare = shares.FirstOrDefault(o => o.Name == queryName);
                shares.Remove(oldShare);
                var shareData = new List<LineModel>();
                shareData.AddRange(newParameters.Select(ConvertData));
                var newShare = new ShareModel() {
                    FilePath = fileName,
                    Name = name,
                    Data = shareData
                };
                shares.Add(newShare);
                var libvirt = new LibvirtModel {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(libvirt);
            }

            public static void DumpShare(string shareName) {
                var share = MapFile.Get().Share.FirstOrDefault(s => s.Name == shareName);
                if (share == null)
                    return;
                var parameters = share.Data.ToArray();
                var file = share.FilePath;
                CleanFile(file);
                AppendLine(file, $"{MapRules.CharSectionOpen}{share.Name}{MapRules.CharSectionClose}");
                foreach (var t in parameters) {
                    var line = $"{t.Key} {MapRules.CharKevValueSeparator} {t.Value}";
                    AppendLine(t.FilePath, line);
                }
            }

            public static void AddParameterToGlobal(string key, string value) {
                SetCustomFile();
                ServiceDataType type = Helper.ServiceData.SupposeDataType(value);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value);
                var line = new LineModel() {
                    FilePath = $"{_mntDir}/{_antdLibvirtFile}",
                    Key = key,
                    Value = value,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                var shares = MapFile.Get().Share;
                var data = MapFile.Get().Data;
                data.Add(line);
                var libvirt = new LibvirtModel {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(libvirt);
            }

            private static void SetCustomFile() {
                var path = $"{_mntDir}/{_antdLibvirtFile}";
                if (!File.Exists(path)) {
                    File.Create(path);
                }
            }

            public static void RewriteSmbconf() {
                var file = $"{_mntDir}/{_mainFile}";
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

            private static IEnumerable<dynamic> GetGlobalPaths() {
                var share = MapFile.Get().Data.Select(s => s.FilePath).ToDynamicHashSet();
                return share;
            }

            private static IEnumerable<dynamic> GetSharePaths() {
                var share = MapFile.Get().Share.Select(s => s.FilePath).ToDynamicHashSet();
                return share;
            }

            public static void AddShare(string name, string directory) {
                SetShareFile(name);
                var shareData = new List<LineModel>();
                var defaultParameter00 = new LineModel {
                    FilePath = $"{_mntDir}/share/{name.Replace(" ", "_")}.conf",
                    Key = "path",
                    Value = directory,
                    Type = ServiceDataType.String,
                    BooleanVerbs = new KeyValuePair<string, string>("", "")
                };
                shareData.Add(defaultParameter00);
                var defaultParameter01 = new LineModel() {
                    FilePath = $"{_mntDir}/share/{name.Replace(" ", "_")}.conf",
                    Key = "browseable",
                    Value = "yes",
                    Type = ServiceDataType.Boolean,
                    BooleanVerbs = new KeyValuePair<string, string>("yes", "no")
                };
                shareData.Add(defaultParameter01);
                var sh = new ShareModel() {
                    FilePath = $"{_mntDir}/share/{name.Replace(" ", "_")}.conf",
                    Name = name,
                    Data = shareData
                };
                var shares = MapFile.Get().Share;
                var data = MapFile.Get().Data;
                shares.Add(sh);
                var libvirt = new LibvirtModel() {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Share = shares,
                    Data = data
                };
                DeNSo.Session.New.Set(libvirt);
            }

            private static void SetShareFile(string shareName) {
                var sharePath = $"{_mntDir}/share/{shareName.Replace(" ", "_")}.conf";
                if (!File.Exists(sharePath)) {
                    File.Create(sharePath);
                }
            }
        }
    }
}
