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
                vmod.Command = _commandRepo.GetAll();
                vmod.Value = _commandValuesRepo.GetAll();
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
                    {"Command", command},
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
                    {"Command", command},
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
                var result = _commandRepo.GetAll().ToDictionary(_ => _.Name, _ => _.Command).OrderBy(_ => _.Key);
                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                var path = $"{Parameter.AntdCfgCommands}/commands.json";
                if (File.Exists(path)) {
                    File.Delete(path);
                }
                File.WriteAllText(path, json);

                var result2 = _commandValuesRepo.GetAll().ToDictionary(_ => _.Name, _ => _.Value ?? "").OrderBy(_ => _.Key);
                var json2 = JsonConvert.SerializeObject(result2, Formatting.Indented);
                var path2 = $"{Parameter.AntdCfgCommands}/values.json";
                if (File.Exists(path2)) {
                    File.Delete(path2);
                }
                File.WriteAllText(path2, json2);

                return Response.AsRedirect("/cmd");
            };

            //Post["/command/mgmt/launch/{guid}"] = x => {
            //    string guid = x.guid;
            //    var result = _commandRepositoryRepo.Launch(guid);
            //    return Response.AsJson(result);
            //};

            //Post["/command/mgmt/delete/{guid}"] = x => {
            //    string guid = x.guid;
            //    _commandRepositoryRepo.Delete(guid);
            //    return HttpStatusCode.OK;
            //};

            //Get["/command/mgmt/ex/{inputid}"] = x => {
            //    string inputid = x.inputid;
            //    var r = _commandRepositoryRepo.LaunchAndGetOutputUsingNewValue(inputid);
            //    return Response.AsJson(r);
            //};

            //Get["/command/mgmt/ex/{inputid}/{Value}"] = x => {
            //    string inputid = x.inputid;
            //    string value = x.value;
            //    var r = _commandRepositoryRepo.LaunchAndGetOutputUsingNewValue(inputid, value);
            //    return Response.AsJson(r);
            //};
        }
    }
}