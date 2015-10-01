
using System;
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
using System.IO;
using System.Linq;

namespace antdlib.Firewall {
    public class NFTables {
        private static string fileName = $"{Folder.Dirs}/{AntdFile.FirewallConfig}";

        public static void Set() {
            Terminal.Execute($"nft -f {fileName}");
        }

        public static void Set(string file) {
            Terminal.Execute($"nft -f {file}");
        }

        private static string[] ChainType = new string[] {
            "filter",
            "route",
            "nat"
        };

        private static string[] HookType = new string[] {
            "prerouting",
            "input",
            "forward",
            "output",
            "postrouting"
        };

        private static string[] HookTypeForArp = new string[] {
            "input",
            "output"
        };

        private static string[] HookTypeForBridge = new string[] {
            "input",
            "forward",
            "output"
        };

        public static void WriteFile() {
            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }
            using (StreamWriter sw = File.CreateText(fileName)) {
                sw.WriteLine("flush ruleset;");
                sw.WriteLine("");
                WriteTable(sw, "ip", ChainType, HookType);
                WriteTable(sw, "ip6", ChainType, HookType);
                WriteTable(sw, "arp", ChainType, HookTypeForArp);
                WriteTable(sw, "bridge", ChainType, HookTypeForBridge);
            }
        }

        private static void WriteTable(StreamWriter sw, string table, string[] chains, string[] hooks) {
            for (int c = 0; c < chains.Length; c++) {
                sw.WriteLine($"table {table} {chains[c]} {{");
                WriteChain(sw, table, chains[c], hooks);
                sw.WriteLine("}");
            }
        }

        private static void WriteChain(StreamWriter sw, string table, string chain, string[] hooks) {
            for (int h = 0; h < hooks.Length; h++) {
                var ruleset = NFTableRepository.GetRuleSet(table, chain, hooks[h]);
                sw.WriteLine($"chain {chain} {hooks[h]} {{");
                if (ruleset != null) {
                    sw.WriteLine($"type {chain} {hooks[h]} priority {ruleset.Priority.ToString()};");
                    WriteRules(sw, table, ruleset);
                }
                else {
                    sw.WriteLine("");
                }
                sw.WriteLine("}");
            }
        }

        private static void WriteRules(StreamWriter sw, string table, NFTableRuleSet ruleset) {
            string[] rules;
            switch (table) {
                case "ip":
                    rules = ruleset.RulesForIp.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    foreach (var line in rules) {
                        sw.WriteLine($"{line}");
                    }
                    break;
                case "ip6":
                    rules = ruleset.RulesForIp6.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    foreach (var line in rules) {
                        sw.WriteLine($"{line}");
                    }
                    break;
                case "arp":
                    rules = ruleset.RulesForArp.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    foreach (var line in rules) {
                        sw.WriteLine($"{line}");
                    }
                    break;
                case "bridge":
                    rules = ruleset.RulesForBridge.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    foreach (var line in rules) {
                        sw.WriteLine($"{line}");
                    }
                    break;
                default:
                    break;
            }
        }

        public static void ReadFile() {
            if (File.Exists(fileName)) {
                //leggi e splitta
            }
        }
    }
}
