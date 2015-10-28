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
            public string RegexTag { get { return $"{{{Tag}:{Key}}}"; } }
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
            var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
            getmodel.IsEnabled = true;
            DeNSo.Session.New.Set(getmodel);
        }

        public static void DisableCommand(string guid) {
            var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
            getmodel.IsEnabled = false;
            DeNSo.Session.New.Set(getmodel);
        }

        public static void DeleteCommandsBundle(string guid) {
            var getmodel = GetCommandsBundle().Where(vv => vv.Guid == guid).FirstOrDefault();
            if (getmodel != null) {
                DeNSo.Session.New.Delete(getmodel);
            }
        }

        public static string SupposeCommandReplacement(string command) {
            string memCommamnd = command;
            var matches = new Regex("{(.*?)}").Matches(command);
            if (matches.Count > 0) {
                for (int i = 0; i < matches.Count; i++) {
                    var taginfo = matches[i].Value.Replace("{", "").Replace("}", "").Trim().Split(':');
                    var value = GetValuesBundleValue(taginfo[0], taginfo[1]);
                    var stringToReplace = $"{{{string.Join(":", taginfo)}}}";
                    if (value == null) {
                        memCommamnd = memCommamnd.Replace(stringToReplace, "[valueNOTfound]");
                    }
                    else {
                        memCommamnd = memCommamnd.Replace(stringToReplace, value);
                    }
                }
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
                return Directory.EnumerateFiles(configFolder, "*.export", SearchOption.TopDirectoryOnly).Select(f => Path.GetFileName(f));
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
            private static IEnumerable<ValuesBundle> DefaultNFTTableType() {
                return new List<ValuesBundle> {
                    new ValuesBundle() { Tag = "nft-tables", Key = "0", Value = "ip" },
                    new ValuesBundle() { Tag = "nft-tables", Key = "1", Value = "ip6" },
                    new ValuesBundle() { Tag = "nft-tables", Key = "2", Value = "arp" },
                    new ValuesBundle() { Tag = "nft-tables", Key = "3", Value = "bridge" },
                };
            }

            private static IEnumerable<ValuesBundle> DefaultNFTChainType() {
                return new List<ValuesBundle> {
                    new ValuesBundle() { Tag = "nft-chains", Key = "0", Value = "filter" },
                    new ValuesBundle() { Tag = "nft-chains", Key = "1", Value = "route" },
                    new ValuesBundle() { Tag = "nft-chains", Key = "2", Value = "nat" },
                };
            }

            private static IEnumerable<ValuesBundle> DefaultNFTHookType() {
                return new List<ValuesBundle> {
                    new ValuesBundle() { Tag = "nft-hooks", Key = "0", Value = "prerouting" },
                    new ValuesBundle() { Tag = "nft-hooks", Key = "1", Value = "input" },
                    new ValuesBundle() { Tag = "nft-hooks", Key = "2", Value = "forward" },
                    new ValuesBundle() { Tag = "nft-hooks", Key = "3", Value = "output" },
                    new ValuesBundle() { Tag = "nft-hooks", Key = "4", Value = "postrouting" },
                };
            }

            private static IEnumerable<ValuesBundle> DefaultNFTHookTypeForArp() {
                return new List<ValuesBundle> {
                    new ValuesBundle() { Tag = "nft-arp-hooks", Key = "0", Value = "input" },
                    new ValuesBundle() { Tag = "nft-arp-hooks", Key = "1", Value = "output" },
                };
            }

            private static IEnumerable<ValuesBundle> DefaultNFTHookTypeForBridge() {
                return new List<ValuesBundle> {
                    new ValuesBundle() { Tag = "nft-bridge-hooks", Key = "0", Value = "input" },
                    new ValuesBundle() { Tag = "nft-bridge-hooks", Key = "1", Value = "forward" },
                    new ValuesBundle() { Tag = "nft-bridge-hooks", Key = "2", Value = "output" },
                };
            }

            private static IEnumerable<ValuesBundle> DefaultNFTRuleStatements() {
                return new List<ValuesBundle> {
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "0", Value = "accept" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "1", Value = "drop" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "2", Value = "reject" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "2", Value = "queue" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "2", Value = "continue" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "2", Value = "reject" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "2", Value = "jump" },
                    new ValuesBundle() { Tag = "nft-rule-verbs", Key = "2", Value = "goto" },
                };
            }

            public static IEnumerable<ValuesBundle> DefaultValuesBundle() {
                //todo concat con tutte le liste di default
                return DefaultNFTTableType()
                    .Concat(DefaultNFTChainType())
                    .Concat(DefaultNFTHookType())
                    .Concat(DefaultNFTHookTypeForArp())
                    .Concat(DefaultNFTHookTypeForBridge())
                    .Concat(DefaultNFTRuleStatements())
                    ;
            }

            private static string tag = "{tag:x}";

            public static IEnumerable<CommandsBundleLayout> DefaultCommandsBundle() {
                return new List<CommandsBundleLayout>() {
                    new CommandsBundleLayout() { CommandLayout = $"hostnamectl {tag}" },

                    new CommandsBundleLayout() { CommandLayout = $"nft add table ip {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add table ip6 {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add table arp {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add table bridge {tag}" },

                    new CommandsBundleLayout() { CommandLayout = $"nft -a list table ip {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft -a list table ip6 {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft -a list table arp {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft -a list table bridge {tag}" },

                    new CommandsBundleLayout() { CommandLayout = $"nft delete table ip {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete table ip6 {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete table arp {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete table bridge {tag}" },

                    new CommandsBundleLayout() { CommandLayout = $"nft add chain ip {tag} {tag} (??)" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add chain ip6 {tag} {tag} (??)" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add chain arp {tag} {tag} (??)" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add chain bridge {tag} {tag} (??)" },

                    new CommandsBundleLayout() { CommandLayout = $"nft delete chain ip {tag} {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete chain ip6 {tag} {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete chain arp {tag} {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete chain bridge {tag} {tag}" },

                    new CommandsBundleLayout() { CommandLayout = $"nft add rule ip {tag} {tag} {tag} (??)" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add rule ip6 {tag} {tag} {tag} (??)" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add rule arp {tag} {tag} {tag} (??)" },
                    new CommandsBundleLayout() { CommandLayout = $"nft add rule bridge {tag} {tag} {tag} (??)" },

                    new CommandsBundleLayout() { CommandLayout = $"nft delete rule ip {tag} {tag} {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete rule ip6 {tag} {tag} {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete rule arp {tag} {tag} {tag}" },
                    new CommandsBundleLayout() { CommandLayout = $"nft delete rule bridge {tag} {tag} {tag}" },
                };
            }
        }
    }
}
