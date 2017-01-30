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

using System.Linq;
using antdlib.common;
using Antd.Firewall;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ServiceFirewallModule : NancyModule {

        public ServiceFirewallModule() {
            this.RequiresAuthentication();

            Post["/services/firewall/set"] = x => {
                var firewallConfiguration = new FirewallConfiguration();
                firewallConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/restart"] = x => {
                var firewallConfiguration = new FirewallConfiguration();
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/stop"] = x => {
                var firewallConfiguration = new FirewallConfiguration();
                firewallConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/enable"] = x => {
                var firewallConfiguration = new FirewallConfiguration();
                firewallConfiguration.Enable();
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/disable"] = x => {
                var firewallConfiguration = new FirewallConfiguration();
                firewallConfiguration.Disable();
                firewallConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            #region [    IPV4    ]
            Post["/services/firewall/ipv4/filter/set"] = x => {
                string set = Request.Form.Set;
                string type = Request.Form.Type;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(set) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4FilterTable;
                var sets = table.Sets.ToList();
                var tryGetSet = sets.FirstOrDefault(_ => _.Name == set && _.Type == type);
                if(tryGetSet == null) {
                    sets.Add(new FirewallSet { Name = set, Type = type, Elements = elements.SplitToList().ToArray() });
                }
                else {
                    sets.Remove(tryGetSet);
                    tryGetSet.Elements = elements.SplitToList().ToArray();
                    sets.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/ipv4/filter/chain"] = x => {
                string chain = Request.Form.Chain;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(chain) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4FilterTable;
                var chains = table.Chains.ToList();
                var tryGetSet = chains.FirstOrDefault(_ => _.Name == chain);
                if(tryGetSet == null) {
                    chains.Add(new FirewallChain { Name = chain, Rules = elements.SplitToList().ToArray() });
                }
                else {
                    chains.Remove(tryGetSet);
                    tryGetSet.Rules = elements.SplitToList().ToArray();
                    chains.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/ipv4/nat/set"] = x => {
                string set = Request.Form.Set;
                string type = Request.Form.Type;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(set) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4NatTable;
                var sets = table.Sets.ToList();
                var tryGetSet = sets.FirstOrDefault(_ => _.Name == set && _.Type == type);
                if(tryGetSet == null) {
                    sets.Add(new FirewallSet { Name = set, Type = type, Elements = elements.SplitToList().ToArray() });
                }
                else {
                    sets.Remove(tryGetSet);
                    tryGetSet.Elements = elements.SplitToList().ToArray();
                    sets.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/ipv4/nat/chain"] = x => {
                string chain = Request.Form.Chain;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(chain) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4NatTable;
                var chains = table.Chains.ToList();
                var tryGetSet = chains.FirstOrDefault(_ => _.Name == chain);
                if(tryGetSet == null) {
                    chains.Add(new FirewallChain { Name = chain, Rules = elements.SplitToList().ToArray() });
                }
                else {
                    chains.Remove(tryGetSet);
                    tryGetSet.Rules = elements.SplitToList().ToArray();
                    chains.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };
            #endregion

            #region [    IPV6    ]
            Post["/services/firewall/ipv4/filter/set"] = x => {
                string set = Request.Form.Set;
                string type = Request.Form.Type;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(set) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4FilterTable;
                var sets = table.Sets.ToList();
                var tryGetSet = sets.FirstOrDefault(_ => _.Name == set && _.Type == type);
                if(tryGetSet == null) {
                    sets.Add(new FirewallSet { Name = set, Type = type, Elements = elements.SplitToList().ToArray() });
                }
                else {
                    sets.Remove(tryGetSet);
                    tryGetSet.Elements = elements.SplitToList().ToArray();
                    sets.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/ipv4/filter/chain"] = x => {
                string chain = Request.Form.Chain;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(chain) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4FilterTable;
                var chains = table.Chains.ToList();
                var tryGetSet = chains.FirstOrDefault(_ => _.Name == chain);
                if(tryGetSet == null) {
                    chains.Add(new FirewallChain { Name = chain, Rules = elements.SplitToList().ToArray() });
                }
                else {
                    chains.Remove(tryGetSet);
                    tryGetSet.Rules = elements.SplitToList().ToArray();
                    chains.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/ipv4/nat/set"] = x => {
                string set = Request.Form.Set;
                string type = Request.Form.Type;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(set) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4NatTable;
                var sets = table.Sets.ToList();
                var tryGetSet = sets.FirstOrDefault(_ => _.Name == set && _.Type == type);
                if(tryGetSet == null) {
                    sets.Add(new FirewallSet { Name = set, Type = type, Elements = elements.SplitToList().ToArray() });
                }
                else {
                    sets.Remove(tryGetSet);
                    tryGetSet.Elements = elements.SplitToList().ToArray();
                    sets.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/firewall/ipv4/nat/chain"] = x => {
                string chain = Request.Form.Chain;
                string elements = Request.Form.Elements;
                if(string.IsNullOrEmpty(chain) || string.IsNullOrEmpty(elements)) {
                    return HttpStatusCode.BadRequest;
                }
                var firewallConfiguration = new FirewallConfiguration();
                var table = firewallConfiguration.Get().Ipv4NatTable;
                var chains = table.Chains.ToList();
                var tryGetSet = chains.FirstOrDefault(_ => _.Name == chain);
                if(tryGetSet == null) {
                    chains.Add(new FirewallChain { Name = chain, Rules = elements.SplitToList().ToArray() });
                }
                else {
                    chains.Remove(tryGetSet);
                    tryGetSet.Rules = elements.SplitToList().ToArray();
                    chains.Add(tryGetSet);
                }
                firewallConfiguration.Start();
                return HttpStatusCode.OK;
            };
            #endregion
        }
    }
}