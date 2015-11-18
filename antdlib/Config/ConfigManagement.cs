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
using System.Text.RegularExpressions;
using antdlib.Common;

namespace antdlib.Config {
    public class ConfigManagement {
        public class ValuesBundle {
            public string _Id { get; set; } = Guid.NewGuid().ToString();
            public string Tag { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public string RegexTag => $"[{Tag}:{Key}]";
            public bool IsDefault { get; set; } = true;
        }

        public class CommandsBundle {
            public string _Id { get; set; }
            public string Index { get; set; }
            public string Guid { get; set; }
            public string Command { get; set; }
            public string CommandWithValues { get; set; }
            public bool IsEnabled { get; set; } = true;
        }

        public class CommandsBundleLayout {
            public string _Id { get; set; } = Guid.NewGuid().ToString();
            public string CommandLayout { get; set; }
        }

        public static IEnumerable<ValuesBundle> GetValuesBundle() {
            return DeNSo.Session.New.Get<ValuesBundle>().Concat(Default.DefaultValuesBundle()).OrderBy(_ => _.Tag).ThenBy(_ => _.Key);
        }

        public static IEnumerable<ValuesBundle> GetValuesBundleByTag(string tag) {
            return GetValuesBundle().Where(_ => _.Tag == tag);
        }

        public static IEnumerable<string> GetTagsBundleValue() {
            return GetValuesBundle().Select(f => f.Tag).ToStringHashSet();
        }

        public static string GetValuesBundleValue(string tag, string k) {
            return DeNSo.Session.New.Get<ValuesBundle>(_ => _.Tag == tag && _.Key == k).Select(f => f.Value).FirstOrDefault();
        }

        public static string GetTagValuesBundleLastKey(string tag) {
            try {
                return GetValuesBundleByTag(tag).OrderBy(_ => _.Key).Select(_ => _.Key).Last();
            }
            catch (Exception) {
                return "-1";
            }
        }

        public static string IncreaseTagValuesBundleKey(string tag) {
            return (Convert.ToInt32(GetTagValuesBundleLastKey(tag)) + 1).ToString();
        }

        public static void AddValuesBundle(string tag, string k, string v) {
            var getmodel = GetValuesBundle().FirstOrDefault(vv => vv.Tag == tag && vv.Key == k);
            if (getmodel == null && tag.Length > 0 && k.Length > 0 && v.Length > 0) {
                var model = new ValuesBundle {
                    _Id = Guid.NewGuid().ToString(),
                    IsDefault = false,
                    Tag = tag.ToLower(),
                    Key = k,
                    Value = v
                };
                DeNSo.Session.New.Set(model);
            }
            else {
                if (getmodel == null)
                    return;
                getmodel.Value = v;
                DeNSo.Session.New.Set(getmodel);
            }
        }

        public static void AddValuesBundle(string tag, string v) {
            var getmodel = GetValuesBundle().FirstOrDefault(vv => vv.Tag == tag && vv.Value == v);
            if (getmodel == null && tag.Length > 0 && v.Length > 0) {
                var model = new ValuesBundle {
                    _Id = Guid.NewGuid().ToString(),
                    IsDefault = false,
                    Tag = tag.ToLower(),
                    Key = IncreaseTagValuesBundleKey(tag),
                    Value = v
                };
                DeNSo.Session.New.Set(model);
            }
            else {
                if (getmodel == null)
                    return;
                getmodel.Value = v;
                DeNSo.Session.New.Set(getmodel);
            }
        }

        public static void DeleteValuesBundle(string tag, string k, string v) {
            var getmodel = GetValuesBundle().FirstOrDefault(vv => vv.Tag == tag && vv.Value == v && vv.Key == k);
            if (getmodel != null && tag.Length > 0 && k.Length > 0 && v.Length > 0) {
                DeNSo.Session.New.Delete(getmodel);
            }
        }

        public static IEnumerable<CommandsBundle> GetCommandsBundle() {
            var list = new List<CommandsBundle>();
            foreach (var command in DeNSo.Session.New.Get<CommandsBundle>().OrderBy(_ => _.Index)) {
                command.CommandWithValues = SupposeCommandReplacement(command.Command);
                list.Add(command);
            }
            return list;
        }

        public static IEnumerable<CommandsBundleLayout> GetCommandsBundleLayout() {
            return DeNSo.Session.New.Get<CommandsBundleLayout>().OrderBy(_ => _.CommandLayout).Concat(Default.DefaultCommandsBundle());
        }

        private static IEnumerable<CommandsBundle> GetEnabledCommandsBundle() {
            return DeNSo.Session.New.Get<CommandsBundle>().Where(_ => _.IsEnabled).OrderBy(_ => _.Index);
        }

        public static void AddCommandsBundle(string command) {
            var getmodel = GetCommandsBundle().FirstOrDefault(vv => vv.Command == command);
            if (getmodel != null || command.Length <= 0)
                return;
            var model = new CommandsBundle {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                Index = "0",
                Command = command
            };
            DeNSo.Session.New.Set(model);
            AddCommandsBundleLayout(command);
        }

        public static void AddCommandsBundleLayout(string command) {
            var commandLayout = command.ReplaceAllTextBetweenWith('{', '}', "{tag:x}");
            var getmodel = GetCommandsBundleLayout().FirstOrDefault(vv => vv.CommandLayout == commandLayout);
            if (getmodel != null || command.Length <= 0)
                return;
            var model = new CommandsBundleLayout {
                _Id = Guid.NewGuid().ToString(),
                CommandLayout = commandLayout
            };
            DeNSo.Session.New.Set(model);
        }

        public static void AssignIndexToCommandsBundle(string guid, string index) {
            var getmodel = GetCommandsBundle().FirstOrDefault(vv => vv.Guid == guid);
            if (getmodel == null)
                return;
            getmodel.Index = index;
            DeNSo.Session.New.Set(getmodel);
        }

        public static void EnableCommand(string guid) {
            var getmodel = GetCommandsBundle().FirstOrDefault(vv => vv.Guid == guid);
            if (getmodel == null)
                return;
            getmodel.IsEnabled = true;
            DeNSo.Session.New.Set(getmodel);
        }

        public static void DisableCommand(string guid) {
            var getmodel = GetCommandsBundle().FirstOrDefault(vv => vv.Guid == guid);
            if (getmodel == null)
                return;
            getmodel.IsEnabled = false;
            DeNSo.Session.New.Set(getmodel);
        }

        public static void LaunchCommand(string guid) {
            var getmodel = GetCommandsBundle().FirstOrDefault(vv => vv.Guid == guid);
            if (getmodel != null) {
                Terminal.Terminal.Execute(SupposeCommandReplacement(getmodel.Command));
            }
        }

        public static void DeleteCommandsBundle(string guid) {
            var getmodel = GetCommandsBundle().FirstOrDefault(vv => vv.Guid == guid);
            if (getmodel != null) {
                DeNSo.Session.New.Delete(getmodel);
            }
        }

        public static string SupposeCommandReplacement(string command) {
            const string valueNotFound = "[valueNOTfound]";
            var memCommamnd = command;
            var matches = new Regex("\\[(.*?)\\]").Matches(memCommamnd);
            if (matches.Count > 0) {
                for (var i = 0; i < matches.Count; i++) {
                    if (matches[i].Value == valueNotFound)
                        continue;
                    var taginfo = matches[i].Value.Replace("[", "").Replace("]", "").Trim().Split(':');
                    var value = GetValuesBundleValue(taginfo[0], taginfo[1]);
                    var stringToReplace = $"[{string.Join(":", taginfo)}]";
                    memCommamnd = memCommamnd.Replace(stringToReplace, value ?? "[valueNOTfound]");
                }
            }
            if (memCommamnd.Contains("[") && memCommamnd.Contains(":") && memCommamnd.Contains("]")) {
                memCommamnd = SupposeCommandReplacement(memCommamnd);
            }
            return memCommamnd;
        }

        public static void ExecuteAll() {
            foreach (var command in GetEnabledCommandsBundle()) {
                Terminal.Terminal.Execute(SupposeCommandReplacement(command.Command));
            }
        }

        public static bool Exists => GetEnabledCommandsBundle().Any();

        public class Export {
            private static readonly string ConfigFolder = Folder.RepoConfig;

            private static IEnumerable<string> GetConfigurationFileNames() {
                return Directory.EnumerateFiles(ConfigFolder, "*configuration.export", SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
            }

            private static string GetLastFileName() {
                if (!GetConfigurationFileNames().Any())
                    return "0_antd_configuration.export";
                var lastFile = GetConfigurationFileNames().Last();
                var num = Convert.ToInt32(lastFile.Split('_')[0]);
                return $"{(num + 1)}_antd_configuration.export";
            }

            public static void ExportConfigurationToFile() {
                var linesToWrite = GetEnabledCommandsBundle().Select(c => SupposeCommandReplacement(c.Command));
                File.WriteAllLines(Path.Combine(ConfigFolder, GetLastFileName()), linesToWrite);
            }
        }

        public class Default {
            public static IEnumerable<ValuesBundle> DefaultValuesBundle() {
                return new List<ValuesBundle> {
                    new ValuesBundle { Tag = "nft-table-func", Key = "0", Value = "add table" },
                    new ValuesBundle { Tag = "nft-table-func", Key = "1", Value = "delete table" },
                    new ValuesBundle { Tag = "nft-table-func", Key = "2", Value = "-a list table" },
                    new ValuesBundle { Tag = "nft-table-func", Key = "3", Value = "flush table" },

                    new ValuesBundle { Tag = "nft-table-family", Key = "0", Value = "ip" },
                    new ValuesBundle { Tag = "nft-table-family", Key = "1", Value = "ip6" },
                    new ValuesBundle { Tag = "nft-table-family", Key = "2", Value = "arp" },
                    new ValuesBundle { Tag = "nft-table-family", Key = "3", Value = "bridge" },

                    new ValuesBundle { Tag = "nft-table-name", Key = "0", Value = "filter" },
                    new ValuesBundle { Tag = "nft-table-name", Key = "1", Value = "route" },
                    new ValuesBundle { Tag = "nft-table-name", Key = "2", Value = "nat" },

                    new ValuesBundle { Tag = "nft-chain-func", Key = "0", Value = "add chain" },
                    new ValuesBundle { Tag = "nft-chain-func", Key = "1", Value = "delete chain" },
                    new ValuesBundle { Tag = "nft-chain-func", Key = "3", Value = "flush chain" },

                    new ValuesBundle { Tag = "nft-chain-type", Key = "0", Value = "filter" },
                    new ValuesBundle { Tag = "nft-chain-type", Key = "1", Value = "route" },
                    new ValuesBundle { Tag = "nft-chain-type", Key = "2", Value = "nat" },

                    new ValuesBundle { Tag = "nft-chain-priority", Key = "0", Value = "0" },

                    new ValuesBundle { Tag = "nft-chain-hook", Key = "0", Value = "prerouting" },
                    new ValuesBundle { Tag = "nft-chain-hook", Key = "1", Value = "input" },
                    new ValuesBundle { Tag = "nft-chain-hook", Key = "2", Value = "forward" },
                    new ValuesBundle { Tag = "nft-chain-hook", Key = "3", Value = "output" },
                    new ValuesBundle { Tag = "nft-chain-hook", Key = "4", Value = "postrouting" },

                    new ValuesBundle { Tag = "nft-rule-func", Key = "0", Value = "add rule" },
                    new ValuesBundle { Tag = "nft-rule-func", Key = "1", Value = "delete rule" },
                    new ValuesBundle { Tag = "nft-rule-func", Key = "3", Value = "insert rule" },

                    new ValuesBundle { Tag = "nft-rule-verbs", Key = "0", Value = "accept" },
                    new ValuesBundle { Tag = "nft-rule-verbs", Key = "1", Value = "drop" },
                    new ValuesBundle { Tag = "nft-rule-verbs", Key = "2", Value = "reject" },
                    new ValuesBundle { Tag = "nft-rule-verbs", Key = "3", Value = "queue" },
                    new ValuesBundle { Tag = "nft-rule-verbs", Key = "4", Value = "continue" },
                    new ValuesBundle { Tag = "nft-rule-verbs", Key = "5", Value = "jump" },
                    new ValuesBundle { Tag = "nft-rule-verbs", Key = "6", Value = "goto" },

                    new ValuesBundle { Tag = "nft-rule", Key = "0", Value = "" },
                };
            }

            private static string tag = "[tag:x]";

            public static IEnumerable<CommandsBundleLayout> DefaultCommandsBundle() {
                return new List<CommandsBundleLayout>
                {
                    new CommandsBundleLayout { CommandLayout = "nft [nft-table-func:x] [nft-table-family:x] [nft-table-name:x]" },
                    new CommandsBundleLayout { CommandLayout = "nft [nft-chain-func:x] [nft-table-family:x] [nft-table-name:x] [nft-chain-hook:x] { type [nft-chain-type:x] hook [nft-chain-hook:x] priority [nft-chain-priority:0] \\; }" },
                    new CommandsBundleLayout { CommandLayout = $"nft [nft-rule-func:x] [nft-table-family:x] [nft-chain-hook:x] [nft-rule:0]" },

                    new CommandsBundleLayout { CommandLayout = "systemd-machine-id-setup" },
                    new CommandsBundleLayout { CommandLayout = $"hostnamectl set-hostname {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"hostnamectl set-chassis {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"hostnamectl set-deployment {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"hostnamectl set-location {tag}" },
                    new CommandsBundleLayout { CommandLayout = "timedatectl" },
                    new CommandsBundleLayout { CommandLayout = "timedatectl --no-pager --no-ask-password --adjust-system-clock" },
                    new CommandsBundleLayout { CommandLayout = $"set-timezone {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"killall {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"killall dhclient" },
                    new CommandsBundleLayout { CommandLayout = $"systemctl start {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"systemctl restart {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"systemctl stop {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"echo {tag} {tag} > /etc/resolv.conf" },
                    new CommandsBundleLayout { CommandLayout = $"echo {tag} {tag} >> /etc/resolv.conf" },
                    new CommandsBundleLayout { CommandLayout = $"ip link set up dev {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"ip link set dev {tag} promisc on" },
                    new CommandsBundleLayout { CommandLayout = $"brctl addbr {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"brctl addif {tag} {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"brctl stp {tag} off" },
                    new CommandsBundleLayout { CommandLayout = $"ip addr add {tag} dev {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"ip addr flush {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"ntpdate {tag}" },
                    new CommandsBundleLayout { CommandLayout = "timedatectl --no-pager --no-ask-password --adjust-system-clock set-ntp yes" },
                    new CommandsBundleLayout { CommandLayout = $"rmdir {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"mkdir {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"mkdir -p {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"mount -o rw,discard,noatime {tag} {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"mount -o rw,discard,noatime LABEL={tag} {tag}" },

                    new CommandsBundleLayout { CommandLayout = $"nft -f {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"ip route add {tag} via {tag} dev {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"chown -R named:named {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"touch {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"chmod {tag} {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"cp {tag} {tag}" },
                    new CommandsBundleLayout { CommandLayout = $"systemctl daemon-reload" },
                    new CommandsBundleLayout { CommandLayout = $"sleep {tag}" },

                };
            }
        }

        public class FromFile {
            private static readonly string ConfigFolder = Folder.RepoConfig;

            private static IEnumerable<string> Contexts {
                get {
                    return Directory.EnumerateDirectories(ConfigFolder).Where(d => !d.StartsWith("disabled."));
                }
            }

            private static IEnumerable<string> GetContextFiles(string contextName) {
                try {
                    var fullPath = Path.Combine(ConfigFolder, contextName);
                    if (!Contexts.Contains(fullPath)) {
                        throw new Exception();
                    }
                    return Directory.EnumerateFiles(fullPath, "*.cfg", SearchOption.TopDirectoryOnly).Where(d => !d.StartsWith("disabled.")).OrderBy(f => f);
                }
                catch (Exception) {
                    ConsoleLogger.Log($"Nothing to configure for {contextName}");
                    return new List<string>();
                }
            }

            public static void ApplyForAll() {
                try {
                    foreach (var context in Contexts) {
                        ApplyConfigForContext(context);
                    }
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }

            private static void ApplyConfigForContext(string contextName) {
                try {
                    var files = GetContextFiles(contextName);
                    var enumerable = files as string[] ?? files.ToArray();
                    if (!enumerable.Any()) {
                        throw new Exception();
                    }
                    foreach (var file in enumerable) {
                        LaunchConfigurationForFile(file);
                    }
                }
                catch (Exception) {
                    ConsoleLogger.Log($"Nothing to configure for {contextName}");
                }
            }

            private static void LaunchConfigurationForFile(string filename) {
                try {
                    if (!File.Exists(filename)) {
                        throw new Exception();
                    }
                    var lines = File.ReadAllLines(filename);
                    if (!lines.Any()) {
                        throw new Exception();
                    }
                    foreach (var line in lines) {
                        //try {
                        AddCommandsBundle(line);
                        Terminal.Terminal.Execute(line);
                        //}
                        //catch (Exception) {
                        //    ConsoleLogger.Warn($"Error while executing: {line}");
                        //}
                    }
                }
                catch (Exception) {
                    ConsoleLogger.Log($"Nothing to configure for {contextName}");
                    //ConsoleLogger.Warn($"Cannot apply configuration stored in: {filename}");
                    //ConsoleLogger.Warn("The file may not exists or it may be empty");
                    //ConsoleLogger.Warn(ex.Message);
                }
            }

            //public static void SaveConfiguration(string contextName, string file, IEnumerable<string> lines) {
            //    try {
            //        var enumerable = lines as string[] ?? lines.ToArray();
            //        if (!enumerable.Any()) {
            //            throw new Exception();
            //        }
            //        var contextPath = Path.Combine(ConfigFolder, contextName);
            //        if (!Directory.Exists(contextPath)) {
            //            Directory.CreateDirectory(contextPath);
            //        }
            //        if (!file.EndsWith(".cfg")) {
            //            file = file + ".cfg";
            //        }
            //        var filePath = Path.Combine(contextPath, file);
            //        if (File.Exists(filePath)) {
            //            File.Delete(filePath);
            //        }
            //        File.WriteAllLines(filePath, enumerable);
            //    }
            //    catch (Exception ex) {
            //        ConsoleLogger.Warn($"Cannot save {file} configuration for {contextName}");
            //        ConsoleLogger.Warn(ex.Message);
            //    }
            //}
        }
    }
}
