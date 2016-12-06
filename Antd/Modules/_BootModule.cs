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
using Antd.Host;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace Antd.Modules {
    public class BootModule : CoreModule {

        public BootModule() {
            this.RequiresAuthentication();

            #region [    Home    ]
            Get["/boot"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["antd/page-boot", vmod];
            };
            #endregion

            #region [    Partials    ]
            Get["/part/boot/cmd"] = x => {
                try {
                    var setupConfiguration = new SetupConfiguration();
                    dynamic viewModel = new ExpandoObject();
                    viewModel.HasConfiguration = true;
                    viewModel.Controls = setupConfiguration.Get();
                    if(setupConfiguration.Get().Count < 1) {
                        viewModel.HasConfiguration = false;
                    }
                    return View["antd/part/page-boot-cmd", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/boot/mod"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var hostcfg = new HostConfiguration();
                    viewModel.Modules = string.Join("\r\n", hostcfg.GetHostModprobes());
                    viewModel.RmModules = string.Join("\r\n", hostcfg.GetHostRemoveModules());
                    viewModel.Blacklist = string.Join("\r\n", hostcfg.GetHostBlacklistModules());
                    return View["antd/part/page-boot-mod", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/boot/svc"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var hostcfg = new HostConfiguration();
                    viewModel.Services = string.Join("\r\n", hostcfg.GetHostServices());
                    return View["antd/part/page-boot-svc", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/boot/osp"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var hostcfg = new HostConfiguration();
                    viewModel.OsParam = string.Join("\r\n", hostcfg.GetHostOsParameters().Select(_ => $"{_.Key} {_.Value}").ToList());
                    return View["antd/part/page-boot-osp", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };
            #endregion

            #region [    Actions    ]

            Post["/boot/export"] = x => {
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
                var setupConfiguration = new SetupConfiguration();
                setupConfiguration.Export(checkedControl);
                return Response.AsRedirect("/boot");
            };

            Post["/boot/modules"] = x => {
                var modulesText = (string)Request.Form.Config;
                var modules = modulesText.SplitToList(Environment.NewLine);
                var hostcfg = new HostConfiguration();
                hostcfg.SetHostModprobes(modules);
                hostcfg.ApplyHostModprobes();
                return Response.AsRedirect("/boot");
            };

            Post["/boot/rmmodules"] = x => {
                var modulesText = (string)Request.Form.Config;
                var modules = modulesText.SplitToList(Environment.NewLine);
                var hostcfg = new HostConfiguration();
                hostcfg.SetHostRemoveModules(modules);
                hostcfg.ApplyHostRemoveModules();
                return Response.AsRedirect("/boot");
            };

            Post["/boot/modblacklist"] = x => {
                var modulesText = (string)Request.Form.Config;
                var modules = modulesText.SplitToList(Environment.NewLine);
                var hostcfg = new HostConfiguration();
                hostcfg.SetHostBlacklistModules(modules);
                hostcfg.ApplyHostBlacklistModules();
                return Response.AsRedirect("/boot");
            };

            Post["/boot/services"] = x => {
                var servicesText = (string)Request.Form.Config;
                var services = servicesText.SplitToList(Environment.NewLine);
                var hostcfg = new HostConfiguration();
                hostcfg.SetHostServices(services);
                hostcfg.ApplyHostServices();
                return Response.AsRedirect("/boot");
            };

            Post["/boot/osparam"] = x => {
                var osparamText = (string)Request.Form.Config;
                var services = osparamText.SplitToList(Environment.NewLine);
                var dict = new Dictionary<string, string>();
                foreach(var serv in services) {
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