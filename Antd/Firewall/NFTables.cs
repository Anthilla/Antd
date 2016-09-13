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
using antdlib.common;
using Antd.Database;

namespace Antd.Firewall {
    public class NfTables {

        private static readonly string FilePath = $"{Parameter.RepoDirs}/FILE_etc_nftables.conf";

        public static void Setup() {
            if (!File.Exists(FilePath)) {
                File.Copy($"{Parameter.Resources}/FILE_etc_nftables.conf", FilePath);
            }
        }

        public static bool ReloadConfiguration() {
            if (File.Exists(FilePath)) {
                return false;
            }
            var result = Terminal.Execute($"nft -f {FilePath}");
            return string.IsNullOrEmpty(result.RemoveWhiteSpace());
        }

        private static readonly NftRepository NftRepository = new NftRepository();

        public static void Import() {
            if (File.Exists(FilePath)) {
                var tryGet = NftRepository.Get();
                if (tryGet == null) {
                    var config = File.ReadAllText(FilePath);
                    NftRepository.Set(config);
                }
            }
        }

        public static void Export() {
            if (File.Exists(FilePath)) {
                var tryGet = NftRepository.Get();
                if (tryGet != null) {
                    var config = tryGet.Configuration;
                    File.WriteAllText(FilePath, config);
                }
            }
        }

        public static void Export(IEnumerable<NftModel.Table> tables) {
            if (File.Exists(FilePath)) {
                File.Delete(FilePath);
            }
            var lines = new List<string>();
            lines.Add("flush ruleset;");
            lines.Add("flush ruleset;");
            lines.Add("flush ruleset;");
            lines.Add("flush ruleset;");
            lines.Add("flush ruleset;");
            foreach (var table in tables) {
                lines.Add($"table {table.Type} {table.Name} {{");
                foreach (var set in table.Sets) {
                    lines.Add($"    set {set.Name} {{");
                    lines.Add($"        type {set.Type}");
                    lines.Add($"        elements = {{ {set.Elements}}}");
                    lines.Add("    }");
                }
                foreach (var chain in table.Chains) {
                    lines.Add($"    chain {chain.Name} {{");
                    foreach (var rule in chain.Rules) {
                        lines.Add($"        {rule}");
                    }
                    lines.Add("    }");

                }
                lines.Add("}");
            }
            File.WriteAllLines(FilePath, lines);
            ReloadConfiguration();
        }

        public static IEnumerable<NftModel.Table> Tables() {
            var list = new List<NftModel.Table>();
            if (File.Exists(FilePath)) {
                var result = Terminal.Execute("nft list tables").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var match in result) {
                    var arr = match.Replace("table", "").Trim().Split(new[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);
                    var tableType = arr[0];
                    var tableName = arr[1];
                    var sets = Sets(tableType, tableName);
                    var chains = Chains(tableType, tableName);
                    var table = new NftModel.Table {
                        Type = tableType,
                        Name = tableName,
                        Sets = sets,
                        Chains = chains
                    };
                    list.Add(table);
                }
            }
            return list;
        }

        public static List<NftModel.Set> Sets(string tableType, string tableName) {
            var list = new List<NftModel.Set>();
            var result = Terminal.Execute($"nft list table {tableType} {tableName}");
            var matches = new Regex("set [\\w\\d]* {", RegexOptions.Multiline).Matches(result);
            foreach (var match in matches) {
                var arr = match.ToString().Replace(new[] { " {", "}", "set " }, "").Trim().Split(new[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);
                var setName = arr[0];
                var setResult = Terminal.Execute($"nft list set {tableType} {tableName} {setName}");
                var lines = setResult.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var typeLine = lines.FirstOrDefault(_ => _.Contains("type "));
                var type = typeLine?.Split(new[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries).Last();
                var elementLine = lines.FirstOrDefault(_ => _.Contains("elements ="));
                var elements = elementLine?.Split(new[] { "{" }, 2, StringSplitOptions.RemoveEmptyEntries).Last().Replace("{", "").Replace("}", "");
                var set = new NftModel.Set {
                    Name = setName,
                    Type = type,
                    Elements = elements?.Trim().RemoveWhiteSpaceFromStart().RemoveDoubleSpace()
                };
                list.Add(set);
            }
            return list;
        }

        public static List<NftModel.Chain> Chains(string tableType, string tableName) {
            var list = new List<NftModel.Chain>();
            var result = Terminal.Execute($"nft list table {tableType} {tableName}");
            var matches = new Regex("chain [\\w\\d]* {", RegexOptions.Multiline).Matches(result);
            foreach (var match in matches) {
                var arr = match.ToString().Replace(new[] { "chain", "{", "}" }, "").Trim().Split(new[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);
                var chainName = arr[0];
                var chainResult = Terminal.Execute($"nft list chain {tableType} {tableName} {chainName}");
                var lines = chainResult.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(2).ToList();
                var rules = lines.Take(lines.Count - 2).Select(_ => _.RemoveWhiteSpaceFromStart().Replace("\t", "").RemoveDoubleSpace()).ToList();
                var chain = new NftModel.Chain {
                    Name = chainName,
                    Rules = rules,
                    RulesString = string.Join(Environment.NewLine, rules)
                };
                list.Add(chain);
            }
            return list;
        }
    }
}
