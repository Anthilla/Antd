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

using System;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;

namespace Antd.Modules {
    public class AssetClusterModule : NancyModule {

        public AssetClusterModule() {
            Get["/cluster"] = x => {
                var syncedMachines = ClusterConfiguration.Get();
                var model = new PageAssetClusterModel {
                    Info = ClusterConfiguration.GetClusterInfo(),
                    ClusterNodes = syncedMachines.OrderBy(_ => _.Hostname).ThenBy(_ => _.IpAddress).ToList()
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/cluster/save"] = x => {
                string config = Request.Form.Config;
                string ip = Request.Form.Ip;
                var model = JsonConvert.DeserializeObject<List<Cluster.Node>>(config);
                var model2 = JsonConvert.DeserializeObject<Cluster.Configuration>(ip);
                ClusterConfiguration.Save(model);
                ClusterConfiguration.SaveClusterInfo(model2);
                new Do().ClusterChanges();
                return HttpStatusCode.OK;
            };

            Post["Accept Configuration", "/cluster/accept"] = x => {
                string file = Request.Form.File;
                string content = Request.Form.Content;
                if(string.IsNullOrEmpty(file)) {
                    return HttpStatusCode.BadRequest;
                }
                if(string.IsNullOrEmpty(content)) {
                    return HttpStatusCode.BadRequest;
                }
                ConsoleLogger.Log($"[cluster] received config for file: {file}");

                DirectoryWatcherCluster.Stop();
                try {
                    if(File.Exists(file)) {
                        File.Copy(file, $"{file}.sbck", true);
                    }
                    File.WriteAllText(file, content);
                }
                catch(Exception) {
                    ConsoleLogger.Warn("");
                    DirectoryWatcherCluster.Start();
                    return HttpStatusCode.InternalServerError;
                }
                DirectoryWatcherCluster.Start();

                var dict = Dicts.DirsAndServices;
                if(dict.ContainsKey(file)) {
                    ConsoleLogger.Log("[cluster] restart service bind to config file");
                    var services = dict[file];
                    foreach(var svc in services) {
                        Systemctl.Enable(svc);
                        Systemctl.Restart(svc);
                    }
                }
                ConsoleLogger.Log("[cluster] apply changes after new config");
                new Do().HostChanges();
                new Do().NetworkChanges();
                return HttpStatusCode.OK;
            };
        }
    }
}