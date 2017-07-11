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
using System.Linq;
using anthilla.core;
using System.IO;
using System.Text.RegularExpressions;

namespace Antd.Modules {
    public class AssetClusterModule : NancyModule {

        public AssetClusterModule() {
            Get["/cluster"] = x => {
                var syncedMachines = ClusterConfiguration.GetNodes();
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
                ClusterConfiguration.SaveNodes(model);
                ClusterConfiguration.SaveConfiguration(model2);
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
                    FileWithAcl.WriteAllText(file, content, "644", "root", "wheel");
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

            #region [    Handshake + cluster init    ]
            Post["/asset/handshake/start"] = x => {
                string host = Request.Form.Host;
                string hostname = Request.Form.Name;
                const string pathToPrivateKey = "/root/.ssh/id_rsa";
                const string pathToPublicKey = "/root/.ssh/id_rsa.pub";
                if(!File.Exists(pathToPublicKey)) {
                    var k = Bash.Execute($"ssh-keygen -t rsa -N '' -f {pathToPrivateKey}");
                    ConsoleLogger.Log(k);
                }
                var key = File.ReadAllText(pathToPublicKey);
                if(string.IsNullOrEmpty(key)) {
                    return HttpStatusCode.InternalServerError;
                }
                var dict = new Dictionary<string, string> { { "ApplePie", key } };
                var r = new ApiConsumer().Post($"{host}asset/handshake", dict);
                if(r != HttpStatusCode.OK) {
                    return HttpStatusCode.InternalServerError;
                }
                //ho fatto l'handshake, quindi il nodo richiesto è pronto per essere integrato nel cluster
                //1. controllo la configurazione
                var clusterConfiguration = ClusterConfiguration.GetClusterInfo();
                if(string.IsNullOrEmpty(clusterConfiguration.Guid)) {
                    clusterConfiguration.Guid = Guid.NewGuid().ToString();
                }
                if(string.IsNullOrEmpty(clusterConfiguration.Priority)) {
                    clusterConfiguration.Priority = "100";
                }
                if(string.IsNullOrEmpty(clusterConfiguration.NetworkInterface)) {
                    clusterConfiguration.NetworkInterface = "";
                }
                if(string.IsNullOrEmpty(clusterConfiguration.VirtualIpAddress)) {
                    clusterConfiguration.VirtualIpAddress = "";
                }
                //2. salvo la configurazione
                ClusterConfiguration.SaveConfiguration(clusterConfiguration);
                //3. controllo i nodi presenti nella configurazione
                var clusterNodes = ClusterConfiguration.GetNodes();
                //4. per prima cosa controllo l'host locale
                var localNode = new Cluster.Node() {
                    Hostname = Host2Configuration.Host.HostName,
                    IpAddress = IPv4.GetAllLocalAddress().FirstOrDefault()
                };
                //5. se non c'è lo aggiungo 
                if(clusterNodes.FirstOrDefault(_ => _.Hostname == localNode.Hostname && _.IpAddress == localNode.IpAddress) == null) {
                    clusterNodes.Add(localNode);
                }
                //6. controllo il nodo richiesto (remoto)
                var ipRegex = new Regex("([0-9]{0,3}\\.[0-9]{0,3}\\.[0-9]{0,3}\\.[0-9]{0,3})");
                var remoteIp = ipRegex.Match(host).Groups[1].Value;
                var remoteNode = new Cluster.Node() {
                    Hostname = hostname,
                    IpAddress = remoteIp
                };
                //7. se non c'è lo aggiungo
                if(clusterNodes.FirstOrDefault(_ => _.Hostname == remoteNode.Hostname && _.IpAddress == remoteNode.IpAddress) == null) {
                    clusterNodes.Add(remoteNode);
                }
                //8. salvo la configurazione dei nodi
                ClusterConfiguration.SaveNodes(clusterNodes);
                //9. riavvio/avvio il servizio di cluster
                new Do().ClusterChanges();
                return HttpStatusCode.OK;
            };

            Post["/asset/handshake"] = x => {
                string apple = Request.Form.ApplePie;
                var info = apple.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(info.Length < 2) {
                    return HttpStatusCode.InternalServerError;
                }
                var key = info[0];
                var remoteUser = info[1];
                const string user = "root";
                var model = new AuthorizedKeyModel {
                    RemoteUser = remoteUser,
                    User = user,
                    KeyValue = key
                };
                var authorizedKeysConfiguration = new AuthorizedKeysConfiguration();
                authorizedKeysConfiguration.AddKey(model);
                try {
                    DirectoryWithAcl.CreateDirectory("/root/.ssh");
                    const string authorizedKeysPath = "/root/.ssh/authorized_keys";
                    if(File.Exists(authorizedKeysPath)) {
                        var f = File.ReadAllText(authorizedKeysPath);
                        if(!f.Contains(apple)) {
                            FileWithAcl.AppendAllLines(authorizedKeysPath, new List<string> { apple }, "644", "root", "wheel");
                        }
                    }
                    else {
                        FileWithAcl.WriteAllLines(authorizedKeysPath, new List<string> { apple }, "644", "root", "wheel");
                    }
                    Bash.Execute($"chmod 600 {authorizedKeysPath}", false);
                    Bash.Execute($"chown {user}:{user} {authorizedKeysPath}", false);
                    return HttpStatusCode.OK;
                }
                catch(Exception ex) {
                    ConsoleLogger.Log(ex);
                    return HttpStatusCode.InternalServerError;
                }
            };

            //Post["/asset/wol"] = x => {
            //    string mac = Request.Form.MacAddress;
            //    CommandLauncher.Launch("wol", new Dictionary<string, string> { { "$mac", mac } });
            //    return HttpStatusCode.OK;
            //};

            //Get["/asset/nmasp/{ip}"] = x => {
            //    string ip = x.ip;
            //    var result = CommandLauncher.Launch("nmap-ip-fast", new Dictionary<string, string> { { "$ip", ip } }).Where(_ => !_.Contains("MAC Address")).Skip(5).Reverse().Skip(1).Reverse();
            //    var list = new List<NmapScanStatus>();
            //    foreach(var r in result) {
            //        var a = r.SplitToList(" ").ToArray();
            //        var mo = new NmapScanStatus {
            //            Protocol = a[0],
            //            Status = a[1],
            //            Type = a[2]
            //        };
            //        list.Add(mo);
            //    }
            //    list = list.OrderBy(_ => _.Protocol).ToList();
            //    return JsonConvert.SerializeObject(list);
            //};
            #endregion
        }
    }
}