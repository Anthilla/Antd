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
using System.Linq;
using System.Text;
using antd.commands;
using antdlib.common;
using Nancy;
using Nancy.Responses;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Modules {

    public class CommandModule : CoreModule {
        private readonly CommandLauncher _launcher = new CommandLauncher();

        public CommandModule() {
            this.RequiresAuthentication();

            Get["/cmd/launch/{name}"] = x => {
                string name = x.name;
                try {
                    var result = _launcher.Launch(name);
                    return JsonConvert.SerializeObject(result, Formatting.Indented);
                }
                catch(Exception ex) {
                    return JsonConvert.SerializeObject(ex, Formatting.Indented);
                }
            };

            Get["/cmd/launch/{name}/{values}"] = x => {
                string name = x.name;
                string strValues = x.values;
                if(!string.IsNullOrEmpty(strValues)) {
                    try {
                        var dict = strValues.SplitToList(";")
                            .Select(kv => kv.SplitToList(":").ToArray())
                            .ToDictionary(s => s.First(), s => s.Last());
                        var result = _launcher.Launch(name, dict);
                        return JsonConvert.SerializeObject(result, Formatting.Indented);
                    }
                    catch(Exception ex) {
                        return JsonConvert.SerializeObject(ex, Formatting.Indented);
                    }
                }
                return HttpStatusCode.InternalServerError;
            };

            Post["/cmd/launch"] = x => {
                string name = Request.Form.Command;
                string strValues = Request.Form.Matches;
                if(!string.IsNullOrEmpty(strValues)) {
                    try {
                        var dict = strValues.SplitToList(";")
                            .Select(kv => kv.SplitToList(":").ToArray())
                            .ToDictionary(s => s.First(), s => s.Last());
                        var result = _launcher.Launch(name, dict);
                        return JsonConvert.SerializeObject(result, Formatting.Indented);
                    }
                    catch(Exception ex) {
                        return JsonConvert.SerializeObject(ex, Formatting.Indented);
                    }
                }
                try {
                    var result = _launcher.Launch(name);
                    return JsonConvert.SerializeObject(result, Formatting.Indented);
                }
                catch(Exception ex) {
                    return JsonConvert.SerializeObject(ex, Formatting.Indented);
                }
            };

            Get["/cmd/download"] = x => {
                var header = "NAME,ARGS,GREP,FUNCTION\n";
                foreach(var cmd in Commands.List) {
                    var val = cmd.Value as Command;
                    header += $"{cmd.Key},{val?.Arguments.JoinToString(" ")},{val?.Grep},{val?.Function.ToString()}\n";
                }
                header = header.TrimEnd('\n');
                var byteArray = Encoding.UTF8.GetBytes(header);
                var file = new MemoryStream(byteArray);
                const string fileName = "antd.cmd.csv";
                var response = new StreamResponse(() => file, MimeTypes.GetMimeType(fileName));
                return response.AsAttachment(fileName);
            };
        }
    }
}