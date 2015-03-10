///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.ServiceManagement;
using Antd.UnitFiles;
using Nancy;
using Nancy.Security;

namespace Antd {

    public class AnthillaASModule : NancyModule {

        public AnthillaASModule()
            : base("/anthillaas") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return View["page-m-service-as"];
            };

            Get["/unit/set/anthillaas"] = x => {
                CreateUnit.ForAnthillaAS();
                AnthillaAS.EnableAnthillaAS();
                return Response.AsJson(true);
            };

            Get["/unit/set/anthillafirewall"] = x => {
                CreateUnit.ForAnthillaFirewall();
                AnthillaAS.EnableAnthillaFirewall();
                return Response.AsJson(true);
            };

            Get["/unit/set/anthillastorage"] = x => {
                CreateUnit.ForAnthillaStorage();
                AnthillaAS.EnableAnthillaStorage();
                return Response.AsJson(true);
            };

            Get["/unit/start/anthillaas"] = x => {
                CommandModel start = AnthillaAS.StartAnthillaAS();
                return View["page-m-service-as", start];
                //return Response.AsJson(start);
            };

            Get["/unit/start/anthillafirewall"] = x => {
                CommandModel start = AnthillaAS.StartAnthillaFirewall();
                return View["page-m-service-as", start];
                //return Response.AsJson(start);
            };

            Get["/unit/start/anthillastorage"] = x => {
                CommandModel start = AnthillaAS.StartAnthillaStorage();
                return View["page-m-service-as", start];
                //return Response.AsJson(start);
            };

            Get["/unit/stop/anthillaas"] = x => {
                CommandModel stop = AnthillaAS.StopAnthillaAS();
                return View["page-m-service-as", stop];
                //return Response.AsJson(stop);
            };

            Get["/unit/stop/anthillafirewall"] = x => {
                CommandModel stop = AnthillaAS.StopAnthillaFirewall();
                return View["page-m-service-as", stop];
                //return Response.AsJson(stop);
            };

            Get["/unit/stop/anthillastorage"] = x => {
                CommandModel stop = AnthillaAS.StopAnthillaStorage();
                return View["page-m-service-as", stop];
                //return Response.AsJson(stop);
            };

            Get["/unit/status/anthillaas"] = x => {
                CommandModel status = AnthillaAS.StatusAnthillaAS();
                return View["page-m-service-as", status];
                //return Response.AsJson(status);
            };

            Get["/unit/status/anthillafirewall"] = x => {
                CommandModel status = AnthillaAS.StatusAnthillaFirewall();
                return View["page-m-service-as", status];
                //return Response.AsJson(status);
            };

            Get["/unit/status/anthillastorage"] = x => {
                CommandModel status = AnthillaAS.StatusAnthillaStorage();
                return View["page-m-service-as", status];
                //return Response.AsJson(status);
            };
        }
    }
}