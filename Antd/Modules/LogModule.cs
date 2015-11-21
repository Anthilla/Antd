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
using antdlib.Log;
using Antd.Hubs;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class LogModule : NancyModule {
        public LogModule()
            : base("/log") {
            this.RequiresAuthentication();

            Get["/journalctl/all"] = x => Response.AsJson(Journalctl.GetAllLog());

            Get["/journalctl/all/{filter}"] = x => Response.AsJson(Journalctl.GetAllLog((string)x.filter));

            Get["/journalctl/last/{hours}"] = x => Response.AsXml(Journalctl.GetAllLogSinceHour((string)x.hours));

            Get["/journalctl/antd"] = x => Response.AsJson(Journalctl.GetAntdLog());

            Get["/journalctl/context"] = x => Response.AsJson(Journalctl.GetLogContexts());

            Get["/journalctl/report/{path*}"] = x => Response.AsJson(Journalctl.Report.ReadReport((string)x.path));

            Post["/journalctl/report"] = x => {
                Journalctl.Report.GenerateReport();
                return HttpStatusCode.OK;
            };

            Get["/collectd"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-collectd", vmod];
            };

            Get["/websocket"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-websocket", vmod];
            };

            Post["/websocket/listen", true] = async (x, ct) => {
                var port = Websocketd.GetFirstPort();
                //Websocketd.SetCMD(port, "/usr/bin/vmstat -n 1");
                //System.Threading.Thread.Sleep(20);
                await Websocketd.SetWebsocket(port);
                return Response.AsJson(port);
            };

            Get["/journalctl"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["_page-log-journalctl", vmod];
            };

            Post["/journalctl/listen", true] = async (x, ct) => {
                var port = Websocketd.GetFirstPort();
                Websocketd.SetUnit(port, "todo");
                await Websocketd.LaunchCommandToJournalctl(port);
                return Response.AsJson(port);
            };
        }
    }
}