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
using System.Dynamic;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using antdlib;
using antdlib.common;
using Antd.Avahi;
using Nancy;
using Nancy.Security;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace Antd.Modules {
    public class DiscoveryModule : CoreModule {

        public DiscoveryModule() {

            Get["/discovery"] = x => {
                this.RequiresAuthentication();
                dynamic viewModel = new ExpandoObject();

                viewModel.AntdContext = new[] {
                    "Avahi"
                };

                var avahiBrowse = new AvahiBrowse();
                avahiBrowse.DiscoverService("antd");
                var localServices = avahiBrowse.Locals;
                viewModel.AntdAvahiServices = localServices.Select(_ => new KeyValuePair<string, string>(_.Split(':')[0], _.Split(':')[1])).ToList();

                return View["antd/page-discovery", viewModel];
            };

            Get["/disc/hello"] = x => {
                var myIp = "";
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList) {
                    if (ip.AddressFamily == AddressFamily.InterNetwork) {
                        myIp = ip.ToString();
                    }
                }
                var kvp = new KeyValuePair<string, string>(myIp, "1337");
                return Response.AsJson(kvp);
            };

            Get["/disc/lookaround"] = x => {
                ConsoleLogger.Log("hs > 1139");
                var ava = new AvahiBrowse();
                ava.DiscoverService("antd");
                var a = ava.Locals;
                return Response.AsJson(a);
            };

            Get["/disc/handshake"] = x => {
                ConsoleLogger.Log("hs > 1139");
                const string pathToPrivateKey = "/root/.ssh/antd-root-key";
                const string pathToPublicKey = "/root/.ssh/antd-root-key.pub";
                if (!File.Exists(pathToPublicKey)) {
                    ConsoleLogger.Log("hs > create keys");
                    var k = Bash.Execute($"ssh-keygen -t rsa -N '' -f {pathToPrivateKey}");
                    ConsoleLogger.Log(k);
                }
                var ava = new AvahiBrowse();
                ava.DiscoverService("antd");
                var hosts = ava.Locals;

                var key = "";

                try {
                    key = Bash.Execute($"cat {pathToPublicKey}");
                    ConsoleLogger.Log($"hs > {key}");
                }
                catch (Exception ex) {
                    ConsoleLogger.Log($"hs exc1 > {ex}");
                    return HttpStatusCode.ImATeapot;
                }

                try {
                    var key2 = File.ReadAllText(pathToPublicKey);
                    ConsoleLogger.Log($"hs > {key2}");
                }
                catch (Exception ex) {
                    ConsoleLogger.Log($"hs exc2 > {ex}");
                    return HttpStatusCode.ImATeapot;
                }
                //var key = Bash.Execute($"cat {pathToPublicKey}");
                //ConsoleLogger.Log($"hs > {key}");

                //var key2 = File.ReadAllText(pathToPublicKey);
                //ConsoleLogger.Log($"hs > {key2}");

                if (string.IsNullOrEmpty(key)) {
                    ConsoleLogger.Log("hs > no key to share");
                    return HttpStatusCode.ImATeapot;
                }

                foreach (var host in hosts) {
                    ConsoleLogger.Log($"hs > send request to http://{host}/ak/handshake");
                    var dict = new Dictionary<string, string> {
                            {"ApplePie", key}
                        };
                    var r = new ApiConsumer().Post($"http://{host}/ak/handshake", dict);
                    ConsoleLogger.Log($"hs > request result {r}");
                }
                return HttpStatusCode.OK;
            };
        }
    }
}