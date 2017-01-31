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
using antdlib.common;
using antdlib.config;
using Antd.Database;
using Nancy;

namespace Antd.Modules {
    public class HandshakeModule : NancyModule {

        public HandshakeModule() {

            #region [    Actions    ]
            Get["/asset/handshake"] = x => {
                var hostIp = Request.Query.Host;
                var hostPort = Request.Query.Port;
                if(string.IsNullOrEmpty(hostPort) || string.IsNullOrEmpty(hostIp)) {
                    return HttpStatusCode.NotAcceptable;
                }
                const string pathToPrivateKey = "/root/.ssh/id_rsa";
                const string pathToPublicKey = "/root/.ssh/id_rsa.pub";
                if(!File.Exists(pathToPublicKey)) {
                    var bash = new Bash();
                    var k = bash.Execute($"ssh-keygen -t rsa -N '' -f {pathToPrivateKey}");
                    ConsoleLogger.Log(k);
                }
                var key = File.ReadAllText(pathToPublicKey);
                if(string.IsNullOrEmpty(key)) {
                    return HttpStatusCode.InternalServerError;
                }
                var dict = new Dictionary<string, string> { { "ApplePie", key } };
                var r = new ApiConsumer().Post($"http://{hostIp}:{hostPort}/asset/handshake", dict);
                var kh = new SshKnownHosts();
                kh.Add(hostIp);
                return r;
            };

            Post["/asset/handshake"] = x => {
                string apple = Request.Form.ApplePie;
                if(string.IsNullOrEmpty(apple)) {
                    return HttpStatusCode.InternalServerError;
                }
                var info = apple.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(info.Length < 2) {
                    return HttpStatusCode.InternalServerError;
                }
                var key = info[0];
                var remoteUser = info[1];
                const string user = "root";
                var authorizedKeysRepository = new AuthorizedKeysRepository();
                var r = authorizedKeysRepository.Create2(remoteUser, user, key);
                try {
                    Directory.CreateDirectory("/root/.ssh");
                    const string authorizedKeysPath = "/root/.ssh/authorized_keys";
                    if(File.Exists(authorizedKeysPath)) {
                        var f = File.ReadAllText(authorizedKeysPath);
                        if(!f.Contains(apple)) {
                            File.AppendAllLines(authorizedKeysPath, new List<string> { apple });
                        }
                    }
                    else {
                        File.WriteAllLines(authorizedKeysPath, new List<string> { apple });
                    }
                    var bash = new Bash();
                    bash.Execute($"chmod 600 {authorizedKeysPath}", false);
                    bash.Execute($"chown {user}:{user} {authorizedKeysPath}", false);
                    return r ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                }
                catch(Exception ex) {
                    ConsoleLogger.Log(ex);
                    return HttpStatusCode.InternalServerError;
                }
            };
            #endregion

            #region [    Hooks    ]
            After += ctx => {
                if(ctx.Response.ContentType == "text/html") {
                    ctx.Response.ContentType = "text/html; charset=utf-8";
                }
            };
            #endregion
        }
    }
}