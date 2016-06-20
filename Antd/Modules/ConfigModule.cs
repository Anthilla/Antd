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

using System.Collections.Generic;
using System.Linq;
using Antd.Database;
using Antd.Helpers;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ConfigModule : CoreModule {

        private readonly CommandRepository _commandRepositoryRepo = new CommandRepository();
        private readonly CommandValuesRepository _commandValuesRepositoryRepo = new CommandValuesRepository();

        public ConfigModule() {
            this.RequiresAuthentication();

            Post["/cfg/addvalue"] = x => {
                var name = (string)Request.Form.Name;
                var index = (string)Request.Form.Index;
                var value = (string)Request.Form.Value;
                _commandValuesRepositoryRepo.Create(new Dictionary<string, string> {
                    { "Name", name },
                    { "Index", index },
                    { "Value", value }
                });
                return Response.AsRedirect("/");
            };

            Post["/cfg/delvalue"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandValuesRepositoryRepo.Delete(guid);
                return Response.AsRedirect("/");
            };

            Get["/cfg/tags"] = x => {
                var data = _commandValuesRepositoryRepo.GetAll().Select(_ => _.Name);
                var map = SelectizerMapModel.MapRawTagOfValueBundle(data);
                return Response.AsJson(map);
            };

            Post["/cfg/addcommand"] = x => {
                var command = (string)Request.Form.Command;
                _commandRepositoryRepo.Create(new Dictionary<string, string> {
                    { "Command", command },
                    { "Layout", "" },
                    { "Notes", "" }
                });
                return Response.AsRedirect("/");
            };

            Post["/cfg/delcommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Delete(guid);
                return Response.AsRedirect("/");
            };

            Post["/cfg/enablecommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Edit(new Dictionary<string, string> {
                    { "Id", guid },
                    { "IsEnabled", "true" }
                });
                return HttpStatusCode.OK;
            };

            Post["/cfg/disablecommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Edit(new Dictionary<string, string> {
                    { "Id", guid },
                    { "IsEnabled", "false" }
                });
                return HttpStatusCode.OK;
            };

            Post["/cfg/launchcommand"] = x => {
                var guid = (string)Request.Form.Guid;
                _commandRepositoryRepo.Launch(guid);
                return Response.AsRedirect("/");
            };

            Get["/cfg/getenabled"] = x => {
                var data = _commandRepositoryRepo.GetAll().Where(_ => _.IsEnabled == true);
                return Response.AsJson(data);
            };

            Get["/cfg/layouts"] = x => {
                var data = _commandRepositoryRepo.GetAll().Select(_ => _.Layout);
                var map = SelectizerMapModel.MapRawCommandBundleLayout(data);
                return Response.AsJson(map);
            };

            Post["/cfg/export"] = x => {
                //todo ConfigManagement.Export.ExportConfigurationToFile();
                return HttpStatusCode.OK;
            };
        }
    }
}