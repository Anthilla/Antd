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
using antdlib.config;
using antdlib.models;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;

namespace Antd.Modules {

    public class AuthorizedKeysModule : NancyModule {

        public AuthorizedKeysModule() {

            Post["/ak/create"] = x => {
                string remoteUser = Request.Form.RemoteUser;
                string user = Request.Form.User;
                string key = Request.Form.Key;
                var model = new AuthorizedKeyModel {
                    RemoteUser = remoteUser,
                    User = user,
                    KeyValue = key
                };
                var authorizedKeysConfiguration = new AuthorizedKeysConfiguration();
                authorizedKeysConfiguration.AddKey(model);
                var home = user == "root" ? "/root/.ssh" : $"/home/{user}/.ssh";
                var authorizedKeysPath = $"{home}/authorized_keys";
                if(!File.Exists(authorizedKeysPath)) {
                    File.Create(authorizedKeysPath);
                }
                var line = $"{key} {remoteUser}";
                FileWithAcl.AppendAllLines(authorizedKeysPath, new List<string> { line }, "644", "root", "wheel");
                Bash.Execute($"chmod 600 {authorizedKeysPath}", false);
                Bash.Execute($"chown {user}:{user} {authorizedKeysPath}", false);
                return HttpStatusCode.OK;
            };

            Get["/ak/introduce"] = x => {
                var remoteHost = Request.Query.Host;
                var remoteUser = $"{Environment.UserName}@{Environment.MachineName}";
                Console.WriteLine(remoteUser);
                string user = Request.Query.User;
                string key = Request.Query.Key;
                var dict = new Dictionary<string, string> {
                    {"RemoteUser", remoteUser},
                    {"User", user},
                    {"Key", key}
                };
                var r = new ApiConsumer().Post($"http://{remoteHost}/ak/create", dict);
                return r;
            };

            Post["/ak/introduce"] = x => {
                var remoteHost = Request.Form.Host;
                var remoteUser = $"{Environment.UserName}@{Environment.MachineName}";
                Console.WriteLine(remoteUser);
                string user = Request.Form.User;
                string key = Request.Form.Key;
                var dict = new Dictionary<string, string> {
                    {"RemoteUser", remoteUser},
                    {"User", user},
                    {"Key", key}
                };
                var r = new ApiConsumer().Post($"http://{remoteHost}/ak/create", dict);
                return r;
            };
        }
    }
}