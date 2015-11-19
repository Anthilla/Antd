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
using antdlib.Terminal;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class CommandModule : NancyModule {

        public CommandModule()
            : base("/command/mgmt") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.list = CommandRepository.GetAll();
                return View["page-command-mgmt", vmod];
            };

            Post["/"] = x => {
                string command = Request.Form.Command;
                string layout = Request.Form.CommandLayout;
                string notes = Request.Form.Notes;
                string inputid = Request.Form.InputID;
                string inputlocation = Request.Url;
                CommandRepository.Create(antdlib.CCTableCommandType.Direct, inputid, command, "", "", layout, inputlocation, notes);
                return Response.AsRedirect("/command/mgmt");
            };

            Post["/launch/{guid}"] = x => {
                string guid = x.guid;
                var result = CommandRepository.LaunchAndGetOutput(guid);
                return Response.AsJson(result);
            };

            Post["/delete/{guid}"] = x => {
                string guid = x.guid;
                CommandRepository.Delete(guid);
                return Response.AsJson(true);
            };

            Get["/ex/{inputid}"] = x => {
                string inputid = x.inputid;
                var r = CommandRepository.LaunchAndGetOutputUsingNewValue(inputid);
                return Response.AsJson(r);
            };

            Get["/ex/{inputid}/{Value}"] = x => {
                string inputid = x.inputid;
                string value = x.value;
                var r = CommandRepository.LaunchAndGetOutputUsingNewValue(inputid, value);
                return Response.AsJson(r);
            };
        }
    }
}