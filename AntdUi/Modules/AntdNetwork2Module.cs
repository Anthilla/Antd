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

using antdlib.common;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class AntdNetwork2Module : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AntdNetwork2Module() {

            Get["/network2"] = x => {
                var model = _api.Get<PageNetwork2Model>($"http://127.0.0.1:{Application.ServerPort}/network2");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/network2/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/restart");
            };

            Post["/network2/interfaceconfiguration"] = x => {
                string id = Request.Form.Id;
                string type = Request.Form.Type;
                string description = Request.Form.Description;
                string verb = Request.Form.Verb;
                string mode = Request.Form.Mode;
                string status = Request.Form.Status;
                string ip = Request.Form.Ip;
                string adapter = Request.Form.Adapter;
                string children = Request.Form.Ifs;
                var dict = new Dictionary<string, string> {
                    { "Id", id },
                    { "Type", type },
                    { "Description", description },
                    { "Verb", verb },
                    { "Mode", mode },
                    { "Status", status },
                    { "Ip", ip },
                    { "Adapter", adapter },
                    { "Ifs", children }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/interfaceconfiguration", dict);
            };

            Post["/network2/interfaceconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/interfaceconfiguration/del", dict);
            };

            Post["/network2/gatewayconfiguration"] = x => {
                string id = Request.Form.Id;
                string route = Request.Form.Route;
                string gatewayAddress = Request.Form.GatewayAddress;
                var dict = new Dictionary<string, string> {
                    { "Id", id },
                    { "Route", route },
                    { "GatewayAddress", gatewayAddress }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/gatewayconfiguration", dict);
            };

            Post["/network2/gatewayconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/gatewayconfiguration/del", dict);
            };

            Post["/network2/dnsconfiguration"] = x => {
                string id = Request.Form.Id;
                string type = Request.Form.Type;
                string mode = Request.Form.Mode;
                string dest = Request.Form.Destination;
                string domain = Request.Form.Domain;
                string ip = Request.Form.Ip;
                string auth = Request.Form.Auth;
                var dict = new Dictionary<string, string> {
                    { "Id", id },
                    { "Type", type },
                    { "Mode", mode },
                    { "Destination", dest },
                    { "Domain", domain },
                    { "Ip", ip },
                    { "Auth", auth }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/dnsconfiguration", dict);
            };

            Post["/network2/dnsconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/dnsconfiguration/del", dict);
            };

            Post["/network2/dnsconfiguration/active"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/dnsconfiguration/active", dict);
            };

            Post["/network2/dnsconfiguration/active/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/dnsconfiguration/active/del", dict);
            };

            Post["/network2/nsupdateconfiguration"] = x => {
                string id = Request.Form.Id;
                string serverName = Request.Form.ServerName;
                string serverPort = Request.Form.ServerPort;
                string localAddress = Request.Form.LocalAddress;
                string localPort = Request.Form.LocalPort;
                string zoneName = Request.Form.ZoneName;
                string className = Request.Form.ClassName;
                string nxDomain = Request.Form.NxDomain;
                string yxDomain = Request.Form.YxDomain;
                string nxRrset = Request.Form.NxRrset;
                string yxRrset = Request.Form.YxRrset;
                string delete = Request.Form.Delete;
                string add = Request.Form.Add;
                var dict = new Dictionary<string, string> {
                    { "Id", id },
                    { "ServerName", serverName },
                    { "ServerPort", serverPort },
                    { "LocalAddress", localAddress },
                    { "LocalPort", localPort },
                    { "ZoneName", zoneName },
                    { "ClassName", className },
                    { "NxDomain", nxDomain },
                    { "YxDomain", yxDomain },
                    { "NxRrset", nxRrset },
                    { "YxRrset", yxRrset },
                    { "Delete", delete },
                    { "Add", add }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/nsupdateconfiguration", dict);
            };

            Post["/network2/nsupdateconfiguration/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/nsupdateconfiguration/active/del", dict);
            };

            Post["/network2/interface"] = x => {
                string dev = Request.Form.Device;
                string conf = Request.Form.Configuration;
                string confs = Request.Form.AdditionalConfigurations;
                string gwConf = Request.Form.GatewayConfiguration;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                var dict = new Dictionary<string, string> {
                    { "Device", dev },
                    { "Configuration", conf },
                    { "AdditionalConfigurations", confs },
                    { "GatewayConfiguration", gwConf },
                    { "Txqueuelen", txqueuelen },
                    { "Mtu", mtu }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/interface", dict);
            };

            Post["/network2/interface/del"] = x => {
                string dev = Request.Form.Device;
                var dict = new Dictionary<string, string> {
                    { "Device", dev }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network2/interface/del", dict);
            };


            //bdbdbdbrbrbr
        }
    }
}