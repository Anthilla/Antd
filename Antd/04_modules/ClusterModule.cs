using Antd.models;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.Modules {
    public class ClusterModule : NancyModule {

        public ClusterModule() : base("/cluster") {

            Get["/"] = x => {
                return JsonConvert.SerializeObject(Application.CurrentConfiguration.Cluster);
            };

            Get["/status"] = x => {
                var nodes = Application.CurrentConfiguration.Cluster.Nodes;
                var nodesStatus = new ClusterNodeStatusModel[nodes.Length];
                for(var i = 0; i < nodes.Length; i++) {
                    var status = Application.ClusterChecklist.FirstOrDefault(_ => _.TargetNodeMachineUid == nodes[i].MachineUid);
                    nodesStatus[i] = new ClusterNodeStatusModel() {
                        Node = nodes[i],
                        Status = status
                    };
                }
                return JsonConvert.SerializeObject(nodesStatus);
            };

            Post["/save"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<Cluster>(data);
                Application.CurrentConfiguration.Cluster = objects;
                ConfigRepo.Save();
                ConsoleLogger.Log("[cluster] save local configuration");
                return HttpStatusCode.OK;
            };

            Post["/import"] = x => {
                string data = Request.Form.Data;
                var objects = JsonConvert.DeserializeObject<Cluster>(data);
                Application.CurrentConfiguration.Cluster = objects;
                ConfigRepo.Save();
                ConsoleLogger.Log("[cluster] save cluster configuration");
                return HttpStatusCode.OK;
            };

            /// <summary>
            /// Inizia ANCHE la procedura di "condivisione della configurazione nel cluster"
            /// In questo contesto passerà SOLO la configurazione relativa al cluster stesso
            /// Questa API viene richiesta da antdui
            /// Per ogni nodo configurato (escludendo se stesso -> vedi uid) invia la conf
            /// </summary>
            Post["/apply"] = x => {
                //Inizio ad applicarla localmente
                cmds.Cluster.ApplyNetwork();
                cmds.Cluster.ApplyServices();
                cmds.Cluster.ApplyFs();
                ConsoleLogger.Log("[cluster] apply local configuration");
                return HttpStatusCode.OK;
            };

            Post["/deploy"] = x => {
                //Poi aggiorno gli altri nodi
                var nodes = Application.CurrentConfiguration.Cluster.Nodes;
                var configuration = JsonConvert.SerializeObject(Application.CurrentConfiguration.Cluster);
                var localMachineUid = Application.CurrentConfiguration.Host.MachineUid.ToString();
                for(var i = 0; i < nodes.Length; i++) {
                    var node = nodes[i];
                    if(CommonString.AreEquals(localMachineUid, node.MachineUid)) {
                        continue;
                    }
                    ConsoleLogger.Log($"[cluster] deploy configuration on node: {node.Hostname}");
                    var dict = new Dictionary<string, string> {
                        { "Data", configuration }
                    };
                    ConsoleLogger.Log($"[cluster] {node.Hostname}: send configuration to node");
                    var status = ApiConsumer.Post(CommonString.Append(node.EntryPoint, "cluster/import"), dict);
                    if(status == HttpStatusCode.OK) {
                        ConsoleLogger.Log($"[cluster] {node.Hostname}: send apply command to node");
                        ApiConsumer.Post(CommonString.Append(node.EntryPoint, "cluster/apply"));
                    }
                }
                return HttpStatusCode.OK;
            };

            #region [    Handshake + cluster init    ]
            Post["/handshake/begin"] = x => {
                string conf = Request.Form.Data;
                var remoteNode = JsonConvert.DeserializeObject<NodeModel[]>(conf);
                if(remoteNode == null) {
                    return HttpStatusCode.InternalServerError;
                }
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

                //1. controllo la configurazione
                var cluster = Application.CurrentConfiguration.Cluster;
                if(cluster == null) {
                    cluster = new Cluster();
                    cluster.Label = CommonString.Append("AntdCluster-", cluster.Id.ToString().Substring(0, 8));
                }

                var nodes = cluster.Nodes.ToList();
                for(var i = 0; i < remoteNode.Length; i++) {
                    var handshakeResult = ApiConsumer.Post($"{remoteNode[i].ModelUrl}cluster/handshake", dict);
                    if(handshakeResult != HttpStatusCode.OK) {
                        return HttpStatusCode.InternalServerError;
                    }
                    //ottengo i servizi pubblicati da quel nodo
                    var publishedServices = ApiConsumer.Get<ClusterNodeService[]>($"{remoteNode[i].ModelUrl}device/services");
                    nodes.Add(new ClusterNode() {
                        MachineUid = remoteNode[i].MachineUid,
                        Hostname = remoteNode[i].Hostname,
                        PublicIp = remoteNode[i].PublicIp,
                        EntryPoint = remoteNode[i].ModelUrl,
                        Services = publishedServices
                    });
                }
                //ho fatto gli handshake, quindi il nodo richiesto è pronto per essere integrato nel cluster

                cluster.Active = true;
                if(cluster.Id == Guid.Empty) {
                    cluster.Id = Guid.NewGuid();
                }
                cluster.Nodes = nodes.ToArray();

                cluster.SharedNetwork.Active = false;
                var virtualPorts = cluster.SharedNetwork.PortMapping.ToList();
                foreach(var node in nodes) {
                    foreach(var svc in node.Services) {
                        var checkPort = virtualPorts.FirstOrDefault(_ => _.ServicePort == svc.Port.ToString());
                        if(checkPort == null) {
                            virtualPorts.Add(new PortMapping() {
                                ServiceName = svc.Name,
                                ServicePort = svc.Port.ToString(),
                                VirtualPort = string.Empty
                            });
                        }
                    }
                }
                cluster.SharedNetwork.PortMapping = virtualPorts.ToArray();

                Application.CurrentConfiguration.Cluster = cluster;
                ConfigRepo.Save();
                return HttpStatusCode.OK;
            };

            Post["/handshake"] = x => {
                string apple = Request.Form.ApplePie;
                var info = apple.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var key = info[0];
                var keys = Application.CurrentConfiguration.Services.Ssh.AuthorizedKey.ToList();
                if(!keys.Any(_ => _.Key == key)) {
                    var userInfo = info[1].Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    var model = new AuthorizedKey {
                        User = userInfo[0],
                        Host = userInfo[1],
                        Key = key
                    };
                    keys.Add(model);
                }
                Application.CurrentConfiguration.Services.Ssh.AuthorizedKey = keys.ToArray();
                ConfigRepo.Save();
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
                return HttpStatusCode.OK;
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