
using antdlib.Config;
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
using antdlib.Common;

namespace antdlib.Firewall {
    public class NfTables {
        public static IEnumerable<ConfigManagement.CommandsBundle> GetNftCommandsBundle() {
            return ConfigManagement.GetCommandsBundle().Where(_ => _.Command.StartsWith("nft"));
        }

        public static List<string> GetRulesFromCommand(string table, string chain, string hook) {
            var list = new List<string>();
            var query = $"nft add rule {table} {chain} {hook}";
            list.AddRange(GetNftCommandsBundle().Where(_ => _.Command.Contains(query)).Select(commandFound => commandFound.Command.Replace(query, "").Trim()));
            return list;
        }

        public static void AddNftRule(string prefix, string rule) {
            ConfigManagement.AddCommandsBundle($"{prefix} {rule}");
            if (rule.Length > 0) {
                Terminal.Execute($"{prefix} {rule}");
            }
        }

        public static void DeleteNftRule(string guid) {
            var command = ConfigManagement.GetCommandsBundle().Where(_ => _.Guid == guid).Select(_ => _.Command).FirstOrDefault();
            if (command == null || command.Length <= 0)
                return;
            var chain = command.Split(' ')[4];
            var hook = command.Split(' ')[5];
            var checkTables = Terminal.Execute($"nft list table {chain} -a | grep \"{command}\"");
            if (checkTables.Length > 0) {
                var handle = checkTables.Split(' ').Last();
                Terminal.Execute($"nft delete rule {chain} {hook} handle {handle}");
            }
            ConfigManagement.DeleteCommandsBundle(guid);
        }

        public class Export {
            public class NfTableRuleModel {
                public string _Id { get; set; }
                public string Guid { get; set; }
                public string Table { get; set; }
                public string Type { get; set; }
                public string Hook { get; set; }
                public string Rule { get; set; }
            }

            private static IEnumerable<string> GetConfigurationFileNames() {
                return Directory.EnumerateFiles(Folder.Config, "*firewall.export", SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
            }

            private static string GetLastFileName() {
                if (!GetConfigurationFileNames().Any())
                    return "0_antd_firewall.export";
                var lastFile = GetConfigurationFileNames().Last();
                var num = Convert.ToInt32(lastFile.Split('_')[0]);
                return $"{(num + 1)}_antd_firewall.export";
            }

            private static readonly string[] ChainType = { "filter", "route", "nat" };
            private static readonly string[] HookType = { "prerouting", "input", "forward", "output", "postrouting" };
            private static readonly string[] TableType = { "ip", "ip6", "arp", "bridge" };

            public static void WriteFile() {
                FlushDb();
                SaveRules();
                using (var sw = File.CreateText(Path.Combine(Folder.Config, GetLastFileName()))) {
                    sw.WriteLine("flush ruleset;");
                    sw.WriteLine("");
                    foreach (var t3 in TableType) {
                        foreach (var t2 in ChainType) {
                            foreach (var t1 in HookType) {
                                var ruleset = GetRules(t3, t2, t1);
                                if (!ruleset.Any())
                                    continue;
                                sw.WriteLine($"table {t3} {t2} {{");
                                sw.WriteLine($"    chain {t2} {t1} {{");
                                sw.WriteLine($"        type {t2} {t1} priority 0;");
                                foreach (var rule in ruleset) {
                                    sw.WriteLine($"        {rule};");
                                }
                                sw.WriteLine("    }");
                                sw.WriteLine("}");
                            }
                        }
                    }
                }
            }

            private static IEnumerable<NfTableRuleModel> GetAll() {
                return DeNSo.Session.New.Get<NfTableRuleModel>();
            }

            private static void FlushDb() {
                foreach (var rule in GetAll()) {
                    DeNSo.Session.New.Delete(rule);
                }
            }

            private static IEnumerable<string> GetRules(string table, string type, string hook) {
                return DeNSo.Session.New.Get<NfTableRuleModel>().Where(_ => _.Table == table && _.Type == type && _.Hook == hook).Select(_ => _.Rule);
            }

            private static void SaveRules() {
                var q = "nft add rule";
                var commands = GetNftCommandsBundle().Where(_ => _.Command.StartsWith(q));
                foreach (var a in commands.Select(command => command.Command.Replace(q, "").Trim()).Select(c => c.Split(' ')).Where(a => a.Length > 3)) {
                    SaveRule(a[0], a[1], a[2], string.Join(" ", a.SubArray(3, (a.Length) - 3)));
                }
            }

            private static void SaveRule(string table, string type, string hook, string rule) {
                var set = new NfTableRuleModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    Table = table,
                    Type = type,
                    Hook = hook,
                    Rule = rule
                };
                DeNSo.Session.New.Set(set);
            }
        }
    }
}
