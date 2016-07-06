//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www..com)
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
using antdlib.Apps;
using antdlib.common;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class AppsModule : CoreModule {

        public AppsModule() {
            this.RequiresAuthentication();

            Get["/apps"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.AppList = Management.DetectApps();
                vmod.AppExists = AnthillaSp.Setting.CheckSquash();
                vmod.AnthillaSpIsActive = AnthillaSp.Status.IsActiveAnthillaSp();
                vmod.AnthillaSpStatus = AnthillaSp.Status.AnthillaSp();
                vmod.AnthillaServerIsActive = AnthillaSp.Status.IsActiveAnthillaServer();
                vmod.AnthillaServerStatus = AnthillaSp.Status.AnthillaServer();
                vmod.Message = "";
                return View["_page-apps", vmod];
            };

            //Get["/apps/set/anthillasp"] = x => {
            //    if (AnthillaSp.Units.CheckFiles() == false) {
            //        AnthillaSp.CreateUnits();
            //    }
            //    AnthillaSp.Start();
            //    return HttpStatusCode.OK;
            //};

            Get["/apps/apply/anthillasp"] = x => {
                AnthillaSp.Start();
                return HttpStatusCode.OK;
            };

            Get["/apps/Launch"] = x => {
                ConsoleLogger.Log(">> App >> AnthillaSP");
                ConsoleLogger.Log(">> Check squashfs");
                if (AnthillaSp.Setting.CheckSquash() == false) {
                    ConsoleLogger.Warn(">> Squashfs does not exist!");
                    return Response.AsJson(false);
                }
                ConsoleLogger.Log(">> Mount squashfs in /framework/anthillasp");
                AnthillaSp.Setting.MountSquash();
                ConsoleLogger.Log(">> Create AnthillaSP units in /mnt/cdrom/Overlay/anthillasp/");
                //AnthillaSp.CreateUnits();
                AnthillaSp.Start();
                return HttpStatusCode.OK;
            };

            Post["/apps/start/sp"] = x => {
                AnthillaSp.StartSp();
                return Response.AsJson("sp process started");
            };

            Post["/apps/start/server"] = x => {
                AnthillaSp.StartServer();
                return Response.AsJson("server process started");
            };

            Post["/apps/stop/sp"] = x => {
                AnthillaSp.StopSp();
                return Response.AsJson("sp process stopped");
            };

            Post["/apps/stop/server"] = x => {
                AnthillaSp.StopServer();
                return Response.AsJson("server process stopped");
            };

            Get["/apps/status/sp"] = x => Response.AsJson(AnthillaSp.Status.AnthillaSp());

            Get["/apps/status/server"] = x => Response.AsJson(AnthillaSp.Status.AnthillaServer());

            Post["/apps/Mount"] = x => {
                var f = (string)Request.Form.Folder;
                var m = (string)Request.Form.Mount;
                new Terminal().Execute("Mount " + f + " " + m);
                return Response.AsJson(AnthillaSp.Status.AnthillaServer());
            };

            Post["/apps/set/anthillasp"] = x => {
                AnthillaSp.SetApp();
                return Response.AsJson("/set/anthillasp");
            };
        }
    }
}