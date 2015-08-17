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
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                //vmod.FirewallMaximumStates = "9000";
                //vmod.FirewallMaximumTableEntries = "20000";
                //vmod.AliasesHostnamesResolveInterval = "300";
                //vmod.CurrentContext = this.Request.Path;
                //vmod.CCTable = CCTableRepository.GetAllByContext(this.Request.Path);
                //vmod.Count = CCTableRepository.GetAllByContext(this.Request.Path).ToArray().Length;
                //vmod.DisplayTable = (NFTableRepository.Get() == null) ? false : true;
                //vmod.Table = NFTableRepository.Get();
                return View["_page-firewall-nft", vmod];
            };

            Post["/rule"] = x => {
                var type = (string)Request.Form.Type;
                var hook = (string)Request.Form.Hook;
                var rules = (string)Request.Form.Ruleset;
                Console.WriteLine(type, hook, rules);
                NFTableRepository.SaveRuleSet(type, hook, rules);
                return Response.AsRedirect("/firewall");
            };

            Get["/rule/{type}/{hook}"] = x => {
                string type = x.type;
                string hook = x.hook;
                var rules = NFTableRepository.GetRuleSet(type, hook);
                return Response.AsJson(rules);
            };

            Get["/nft"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.DisplayTable = (NFTableRepository.Get() == null) ? false : true;
                vmod.Table = NFTableRepository.Get();
                return View["_page-firewall-nft", vmod];
            };

            Post["/nft"] = x => {
                NFTableRepository.Create();
                return Response.AsJson(true);
            };

            Post["/nft/table"] = x => {
                var type = Request.Form.TableType;
                var name = Request.Form.TableName;
                NFTableRepository.AddTable(type, name);
                return Response.AsRedirect("/firewall/nft");
            };

            Post["/nft/chain"] = x => {
                var tableGuid = Request.Form.TableGuid;
                var name = Request.Form.ChainName;
                var type = Request.Form.ChainType;
                var hook = Request.Form.ChainHook;
                NFTableRepository.AddChain(tableGuid, name, type, hook);
                return Response.AsRedirect("/firewall/nft");
            };

            Post["/nft/rule"] = x => {
                var tableGuid = Request.Form.TableGuid;
                var chainGuid = Request.Form.ChainGuid;
                var rules = (string)Request.Form.Rules;
                NFTableRepository.AddRules(tableGuid, chainGuid, rules.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray());
                return Response.AsRedirect("/firewall/nft");
            };
        }
    }
}