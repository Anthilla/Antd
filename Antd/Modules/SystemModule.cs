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
                vmod.TCPport = "";
                vmod.MaxProcesses = "2";
                vmod.AlternateHostnames = "";
                return View["page-system-advanced", vmod];
            };

            Post["/update/protocol/{protocol}"] = x => {
                string protocol = x.protocol;
                ConsoleLogger.Info("New Protocol: {0}", protocol);
                return Response.AsJson(protocol);
            };

            Post["/update/tcpport/{tcpport}"] = x => {
                string tcpport = x.tcpport;
                ConsoleLogger.Info("New TCP Port: {0}", tcpport);
                return Response.AsJson(tcpport);
            };

            Post["/update/maxprocs/{maxprocs}"] = x => {
                string maxprocs = x.maxprocs;
                ConsoleLogger.Info("New Max Processes: {0}", maxprocs);
                return Response.AsJson(maxprocs);
            };

            Post["/update/webguiredirects/{webguiredirects}"] = x => {
                string webguiredirects = x.webguiredirects;
                ConsoleLogger.Info("New WebGUI Redirect: {0}", webguiredirects);
                return Response.AsJson(webguiredirects);
            };

            Post["/update/webguiloginautocomplete/{webguiloginautocomplete}"] = x => {
                string webguiloginautocomplete = x.webguiloginautocomplete;
                ConsoleLogger.Info("New WebGUI Login Autocomplete: {0}", webguiloginautocomplete);
                return Response.AsJson(webguiloginautocomplete);
            };

            Post["/update/webguiloginmessages/{webguiloginmessages}"] = x => {
                string webguiloginmessages = x.webguiloginmessages;
                ConsoleLogger.Info("New WebGUI login messages: {0}", webguiloginmessages);
                return Response.AsJson(webguiloginmessages);
            };

            Post["/update/antilockout/{antilockout}"] = x => {
                string antilockout = x.antilockout;
                ConsoleLogger.Info("New Anti-lockout: {0}", antilockout);
                return Response.AsJson(antilockout);
            };

            Post["/update/dnsrebindcheck/{dnsrebindcheck}"] = x => {
                string dnsrebindcheck = x.dnsrebindcheck;
                ConsoleLogger.Info("New DNS Rebind Check: {0}", dnsrebindcheck);
                return Response.AsJson(dnsrebindcheck);
            };

            Post["/update/alternatehostnames/{alternatehostnames}"] = x => {
                string alternatehostnames = x.alternatehostnames;
                ConsoleLogger.Info("New Alternate Hostnames: {0}", alternatehostnames);
                return Response.AsJson(alternatehostnames);
            };

            Post["/update/refererenforcement/{refererenforcement}"] = x => {
                string refererenforcement = x.refererenforcement;
                ConsoleLogger.Info("New Browser HTTP_REFERER enforcement: {0}", refererenforcement);
                return Response.AsJson(refererenforcement);
            };

            Post["/update/tabtext/{tabtext}"] = x => {
                string tabtext = x.tabtext;
                ConsoleLogger.Info("New Browser tab text: {0}", tabtext);
                return Response.AsJson(tabtext);
            };

            Post["/update/enablesecureshell/{enablesecureshell}"] = x => {
                string enablesecureshell = x.enablesecureshell;
                ConsoleLogger.Info("New Secure Shell Server: {0}", enablesecureshell);
                return Response.AsJson(enablesecureshell);
            };

            Post["/update/authenticationmethod/{authenticationmethod}"] = x => {
                string authenticationmethod = x.authenticationmethod;
                ConsoleLogger.Info("New Authentication Method: {0}", authenticationmethod);
                return Response.AsJson(authenticationmethod);
            };

            Get["/certmanager"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-certmanager", vmod];
            };

            Get["/firmware"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-firmware", vmod];
            };

            Get["/highavailsync"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-system-highavailsync", vmod];
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