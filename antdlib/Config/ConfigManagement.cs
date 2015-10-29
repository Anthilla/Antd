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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace antdlib.Config {
    public class ConfigManagement {
        public class ValuesBundle {
            public string _Id { get; set; } = Guid.NewGuid().ToString();
            public string Tag { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public string RegexTag { get { return $"[{Tag}:{Key}]"; } }
            public bool IsDefault { get; set; } = true;
        }

        public class CommandsBundle {
            public string _Id { get; set; }
            public string Index { get; set; }
            public string Guid { get; set; }
            public string Command { get; set; }
            public bool IsEnabled { get; set; } = true;
        }

        /// <summary>
        /// selectize menu
        /// {tag:x}
        /// </summary>
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
            var getmodel = GetValuesBundle().Where(vv => vv.Tag == tag && vv.Key == k).FirstOrDefault();
            if (getmodel == null && tag.Length > 0 && k.Length > 0 && v.Length > 0) {
                var model = new ValuesBundle() {
                    _Id = Guid.NewGuid().ToString(),
                    IsDefault = false,
                    Tag = tag.ToLower(),
                    Key = k,
                    Value = v
                };
                DeNSo.Session.New.Set(model);
            }
            else {
                getmodel.Value = v;
                DeNSo.Session.New.Set(getmodel);
            }
        }

        public static void AddValuesBundle(string tag, string v) {
            var getmodel = GetValuesBundle().Where(vv => vv.Tag == tag && vv.Value == v).FirstOrDefault();
            if (getmodel == null && tag.Length > 0 && v.Length > 0) {
                var model = new ValuesBundle() {
                    _Id = Guid.NewGuid().ToString(),
                    IsDefault = false,
                    Tag = tag.ToLower(),
                    Key = IncreaseTagValuesBundleKey(tag),
                    Value = v
                };
                DeNSo.Session.New.Set(model);
            }
            else {
                getmodel.Value = v;
                DeNSo.Session.New.Set(getmodel);
            }
        }

        public static void DeleteValuesBundle(string tag, string k, string v) {
            var getmodel = GetValuesBundle().Where(vv => vv.Tag == tag && vv.Value == v && vv.Key == k).FirstOrDefault();
            if (getmodel != null && tag.Length > 0 && k.Length > 0 && v.Length > 0) {
                DeNSo.Session.New.Delete(getmodel);
            }
        }

        public static IEnumerable<CommandsBundle> GetCommandsBundle() {
            return DeNSo.Session.New.Get<CommandsBundle>().OrderBy(_ => _.Index);
        }

        public static IEnumerable<CommandsBundleLayout> GetCommandsBundleLayout() {
            return DeNSo.Session.New.Get<CommandsBundleLayout>().OrderBy(_ => _.CommandLayout).Concat(Default.DefaultCommandsBundle());
        }

        private static IEnumerable<CommandsBundle> GetEnabledCommandsBundle() {
            return DeNSo.Session.New.Get<CommandsBundle>().Where(_ => _.IsEnabled == true).OrderBy(_ => _.Index);
        }

        public static void AddCommandsBundle(string command) {
            var getmodel = GetCommandsBundle().Where(vv => vv.Command == command).FirstOrDefault();
            if (getmodel == null && command.Length > 0) {
                var model = new CommandsBundle() {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    Index = "0",
                    Command = command
                };
                DeNSo.Session.New.Set(model);
                AddCommandsBundleLayout(command);
            }
        }

        public static void AddCommandsBundleLayout(string command) {
            var commandLayout = command.ReplaceAllTextBetweenWith('{', '}', "{tag:x}");
            var getmodel = GetCommandsBundleLayout().Where(vv => vv.CommandLayout == commandLayout).FirstOrDefault();
            if (getmodel == null && command.Length > 0) {
                var model = new CommandsBundleLayout() {
                    _Id = Guid.NewGuid().ToString(),
                    CommandLayout = commandLayout
                };
                DeNSo.Session.New.Set(model);
            }
        }

        public static void AssignIndexToCommandsBundle(string guid, string index) {
            var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
            getmodel.Index = index;
            DeNSo.Session.New.Set(getmodel);
        }

        public static void EnableCommand(string guid) {
            try {
                var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
                getmodel.IsEnabled = true;
                DeNSo.Session.New.Set(getmodel);
            }
            catch (Exception) { }
        }

        public static void DisableCommand(string guid) {
            try {
                var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
                getmodel.IsEnabled = false;
                DeNSo.Session.New.Set(getmodel);
            }
            catch (Exception) { }
        }

        public static void LaunchCommand(string guid) {
            try {
                var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
                if (getmodel != null) {
                    Terminal.Execute(SupposeCommandReplacement(getmodel.Command));
                }
            }
            catch (Exception) { }
        }

        public static void DeleteCommandsBundle(string guid) {
            var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
            if (getmodel != null) {
                DeNSo.Session.New.Delete(getmodel);
            }
        }

        public static string SupposeCommandReplacement(string command) {
            var valueNotFound = "[valueNOTfound]";
            string memCommamnd = command;
            var matches = new Regex("\\[(.*?)\\]").Matches(memCommamnd);
            if (matches.Count > 0) {
                for (int i = 0; i < matches.Count; i++) {
                    if (matches[i].Value != valueNotFound) {
                        var taginfo = matches[i].Value.Replace("[", "").Replace("]", "").Trim().Split(':');
                        var value = GetValuesBundleValue(taginfo[0], taginfo[1]);
                        var stringToReplace = $"[{string.Join(":", taginfo)}]";
                        if (value == null) {
                            memCommamnd = memCommamnd.Replace(stringToReplace, "[valueNOTfound]");
                        }
                        else {
                            memCommamnd = memCommamnd.Replace(stringToReplace, value);
                        }
                    }
                }
            }
            if (memCommamnd.Contains("[") && memCommamnd.Contains(":") && memCommamnd.Contains("]")) {
                memCommamnd = SupposeCommandReplacement(memCommamnd);
            }
            return memCommamnd;
        }

        public static void ExecuteAll() {
            foreach (var command in GetEnabledCommandsBundle()) {
                Terminal.Execute(SupposeCommandReplacement(command.Command));
            }
        }

        public class Export {
            private static string configFolder = Folder.Config;

            private static IEnumerable<string> GetConfigurationFileNames() {
                return Directory.EnumerateFiles(configFolder, "*configuration.export", SearchOption.TopDirectoryOnly).Select(f => Path.GetFileName(f));
            }

            private static string GetLastFileName() {
                if (GetConfigurationFileNames().Count() > 0) {
                    var lastFile = GetConfigurationFileNames().Last();
                    var num = Convert.ToInt32(lastFile.Split('_')[0]);
                    return $"{(num + 1).ToString()}_antd_configuration.export";
                }
                else {
                    return "0_antd_configuration.export";
                }
            }

            public static void ExportConfigurationToFile() {
                var linesToWrite = GetEnabledCommandsBundle().Select(c => SupposeCommandReplacement(c.Command));
                File.WriteAllLines(Path.Combine(configFolder, GetLastFileName()), linesToWrite);
            }
        }

        public class Default {
            public static IEnumerable<ValuesBundle> DefaultValuesBundle() {
                return new List<ValuesBundle> {
                    new ValuesBundle() { Tag = "nft-table-func", Key = "0", Value = "add table" },
                    new ValuesBundle() { Tag = "nft-table-func", Key = "1", Value = "delete table" },
                    new ValuesBundle() { Tag = "nft-table-func", Key = "2", Value = "-a list table" },
                    new ValuesBundle() { Tag = "nft-table-func", Key = "3", Value = "flush table" },

                    new ValuesBundle() { Tag = "nft-table-family", Key = "0", Value = "ip" },
                    new ValuesBundle() { Tag = "nft-table-family", Key = "1", Value = "ip6" },
                    new ValuesBundle() { Tag = "nft-table-family", Key = "2", Value = "arp" },
                    new ValuesBundle() { Tag = "nft-table-family", Key = "3", Value = "bridge" },

                    new ValuesBundle() { Tag = "nft-table-name", Key = "0", Value = "filter" },
                    new ValuesBundle() { Tag = "nft-table-name", Key = "1", Value = "route" },
                    new ValuesBundle() { Tag = "nft-table-name", Key = "2", Value = "nat" },

                    new ValuesBundle() { Tag = "nft-chain-func", Key = "0", Value = "add chain" },
                    new ValuesBundle() { Tag = "nft-chain-func", Key = "1", Value = "delete chain" },
                    new ValuesBundle() { Tag = "nft-chain-func", Key = "3", Value = "flush chain" },

                    new ValuesBundle() { Tag = "nft-chain-type", Key = "0", Value = "filter" },
                    new ValuesBundle() { Tag = "nft-chain-type", Key = "1", Value = "route" },
                    new ValuesBundle() { Tag = "nft-chain-type", Key = "2", Value = "nat" },

                    new ValuesBundle() { Tag = "nft-chain-priority", Key = "0", Value = "0" },

                    new ValuesBundle() { Tag = "nft-chain-hook", Key = "0", Value = "prerouting" },
                    new ValuesBundle() { Tag = "nft-chain-hook", Key = "1", Value = "input" },
                    new ValuesBundle() { Tag = "nft-chain-hook", Key = "2", Value = "forward" },
                    new ValuesBundle() { Tag = "nft-chain-hook", Key = "3", Value = "output" },
                    new ValuesBundle() { Tag = "nft-chain-hook", Key = "4", Value = "postrouting" },

                    new ValuesBundle() { Tag = "nft-rule-func", Key = "0", Value = "add rule" },
                    new ValuesBundle() { Tag = "nft-rule-func", Key = "1", Value = "delete rule" },
                    new ValuesBundle() { Tag = "nft-rule-func", Key = "3", Value = "insert rule" },

                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "0", Value = "accept" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "1", Value = "drop" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "2", Value = "reject" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "3", Value = "queue" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "4", Value = "continue" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "5", Value = "jump" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "6", Value = "goto" },

                    new ValuesBundle() { Tag = "nft-rule", Key = "0", Value = "" },
                };
            }

            private static string tag = "[tag:x]";

            public static IEnumerable<CommandsBundleLayout> DefaultCommandsBundle() {
                return new List<CommandsBundleLayout>() {
                    new CommandsBundleLayout() { CommandLayout = $"hostnamectl {tag}" },
                    new CommandsBundleLayout() { CommandLayout = "nft [nft-table-func:x] [nft-table-family:x] [nft-table-name:x]" },
                    new CommandsBundleLayout() { CommandLayout = "nft [nft-chain-func:x] [nft-table-family:x] [nft-table-name:x] [nft-chain-hook:x] { type [nft-chain-type:x] hook [nft-chain-hook:x] priority [nft-chain-priority:0] \\; }" },
                    new CommandsBundleLayout() { CommandLayout = $"nft [nft-rule-func:x] [nft-table-family:x] [nft-chain-hook:x] [nft-rule:0]" },
                };
            }
        }
    }
}
