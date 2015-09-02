
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
        public static void SaveRuleSet(string table, string type, string hook, string rulesForIp, string rulesForIp6, string rulesForArp,string rulesForBridge) {
            Log.Logger.TraceMethod("NFTables", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var set = new NFTableRuleSet() {
                _Id = $"{table}-{type}-{hook}",
                Guid = Guid.NewGuid().ToString(),
                Table = table,
                Type = type,
                Hook = hook,
                Priority = 0
            };
            set.RulesForIp = rulesForIp;
            set.RulesForIp6 = rulesForIp6;
            set.RulesForArp = rulesForArp;
            set.RulesForBridge = rulesForBridge;
            DeNSo.Session.New.Set(set);
        }

        public static NFTableRuleSet GetRuleSet(string table, string type, string hook) {
            Log.Logger.TraceMethod("NFTables", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var ruleset = DeNSo.Session.New.Get<NFTableRuleSet>(t => t.Table == table && t.Type == type && t.Hook == hook).FirstOrDefault();
            return ruleset;
        }

        public static NFTableRuleSet[] GetAll() {
            Log.Logger.TraceMethod("NFTables", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var ruleset = DeNSo.Session.New.Get<NFTableRuleSet>().ToArray();
            return ruleset;
        }
    }
}
