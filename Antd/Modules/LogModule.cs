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

using System.Dynamic;
using Antd.Configuration;
using Antd.Database;
using Antd.Log;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class LogModule : CoreModule {
        private readonly SyslogRepository _syslogRepository = new SyslogRepository();
        private readonly SyslogConfiguration _syslogConfiguration = new SyslogConfiguration();
        private readonly Journalctl _journalctl = new Journalctl();
        private readonly Journalctl.Report _journalctlReport = new Journalctl.Report();

        public LogModule() {
            this.RequiresAuthentication();

            After += ctx => {
                if(ctx.Response.ContentType == "text/html") {
                    ctx.Response.ContentType = "text/html; charset=utf-8";
                }
            };

            Post["/log/syslog/set"] = x => {
                var root = Request.Form.Root;
                var p1 = Request.Form.Path1;
                var p2 = Request.Form.Path2;
                var p3 = Request.Form.Path3;
                _syslogRepository.Set(root, p1, p2, p3);
                _syslogConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/log/syslog/enable"] = x => {
                _syslogRepository.Enable();
                return HttpStatusCode.OK;
            };

            Post["/log/syslog/disable"] = x => {
                _syslogRepository.Disable();
                return HttpStatusCode.OK;
            };

            Get["/log/journalctl/all"] = x => Response.AsJson(_journalctl.GetAllLog());

            Get["/log/journalctl/all/{filter}"] = x => Response.AsJson(_journalctl.GetAllLog((string)x.filter));

            Get["/log/journalctl/last/{hours}"] = x => Response.AsXml(_journalctl.GetAllLogSinceHour((string)x.hours));

            Get["/log/journalctl/antd"] = x => Response.AsJson(_journalctl.GetAntdLog());

            Get["/log/journalctl/context"] = x => Response.AsJson(_journalctl.GetLogContexts());

            Get["/log/journalctl/report/{path*}"] = x => Response.AsJson(_journalctlReport.ReadReport((string)x.path));

            Post["/log/journalctl/report"] = x => {
                _journalctlReport.GenerateReport();
                return HttpStatusCode.OK;
            };

            Get["/log/collectd"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-collectd", vmod];
            };

            Get["/log/websocket"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-websocket", vmod];
            };

            //Post["/log/websocket/listen", true] = async (x, ct) => {
            //    var port = Websocketd.GetFirstPort();
            //    //Websocketd.SetCMD(port, "/usr/bin/vmstat -n 1");
            //    //System.Threading.Thread.Sleep(20);
            //    await Websocketd.SetWebsocket(port);
            //    return Response.AsJson(port);
            //};

            Get["/log/journalctl"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-journalctl", vmod];
            };

            //Post["/log/journalctl/listen", true] = async (x, ct) => {
            //    var port = Websocketd.GetFirstPort();
            //    Websocketd.SetUnit(port, "todo");
            //    await Websocketd.LaunchCommandToJournalctl(port);
            //    return Response.AsJson(port);
            //};
        }
    }
}