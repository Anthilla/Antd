using Antd.models;
using anthilla.core;
using anthilla.scheduler;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Antd {
    public class ClusterHeartbeatCheckJob : Job {

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

        private const byte Ok = 0;
        private const byte Ko = 1;
        private const string serviceStatusPath = "/device/ok";
        private const string machineChecklistPath = "/device/checklist";
        private const string virshStatusPath = "/device/vm";
        private const string appUptimePath = "/device/antduptime";
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
            var statusList = new ClusterNodeChecklistModel[nodes.Length];
            for(var i = 0; i < nodes.Length; i++) {
                statusList[i] = NodeStatus(nodes[i]);
            }
            Application.ClusterChecklist = statusList;
        }

        private ClusterNodeChecklistModel NodeStatus(ClusterNode node) {
            //ConsoleLogger.Log($"[hb] check node {node.Hostname} {node.MachineUid}");
            var status = new ClusterNodeChecklistModel();
            status.TargetNodeMachineUid = node.MachineUid;
            status.Hostname = node.Hostname;

            //controllo l'IP pubblico
            status.KnownPublicIpReach = PingStatus(node.PublicIp);
            if(status.KnownPublicIpReach == 1) {
                //ConsoleLogger.Warn($"[hb] {node.Hostname} is unreachable at its known public ip");
                return status;
            }

            //controllo antd
            var serviceStatus = ApiConsumer.Post(CommonString.Append(node.EntryPoint, serviceStatusPath));
            if(serviceStatus == Nancy.HttpStatusCode.OK) {
                status.ServiceReach = 0;
            }

            //controllo se ho già salvato delle informazioni
            var storedNodeIps = Application.ClusterChecklist?.FirstOrDefault(_ => _.TargetNodeMachineUid == node.MachineUid)?.DiscoveredIpsReach?.Select(_ => _.IpAddress) ?? new string[0];

            //controllo gli IP scoperti
            var nodeIps = ApiConsumer.Get<string[]>(CommonString.Append(node.EntryPoint, networkAddressPath)) ?? new string[0];
            nodeIps = storedNodeIps.Union(nodeIps).Where(_ => _ != localIp).ToArray();

            var ipStatusList = new ClusterNodeIpStatusModel[nodeIps.Length];
            for(var n = 0; n < nodeIps.Length; n++) {
                var ipStatus = new ClusterNodeIpStatusModel();
                ipStatus.IpAddress = nodeIps[n];
                ipStatus.Status = PingStatus(nodeIps[n]);
                ipStatusList[n] = ipStatus;
            }
            status.DiscoveredIpsReach = ipStatusList;

            var uptime = ApiConsumer.GetString(CommonString.Append(node.EntryPoint, appUptimePath));
            status.ApplicationUptime = uptime;

            //controllo stato nodo
            var nodeChecklist = ApiConsumer.Get<MachineStatusChecklistModel>(CommonString.Append(node.EntryPoint, machineChecklistPath)) ?? new MachineStatusChecklistModel();
            status.InternetReach = nodeChecklist.InternetReach;
            status.InternetDnsReach = nodeChecklist.InternetDnsReach;

            //controllo servizio: virsh
            var virshStatus = ApiConsumer.Get<VirshModel>(CommonString.Append(node.EntryPoint, virshStatusPath)) ?? new VirshModel();
            status.VirshService = virshStatus;

            return status;
        }

        private byte PingStatus(string ip) {
            try {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(ip, 200);
                return reply.Status == IPStatus.Success ? Ok : Ko;
            }
            catch(Exception) {
                return Ko;
            }
        }
    }
}
