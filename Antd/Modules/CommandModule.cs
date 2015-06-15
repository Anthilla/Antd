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

using Antd.CommandManagement;
using Nancy;
using Nancy.Security;
using System.Dynamic;

namespace Antd {

    public class CommandModule : NancyModule {

        public CommandModule()
            : base("/command/mgmt") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.list = CommandDB.GetAll();
                return View["page-command-mgmt", vmod];
            };

            Post["/"] = x => {
                string command = this.Request.Form.Command;
                string layout = this.Request.Form.CommandLayout;
                string notes = this.Request.Form.Notes;
                string inputid = this.Request.Form.InputID;
                string inputlocation = this.Request.Url;
                CommandDB.Create(inputid, command, layout, inputlocation, notes);
                return Response.AsRedirect("/command/mgmt");
            };

            Post["/launch/{guid}"] = x => {
                string guid = x.guid;
                string result = CommandDB.LaunchAndGetOutput(guid);
                return Response.AsJson(result);
            };

            Post["/delete/{guid}"] = x => {
                string guid = x.guid;
                CommandDB.Delete(guid);
                return Response.AsJson(true);
            };

            Get["/ex/{inputid}/{value}"] = x => {
                string inputid = x.inputid;
                string value = x.value;
                var r = CommandDB.LaunchAndGetOutputUsingNewValue(inputid, value);
                return Response.AsJson(r);
            };
        }
    }
}