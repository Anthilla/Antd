
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

namespace antdlib.Ssh {
    public class SshConfig {

        private static string _serviceGuid = "A7A04748-9A03-4B1B-9D0F-BBF738F8C8E9";

        private static string _dir = "/etc/ssh";

        private static string _mntDir = Mount.SetDirsPath(_dir);

        private static string _mainFile = "sshd_config";

        public static void SetReady() {
            Terminal.Execute($"cp {_dir} {_mntDir}");
            FileSystem.CopyDirectory(_dir, _mntDir);
            Mount.Dir(_dir);
        }

        private static bool CheckIsActive() {
            return (MountRepository.Get(_dir) != null);
        }

        public static bool IsActive => CheckIsActive();

        public static void ReloadConfig() {
            //todo cerca comando
            //Terminal.Execute($"smbcontrol all reload-config");
        }

        public class MapRules {
            public static char CharComment => '#';

            public static char CharKevValueSeparator => ' ';

            public static char CharEndOfLine => '\n';
        }

        public class LineModel {
            public string FilePath { get; set; }

            public string Key { get; set; }

            public string Value { get; set; }

            public ServiceDataType Type { get; set; }

            public KeyValuePair<string, string> BooleanVerbs { get; set; }
        }

        public class SshModel {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public string Timestamp { get; set; }

            public List<LineModel> Data { get; set; } = new List<LineModel>() { };
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
                return (from line in lines where line != "" && !line.StartsWith("include") select CleanLine(line) into cleanLine select ReadLine(path, cleanLine)).ToList();
            }

            private static LineModel ReadLine(string path, string line) {
                var keyValuePair = line.Split(new[] { MapRules.CharKevValueSeparator.ToString() }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                ServiceDataType type;
                var key = (keyValuePair.Length > 0) ? keyValuePair[0] : "";
                var value = "";
                if (line.StartsWith(MapRules.CharComment.ToString())) {
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
                var lines = ReadFile(_mainFile);
                var data = lines.ToList();
                var ssh = new SshModel {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Data = data
                };
                DeNSo.Session.New.Set(ssh);
            }

            public static SshModel Get() {
                var ssh = DeNSo.Session.New.Get<SshModel>(s => s.Guid == _serviceGuid).FirstOrDefault();
                return ssh;
            }
        }

        public class WriteFile {
            private static LineModel ConvertData(ServiceSsh parameter) {
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

            public static void SaveGlobalConfig(List<ServiceSsh> newParameters) {
                var data = newParameters.Select(ConvertData).ToList();
                var ssh = new SshModel {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Data = data
                };
                DeNSo.Session.New.Set(ssh);
            }

            public static void DumpGlobalConfig() {
                var parameters = MapFile.Get().Data.ToArray();
                var filesToClean = parameters.Select(p => p.FilePath).ToDynamicHashSet();
                foreach (var file in filesToClean) {
                    CleanFile(file);
                }
                for (int i = 0; i < parameters.Length; i++) {
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

            public static void AddParameterToGlobal(string key, string value) {
                ServiceDataType type = Helper.ServiceData.SupposeDataType(value);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value);
                var line = new LineModel() {
                    FilePath = $"{_mntDir}/{_mainFile}",
                    Key = key,
                    Value = value,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                var data = MapFile.Get().Data;
                data.Add(line);
                var ssh = new SshModel() {
                    _Id = _serviceGuid,
                    Guid = _serviceGuid,
                    Timestamp = Timestamp.Now,
                    Data = data
                };
                DeNSo.Session.New.Set(ssh);
            }

            public static void RewriteSshdconf() {
                var file = $"{_mntDir}/{_mainFile}";
                CleanFile(file);
            }
        }

        public class Keys {

            private static string fileStartsWith = "ssh_host_";

            private static string privateEndsWith = "_key";

            private static string publicEndsWith = "_key.pub";

            public class KeyModel {
                public string _Id { get; set; }

                public string Guid { get; set; }

                public string Timestamp { get; set; }

                public string Name { get; set; }

                public string Content { get; set; }

                public SSHKeyType KeyType { get; set; }
            }

            public static List<KeyModel> GetAll() {
                var list = new List<KeyModel>();
                var files = Directory.EnumerateFiles(_dir).Where(f => f.Contains(fileStartsWith)).ToArray();
                foreach (var k in from file in files
                                  let type = file.EndsWith(publicEndsWith) ? SSHKeyType.Public : SSHKeyType.Private
                                  select new KeyModel {
                                      _Id = Guid.NewGuid().ToString(),
                                      Guid = Guid.NewGuid().ToString(),
                                      Timestamp = Timestamp.Now,
                                      Name = file/*.Replace("", "")*/,
                                      Content = FileSystem.ReadFile(file),
                                      KeyType = type
                                  }) {
                    DeNSo.Session.New.Set(k);
                    list.Add(k);
                }
                return list;
            }

            public static KeyModel Get(string name) {
                return DeNSo.Session.New.Get<KeyModel>(k => k.Name == name).FirstOrDefault();
            }

            public static void Generate(string keyName) {
                Terminal.Execute($"ssh-keygen -t rsa -f {_dir}/{fileStartsWith}{keyName}{privateEndsWith} -N {Guid.NewGuid()}");
            }

            public static void SendKey(string host, string keyName, string user = "") {
                var at = ((user.Length > 0) ? user + "@" : "") + $"{host}";
                Terminal.Execute($"scp {keyName} {at} ~/.ssh/authorized_keys");
            }
        }
    }
}
