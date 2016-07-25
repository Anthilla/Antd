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
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;
using Antd.Database;
using Nancy;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Modules {

    public class CommandModule : CoreModule {

        private readonly CommandRepository _commandRepo = new CommandRepository();
        private readonly CommandValuesRepository _commandValuesRepo = new CommandValuesRepository();

        public CommandModule() {
            this.RequiresAuthentication();

            Get["/cmd"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Command = _commandRepo.GetAll().OrderBy(_ => _.Name);
                vmod.Value = _commandValuesRepo.GetAll().OrderBy(_ => _.Name);
                return View["antd/page-cmd", vmod];
            };

            Get["/cmd/commands"] = x => {
                var result = _commandRepo.GetAll();
                return JsonConvert.SerializeObject(result);
            };

            Post["/cmd/commands"] = x => {
                string alias = Request.Form.Name;
                string command = Request.Form.Command;
                _commandRepo.Create(new Dictionary<string, string> {
                    {"Name", alias},
                    {"Command", command.Replace("\n", Environment.NewLine)},
                });
                return HttpStatusCode.OK;
            };

            Put["/cmd/commands"] = x => {
                string id = Request.Form.Id;
                string alias = Request.Form.Name;
                string command = Request.Form.Command;
                _commandRepo.Edit(new Dictionary<string, string> {
                    {"Id", id},
                    {"Name", alias},
                    {"Command", command.Replace("\n", Environment.NewLine)},
                });
                return HttpStatusCode.OK;
            };

            Delete["/cmd/commands"] = x => {
                string id = Request.Form.Id;
                _commandRepo.Delete(id);
                return HttpStatusCode.OK;
            };

            Get["/ex/command/{alias}"] = x => {
                string alias = x.alias;
                var result = _commandRepo.Launch(alias);
                return JsonConvert.SerializeObject(result);
            };

            Get["/cmd/values"] = x => {
                var result = _commandValuesRepo.GetAll();
                return JsonConvert.SerializeObject(result.Select(_ => new KeyValuePair<string, string>(_.Name, _.Value)));
            };

            Post["/cmd/values"] = x => {
                string name = Request.Form.Name;
                if (!name.StartsWith("$")) {
                    name = "$" + name;
                }
                string value = Request.Form.Value;
                _commandValuesRepo.Create(new Dictionary<string, string> {
                    {"Name", name},
                    {"Value", value},
                });
                return HttpStatusCode.OK;
            };

            Put["/cmd/values"] = x => {
                string id = Request.Form.Id;
                string name = Request.Form.Name;
                if (!name.StartsWith("$")) {
                    name = "$" + name;
                }
                string value = Request.Form.Value;
                _commandValuesRepo.Edit(new Dictionary<string, string> {
                    {"Id", id},
                    {"Name", name},
                    {"Value", value},
                });
                return HttpStatusCode.OK;
            };

            Delete["/cmd/values"] = x => {
                string id = Request.Form.Id;
                _commandValuesRepo.Delete(id);
                return HttpStatusCode.OK;
            };

            Get["/cmd/values/{name}"] = x => {
                string name = x.name;
                var result = _commandValuesRepo.GetByName(name);
                return JsonConvert.SerializeObject(result);
            };

            Post["/cmd/export"] = x => {
                Directory.CreateDirectory(Parameter.AntdCfgCommands);
                var result = _commandRepo.GetAll();
                var commandDict = new Dictionary<string, IEnumerable<string>> { { "_version", new List<string> { Timestamp.Now } } };
                foreach (var r in result) {
                    if (!commandDict.ContainsKey(r.Name)) {
                        commandDict.Add(r.Name, r.Command.SplitToList(Environment.NewLine));
                    }
                }
                var json = JsonConvert.SerializeObject(commandDict.OrderBy(_ => _.Key), Formatting.Indented);
                var path = $"{Parameter.AntdCfgCommands}/commands.json";
                if (File.Exists(path)) {
                    File.Delete(path);
                }
                File.WriteAllText(path, json);

                var result2 = _commandValuesRepo.GetAll();
                var valueDict = new Dictionary<string, string> { { "_version", Timestamp.Now } };
                foreach (var r in result2) {
                    if (!valueDict.ContainsKey(r.Name)) {
                        valueDict.Add(r.Name, r.Value);
                    }
                }
                var json2 = JsonConvert.SerializeObject(valueDict.OrderBy(_ => _.Key), Formatting.Indented);
                var path2 = $"{Parameter.AntdCfgCommands}/values.json";
                if (File.Exists(path2)) {
                    File.Delete(path2);
                }
                File.WriteAllText(path2, json2);

                return Response.AsRedirect("/cmd");
            };

            Post["/cmd/launch"] = x => {
                string name = Request.Form.Command;
                string strValues = Request.Form.Matches;
                var cmd = _commandRepo.GetByName(name);
                if (cmd == null) {
                    return string.Empty;
                }
                var command = cmd.Command;
                var matches = Regex.Matches(command, "\\$[a-zA-Z0-9_]+");
                var kvp = strValues.SplitToList(";");
                var values = kvp.Select(kv => kv.SplitToList(":").ToArray()).ToDictionary(s => s.First(), s => s.Last());
                foreach (var match in matches) {
                    var val = values.FirstOrDefault(_ => _.Key == match.ToString());
                    if (string.IsNullOrEmpty(val.Value)) {
                        continue;
                    }
                    command = command.Replace(match.ToString(), val.Value);
                }
                var result = Terminal.Execute(command);
                return Response.AsJson(result);
            };
        }
    }
}