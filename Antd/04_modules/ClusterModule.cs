using Antd.cmds;
using Antd.models;
using Antd.Mqtt;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                foreach(var node in objects.Nodes) {
                    if(string.IsNullOrEmpty(node.EntryPoint)) {
                        node.EntryPoint = $"http://{node.PublicIp}:8086/";
                    }
                }
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
            Post["/apply", true] = async (x, ct) => {
                //Inizio ad applicarla localmente
                ConsoleLogger.Log("[cluster] apply local configuration");
                ClusterSetup.ApplyNetwork();
                ClusterSetup.ApplyServices();
                ClusterSetup.ApplyFs();
                await MqttHandler.MqttServerSetupForCluster();
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
                    try {
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
                    catch(System.Exception ex) {
                        ConsoleLogger.Error(ex.ToString());
                    }
                }
                return HttpStatusCode.OK;
            };

            #region [    Handshake + cluster init    ]
            Post["/handshake/begin"] = x => {
                string conf = Request.Form.Data;
                var remoteNode = JsonConvert.DeserializeObject<NodeModel[]>(conf);
                if(remoteNode == null) {
                    return HttpStatusCode.BadRequest;
                }
                var result = ClusterSetup.HandshakeBegin(remoteNode);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            Post["/handshake"] = x => {
                string apple = Request.Form.ApplePie;
                ClusterSetup.Handshake(apple);
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