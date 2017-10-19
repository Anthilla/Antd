using Antd.cmds;
using anthilla.core;
using anthilla.scheduler;
using System.Collections.Generic;
using System.Linq;

namespace Antd {
    /// <summary>
    /// Controlla gli ip dei nodi del cluster
    /// Per ogni ip gli associa un hostname 
    /// da importare in KnownHosts 
    /// e poi da salvare in /etc/hosts
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
            var clusterStatus = Application.ClusterChecklist;
            if(clusterStatus == null) {
                return;
            }
            if(clusterStatus.Length < 1) {
                return;
            }
            var nodesKnownHosts = new List<KnownHost>();
            for(var i = 0; i < clusterStatus.Length; i++) {
                var nodeIPs = clusterStatus[i].DiscoveredIpsReach;
                var nodeName = clusterStatus[i].Hostname;
                var commonNames = new string[] {
                    CommonString.Append(nodeName, "int", i.ToString())
                };
                for(var p = 0; p < nodeIPs.Length; p++) {
                    var knownHost = new KnownHost() {
                        IpAddr = nodeIPs[p].IpAddress,
                        CommonNames = commonNames
                    };
                    nodesKnownHosts.Add(knownHost);
                }
            }
            var currentKnownHosts = Application.CurrentConfiguration.Network.KnownHosts.ToList();
            foreach(var nodeHost in nodesKnownHosts) {
                //i casi possono essere tre:
                //  1) l'ip non è presente nella CurrentConfiguration   -> aggiungo il nuovo KnownHost
                //  2) l'ip è presente ma i CommonNames sono differenti -> aggiorno solamente i CommonNames del KnownHost corrispondente
                //  3) l'ip è presente e i CommonNames coincidono       -> non faccio nulla
                if(!currentKnownHosts.Any(_ => CommonString.AreEquals(_.IpAddr, nodeHost.IpAddr) == true)) {
                    currentKnownHosts.Add(nodeHost);
                }
                else {
                    var existingCn = currentKnownHosts.FirstOrDefault(_ => CommonString.AreEquals(_.IpAddr, nodeHost.IpAddr) == true).CommonNames;
                    if(CommonString.AreEquals(CommonString.Build(existingCn), CommonString.Build(nodeHost.CommonNames)) == false) {
                        currentKnownHosts.FirstOrDefault(_ => CommonString.AreEquals(_.IpAddr, nodeHost.IpAddr) == true).CommonNames = nodeHost.CommonNames;
                    }
                }
            }
            Application.CurrentConfiguration.Network.KnownHosts = currentKnownHosts.ToArray();
            ConfigRepo.Save();
            Dns.Set();
        }
    }
}
