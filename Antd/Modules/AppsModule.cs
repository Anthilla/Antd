///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www..com)
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

using Antd.Apps;
using Nancy;
using Nancy.Security;
using System.Dynamic;

namespace Antd {

    public class AppsModule : NancyModule {

        public AppsModule()
            : base("/apps") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                bool b;
                if (AnthillaSP.Setting.CheckSquash() == false) {
                    //ViewBag.Message = "No squashfs detected in " + Folder.Apps;
                    b = false;
                }
                else {
                    b = true;
                }
                vmod.AppExists = b;
                return View["_page-apps", vmod];
            };

            Get["/set/anthillasp"] = x => {
                if (Antd.Apps.AnthillaSP.Units.CheckFiles() == false) {
                    AnthillaSP.CreateUnits();
                }
                AnthillaSP.Start();
                return Response.AsJson(true);
            };

            Get["/apply/anthillasp"] = x => {
                AnthillaSP.Start();
                return Response.AsJson(true);
            };

            Get["/launch"] = x => {
                ConsoleLogger.Log(">> App >> AnthillaSP");
                ConsoleLogger.Log(">> Check squashfs");
                var b = AnthillaSP.Setting.CheckSquash();
                if (b == false) {
                    ConsoleLogger.Warn(">> Squashfs does not exist!");
                    return Response.AsJson(b);
                }
                else {
                    ConsoleLogger.Log(">> Mount squashfs in /framework/anthillasp");
                    AnthillaSP.Setting.MountSquash();
                    ConsoleLogger.Log(">> Create AnthillaSP units in /mnt/cdrom/Overlay/anthillasp/");
                    AnthillaSP.CreateUnits();
                    AnthillaSP.Start();
                    return Response.AsJson(true);
                }
            };

            Get["/start/sp"] = x => {
                AnthillaSP.StartSP();
                return Response.AsJson("AnthillaSP process started");
            };

            Get["/start/server"] = x => {
                AnthillaSP.StartServer();
                return Response.AsJson("AnthillaSP process started");
            };

            Get["/stop/sp"] = x => {
                AnthillaSP.StopSP();
                return Response.AsJson("AnthillaSP process stopped");
            };

            Get["/stop/server"] = x => {
                AnthillaSP.StopServer();
                return Response.AsJson("AnthillaSP process stopped");
            };

            Get["/status/sp"] = x => {
                return Response.AsJson(AnthillaSP.Status.AnthillaSP());
            };

            Get["/status/server"] = x => {
                return Response.AsJson(AnthillaSP.Status.AnthillaServer());
            };

            Post["/mount"] = x => {
                var f = (string)this.Request.Form.Folder;
                var m = (string)this.Request.Form.Mount;
                Terminal.Execute("mount "+ f + " " + m);
                return Response.AsJson(AnthillaSP.Status.AnthillaServer());
            };
        }
    }
}