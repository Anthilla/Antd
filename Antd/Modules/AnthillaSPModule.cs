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

using Antd.ManageApplications;
using Antd.UnitFiles;
using Nancy;
using Nancy.Security;

namespace Antd {

    public class AnthillaSPModule : NancyModule {

        public AnthillaSPModule()
            : base("/anthillasp") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return View["page-m-service-sp"];
            };

            Get["/unit/set/anthillasp"] = x => {
                CreateUnit.ForAnthillaSP();
                return View["page-m-service-sp"];
            };

            Get["/unit/set/anthillaserver"] = x => {
                CreateUnit.ForAnthillaServer();
                return View["page-m-service-sp"];
            };

            Get["/unit/start/anthillasp"] = x => {
                CommandModel start = AnthillaSP.StartAnthillaSP();
                return View["page-m-service-sp", start];
                //return Response.AsJson(start);
            };

            Get["/unit/start/anthillaserver"] = x => {
                CommandModel start = AnthillaSP.StartAnthillaServer();
                return View["page-m-service-sp", start];
                //return Response.AsJson(start);
            };

            Get["/unit/stop/anthillasp"] = x => {
                CommandModel stop = AnthillaSP.StopAnthillaSP();
                return View["page-m-service-sp", stop];
                //return Response.AsJson(stop);
            };

            Get["/unit/stop/anthillaserver"] = x => {
                CommandModel stop = AnthillaSP.StopAnthillaServer();
                return View["page-m-service-sp", stop];
                //return Response.AsJson(stop);
            };

            Get["/unit/status/anthillasp"] = x => {
                CommandModel status = AnthillaSP.StatusAnthillaSP();
                return View["page-m-service-sp", status];
                //return Response.AsJson(status);
            };

            Get["/unit/status/anthillaserver"] = x => {
                CommandModel status = AnthillaSP.StatusAnthillaServer();
                return View["page-m-service-sp", status];
                //return Response.AsJson(status);
            };
        }
    }
}