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

using antdlib.config;
using antdlib.models;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ServiceRsyncModule : NancyModule {

        public ServiceRsyncModule() {
            this.RequiresAuthentication();

            Post["/services/rsync/set"] = x => {
                var rsyncConfiguration = new RsyncConfiguration();
                rsyncConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/rsync/restart"] = x => {
                var rsyncConfiguration = new RsyncConfiguration();
                rsyncConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/rsync/stop"] = x => {
                var rsyncConfiguration = new RsyncConfiguration();
                rsyncConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/rsync/enable"] = x => {
                var dhcpdConfiguration = new RsyncConfiguration();
                dhcpdConfiguration.Enable();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/rsync/disable"] = x => {
                var dhcpdConfiguration = new RsyncConfiguration();
                dhcpdConfiguration.Disable();
                dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/rsync/options"] = x => {
                var rsyncConfiguration = new RsyncConfiguration();
                rsyncConfiguration.Save(new RsyncConfigurationModel());
                return Response.AsRedirect("/");
            };

            Post["/services/rsync/directory"] = x => {
                string source = Request.Form.Source;
                string destination = Request.Form.Destination;
                string type = Request.Form.Type;
                var model = new RsyncObjectModel {
                    Source = source,
                    Destination = destination,
                    Type = type
                };
                var rsyncConfiguration = new RsyncConfiguration();
                rsyncConfiguration.AddDirectory(model);
                return Response.AsRedirect("/");
            };

            Post["/services/rsync/directory/del"] = x => {
                string guid = Request.Form.Guid;
                var rsyncConfiguration = new RsyncConfiguration();
                rsyncConfiguration.RemoveDirectory(guid);
                return HttpStatusCode.OK;
            };
        }
    }
}