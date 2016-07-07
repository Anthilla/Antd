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
using antdlib;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.ViewBinds;
using Antd.Database;
using Antd.MountPoint;

namespace Antd.Ssh {
    public class SshConfig {
        private const string ServiceGuid = "A7A04748-9A03-4B1B-9D0F-BBF738F8C8E9";

        private const string Dir = "/etc/ssh";

        private static readonly string MntDir = Mounts.SetDirsPath(Dir);

        private const string MainFile = "sshd_config";

        public static void SetReady() {
            Terminal.Execute($"cp {Dir} {MntDir}");
            FileSystem.CopyDirectory(Dir, MntDir);
            Mount.Dir(Dir);
        }

        private static bool CheckIsActive() {
            return new MountRepository().GetByPath(Dir) != null;
        }

        public static bool IsActive => CheckIsActive();

        public static void ReloadConfig() {
            Terminal.Execute("systemctl restart sshd");
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
            public List<LineModel> Data { get; set; } = new List<LineModel>();
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

            private static LineModel ReadLine(string path, string line) {
                var keyValuePair = line.Split(new[] { MapRules.CharKevValueSeparator.ToString() }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                ServiceDataType type;
                var key = keyValuePair.Length > 0 ? keyValuePair[0] : "";
                var value = "";
                if (line.StartsWith(MapRules.CharComment.ToString())) {
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
                var lines = ReadFile(MainFile);
                var data = lines.ToList();
                var ssh = new SshModel {
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Data = data
                };
                //DeNSo.Session.New.Set(ssh);
            }

            public static SshModel Get() {
                //var ssh = DeNSo.Session.New.Get<SshModel>(s => s.Guid == ServiceGuid).FirstOrDefault();
                //return ssh;
                throw new NotImplementedException();
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
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Data = data
                };
                //DeNSo.Session.New.Set(ssh);
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

            public static void AddParameterToGlobal(string key, string value) {
                var type = Helper.ServiceData.SupposeDataType(value);
                var booleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(value);
                var line = new LineModel {
                    FilePath = $"{MntDir}/{MainFile}",
                    Key = key,
                    Value = value,
                    Type = type,
                    BooleanVerbs = booleanVerbs
                };
                var data = MapFile.Get().Data;
                data.Add(line);
                var ssh = new SshModel {
                    _Id = ServiceGuid,
                    Guid = ServiceGuid,
                    Timestamp = Timestamp.Now,
                    Data = data
                };
                //DeNSo.Session.New.Set(ssh);
            }

            public static void RewriteSshdconf() {
                var file = $"{MntDir}/{MainFile}";
                CleanFile(file);
            }
        }

        public class Keys {
            public static void SendKey(string host, string keyName, string user = "") {
                var at = (user.Length > 0 ? user + "@" : "") + $"{host}";
                Terminal.Execute($"scp {keyName} {at} /root/.ssh/authorized_keys");
            }

            private static string GenerateForUser(string userName) {
                var privateKeyPath = $"/home/{userName}/.ssh/{userName}-key";
                var publicKeyPath = $"/home/{userName}/.ssh/{userName}-key.pub";
                Terminal.Execute($"sudo -H -u {userName} bash -c 'echo y\n | ssh-keygen -b 2048 -t rsa -f {privateKeyPath} -q -N \"\"'");
                return publicKeyPath;
            }

            private static void ExportKeysToRemote(string user, string remoteMachine) {
                var userPubKeyPath = GenerateForUser(user);
                var keyContent = File.ReadAllText(userPubKeyPath).Replace(Environment.NewLine, " ");
                //todo comando per aggiungere la chiave al file di là
                Terminal.Execute($"scp {keyContent} {remoteMachine}/{Parameter.AuthKeys}");
            }

            public static void PropagateKeys(IEnumerable<string> users, IEnumerable<string> remoteMachines) {
                var enumerable = remoteMachines as string[] ?? remoteMachines.ToArray();
                foreach (var user in users) {
                    foreach (var remoteMachine in enumerable) {
                        ExportKeysToRemote(user, remoteMachine);
                    }
                }
            }
        }
    }
}
