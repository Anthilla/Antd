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
using antd.commands;
using antdlib.common;
using antdlib.views;
using Antd.Configuration;
using Antd.Database;
using Antd.Log;
using Nancy;
using Nancy.Security;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class LogModule : CoreModule {

        public LogModule() {
            this.RequiresAuthentication();

            #region [    Home    ]
            Get["/log"] = x => {
                dynamic viewModel = new ExpandoObject();
                return View["antd/page-log", viewModel];
            };
            #endregion

            #region [    Partials    ]
            Get["/part/log"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var journalctl = new Journalctl();
                    viewModel.Logs = journalctl.GetAntdLog().ToList();
                    return View["antd/part/page-log", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/log/system"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var journalctl = new Journalctl();
                    var result = journalctl.GetAllLogSinceHour("4").ToList();
                    viewModel.Logs = result;
                    return View["antd/part/page-log-system", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/log/report"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var journalctlReport = new Journalctl.Report();
                    viewModel.LogReports = journalctlReport.Get();
                    return View["antd/part/page-log-report", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/log/syslog"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var syslogRepository = new SyslogRepository();
                    var syslogConfig = syslogRepository.Get();
                    viewModel.SyslogConfig = syslogConfig ?? new SyslogSchema();
                    var syslogNg = new SyslogNg();
                    viewModel.SyslogNgContent = syslogNg.GetAll().OrderBy(_ => _.Host).ThenByDescending(_ => _.DateTime);
                    return View["antd/part/page-log-syslog", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };
            #endregion

            #region [    Actions    ]
            Post["/log/syslog/set"] = x => {
                var root = Request.Form.Root;
                var p1 = Request.Form.Path1;
                var p2 = Request.Form.Path2;
                var p3 = Request.Form.Path3;
                var syslogRepository = new SyslogRepository();
                syslogRepository.Set(root, p1, p2, p3);
                var syslogConfiguration = new SyslogConfiguration();
                syslogConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/log/syslog/enable"] = x => {
                var syslogRepository = new SyslogRepository();
                syslogRepository.Enable();
                return HttpStatusCode.OK;
            };

            Post["/log/syslog/disable"] = x => {
                var syslogRepository = new SyslogRepository();
                syslogRepository.Disable();
                return HttpStatusCode.OK;
            };

            Get["/log/journalctl/all"] = x => {
                var journalctl = new Journalctl();
                var result = journalctl.GetAllLog().ToList();
                return JsonConvert.SerializeObject(result);
            };

            Get["/log/journalctl/all/{filter}"] = x => {
                var journalctl = new Journalctl();
                var result = journalctl.GetAllLog((string)x.filter).ToList();
                return JsonConvert.SerializeObject(result);
            };

            Get["/log/journalctl/last/{hours}"] = x => {
                var journalctl = new Journalctl();
                var result = journalctl.GetAllLogSinceHour((string)x.filter).ToList();
                return JsonConvert.SerializeObject(result);
            };

            Get["/log/journalctl/antd"] = x => {
                var journalctl = new Journalctl();
                var result = journalctl.GetAntdLog().ToList();
                return JsonConvert.SerializeObject(result);
            };

            Get["/log/journalctl/context"] = x => {
                var journalctl = new Journalctl();
                var result = journalctl.GetLogContexts().ToList();
                return JsonConvert.SerializeObject(result);
            };

            Get["/log/journalctl/report/{path*}"] = x => {
                var journalctlReport = new Journalctl.Report();
                var result = journalctlReport.ReadReport((string)x.path).ToList();
                return JsonConvert.SerializeObject(result);
            };

            Post["/log/journalctl/report"] = x => {
                var journalctlReport = new Journalctl.Report();
                journalctlReport.GenerateReport();
                return HttpStatusCode.OK;
            };

            Get["/log/journalctl/service/{service}"] = x => {
                var launcher = new CommandLauncher();
                var result = launcher.Launch("journactl-service", new Dictionary<string, string> { { "$service", (string)x.service } });
                return JsonConvert.SerializeObject(result.JoinToString("<br />"));
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