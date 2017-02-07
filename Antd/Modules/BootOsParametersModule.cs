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
using System.Linq;
using antdlib.common;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class BootOsParametersModule : NancyModule {

        public BootOsParametersModule() {
            Get["/boot/osparameter"] = x => {
                var hostcfg = new HostConfiguration();
                var model = new PageBootOsParametersModel {
                    OsParameters = string.Join(Environment.NewLine, hostcfg.GetHostOsParameters().Select(_ => $"{_.Key} {_.Value}").ToList())
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/boot/osparameter"] = x => {
                string osparamText = Request.Form.Config;
                var osparameter = osparamText.SplitToList(Environment.NewLine).Where(_ => !string.IsNullOrEmpty(_));
                var dict = new Dictionary<string, string>();
                foreach(var serv in osparameter) {
                    var kvp = serv.Split(new[] { " " }, 2, StringSplitOptions.None);
                    if(!dict.ContainsKey(kvp[0])) {
                        dict.Add(kvp[0], kvp[1]);
                    }
                }
                var hostcfg = new HostConfiguration();
                hostcfg.SetHostOsParameters(dict);
                hostcfg.ApplyHostOsParameters();
                return Response.AsRedirect("/boot");
            };
        }
    }
}