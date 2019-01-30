using Antd.cmds;
using anthilla.core;
using anthilla.scheduler;
using System.Linq;

namespace Antd {

    /// <summary>
    /// Controlla gli ip dei nodi del cluster
    /// Per ogni ip gli associa un hostname 
    /// da importare in KnownHosts 
    /// e poi da salvare in /etc/hosts
    /// 
    /// In /etc/hosts posso avere più righe con lo stesso ip, per esempio:
    ///  
    ///     10.1.2.3 hostname.local hostname
    ///     
    /// può anche essere scritto
    ///     
    ///     10.1.2.4 hostname.local
    ///     10.1.2.4 hostname
    ///     
    /// </summary>
    public class ClusterCheckHostnameJob : Job {

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

        private int _repetitionIntervalTime = 300000; //5 minuti

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

        public override void DoJob() {
            if(Application.CurrentConfiguration.Cluster.Active == false) {
                return;
            }
            var clusterNodes = Application.CurrentConfiguration.Cluster.Nodes;
            if(clusterNodes == null) {
                return;
            }
            if(clusterNodes.Length < 1) {
                return;
            }
            var currentKnownHosts = Application.CurrentConfiguration.Network.KnownHosts.ToList();
            bool configIsChanged = false;

            //per ogni nodo del cluster
            for(var i = 0; i < clusterNodes.Length; i++) {
                var nodeIPs = clusterNodes[i].PublicIp;
                var nodeName = clusterNodes[i].Hostname;

                //controllo nella configurazione di KnownHosts se contiene già info sul nodo
                var currentKnownNode = currentKnownHosts.FirstOrDefault(_ => CommonString.AreEquals(_.IpAddr, nodeIPs));
                if(currentKnownNode == null) {
                    //aggiungo uno nuovo host
                    var host = new KnownHost() {
                        IpAddr = nodeIPs,
                        CommonNames = new string[] { nodeName }
                    };
                    currentKnownHosts.Add(host);
                    configIsChanged = true;
                }
                else {
                    //controllo i common names
                    if(!currentKnownNode.CommonNames.Contains(nodeName)) {
                        var commonNamesUpdate = currentKnownNode.CommonNames.ToList();
                        commonNamesUpdate.Add(nodeName);
                        currentKnownNode.CommonNames = commonNamesUpdate.ToArray();
                        configIsChanged = true;
                    }
                }
            }
            Application.CurrentConfiguration.Network.KnownHosts = currentKnownHosts.ToArray();
            if(configIsChanged) {
                ConfigRepo.Save();
            }
            Dns.Set();
        }
    }
}
