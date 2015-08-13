
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
using System.Linq;

namespace antdlib.Firewall {
    public class NFTableRepository {
        private static NFTableFile NFT = DeNSo.Session.New.Get<NFTableFile>(n => n._Id == "AD11998C-91E2-48A5-A460-2EB550259A24").FirstOrDefault();

        public static NFTableFile Get() {
            return NFT;
        }

        public static void Create() {
            //DeNSo.Session.New.DeleteAll<NFTableFile>(DeNSo.Session.New.Get<NFTableFile>().ToList());
            var nft = new NFTableFile() {
                _Id = "AD11998C-91E2-48A5-A460-2EB550259A24",
                FileName = "nftable001.list"
            };
            DeNSo.Session.New.Set(nft);
        }

        public static void AddTable(string name, string type) {
            var table = new NFTableTable() {
                Guid = Guid.NewGuid().ToString(),
                Name = name,
                Type = type
            };
            NFT.Tables.Add(table);
            DeNSo.Session.New.Set(NFT);
        }

        public static void AddChain(string tableName, string name, string type, string hook) {
            var table = NFT.Tables.Where(t => t != null && t.Name == tableName).FirstOrDefault();
            NFT.Tables.Remove(table);
            var chain = new NFTableChain() {
                Guid = Guid.NewGuid().ToString(),
                Name = name,
                Type = type,
                Hook = hook
            };
            table.Chain.Add(chain);
            NFT.Tables.Add(table);
            DeNSo.Session.New.Set(NFT);
        }

        public static void AddRules(string tableName, string chainName, string[] rules, string rulesGuid = null) {
            var table = NFT.Tables.Where(t => t != null && t.Name == tableName).FirstOrDefault();
            NFT.Tables.Remove(table);
            var chain = table.Chain.Where(c => c.Name == chainName).FirstOrDefault();
            table.Chain.Remove(chain);
            if (rulesGuid != null) {
                var ruleSet = chain.Rules.Where(r => r.Guid == rulesGuid).FirstOrDefault();
                chain.Rules.Remove(ruleSet);
            }
            foreach (var value in rules) {
                var rule = new NFTableRule() {
                    Guid = Guid.NewGuid().ToString(),
                    Value = value
                };
                chain.Rules.Add(rule);
            }
            table.Chain.Add(chain);
            NFT.Tables.Add(table);
            DeNSo.Session.New.Set(NFT);
        }
    }
}
