﻿///-------------------------------------------------------------------------------------
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

using Antd.Common;
using Nancy;
using Nancy.Security;
using System.Dynamic;
using System.Linq;

namespace Antd {

    public class SystemModule : NancyModule {

        public SystemModule()
            : base("/system") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return Response.AsRedirect("/system/general");
            };

            Get["/general"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Hostname = Command.Launch("hostname", "").output;
                vmod.Domainname = Command.Launch("hostname", "-f").output;
                vmod.Timezone = Command.Launch("timedatectl", "").output;
                //vmod.TimezonesList = Command.Launch("timedatectl", "list-timezones").output.Split(new char[]{'.'}).ToArray();
                vmod.TimezonesList = new string[] { "uno", "due" };
                vmod.Timeserver = "time.server.net";
                vmod.Language = "English";
                return View["page-system-general", vmod];
            };

            Post["/update/hostname/{hostname}"] = x => {
                string hostname = x.hostname;
                ConsoleLogger.Info("New Hostname: {0}", hostname);
                return Response.AsJson(hostname);
            };

            Post["/update/domainname/{domainname}"] = x => {
                string domainname = x.domainname;
                ConsoleLogger.Info("New Domainname: {0}", domainname);
                return Response.AsJson(domainname);
            };

            Post["/update/timezone/{timezone}"] = x => {
                string timezone = x.timezone;
                ConsoleLogger.Info("New Timezone: {0}", timezone);
                return Response.AsJson(timezone);
            };

            Post["/update/timeserver/{timeserver}"] = x => {
                string timeserver = x.timeserver;
                ConsoleLogger.Info("New Timeserver: {0}", timeserver);
                return Response.AsJson(timeserver);
            };

            Post["/update/language/{language}"] = x => {
                string language = x.language;
                ConsoleLogger.Info("New Language: {0}", language);
                return Response.AsJson(language);
            };

            Post["/update/all/general"] = x => {
                ConsoleLogger.Info("New All...");
                return Response.AsJson(true);
            };

            Get["/advanced"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-advanced", vmod];
            };

            Get["/certmanager"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-certmanager", vmod];
            };

            Get["/routing"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-routing", vmod];
            };

            Get["/usermanager"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-usermanager", vmod];
            };

            Get["/wizard"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-wizard", vmod];
            };
        }
    }
}