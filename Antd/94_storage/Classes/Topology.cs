using Antd;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kvpbase {
    public class Topology {
        #region Public-Members

        public DateTime? LastProcessed { get; set; }
        public int? CurrNodeId { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Node> Replicas { get; set; }

        #endregion

        #region Constructors-and-Factories

        public Topology() {

        }

        public static Topology FromFile(string filename) {
            if(String.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));
            if(!Common.FileExists(filename))
                throw new FileNotFoundException(nameof(filename));

            ConsoleLogger.Log("Reading topology from " + filename);
            string contents = Common.ReadTextFile(@filename);

            if(String.IsNullOrEmpty(contents)) {
                Common.ExitApplication("Topology", "Unable to read contents of " + filename, -1);
                return null;
            }

            ConsoleLogger.Log("Deserializing " + filename);
            Topology ret = null;

            try {
                ret = Common.DeserializeJson<Topology>(contents);
                if(ret == null) {
                    Common.ExitApplication("Topology", "Unable to deserialize " + filename + " (null)", -1);
                    return null;
                }
            }
            catch(Exception e) {
                ConsoleLogger.Warn("Topology Deserialization issue with " + filename + e.ToString());
                Common.ExitApplication("Topology", "Unable to deserialize " + filename + " (exception)", -1);
                return null;
            }

            return ret;
        }

        public static Topology Import(ClusterNode[] nodes) {
            var topologyNodes = new List<Node>();
            for(var i = 0; i < nodes.Length; i++) {
                var ntn = new Node() {
                    NodeId = i + 1,
                    NodeUid = nodes[i].MachineUid,
                    DnsHostname = nodes[i].PublicIp,
                    Port = Application.STORAGESERVER_PORT,
                    Ssl = 0
                };
                topologyNodes.Add(ntn);
            }
            foreach(var node in topologyNodes) {
                node.Neighbors = topologyNodes.Where(_ => _.NodeUid != node.NodeUid).Select(_ => _.NodeId).ToList();
            }
            var topology = new Topology() {
                CurrNodeId = topologyNodes.FirstOrDefault(_ => _.NodeUid == Application.CurrentConfiguration.Host.MachineUid.ToString()).NodeId,
                Nodes = topologyNodes
            };
            return topology;
        }

        #endregion

        #region Public-Methods

        public bool PopulateReplicas(Node currentNode) {
            if(currentNode == null)
                throw new ArgumentNullException(nameof(currentNode));

            Replicas = new List<Node>();
            if(currentNode.Neighbors != null && currentNode.Neighbors.Count > 0) {
                ConsoleLogger.Log("Topology has " + currentNode.Neighbors.Count + " defined");
                foreach(int currReplicaNodeId in currentNode.Neighbors) {
                    ConsoleLogger.Log("  Evaluating node ID " + currReplicaNodeId);

                    foreach(Node curr in Nodes) {
                        if(currReplicaNodeId == curr.NodeId) {
                            ConsoleLogger.Log("  Added node ID " + currReplicaNodeId + " as a replica");
                            Replicas.Add(curr);
                        }
                        else {
                            ConsoleLogger.Log("  Skipping node ID " + curr.NodeId + ", not a neighbor");
                        }
                    }
                }
            }
            else {
                ConsoleLogger.Log("Topology has no neighbors defined");
            }

            ConsoleLogger.Log("Topology validated without error (populated " + Replicas.Count + " replicas)");
            return true;
        }

        public bool ValidateTopology(out Node currentNode) {
            currentNode = null;
            List<int> allNodeIds = new List<int>();

            #region Build-All-Node-ID-List

            if(Nodes == null || Nodes.Count < 1) {
                ConsoleLogger.Log("No nodes found in topology");
                return false;
            }

            foreach(Node curr in Nodes) {
                allNodeIds.Add(curr.NodeId);
            }

            #endregion

            #region Find-Current-Node

            bool currentNodeFound = false;

            foreach(Node curr in Nodes) {
                if(CurrNodeId == curr.NodeId) {
                    currentNode = curr;
                    currentNodeFound = true;
                    break;
                }
            }

            if(!currentNodeFound) {
                ConsoleLogger.Log("Unable to find local node in topology");
                return false;
            }

            #endregion

            #region Verify-Replicas-Exit

            foreach(Node currNode in Nodes) {
                if(currNode.Neighbors == null || currNode.Neighbors.Count < 1)
                    continue;

                foreach(int currReplicaNodeId in currNode.Neighbors) {
                    if(!allNodeIds.Contains(currReplicaNodeId)) {
                        ConsoleLogger.Log("Replica node ID " + currReplicaNodeId + " not found in node list");
                        return false;
                    }
                }
            }

            #endregion

            return true;
        }

        public bool IsEmpty() {
            if(Nodes.Count < 2)
                return true;
            return false;
        }

        #endregion
    }
}
