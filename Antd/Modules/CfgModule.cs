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
using System.Linq;
using antdlib.common;
using Antd.Configuration;
using Antd.Database;
using Antd.Helpers;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace Antd.Modules {
    public class CfgModule : CoreModule {

        private readonly CommandRepository _commandRepositoryRepo = new CommandRepository();
        private readonly CommandValuesRepository _commandValuesRepositoryRepo = new CommandValuesRepository();

        private readonly BootModuleLoadRepository _bootModuleLoadRepo = new BootModuleLoadRepository();
        private readonly BootServiceLoadRepository _bootServiceLoadRepo = new BootServiceLoadRepository();
        private readonly BootOsParametersLoadRepository _bootOsParametersLoadRepo = new BootOsParametersLoadRepository();

        private readonly SetupConfiguration _setupConfiguration = new SetupConfiguration();

        public CfgModule() {
            this.RequiresAuthentication();

            Get["/cfg"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.HasConfiguration = true;
                vmod.Controls = _setupConfiguration.Get();
                if(_setupConfiguration.Get().Count < 1) {
                    vmod.HasConfiguration = false;
                }

                var modules = _bootModuleLoadRepo.Retrieve();
                vmod.Modules = modules == null ? "" : string.Join("\r\n", modules);
                var services = _bootModuleLoadRepo.Retrieve();
                vmod.Services = services == null ? "" : string.Join("\r\n", services);
                var ospar = _bootOsParametersLoadRepo.Retrieve();
                var osparList = ospar?.Select(_ => $"{_.Key} {_.Value}").ToList();
                vmod.OsParam = osparList == null ? "" : string.Join("\r\n", osparList);

                return View["antd/page-cfg", vmod];
            };

            Post["/cfg/export"] = x => {
                var control = this.Bind<List<Control>>();
                var checkedControl = new List<Control>();
                foreach(var cr in control.Where(_ => !string.IsNullOrEmpty(_.FirstCommand?.Trim())).ToList()) {
                    var s = new Control {
                        Index = cr.Index,
                        FirstCommand = cr.FirstCommand,
                        ControlCommand = string.IsNullOrEmpty(cr.ControlCommand) ? "" : cr.ControlCommand,
                        Check = string.IsNullOrEmpty(cr.Check) ? "" : cr.Check,
                    };

                    checkedControl.Add(s);
                }
                _setupConfiguration.Export(checkedControl);
                return Response.AsRedirect("/cfg");
            };

            Post["/cfg/addvalue"] = x => {
                var name = (string)Request.Form.Name;
                var index = (string)Request.Form.Index;
                var value = (string)Request.Form.Value;
                _commandValuesRepositoryRepo.Create(new Dictionary<string, string> {
                    { "Name", name },
                    { "Index", index },
                    { "Value", value }
                });
                return Response.AsRedirect("/");
            };

            Post["/cfg/delvalue"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandValuesRepositoryRepo.Delete(guid);
                return Response.AsRedirect("/");
            };

            Get["/cfg/tags"] = x => {
                var data = _commandValuesRepositoryRepo.GetAll().Select(_ => _.Name);
                var map = SelectizerMapModel.MapRawTagOfValueBundle(data);
                return Response.AsJson(map);
            };

            Post["/cfg/addcommand"] = x => {
                var command = (string)Request.Form.Command;
                _commandRepositoryRepo.Create(new Dictionary<string, string> {
                    { "Command", command },
                    { "Layout", "" },
                    { "Notes", "" }
                });
                return Response.AsRedirect("/");
            };

            Post["/cfg/delcommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Delete(guid);
                return Response.AsRedirect("/");
            };

            Post["/cfg/enablecommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Edit(new Dictionary<string, string> {
                    { "Id", guid },
                    { "IsEnabled", "true" }
                });
                return HttpStatusCode.OK;
            };

            Post["/cfg/disablecommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Edit(new Dictionary<string, string> {
                    { "Id", guid },
                    { "IsEnabled", "false" }
                });
                return HttpStatusCode.OK;
            };

            Post["/cfg/launchcommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Launch(guid);
                return Response.AsRedirect("/");
            };

            Post["/cfg/modules"] = x => {
                var modulesText = (string)Request.Form.Config;
                var modules = modulesText.SplitToList(Environment.NewLine);
                _bootModuleLoadRepo.Dump(modules);
                return Response.AsRedirect("/cfg");
            };

            Post["/cfg/services"] = x => {
                var servicesText = (string)Request.Form.Config;
                var services = servicesText.SplitToList(Environment.NewLine);
                _bootServiceLoadRepo.Dump(services);
                return Response.AsRedirect("/cfg");
            };

            Post["/cfg/osparam"] = x => {
                var osparamText = (string)Request.Form.Config;
                var services = osparamText.SplitToList(Environment.NewLine);
                var dict = new Dictionary<string, string>();
                foreach(var serv in services) {
                    try {
                        var kvp = serv.Split(new[] { " " }, 2, StringSplitOptions.None);
                        dict.Add(kvp[0], kvp[1]);
                    }
                    catch(Exception) {
                        continue;
                    }
                }
                _bootOsParametersLoadRepo.Dump(dict);
                return Response.AsRedirect("/cfg");
            };
        }
    }
}