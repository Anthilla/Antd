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

using System.Linq;
using antdlib.Config;
using Antd.ViewHelpers;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ConfModule : CoreModule {
        public ConfModule() {
            this.RequiresAuthentication();

            Post["/cfg/addvalue"] = x => {
                var tag = (string)Request.Form.Tag;
                var key = (string)Request.Form.Key;
                var value = (string)Request.Form.Value;
                if (key.Length > 0) {
                    ConfigManagement.AddValuesBundle(tag, key, value);
                }
                else {
                    ConfigManagement.AddValuesBundle(tag, value);
                }
                return Response.AsRedirect("/cfg");
            };

            Post["/cfg/delvalue"] = x => {
                var tag = (string)Request.Form.Tag;
                var key = (string)Request.Form.Key;
                var value = (string)Request.Form.Value;
                ConfigManagement.DeleteValuesBundle(tag, key, value);
                return Response.AsRedirect("/cfg");
            };

            Get["/cfg/tags"] = x => {
                var data = ConfigManagement.GetTagsBundleValue();
                var map = SelectizerMapModel.MapRawTagOfValueBundle(data);
                return Response.AsJson(map);
            };

            Post["/cfg/addcommand"] = x => {
                var command = (string)Request.Form.Command;
                if (command.Length > 0) {
                    ConfigManagement.AddCommandsBundle(command);
                }
                return Response.AsRedirect("/cfg");
            };

            Post["/cfg/delcommand"] = x => {
                var guid = (string)Request.Form.Guid;
                ConfigManagement.DeleteCommandsBundle(guid);
                return Response.AsRedirect("/cfg");
            };

            Post["/cfg/enablecommand"] = x => {
                var guid = (string)Request.Form.Guid;
                ConfigManagement.EnableCommand(guid);
                return Response.AsJson(true);
            };

            Post["/cfg/disablecommand"] = x => {
                var guid = (string)Request.Form.Guid;
                ConfigManagement.DisableCommand(guid);
                return Response.AsJson(true);
            };

            Post["/cfg/launchcommand"] = x => {
                var guid = (string)Request.Form.Guid;
                ConfigManagement.LaunchCommand(guid);
                return Response.AsRedirect("/cfg");
            };

            Post["/cfg/reindex"] = x => {
                var guid = (string)Request.Form.Guid;
                var index = (string)Request.Form.Index;
                var guids = guid.Split(',');
                var indexes = index.Split(',');
                for (var i = 0; i < guids.Length; i++) {
                    ConfigManagement.AssignIndexToCommandsBundle(guids[i], indexes[i]);
                }
                return Response.AsRedirect("/cfg");
            };

            Get["/cfg/getenabled"] = x => {
                var data = ConfigManagement.GetCommandsBundle().Where(_ => _.IsEnabled);
                return Response.AsJson(data);
            };

            Get["/cfg/layouts"] = x => {
                var data = ConfigManagement.GetCommandsBundleLayout();
                var map = SelectizerMapModel.MapRawCommandBundleLayout(data);
                return Response.AsJson(map);
            };

            Post["/cfg/export"] = x => {
                ConfigManagement.Export.ExportConfigurationToFile();
                return Response.AsJson(true);
            };
        }
    }
}