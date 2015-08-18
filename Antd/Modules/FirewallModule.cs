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

using antdlib.CCTable;
using antdlib.Firewall;
using Nancy;
using Nancy.Security;
using System;
using System.Dynamic;
using System.Linq;

namespace Antd {

    public class FirewallModule : NancyModule {

        public FirewallModule()
            : base("/firewall") {
            //this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-firewall-nft", vmod];
            };

            Post["/rule"] = x => {
                var type = (string)Request.Form.Type;
                var hook = (string)Request.Form.Hook;
                var rulesForIp = (string)Request.Form.RuleIp;
                var rulesForIp6 = (string)Request.Form.RuleIp6;
                var rulesForBridge = (string)Request.Form.RuleBridge;
                NFTableRepository.SaveRuleSet(type, hook, rulesForIp, rulesForIp6, rulesForBridge);
                return Response.AsRedirect("/firewall");
            };

            Get["/rule/{type}/{hook}"] = x => {
                string type = x.type;
                string hook = x.hook;
                var rules = NFTableRepository.GetRuleSet(type, hook);
                var array = new string[] {
                    (rules == null)? "" : rules.RulesForIp,
                    (rules == null)? "" : rules.RulesForIp6,
                    (rules == null)? "" : rules.RulesForBridge
                };
                return Response.AsJson(array);
            };
        }
    }
}