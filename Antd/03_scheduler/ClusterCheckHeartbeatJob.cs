using anthilla.core;
using anthilla.scheduler;
using System.Net.NetworkInformation;

namespace Antd {
    public class ClusterCheckHeartbeatJob : Job {

        #region [    Core Parameter    ]
        private bool _isRepeatable = true;

        public override bool IsRepeatable {
            get {
                return _isRepeatable;
            }
            set {
                value = _isRepeatable;
            }
        }

        //private int _repetitionIntervalTime = 250;
        private int _repetitionIntervalTime = 2000;

        public override int RepetitionIntervalTime {
            get {
                return _repetitionIntervalTime;
            }

            set {
                value = _repetitionIntervalTime;
            }
        }

        public override string Name {
            get {
                return GetType().Name;
            }

            set {
                value = GetType().Name;
            }
        }
        #endregion

        private const string serviceStatusPath = "/device/ok";
        private const string networkAddressPath = "/network/devices/addr";
        private const string localIp = "127.0.0.1";

        /// <summary>
        /// flusso
        /// - controllo lista ip pubblici noti (ping)
        ///     - KO -> esco
        ///     - OK -> 
        ///         - controllo servizio (antd)
        ///             - KO -> esco: la macchina è raggiungibile ma antd no
        ///             - OK ->
        ///                 - controllo stato macchina (antd)
        ///                     - KO ->
        ///                         - applico correzioni
        ///                     - OK ->
        ///                         - salvo stato macchina
        ///                         - aggiungo tutti gli ip alla lista ip pubblici noti
        /// </summary>
        public override void DoJob() {
            if(Application.CurrentConfiguration.Cluster.Active == false) {
                return;
            }
            var nodes = Application.CurrentConfiguration.Cluster.Nodes;
            for(var i = 0; i < nodes.Length; i++) {
                var node = nodes[i];
                if(CommonString.AreEquals(node.MachineUid, Application.CurrentConfiguration.Host.MachineUid.ToString()) == true) {
                    continue;
                }
                var entryPoint = node.EntryPoint;
                ConsoleLogger.Log($"[hb] entry point found at {entryPoint}");

                var nodesIps = ApiConsumer.Get<string[]>(CommonString.Append(entryPoint, networkAddressPath));
                if(nodesIps == null) {
                    continue;
                }
                for(var n = 0; n < nodesIps.Length; n++) {
                    if(CommonString.AreEquals(nodesIps[n], localIp) == true) {
                        continue;
                    }
                    var status = NodeStatus(nodesIps[n]);
                    if(!status) {
                        ConsoleLogger.Log($"[hb] 2/X ping node ip '{nodesIps[n]}': fail");
                    }
                    else {
                        ConsoleLogger.Log($"[hb] 2/X ping node ip '{nodesIps[n]}': success");
                    }
                }

                var ping01 = ApiConsumer.Post(CommonString.Append(entryPoint, serviceStatusPath));
                if(ping01 != Nancy.HttpStatusCode.OK) {
                    ConsoleLogger.Log($"[hb] 1/X ping node service: fail");
                }
                else {
                    ConsoleLogger.Log($"[hb] 1/X ping node service: success");
                }

            }
        }

        private bool NodeStatus(ClusterNode clusterNode) {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(clusterNode.PublicIp, 200);
            return reply.Status == IPStatus.Success;
        }

        private bool NodeStatus(string ip) {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(ip, 200);
            return reply.Status == IPStatus.Success;
        }
    }
}
