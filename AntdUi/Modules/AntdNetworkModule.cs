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

using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using anthilla.core;

namespace AntdUi.Modules {
    public class AntdNetworkModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AntdNetworkModule() {

            Get["/network"] = x => {
                var model = _api.Get<PageNetworkModel>($"http://127.0.0.1:{Application.ServerPort}/network");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/network/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network/restart");
            };

            Post["/network/interface"] = x => {
                string Interface = Request.Form.Interface;
                string mode = Request.Form.Mode;
                string status = Request.Form.Status;
                string staticAddres = Request.Form.StaticAddres;
                string staticRange = Request.Form.StaticRange;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string route = Request.Form.Route;
                string gateway = Request.Form.Gateway;
                var dict = new Dictionary<string, string> {
                    { "Interface", Interface },
                    { "Mode", mode },
                    { "Status", status },
                    { "StaticAddres", staticAddres },
                    { "StaticRange", staticRange },
                    { "Txqueuelen", txqueuelen },
                    { "Mtu", mtu },
                    { "Route", route },
                    { "Gateway", gateway }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network/interface", dict);
            };

            Post["/network/interface/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network/interface/del", dict);
            };

            Post["/network/interface/bridge"] = x => {
                string Interface = Request.Form.Interface;
                string mode = Request.Form.Mode;
                string status = Request.Form.Status;
                string staticAddres = Request.Form.StaticAddres;
                string staticRange = Request.Form.StaticRange;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string ifs = Request.Form.InterfaceList;
                string route = Request.Form.Route;
                string gateway = Request.Form.Gateway;
                ConsoleLogger.Log("");
                ConsoleLogger.Log(ifs);
                ConsoleLogger.Log("");
                var dict = new Dictionary<string, string> {
                    { "Interface", Interface },
                    { "Mode", mode },
                    { "Status", status },
                    { "StaticAddres", staticAddres },
                    { "StaticRange", staticRange },
                    { "Txqueuelen", txqueuelen },
                    { "Mtu", mtu },
                    { "InterfaceList", ifs },
                    { "Route", route },
                    { "Gateway", gateway }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network/interface/bridge", dict);
            };

            Post["/network/interface/bond"] = x => {
                string Interface = Request.Form.Interface;
                string mode = Request.Form.Mode;
                string status = Request.Form.Status;
                string staticAddres = Request.Form.StaticAddres;
                string staticRange = Request.Form.StaticRange;
                string txqueuelen = Request.Form.Txqueuelen;
                string mtu = Request.Form.Mtu;
                string ifs = Request.Form.InterfaceList;
                string route = Request.Form.Route;
                string gateway = Request.Form.Gateway;
                var dict = new Dictionary<string, string> {
                    { "Interface", Interface },
                    { "Mode", mode },
                    { "Status", status },
                    { "StaticAddres", staticAddres },
                    { "StaticRange", staticRange },
                    { "Txqueuelen", txqueuelen },
                    { "Mtu", mtu },
                    { "InterfaceList", ifs },
                    { "Route", route },
                    { "Gateway", gateway }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/network/interface/bond", dict);
            };
        }
    }
}