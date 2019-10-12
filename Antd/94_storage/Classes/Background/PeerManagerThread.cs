using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kvpbase {
    public class PeerManagerThread {
        public PeerManagerThread(Settings settings, Topology topology, Node self) {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));
            if(self == null)
                throw new ArgumentNullException(nameof(self));
            Task.Run(() => PeerManagerWorker(settings, topology, self));
        }

        private void PeerManagerWorker(Settings settings, Topology topology, Node self) {
            #region Setup

            if(topology == null || topology.IsEmpty()) {
                ConsoleLogger.Warn("PeerManagerWorker exiting, no topology");
                return;
            }

            if(settings.DefaultRefreshSec <= 0) {
                ConsoleLogger.Warn("PeerManagerWorker setting topology refresh timer to 10 sec (config value too low: " + settings.DefaultRefreshSec + " sec)");
                settings.DefaultRefreshSec = 10;
            }

            ConsoleLogger.Warn("PeerManagerWorker starting with topology refresh timer set to " + settings.DefaultRefreshSec + " sec");

            #endregion

            #region Process

            bool firstRun = true;
            while(true) {
                #region Wait

                if(!firstRun) {
                    Thread.Sleep(settings.DefaultRefreshSec * 1000);
                }
                else {
                    firstRun = false;
                }

                #endregion

                #region Session-Variables

                List<Node> updatedNodeList = new List<Node>();
                List<Node> updatedNeighborList = new List<Node>();

                #endregion

                #region Process-the-List

                foreach(Node curr in topology.Nodes) {
                    #region Skip-if-Self

                    if(self.NodeId == curr.NodeId) {
                        updatedNodeList.Add(curr);
                        continue;
                    }

                    #endregion

                    #region Set-URL

                    string url = "";
                    if(Common.IsTrue(curr.Ssl)) {
                        url = "https://" + curr.DnsHostname + ":" + curr.Port + "/admin/heartbeat";
                    }
                    else {
                        url = "http://" + curr.DnsHostname + ":" + curr.Port + "/admin/heartbeat";
                    }

                    #endregion

                    #region Process-REST-Request

                    RestWrapper.RestResponse resp = RestRequest.SendRequestSafe(
                        url, "application/json", "GET", null, null, false,
                        Common.IsTrue(settings.Rest.AcceptInvalidCerts),
                        Common.AddToDictionary(settings.Server.HeaderApiKey, settings.Server.AdminApiKey, null),
                        null);

                    if(resp == null) {
                        #region No-REST-Response

                        curr.NumFailures++;
                        curr.LastAttempt = DateTime.Now;
                        ConsoleLogger.Warn("PeerManagerWorker null response connecting to " + url + " (" + curr.NumFailures + " failed attempts)");
                        updatedNodeList.Add(curr);
                        if(self.IsNeighbor(curr))
                            updatedNeighborList.Add(curr);
                        continue;

                        #endregion
                    }
                    else {
                        if(resp.StatusCode != 200) {
                            #region Failed-Heartbeat

                            curr.NumFailures++;
                            curr.LastAttempt = DateTime.Now;
                            ConsoleLogger.Warn("PeerManagerWorker non-200 (" + resp.StatusCode + ") response connecting to " + url + " (" + curr.NumFailures + " failed attempts)");
                            updatedNodeList.Add(curr);
                            if(self.IsNeighbor(curr))
                                updatedNeighborList.Add(curr);
                            continue;

                            #endregion
                        }
                        else {
                            #region Successful-Heartbeat

                            curr.NumFailures = 0;
                            curr.LastAttempt = DateTime.Now;
                            curr.LastSuccess = DateTime.Now;
                            updatedNodeList.Add(curr);
                            if(self.IsNeighbor(curr))
                                updatedNeighborList.Add(curr);
                            continue;

                            #endregion
                        }
                    }

                    #endregion
                }

                #endregion

                #region Update-the-Lists

                topology.Nodes = updatedNodeList;
                topology.Replicas = updatedNeighborList;
                topology.LastProcessed = DateTime.Now;

                #endregion
            }

            #endregion
        }
    }
}