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

using System.IO;
using Antd.SyncMachine;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class ServiceSyncMachineModule : NancyModule {

        public ServiceSyncMachineModule() {
            this.RequiresAuthentication();

            Post["/services/syncmachine/set"] = x => {
                var syncmachineConfiguration = new SyncMachineConfiguration();
                syncmachineConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/services/syncmachine/restart"] = x => {
                var syncmachineConfiguration = new SyncMachineConfiguration();
                syncmachineConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/syncmachine/stop"] = x => {
                var syncmachineConfiguration = new SyncMachineConfiguration();
                syncmachineConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/syncmachine/enable"] = x => {
                var dhcpdConfiguration = new SyncMachineConfiguration();
                dhcpdConfiguration.Enable();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/services/syncmachine/disable"] = x => {
                var dhcpdConfiguration = new SyncMachineConfiguration();
                dhcpdConfiguration.Disable();
                dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/services/syncmachine/machine"] = x => {
                string machineAddress = Request.Form.MachineAddress;
                var model = new SyncMachineModel {
                    MachineAddress = machineAddress
                };
                var syncmachineConfiguration = new SyncMachineConfiguration();
                syncmachineConfiguration.AddResource(model);
                return Response.AsRedirect("/");
            };

            Post["/services/syncmachine/machine/del"] = x => {
                string guid = Request.Form.Guid;
                var syncmachineConfiguration = new SyncMachineConfiguration();
                syncmachineConfiguration.RemoveResource(guid);
                return HttpStatusCode.OK;
            };

            Post["Accept Configuration", "/services/syncmachine/accept"] = x => {
                string file = Request.Form.File;
                string content = Request.Form.Content;
                if(File.Exists(file)) {
                    File.Copy(file, $"{file}.sbck", true);
                }
                File.WriteAllText(file, content);
                //todo restart service to apply changes
                return HttpStatusCode.OK;
            };
        }
    }
}