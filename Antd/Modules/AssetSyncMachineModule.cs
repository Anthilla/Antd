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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.Modules {
    public class AssetSyncMachineModule : NancyModule {

        public AssetSyncMachineModule() {
            Get["/syncmachine"] = x => {
                var set = SyncMachineConfiguration.Get();
                var syncedMachines = set.Machines.Any() ? set.Machines : new List<SyncMachineModel>();
                var model = new PageAssetSyncMachineModel {
                    IsActive = set.IsActive,
                    SyncedMachines = syncedMachines.OrderBy(_ => _.MachineAddress)
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/syncmachine/set"] = x => {
                SyncMachineConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/syncmachine/restart"] = x => {
                SyncMachineConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/syncmachine/stop"] = x => {
                SyncMachineConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/syncmachine/enable"] = x => {
                SyncMachineConfiguration.Enable();
                SyncMachineConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/syncmachine/disable"] = x => {
                SyncMachineConfiguration.Disable();
                SyncMachineConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/syncmachine/machine"] = x => {
                string machineAddress = Request.Form.MachineAddress;
                var model = new SyncMachineModel {
                    MachineAddress = machineAddress
                };
                SyncMachineConfiguration.AddResource(model);
                return HttpStatusCode.OK;
            };

            Post["/syncmachine/machine/del"] = x => {
                string guid = Request.Form.Guid;
                SyncMachineConfiguration.RemoveResource(guid);
                return HttpStatusCode.OK;
            };

            Post["Accept Configuration", "/syncmachine/accept"] = x => {
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