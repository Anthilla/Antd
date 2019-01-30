using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Kvpbase {
    public class ReplicationHandler {
        #region Public-Members

        #endregion

        #region Private-Members

        private Settings CurrentSettings;
        private Topology CurrentTopology;
        private Node CurrentNode;
        private UserManager Users;

        #endregion

        #region Constructors-and-Factories

        public ReplicationHandler(Settings settings, Topology topology, Node node, UserManager users) {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));
            if(node == null)
                throw new ArgumentNullException(nameof(node));
            if(users == null)
                throw new ArgumentNullException(nameof(users));

            CurrentSettings = settings;
            CurrentTopology = topology;
            CurrentNode = node;
            Users = users;
        }

        #endregion

        #region Public-Methods

        #region Client-Sender-Methods

        public bool ContainerDelete(Obj currObj, List<Node> nodes) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ContainerDelete null value for currObj");
                    return false;
                }

                if(nodes == null)
                    return true;
                if(nodes.Count < 1)
                    return true;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Build-Dictionaries

                headers.Add(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey);

                #endregion

                #region Process-Each-Node

                foreach(Node curr in nodes) {
                    ConsoleLogger.Warn("ContainerDelete removing container from node " + curr.Name + " (ID " + curr.NodeId + ")");

                    #region Reset-Variables

                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Generate-URL

                    if(Common.IsTrue(curr.Ssl)) {
                        url = "https://" + curr.DnsHostname + ":" + curr.Port + "/admin/replication/container";
                    }
                    else {
                        url = "http://" + curr.DnsHostname + ":" + curr.Port + "/admin/replication/container";
                    }

                    #endregion

                    #region Submit-Cleanup-Request

                    resp = RestRequest.SendRequestSafe(
                        url,
                        "application/json",
                        "DELETE",
                        null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        Encoding.UTF8.GetBytes(Common.SerializeJson(currObj)));

                    if(resp == null) {
                        ConsoleLogger.Warn("ContainerDelete null REST response while writing to " + url + ", queueing message to node ID " + curr.NodeId + " " + curr.Name);
                        Message.SendMessage(CurrentSettings, CurrentNode, curr, "DELETE /admin/replication/container", Common.SerializeJson(currObj));
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("ContainerDelete non-200/201 REST response from " + url + ", queueing message to node ID " + curr.NodeId + " " + curr.Name);
                        Message.SendMessage(CurrentSettings, CurrentNode, curr, "DELETE /admin/replication/container", Common.SerializeJson(currObj));
                        continue;
                    }

                    #endregion
                }

                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ContainerDelete exception encountered");
                ConsoleLogger.Error("ContainerDelete Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ContainerMove(MoveRequest currMove) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ContainerMove null value for move object supplied");
                    return false;
                }

                if(CurrentTopology.Replicas == null) {
                    ConsoleLogger.Warn("ContainerMove null replica list in topology");
                    return true;
                }

                if(CurrentTopology.Replicas.Count < 1) {
                    ConsoleLogger.Warn("ContainerMove empty replica list in topology");
                    return true;
                }

                #endregion

                #region Variables

                List<Node> successfulMoves = new List<Node>();

                #endregion

                #region Replication

                if(CurrentTopology.Replicas != null) {
                    foreach(Node curr in CurrentTopology.Replicas) {
                        if(!ContainerMoveReplica(currMove, curr)) {
                            ConsoleLogger.Warn("ContainerMove failed replication to node ID " + curr.NodeId + " " + curr.Name + " for move operation");
                            Task.Run(() => ContainerMoveReplicaAsync(currMove, curr));
                        }
                        else {
                            successfulMoves.Add(curr);
                        }
                    }
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ContainerMove exception encountered");
                ConsoleLogger.Error("ContainerMove Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ContainerRename(RenameRequest currRename) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ContainerRename null value for rename object supplied");
                    return false;
                }

                if(CurrentTopology.Replicas == null) {
                    ConsoleLogger.Warn("ContainerRename null replica list in topology");
                    return true;
                }

                if(CurrentTopology.Replicas.Count < 1) {
                    ConsoleLogger.Warn("ContainerRename empty replica list in topology");
                    return true;
                }

                #endregion

                #region Variables

                List<Node> successfulRenames = new List<Node>();

                #endregion

                #region Replication

                if(CurrentTopology.Replicas != null) {
                    foreach(Node curr in CurrentTopology.Replicas) {
                        if(!ContainerRenameReplica(currRename, curr)) {
                            ConsoleLogger.Warn("ContainerRename failed replication to node ID " + curr.NodeId + " " + curr.Name + " for rename operation");
                            Task.Run(() => ContainerRenameReplicaAsync(currRename, curr));
                        }
                        else {
                            successfulRenames.Add(curr);
                        }
                    }
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ContainerRename exception encountered");
                ConsoleLogger.Error("ContainerRename Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ContainerWrite(Obj currObj, out List<Node> nodes) {
            nodes = new List<Node>();

            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ContainerWrite null path object supplied");
                    return false;
                }

                #endregion

                #region Variables

                List<Node> successfulWrites = new List<Node>();

                #endregion

                #region Check-Replication-Mode

                switch(currObj.ReplicationMode) {
                    case "none":
                        return true;

                    case "async":
                        if(CurrentTopology.Replicas != null) {
                            nodes = Common.CopyObject<List<Node>>(CurrentTopology.Replicas);
                            Task.Run(() => ContainerWriteReplicaAsync(currObj));
                        }
                        return true;

                    case "sync":
                        if(CurrentTopology.Replicas != null) {
                            nodes = Common.CopyObject<List<Node>>(CurrentTopology.Replicas);
                            currObj.Replicas = nodes;
                        }
                        break;

                    default:
                        ConsoleLogger.Warn("ContainerWrite unknown replication mode in path object: " + currObj.ReplicationMode);
                        Common.ExitApplication("ContainerWrite", "Unknown replication mode", -1);
                        return false;
                }

                #endregion

                #region Sync-Replication

                if(CurrentTopology.Replicas != null) {
                    foreach(Node curr in CurrentTopology.Replicas) {
                        if(!ContainerWriteReplica(currObj, curr)) {
                            ConsoleLogger.Warn("ContainerWrite failed replication to node ID " + curr.NodeId);
                            Task.Run(() => {
                                ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                                rh.ContainerDelete(currObj, successfulWrites);
                            });
                            return false;
                        }
                        else {
                            successfulWrites.Add(curr);
                        }
                    }
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ContainerWrite exception encountered");
                ConsoleLogger.Error("ContainerWrite Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ObjectDelete(Obj currObj, List<Node> nodes) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ObjectDelete null value for currObj");
                    return false;
                }

                if(nodes == null)
                    return true;
                if(nodes.Count < 1)
                    return true;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Build-Dictionaries

                headers.Add(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey);

                #endregion

                #region Process-Each-Node

                foreach(Node curr in nodes) {
                    ConsoleLogger.Warn("ObjectDelete removing object " + currObj.Key + " from node " + curr.Name + " (node ID " + curr.NodeId + ")");

                    #region Reset-Variables

                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Generate-URL

                    if(Common.IsTrue(curr.Ssl)) {
                        url = "https://" + curr.DnsHostname + ":" + curr.Port + "/admin/replication/object";
                    }
                    else {
                        url = "http://" + curr.DnsHostname + ":" + curr.Port + "/admin/replication/object";
                    }

                    #endregion

                    #region Null-Out-Value

                    currObj.Value = null;

                    #endregion

                    #region Submit-Cleanup-Request

                    resp = RestRequest.SendRequestSafe(
                        url,
                        "application/json",
                        "DELETE",
                        null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        Encoding.UTF8.GetBytes(Common.SerializeJson(currObj)));

                    if(resp == null) {
                        ConsoleLogger.Warn("ObjectDelete null REST response while writing to " + url + ", queueing message to node ID " + curr.NodeId + " " + curr.Name);
                        Message.SendMessage(CurrentSettings, CurrentNode, curr, "DELETE /admin/replication/object", Common.SerializeJson(currObj));
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("ObjectDelete non-200/201 REST response from " + url + ", queueing message to node ID " + curr.NodeId + " " + curr.Name);
                        Message.SendMessage(CurrentSettings, CurrentNode, curr, "DELETE /admin/replication/object", Common.SerializeJson(currObj));
                        continue;
                    }

                    #endregion
                }

                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ObjectDelete exception encountered");
                ConsoleLogger.Error("ObjectDelete Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ObjectMove(MoveRequest currMove, Obj currObj) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ObjectMove null value for move object supplied");
                    return false;
                }

                if(currObj == null) {
                    ConsoleLogger.Warn("ObjectMove null obj supplied");
                    return false;
                }

                if(currObj.Replicas == null) {
                    ConsoleLogger.Warn("ObjectMove null replica list for supplied obj");
                    return true;
                }

                if(currObj.Replicas.Count < 1) {
                    ConsoleLogger.Warn("ObjectMove empty replica list for supplied obj");
                    return true;
                }

                #endregion

                #region Variables

                List<Node> successfulMoves = new List<Node>();

                #endregion

                #region Check-Replication-Mode

                switch(currObj.ReplicationMode) {
                    case "none":
                        ConsoleLogger.Warn("ObjectMove none replication mode specified");
                        return true;

                    case "async":
                    case "sync":
                        ConsoleLogger.Warn("ObjectMove sync or async replication mode specified");
                        break;

                    default:
                        ConsoleLogger.Warn("ObjectMove unknown replication mode in obj: " + currObj.ReplicationMode);
                        Common.ExitApplication("ObjectMove", "Unknown replication mode", -1);
                        return false;
                }

                #endregion

                #region Sync-Replication

                if(currObj.Replicas != null) {
                    foreach(Node curr in currObj.Replicas) {
                        switch(currObj.ReplicationMode) {
                            case "none":
                                ConsoleLogger.Warn("ObjectMove none replication mode specified");
                                continue;

                            case "async":
                                ConsoleLogger.Warn("ObjectMove async replication mode specified");
                                Task.Run(() => ObjectMoveReplicaAsync(currMove, curr));
                                break;

                            case "sync":
                                ConsoleLogger.Warn("ObjectMove sync replication mode specified");
                                if(!ObjectMoveReplica(currMove, curr)) {
                                    ConsoleLogger.Warn("ObjectMove failed replication to node ID " + curr.NodeId + " " + curr.Name + " for move operation");
                                    Task.Run(() => ObjectMoveReplicaAsync(currMove, curr));
                                }
                                else {
                                    successfulMoves.Add(curr);
                                }
                                break;

                            default:
                                ConsoleLogger.Warn("ObjectMove unknown replication mode in obj: " + currObj.ReplicationMode);
                                Common.ExitApplication("ObjectMove", "Unknown replication mode", -1);
                                return false;
                        }
                    }
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ObjectMove exception encountered");
                ConsoleLogger.Error("ObjectMove Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ObjectRename(RenameRequest currRename, Obj currObj) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ObjectRename null value for rename object supplied");
                    return false;
                }

                if(currObj == null) {
                    ConsoleLogger.Warn("ObjectRename null obj supplied");
                    return false;
                }

                if(currObj.Replicas == null) {
                    ConsoleLogger.Warn("ObjectRename null replica list for supplied obj");
                    return true;
                }

                if(currObj.Replicas.Count < 1) {
                    ConsoleLogger.Warn("ObjectRename empty replica list for supplied obj");
                    return true;
                }

                #endregion

                #region Variables

                List<Node> successfulRenames = new List<Node>();

                #endregion

                #region Check-Replication-Mode

                switch(currObj.ReplicationMode) {
                    case "none":
                        ConsoleLogger.Warn("ObjectRename none replication mode specified");
                        return true;

                    case "async":
                    case "sync":
                        ConsoleLogger.Warn("ObjectRename sync or async replication mode specified");
                        break;

                    default:
                        ConsoleLogger.Warn("ObjectRename unknown replication mode in obj: " + currObj.ReplicationMode);
                        Common.ExitApplication("ObjectRename", "Unknown replication mode", -1);
                        return false;
                }

                #endregion

                #region Sync-Replication

                if(currObj.Replicas != null) {
                    foreach(Node currNode in currObj.Replicas) {
                        switch(currObj.ReplicationMode) {
                            case "none":
                                ConsoleLogger.Warn("ObjectRename none replication mode specified");
                                continue;

                            case "async":
                                ConsoleLogger.Warn("ObjectRename async replication mode specified");
                                Task.Run(() => ObjectRenameReplicaAsync(currRename, currNode));
                                break;

                            case "sync":
                                ConsoleLogger.Warn("ObjectRename sync replication mode specified");
                                if(!ObjectRenameReplica(currRename, currNode)) {
                                    ConsoleLogger.Warn("ObjectRename failed replication to node ID " + currNode.NodeId + " " + currNode.Name + " for rename operation");
                                    Task.Run(() => ObjectRenameReplicaAsync(currRename, currNode));
                                }
                                else {
                                    successfulRenames.Add(currNode);
                                }
                                break;

                            default:
                                ConsoleLogger.Warn("ObjectRename unknown replication mode in obj: " + currObj.ReplicationMode);
                                Common.ExitApplication("ObjectRename", "Unknown replication mode", -1);
                                return false;
                        }
                    }
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ObjectRename exception encountered");
                ConsoleLogger.Error("ObjectRename Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ObjectWrite(Obj currObj, out List<Node> nodes) {
            nodes = new List<Node>();

            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ObjectWrite null value detected for path");
                    return false;
                }

                #endregion

                #region Variables

                List<Node> successfulWrites = new List<Node>();

                #endregion

                #region Copy-Path-into-Object

                if(currObj.ContainerPath == null) {
                    ConsoleLogger.Warn("ObjectWrite path container path is null");
                }
                else {
                    ConsoleLogger.Warn("ObjectWrite path container path has " + currObj.ContainerPath.Count + " entries");
                }

                #endregion

                #region Check-Replication-Mode

                ConsoleLogger.Warn("ObjectWrite replication mode set to " + currObj.ReplicationMode);
                switch(currObj.ReplicationMode) {
                    case "none":
                        ConsoleLogger.Warn("ObjectWrite replication set to none");
                        return true;

                    case "async":
                        if(CurrentTopology.Replicas != null) {
                            nodes = Common.CopyObject<List<Node>>(CurrentTopology.Replicas);
                            currObj.Replicas = nodes;
                            Task.Run(() => ObjectWriteReplicaAsync(currObj));
                        }
                        return true;

                    case "sync":
                        if(CurrentTopology.Replicas != null) {
                            nodes = Common.CopyObject<List<Node>>(CurrentTopology.Replicas);
                            currObj.Replicas = nodes;
                        }
                        break;

                    default:
                        ConsoleLogger.Warn("ObjectWrite unknown replication mode in obj: " + currObj.ReplicationMode);
                        Common.ExitApplication("ObjectWrite", "Unknown replication mode", -1);
                        return false;
                }

                #endregion

                #region Sync-Replication

                if(CurrentTopology.Replicas != null) {
                    foreach(Node currNode in CurrentTopology.Replicas) {
                        if(!ObjectWriteReplica(currObj, currNode)) {
                            ConsoleLogger.Warn("ObjectWrite failed replication to node ID " + currNode.NodeId + " " + currNode.Name + " for key " + currObj.Key);
                            Task.Run(() => {
                                ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                                rh.ObjectDelete(currObj, successfulWrites);
                            });
                            return false;
                        }
                        else {
                            successfulWrites.Add(currNode);
                        }
                    }
                }

                #endregion

                ConsoleLogger.Warn("ObjectWrite replicated to " + successfulWrites.Count + " nodes");
                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ObjectWrite exception encountered");
                ConsoleLogger.Error("ObjectWrite Outer exception" + e.ToString());
                return false;
            }
        }

        #endregion

        #region Server-Receive-Methods

        public HttpResponse ServerObjectMove(RequestMetadata md) {
            try {
                #region Deserialize-and-Initialize

                MoveRequest req = new MoveRequest();
                try {
                    req = Common.DeserializeJson<MoveRequest>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ServerObjectMove null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ServerObjectMove unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                }

                #endregion

                #region Process

                if(ServerObjectMoveInternal(req)) {
                    ConsoleLogger.Warn("ServerObjectMove successfully processed move request");
                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                }
                else {
                    ConsoleLogger.Warn("ServerObjectMove unable to process move request");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(), true);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerObjectMove exception encountered");
                ConsoleLogger.Error("ServerObjectMove Outer exception" + e.ToString());
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(), true);
            }
        }

        public HttpResponse ServerObjectRename(RequestMetadata md) {
            try {
                #region Deserialize-and-Initialize

                RenameRequest req = new RenameRequest();
                try {
                    req = Common.DeserializeJson<RenameRequest>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ServerObjectRename null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ServerObjectRename unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                }

                #endregion

                #region Process

                if(ServerObjectRenameInternal(req)) {
                    ConsoleLogger.Warn("ServerObjectRename successfully processed rename request");
                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                }
                else {
                    ConsoleLogger.Warn("ServerObjectRename unable to process rename request");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to process rename request.", null).ToJson(), true);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerObjectRename exception encountered");
                ConsoleLogger.Error("ServerObjectRename Outer exception" + e.ToString());
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(), true);
            }
        }

        public HttpResponse ServerContainerMove(RequestMetadata md) {
            try {
                #region Deserialize-and-Initialize

                MoveRequest req = new MoveRequest();
                try {
                    req = Common.DeserializeJson<MoveRequest>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ServerContainerMove null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                            true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ServerContainerMove unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                        true);
                }

                #endregion

                #region Process

                if(ServerContainerMoveInternal(req)) {
                    ConsoleLogger.Warn("ServerContainerMove successfully processed move request");
                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                }
                else {
                    ConsoleLogger.Warn("ServerContainerMove unable to process move request");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(),
                        true);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerContainerMove exception encountered");
                ConsoleLogger.Error("ServerContainerMove Outer exception" + e.ToString());
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(), true);
            }
        }

        public HttpResponse ServerContainerRename(RequestMetadata md) {
            try {
                #region Deserialize-and-Initialize

                RenameRequest req = new RenameRequest();
                try {
                    req = Common.DeserializeJson<RenameRequest>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ServerContainerRename null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ServerContainerRename unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);

                }

                #endregion

                #region Process

                if(ServerContainerRenameInternal(req)) {
                    ConsoleLogger.Warn("ServerContainerRename successfully processed rename request");
                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                }
                else {
                    ConsoleLogger.Warn("ServerContainerRename unable to process rename request");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to process rename request.", null).ToJson(), true);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerContainerRename exception encountered");
                ConsoleLogger.Error("ServerContainerRename Outer exception" + e.ToString());
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(), true);
            }
        }

        public HttpResponse ServerObjectReceive(RequestMetadata md) {
            try {
                #region Deserialize-and-Initialize

                Obj req = new Obj();
                try {
                    req = Common.DeserializeJson<Obj>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ServerObjectReceive null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ServerObjectReceive unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                }

                #endregion

                #region Retrieve-User

                UserMaster currUser = Users.GetUserByGuid(req.UserGuid);
                if(currUser == null) {
                    ConsoleLogger.Warn("ServerObjectReceive unable to retrieve user for GUID " + req.UserGuid);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to find user in request.", null).ToJson(), true);
                }

                #endregion

                #region Overwrite-Path-in-Path-Object

                req.DiskPath = Obj.BuildDiskPath(req, currUser, CurrentSettings);
                if(String.IsNullOrEmpty(req.DiskPath)) {
                    ConsoleLogger.Warn("ServerObjectReceive unable to build disk path from request");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                }

                #endregion

                #region Process

                if(ServerObjectReceiveInternal(req)) {
                    ConsoleLogger.Warn("ServerObjectReceive successfully stored " + req.Key);
                    return new HttpResponse(md.CurrentHttpRequest, true, 201, null, "application/json", null, true);
                }
                else {
                    ConsoleLogger.Warn("ServerObjectReceive unable to store " + req.Key);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to store object.", null).ToJson(), true);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerObjectReceive exception encountered");
                ConsoleLogger.Error("ServerObjectReceive Outer exception" + e.ToString());
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(), true);
            }
        }

        public HttpResponse ServerContainerReceive(RequestMetadata md) {
            try {
                #region Deserialize-and-Initialize

                Obj req = new Obj();
                try {
                    req = Common.DeserializeJson<Obj>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ServerContainerReceive null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                            true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ServerContainerReceive unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                        true);
                }

                #endregion

                #region Retrieve-User

                UserMaster currUser = Users.GetUserByGuid(req.UserGuid);
                if(currUser == null) {
                    ConsoleLogger.Warn("ServerContainerReceive unable to retrieve user for GUID " + req.UserGuid);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to find user in request.", null).ToJson(), true);
                }

                #endregion

                #region Overwrite-Path-in-Path-Object

                req.DiskPath = Obj.BuildDiskPath(req, currUser, CurrentSettings);
                if(String.IsNullOrEmpty(req.DiskPath)) {
                    ConsoleLogger.Warn("ServerContainerReceive unable to build disk path from request");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(),
                        true);
                }

                #endregion

                #region Process

                if(ServerContainerReceiveInternal(req)) {
                    ConsoleLogger.Warn("ServerContainerReceive successfully wrote " + req.DiskPath);
                    return new HttpResponse(md.CurrentHttpRequest, true, 201, null, "application/json", null, true);
                }
                else {
                    ConsoleLogger.Warn("ServerContainerReceive unable to store " + req.Key);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to store container.", null).ToJson(),
                        true);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerContainerReceive exception encountered");
                ConsoleLogger.Error("ServerContainerReceive Outer exception" + e.ToString());
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to process move request.", null).ToJson(), true);
            }
        }

        //
        // These internal methods must be marked as public as they are used elsewhere
        //

        public bool ServerObjectMoveInternal(MoveRequest currMove) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal null value for currMove");
                    return false;
                }

                #endregion

                #region Variables

                Obj currObj = new Obj();

                #endregion

                #region Check-for-Unsafe-Characters

                if(MoveRequest.UnsafeFsChars(currMove)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal unsafe characters detected in request");
                    return false;
                }

                #endregion

                #region Check-if-Original-Object-Exists

                string diskPathOriginalObj = MoveRequest.BuildDiskPath(currMove, true, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathOriginalObj)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal unable to build disk path for original object");
                    return false;
                }

                if(!Common.FileExists(diskPathOriginalObj)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal from object does not exist: " + diskPathOriginalObj);
                    return false;
                }

                #endregion

                #region Check-if-Target-Container-Exists

                string diskPathTargetContainer = MoveRequest.BuildDiskPath(currMove, false, false, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTargetContainer)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal unable to build disk path for target container");
                    return false;
                }

                if(!Common.DirectoryExists(diskPathTargetContainer)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal target container does not exist: " + diskPathOriginalObj);
                    return false;
                }

                #endregion

                #region Check-if-Target-Object-Exists

                string diskPathTargetObj = MoveRequest.BuildDiskPath(currMove, false, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTargetObj)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal unable to build disk path for target object");
                    return false;
                }

                if(Common.FileExists(diskPathTargetObj)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal target object already exists: " + diskPathOriginalObj);
                    return false;
                }

                #endregion

                #region Read-Object

                currObj = Obj.BuildObjFromDisk(diskPathOriginalObj, Users, CurrentSettings, CurrentTopology, CurrentNode);
                if(currObj == null) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal unable to retrieve obj for " + diskPathOriginalObj);
                    return false;
                }

                #endregion

                #region Perform-Move

                if(!Common.MoveFile(diskPathOriginalObj, diskPathTargetObj)) {
                    ConsoleLogger.Warn("ServerObjectMoveInternal unable to move file from " + diskPathOriginalObj + " to " + diskPathTargetObj);
                    return false;
                }

                #endregion

                #region Perform-Background-Rewrite

                if(!Common.IsTrue(currObj.GatewayMode)) {
                    ConsoleLogger.Warn("PostReplicationMoveObject spawning background task to rewrite object with correct metadata");
                    Task.Run(() => RewriteObject(diskPathTargetObj));
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerObjectMoveInternal exception encountered");
                ConsoleLogger.Error("ServerObjectMoveInternal Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ServerObjectRenameInternal(RenameRequest currRename) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ServerObjectRenameInternal null value for currRename");
                    return false;
                }

                #endregion

                #region Variables

                Obj currObj = new Obj();

                #endregion

                #region Check-for-Unsafe-Characters

                if(RenameRequest.UnsafeFsChars(currRename)) {
                    ConsoleLogger.Warn("ServerObjectRenameInternal unsafe characters detected in request");
                    return false;
                }

                #endregion

                #region Check-if-Original-Exists

                string diskPathOriginal = RenameRequest.BuildDiskPath(currRename, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathOriginal)) {
                    ConsoleLogger.Warn("ServerObjectRenameInternal unable to build disk path for original object");
                    return false;
                }

                if(!Common.FileExists(diskPathOriginal)) {
                    ConsoleLogger.Warn("ServerObjectRenameInternal from object does not exist: " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Check-if-Target-Exists

                string diskPathTarget = RenameRequest.BuildDiskPath(currRename, false, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTarget)) {
                    ConsoleLogger.Warn("ServerObjectRenameInternal unable to build disk path for target object");
                    return false;
                }

                if(Common.FileExists(diskPathTarget)) {
                    ConsoleLogger.Warn("ServerObjectRenameInternal target object already exists: " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Read-Object

                currObj = Obj.BuildObjFromDisk(diskPathOriginal, Users, CurrentSettings, CurrentTopology, CurrentNode);
                if(currObj == null) {
                    ConsoleLogger.Warn("PostReplicationRenameObject unable to retrieve obj for " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Perform-Rename

                if(!Common.RenameFile(diskPathOriginal, diskPathTarget)) {
                    ConsoleLogger.Warn("ServerObjectRenameInternal unable to rename file from " + diskPathOriginal + " to " + diskPathTarget);
                    return false;
                }

                #endregion

                #region Perform-Background-Rewrite

                if(!Common.IsTrue(currObj.GatewayMode)) {
                    ConsoleLogger.Warn("PostReplicationRenameObject spawning background task to rewrite object with correct metadata");
                    Task.Run(() => RewriteObject(diskPathTarget));
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerObjectRenameInternal exception encountered");
                ConsoleLogger.Error("ServerObjectRenameInternal Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ServerContainerMoveInternal(MoveRequest currMove) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal null value for currMove");
                    return false;
                }

                #endregion

                #region Variables

                bool userGatewayMode = false;

                #endregion

                #region Validate-Request-Body

                if(currMove.FromContainer == null) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal null value supplied for FromContainer, returning 400");
                    return false;
                }

                if(currMove.ToContainer == null) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal null value supplied for ToContainer, returning 400");
                    return false;
                }

                if(String.IsNullOrEmpty(currMove.MoveFrom)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal null value supplied for MoveFrom, returning 400");
                    return false;
                }

                if(String.IsNullOrEmpty(currMove.MoveTo)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal null value supplied for MoveTo, returning 400");
                    return false;
                }

                if(MoveRequest.UnsafeFsChars(currMove)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal unsafe characters detected in request, returning 400");
                    return false;
                }

                #endregion

                #region Check-if-Original-Exists

                string diskPathOriginal = MoveRequest.BuildDiskPath(currMove, true, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathOriginal)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal unable to build disk path for original container");
                    return false;
                }

                if(!Common.DirectoryExists(diskPathOriginal)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal from container does not exist: " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Check-if-Target-Parent-Exists

                string diskPathTargetParent = MoveRequest.BuildDiskPath(currMove, false, false, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTargetParent)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal unable to build disk path for target container");
                    return false;
                }

                if(!Common.DirectoryExists(diskPathTargetParent)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal target parent container does not exist: " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Check-if-Target-Child-Exists

                string diskPathTargetChild = MoveRequest.BuildDiskPath(currMove, false, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTargetChild)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal unable to build disk path for target container");
                    return false;
                }

                if(Common.FileExists(diskPathTargetChild)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal target container already exists: " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Set-Gateway-Mode

                userGatewayMode = Users.GetGatewayMode(currMove.UserGuid, CurrentSettings);

                #endregion

                #region Move-Directory

                if(!Common.MoveDirectory(diskPathOriginal, diskPathTargetChild)) {
                    ConsoleLogger.Warn("ServerContainerMoveInternal unable to move container from " + diskPathOriginal + " to " + diskPathTargetChild);
                    return false;
                }

                #endregion

                #region Perform-Background-Rewrite

                if(!userGatewayMode) {
                    ConsoleLogger.Warn("PostReplicationMoveContainer spawning background task to rewrite objects with correct metadata");
                    Task.Run(() => RewriteTree(diskPathTargetChild));
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerContainerMoveInternal exception encountered");
                ConsoleLogger.Error("ServerContainerMoveInternal Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ServerContainerRenameInternal(RenameRequest currRename) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal null value for currRename");
                    return false;
                }

                #endregion

                #region Variables

                bool userGatewayMode = false;

                #endregion

                #region Validate-Request-Body

                if(currRename.ContainerPath == null) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal null value supplied for ContainerPath, returning 400");
                    return false;
                }

                if(String.IsNullOrEmpty(currRename.RenameFrom)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal null value supplied for RenameFrom, returning 400");
                    return false;
                }

                if(String.IsNullOrEmpty(currRename.RenameTo)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal null value supplied for RenameTo, returning 400");
                    return false;
                }

                if(RenameRequest.UnsafeFsChars(currRename)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal unsafe characters detected in request, returning 400");
                    return false;
                }

                #endregion

                #region Check-if-Original-Exists

                string diskPathOriginal = RenameRequest.BuildDiskPath(currRename, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathOriginal)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal unable to build disk path for original container");
                    return false;
                }

                if(!Common.DirectoryExists(diskPathOriginal)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal from container does not exist: " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Check-if-Target-Exists

                string diskPathTarget = RenameRequest.BuildDiskPath(currRename, false, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTarget)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal unable to build disk path for target container");
                    return false;
                }

                if(Common.FileExists(diskPathTarget)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal target container already exists: " + diskPathOriginal);
                    return false;
                }

                #endregion

                #region Set-Gateway-Mode

                userGatewayMode = Users.GetGatewayMode(currRename.UserGuid, CurrentSettings);

                #endregion

                #region Rename-Directory

                if(!Common.RenameDirectory(diskPathOriginal, diskPathTarget)) {
                    ConsoleLogger.Warn("ServerContainerRenameInternal unable to rename container from " + diskPathOriginal + " to " + diskPathTarget);
                    return false;
                }

                #endregion

                #region Perform-Background-Rewrite

                if(!Common.IsTrue(userGatewayMode)) {
                    ConsoleLogger.Warn("PostReplicationRenameContainer spawning background task to rewrite objects with correct metadata");
                    Task.Run(() => RewriteTree(diskPathTarget));
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerContainerRenameInternal exception encountered");
                ConsoleLogger.Error("ServerContainerRenameInternal Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ServerObjectReceiveInternal(Obj currObj) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ServerObjectReceiveInternal null obj supplied");
                    return false;
                }

                #endregion

                #region Retrieve-Home-Directory

                string homeDirectory = Users.GetHomeDirectory(currObj.UserGuid, CurrentSettings);
                if(String.IsNullOrEmpty(homeDirectory)) {
                    ConsoleLogger.Warn("ServerObjectReceiveInternal unable to retrieve home directory for user GUID " + currObj.UserGuid);
                    return false;
                }

                #endregion

                #region Create-Directories-if-Needed

                // create home directory if needed
                if(!Common.DirectoryExists(homeDirectory)) {
                    Common.CreateDirectory(homeDirectory);
                }

                // now add each element in the path
                if(currObj.ContainerPath != null) {
                    if(currObj.ContainerPath.Count > 0) {
                        foreach(string currContainer in currObj.ContainerPath) {
                            homeDirectory += Common.GetPathSeparator(CurrentSettings.Environment) + currContainer;
                            if(!Common.DirectoryExists(homeDirectory)) {
                                Common.CreateDirectory(homeDirectory);
                            }
                        }
                    }
                }

                #endregion

                #region Delete-if-Exists

                if(Common.FileExists(currObj.DiskPath)) {
                    if(!Common.DeleteFile(currObj.DiskPath)) {
                        ConsoleLogger.Warn("ServerObjectReceiveInternal file " + currObj.DiskPath + " already exists and was unable to be deleted");
                        return false;
                    }
                }

                #endregion

                #region Write-Expiration-Object

                if(currObj.Expiration != null) {
                    Obj expObj = Common.CopyObject<Obj>(currObj);
                    expObj.Value = null;

                    string expFilename =
                        Convert.ToDateTime(expObj.Expiration).ToString("MMddyyyy-hhmmss") +
                        "-" + Common.RandomString(8) + "-" + expObj.Key;

                    if(!Common.WriteFile(CurrentSettings.Expiration.Directory + expFilename, Common.SerializeJson(expObj), false)) {
                        ConsoleLogger.Warn("ServerObjectReceiveInternal unable to create expiration object " + expFilename);
                        return false;
                    }
                }

                #endregion

                #region Write-File

                if(!Common.IsTrue(currObj.GatewayMode)) {
                    if(!Common.WriteFile(currObj.DiskPath, Common.SerializeJson(currObj), false)) {
                        ConsoleLogger.Warn("ServerObjectReceiveInternal unable to write replica to " + currObj.DiskPath);
                        return false;
                    }
                }
                else {
                    if(!Common.WriteFile(currObj.DiskPath, currObj.Value)) {
                        ConsoleLogger.Warn("ServerObjectReceiveInternal unable to write replica to " + currObj.DiskPath);
                        return false;
                    }
                }

                ConsoleLogger.Warn("ServerObjectReceiveInternal successfully wrote replica to " + currObj.DiskPath);
                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerObjectReceiveInternal exception encountered");
                ConsoleLogger.Error("ServerObjectReceiveInternal Outer exception" + e.ToString());
                return false;
            }
        }

        public bool ServerContainerReceiveInternal(Obj currObj) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ServerContainerReceiveInternal null path object supplied");
                    return false;
                }

                #endregion

                #region Retrieve-User-Home-Directory

                string homeDirectory = Users.GetHomeDirectory(currObj.UserGuid, CurrentSettings);
                if(String.IsNullOrEmpty(homeDirectory)) {
                    ConsoleLogger.Warn("ServerContainerReceiveInternal unable to retrieve home directory for user GUID " + currObj.UserGuid);
                    return false;
                }

                #endregion

                #region Create-Folder-if-Needed

                if(!Common.DirectoryExists(homeDirectory))
                    Common.CreateDirectory(homeDirectory);

                // now add each element in the path
                string currDirectory = String.Copy(homeDirectory);

                if(currObj.ContainerPath != null) {
                    if(currObj.ContainerPath.Count > 0) {
                        foreach(string currContainer in currObj.ContainerPath) {
                            currDirectory += Common.GetPathSeparator(CurrentSettings.Environment) + currContainer;
                            if(!Common.DirectoryExists(currDirectory)) {
                                Common.CreateDirectory(currDirectory);
                            }
                        }
                    }
                }

                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("ServerContainerReceiveInternal exception encountered");
                ConsoleLogger.Error("ServerContainerReceiveInternal Outer exception" + e.ToString());
                return false;
            }
        }

        #endregion

        #endregion

        #region Private-Methods

        #region Client-Sender-Methods

        private void SendReplica(Node to, string subject, string data) {
            try {
                #region Variables

                Message currMessage = new Message();
                bool success = false;
                string req = "";

                #endregion

                #region Setup

                currMessage.From = CurrentNode;
                currMessage.To = to;
                currMessage.Subject = subject;
                currMessage.Data = data;
                currMessage.Created = DateTime.Now;

                #endregion

                #region Set-URL

                string url = "";
                if(Common.IsTrue(currMessage.To.Ssl)) {
                    url = "https://" + currMessage.To.DnsHostname + ":" + currMessage.To.Port + "/admin/message";
                }
                else {
                    url = "http://" + currMessage.To.DnsHostname + ":" + currMessage.To.Port + "/admin/message";
                }

                #endregion

                #region Attempt-to-Send

                req = Common.SerializeJson(currMessage);

                RestWrapper.RestResponse resp = RestRequest.SendRequestSafe(
                    url, "application/json", "POST", null, null, false,
                    Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                    Common.AddToDictionary(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey, null),
                    Encoding.UTF8.GetBytes(req));

                if(resp == null) {
                    #region No-REST-Response

                    ConsoleLogger.Warn("SendReplica null response connecting to " + url + ", message will be queued");
                    success = false;

                    #endregion
                }
                else {
                    if(resp.StatusCode != 200) {
                        #region Failed-Message

                        ConsoleLogger.Warn("SendReplica non-200 response connecting to " + url + ", message will be queued");
                        success = false;

                        #endregion
                    }
                    else {
                        #region Successful-Message

                        success = true;

                        #endregion
                    }
                }

                #endregion

                #region Store-if-Needed

                if(!success) {
                    #region Create-Directory-if-Needed

                    if(!Common.DirectoryExists(CurrentSettings.Replication.Directory + to.Name)) {
                        try {
                            Common.CreateDirectory(CurrentSettings.Replication.Directory + to.Name);
                        }
                        catch(Exception e) {
                            ConsoleLogger.Warn("SendReplica exception while creating directory " + CurrentSettings.Replication.Directory + to.Name);
                            ConsoleLogger.Error("SendReplica exception while creating directory " + CurrentSettings.Replication.Directory + to.Name + e.ToString());
                            Common.ExitApplication("SendReplica", "Unable to create directory", -1);
                            return;
                        }
                    }

                    #endregion

                    #region Generate-New-GUID

                    int loopCount = 0;
                    string guid = "";

                    while(true) {
                        guid = Guid.NewGuid().ToString();
                        if(!Common.FileExists(CurrentSettings.Replication.Directory + to.Name + Common.GetPathSeparator(CurrentSettings.Environment) + guid)) {
                            break;
                        }

                        loopCount++;

                        if(loopCount > 16) {
                            ConsoleLogger.Warn("SendReplica unable to generate unused GUID for folder " + CurrentSettings.Replication.Directory + to.Name + ", exiting");
                            Common.ExitApplication("SendReplica", "Unable to generate unused GUID", -1);
                            return;
                        }
                    }

                    #endregion

                    #region Write-File

                    if(!Common.WriteFile(
                        CurrentSettings.Replication.Directory + to.Name + Common.GetPathSeparator(CurrentSettings.Environment) + guid,
                        Common.SerializeJson(currMessage),
                        false)) {
                        ConsoleLogger.Warn("SendReplica unable to write message to " + CurrentSettings.Replication.Directory + to.Name + Common.GetPathSeparator(CurrentSettings.Environment) + guid + ", exiting");
                        Common.ExitApplication("SendReplica", "Unable to write message", -1);
                        return;
                    }

                    ConsoleLogger.Warn("SendReplica queued message to " + CurrentSettings.Replication.Directory + to.Name + Common.GetPathSeparator(CurrentSettings.Environment) + guid);

                    #endregion
                }

                #endregion

                return;
            }
            catch(Exception e) {
                ConsoleLogger.Error("SendReplica Outer exception" + e.ToString());
                return;
            }
        }

        private void ContainerMoveReplicaAsync(MoveRequest currMove, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ContainerMoveReplicaAsync null value for currMove");
                    return;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ContainerMoveReplicaAsync null value for currNode");
                    return;
                }

                #endregion

                #region Process

                if(!ContainerMoveReplica(currMove, currNode)) {
                    ConsoleLogger.Warn("ContainerMoveReplicaAsync unable to replicate move operation to node ID " + currNode.NodeId + " " + currNode.Name);
                    SendReplica(currNode, "POST /admin/replication/move/container", Common.SerializeJson(currMove));
                }
                else {
                    ConsoleLogger.Warn("ContainerMoveReplicaAsync successfully replicated move operation to node ID " + currNode.NodeId + " " + currNode.Name);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ContainerMoveReplicaAsync Outer exception" + e.ToString());
                return;
            }
        }

        private bool ContainerMoveReplica(MoveRequest currMove, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ContainerMoveReplica null value for currMove");
                    return false;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ContainerMoveReplica null value for currNode");
                    return false;
                }

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Generate-URL

                if(Common.IsTrue(currNode.Ssl)) {
                    url = "https://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/move/container";
                }
                else {
                    url = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/move/container";
                }

                #endregion

                #region Headers

                headers = Common.AddToDictionary(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey, null);

                #endregion

                #region Process

                resp = RestRequest.SendRequestSafe(
                    url,
                    "application/json",
                    "POST",
                    null, null, false,
                    Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                    headers,
                    Encoding.UTF8.GetBytes(Common.SerializeJson(currMove)));

                if(resp == null) {
                    ConsoleLogger.Warn("ContainerMoveReplica null REST response while writing to " + url);
                    return false;
                }

                if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                    ConsoleLogger.Warn("ContainerMoveReplica non-200/201 REST response while writing to " + url);
                    return false;
                }

                ConsoleLogger.Warn("ContainerMoveReplica successfully replicated move operation to " + url);
                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ContainerMoveReplica Outer exception" + e.ToString());
                return false;
            }
        }

        private void ContainerRenameReplicaAsync(RenameRequest currRename, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ContainerRenameReplicaAsync null value for currRename");
                    return;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ContainerRenameReplicaAsync null value for currNode");
                    return;
                }

                #endregion

                #region Process

                if(!ContainerRenameReplica(currRename, currNode)) {
                    ConsoleLogger.Warn("ContainerRenameReplicaAsync unable to replicate rename operation to node ID " + currNode.NodeId + " " + currNode.Name);
                    SendReplica(currNode, "POST /admin/replication/rename/container", Common.SerializeJson(currRename));
                }
                else {
                    ConsoleLogger.Warn("ContainerRenameReplicaAsync successfully replicated rename operation to node ID " + currNode.NodeId + " " + currNode.Name);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ContainerRenameReplicaAsync Outer exception" + e.ToString());
                return;
            }
        }

        private bool ContainerRenameReplica(RenameRequest currRename, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ContainerWriteReplica null value for currRename");
                    return false;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ContainerWriteReplica null value for currNode");
                    return false;
                }

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Generate-URL

                if(Common.IsTrue(currNode.Ssl)) {
                    url = "https://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/rename/container";
                }
                else {
                    url = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/rename/container";
                }

                #endregion

                #region Headers

                headers = Common.AddToDictionary(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey, null);

                #endregion

                #region Process

                resp = RestRequest.SendRequestSafe(
                    url,
                    "application/json",
                    "POST",
                    null, null, false,
                    Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                    headers,
                    Encoding.UTF8.GetBytes(Common.SerializeJson(currRename)));

                if(resp == null) {
                    ConsoleLogger.Warn("ContainerRenameReplica null REST response while writing to " + url);
                    return false;
                }

                if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                    ConsoleLogger.Warn("ContainerRenameReplica non-200/201 REST response while writing to " + url);
                    return false;
                }

                ConsoleLogger.Warn("ContainerRenameReplica successfully replicated rename operation to " + url);
                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ContainerRenameReplica Outer exception" + e.ToString());
                return false;
            }
        }

        private void ContainerWriteReplicaAsync(Obj currObj) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ContainerWriteReplicaAsync null value for currObj");
                    return;
                }

                if(currObj.Replicas == null) {
                    ConsoleLogger.Warn("ContainerWriteReplicaAsync null value for currObj replicas");
                    return;
                }

                if(currObj.Replicas.Count < 1) {
                    ConsoleLogger.Warn("ContainerWriteReplicaAsync empty list for replicas");
                    return;
                }

                #endregion

                #region Process

                foreach(Node curr in currObj.Replicas) {
                    if(!ContainerWriteReplica(currObj, curr)) {
                        ConsoleLogger.Warn("ContainerWriteReplicaAsync unable to replicate container from " + currObj.DiskPath + " to node ID " + curr.NodeId + " " + curr.Name);
                        SendReplica(curr, "POST /admin/replication/container", Common.SerializeJson(currObj));
                    }
                    else {
                        ConsoleLogger.Warn("ContainerWriteReplicaAsync successfully replicated container to node ID " + curr.NodeId + " " + curr.Name);
                    }
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ContainerWriteReplicaAsync Outer exception" + e.ToString());
                return;
            }
        }

        private bool ContainerWriteReplica(Obj currObj, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ContainerWriteReplica null value for currObj");
                    return false;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ContainerWriteReplica null value for currNode");
                    return false;
                }

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Generate-URL

                if(Common.IsTrue(currNode.Ssl)) {
                    url = "https://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/container";
                }
                else {
                    url = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/container";
                }

                #endregion

                #region Headers

                headers = Common.AddToDictionary(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey, null);

                #endregion

                #region Process

                resp = RestRequest.SendRequestSafe(
                    url,
                    "application/json",
                    "POST",
                    null, null, false,
                    Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                    headers,
                    Encoding.UTF8.GetBytes(Common.SerializeJson(currObj)));

                if(resp == null) {
                    ConsoleLogger.Warn("ContainerWriteReplica null REST response while writing to " + url);
                    return false;
                }

                if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                    ConsoleLogger.Warn("ContainerWriteReplica non-200/201 REST response while writing to " + url);
                    return false;
                }

                ConsoleLogger.Warn("ContainerWriteReplica successfully replicated container to " + url);
                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ContainerWriteReplica Outer exception" + e.ToString());
                return false;
            }
        }

        private void ObjectMoveReplicaAsync(MoveRequest currMove, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ObjectMoveReplicaAsync null value for currMove");
                    return;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ObjectMoveReplicaAsync null value for currNode");
                    return;
                }

                #endregion

                #region Process

                if(!ObjectMoveReplica(currMove, currNode)) {
                    ConsoleLogger.Warn("ObjectMoveReplicaAsync unable to replicate move operation to node ID " + currNode.NodeId + " " + currNode.Name);
                    SendReplica(currNode, "POST /admin/replication/move/object", Common.SerializeJson(currMove));
                }
                else {
                    ConsoleLogger.Warn("ObjectMoveReplicaAsync successfully replicated move operation to node ID " + currNode.NodeId + " " + currNode.Name);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ObjectMoveReplicaAsync Outer exception" + e.ToString());
                return;
            }
        }

        private bool ObjectMoveReplica(MoveRequest currMove, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currMove == null) {
                    ConsoleLogger.Warn("ObjectMoveReplica null value for currMove");
                    return false;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ObjectMoveReplica null value for currNode");
                    return false;
                }

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Generate-URL

                if(Common.IsTrue(currNode.Ssl)) {
                    url = "https://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/move/object";
                }
                else {
                    url = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/move/object";
                }

                #endregion

                #region Headers

                headers = Common.AddToDictionary(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey, null);

                #endregion

                #region Process

                resp = RestRequest.SendRequestSafe(
                    url,
                    "application/json",
                    "POST",
                    null, null, false,
                    Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                    headers,
                    Encoding.UTF8.GetBytes(Common.SerializeJson(currMove)));

                if(resp == null) {
                    ConsoleLogger.Warn("ObjectMoveReplica null REST response while writing to " + url);
                    return false;
                }

                if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                    ConsoleLogger.Warn("ObjectMoveReplica non-200/201 REST response while writing to " + url);
                    return false;
                }

                ConsoleLogger.Warn("ObjectMoveReplica successfully replicated move operation to " + url);
                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ObjectMoveReplica Outer exception" + e.ToString());
                return false;
            }
        }

        private void ObjectRenameReplicaAsync(RenameRequest currRename, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ObjectRenameReplicaAsync null value for currRename");
                    return;
                }

                #endregion

                #region Process

                if(!ObjectRenameReplica(currRename, currNode)) {
                    ConsoleLogger.Warn("ObjectRenameReplicaAsync unable to replicate rename operation to node ID " + currNode.NodeId + " " + currNode.Name);
                    SendReplica(currNode, "POST /admin/replication/rename/object", Common.SerializeJson(currRename));
                }
                else {
                    ConsoleLogger.Warn("ObjectRenameReplicaAsync successfully replicated rename operation to node ID " + currNode.NodeId + " " + currNode.Name);
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ObjectRenameReplicaAsync Outer exception" + e.ToString());
                return;
            }
        }

        private bool ObjectRenameReplica(RenameRequest currRename, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currRename == null) {
                    ConsoleLogger.Warn("ObjectRenameReplica null value for currRename");
                    return false;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ObjectRenameReplica null value for currNode");
                    return false;
                }

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Generate-URL

                if(Common.IsTrue(currNode.Ssl)) {
                    url = "https://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/rename/object";
                }
                else {
                    url = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/rename/object";
                }

                #endregion

                #region Headers

                headers = Common.AddToDictionary(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey, null);

                #endregion

                #region Process

                resp = RestRequest.SendRequestSafe(
                    url,
                    "application/json",
                    "POST",
                    null, null, false,
                    Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                    headers,
                    Encoding.UTF8.GetBytes(Common.SerializeJson(currRename)));

                if(resp == null) {
                    ConsoleLogger.Warn("ObjectRenameReplica null REST response while writing to " + url);
                    return false;
                }

                if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                    ConsoleLogger.Warn("ObjectRenameReplica non-200/201 REST response while writing to " + url);
                    return false;
                }

                ConsoleLogger.Warn("ObjectRenameReplica successfully replicated rename operation to " + url);
                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ObjectRenameReplica Outer exception" + e.ToString());
                return false;
            }
        }

        private void ObjectWriteReplicaAsync(Obj currObj) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ObjectWriteReplicaAsync null value for currObj");
                    return;
                }

                if(currObj.Replicas == null) {
                    ConsoleLogger.Warn("ObjectWriteReplicaAsync null value for replicas");
                    return;
                }

                if(currObj.Replicas.Count < 1) {
                    ConsoleLogger.Warn("ObjectWriteReplicaAsync empty list for replicas");
                    return;
                }

                #endregion

                #region Process

                foreach(Node currNode in currObj.Replicas) {
                    if(!ObjectWriteReplica(currObj, currNode)) {
                        ConsoleLogger.Warn("ObjectWriteReplicaAsync unable to replicate object " + currObj.Key + " to node ID " + currNode.NodeId + " " + currNode.Name);
                        SendReplica(currNode, "POST /admin/replication/object", Common.SerializeJson(currObj));
                    }
                    else {
                        ConsoleLogger.Warn("ObjectWriteReplicaAsync successfully replicated object " + currObj.Key + " to node ID " + currNode.NodeId + " " + currNode.Name);
                    }
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ObjectWriteReplicaAsync Outer exception" + e.ToString());
                return;
            }
        }

        private bool ObjectWriteReplica(Obj currObj, Node currNode) {
            try {
                #region Check-for-Null-Values

                if(currObj == null) {
                    ConsoleLogger.Warn("ObjectWriteReplica null value for currObj");
                    return false;
                }

                if(currNode == null) {
                    ConsoleLogger.Warn("ObjectWriteReplica null value for currNode");
                    return false;
                }

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Generate-URL

                if(Common.IsTrue(currNode.Ssl)) {
                    url = "https://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/object";
                }
                else {
                    url = "http://" + currNode.DnsHostname + ":" + currNode.Port + "/admin/replication/object";
                }

                #endregion

                #region Headers

                headers = Common.AddToDictionary(CurrentSettings.Server.HeaderApiKey, CurrentSettings.Server.AdminApiKey, null);

                #endregion

                #region Process

                resp = RestRequest.SendRequestSafe(
                    url,
                    "application/json",
                    "POST",
                    null, null, false,
                    Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                    headers,
                    Encoding.UTF8.GetBytes(Common.SerializeJson(currObj)));

                if(resp == null) {
                    ConsoleLogger.Warn("ObjectWriteReplica null REST response while writing to " + url);
                    return false;
                }

                if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                    ConsoleLogger.Warn("ObjectWriteReplica non-200/201 REST response while writing to " + url);
                    return false;
                }

                ConsoleLogger.Warn("ObjectWriteReplica successfully replicated " + currObj.Key + " to " + url);
                return true;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("ObjectWriteReplica Outer exception" + e.ToString());
                return false;
            }
        }

        #endregion

        #region Server-Receive-Methods

        #endregion

        #region General-Methods

        private bool RewriteTree(string root) {
            try {
                #region Check-for-Null-Values

                if(String.IsNullOrEmpty(root)) {
                    ConsoleLogger.Warn("RewriteTree null root directory supplied");
                    return false;
                }

                #endregion

                #region Variables

                List<string> dirlist = new List<string>();
                List<string> filelist = new List<string>();
                long byteCount = 0;

                #endregion

                #region Get-Full-File-List

                if(!Common.WalkDirectory(CurrentSettings.Environment, 0, root, true, out dirlist, out filelist, out byteCount, true)) {
                    ConsoleLogger.Warn("RewriteTree unable to walk directory for " + root);
                    return false;
                }

                #endregion

                #region Process-Each-File

                foreach(string currFile in filelist) {
                    if(!RewriteObject(currFile)) {
                        ConsoleLogger.Warn("RewriteTree unable to rewrite file " + currFile);
                    }
                }

                #endregion

                return true;
            }
            catch(Exception e) {
                ConsoleLogger.Error("RewriteTree Outer exception" + e.ToString());
                return false;
            }
        }

        private bool RewriteObject(string filename) {
            try {
                #region Check-for-Null-Values

                if(String.IsNullOrEmpty(filename)) {
                    ConsoleLogger.Warn("RewriteObject null filename supplied");
                    return false;
                }

                #endregion

                #region Variables

                Obj currObj = new Obj();
                List<string> containers = new List<string>();
                string random = "";
                bool writeSuccess = false;

                #endregion

                #region Retrieve-Object

                currObj = Obj.BuildObjFromDisk(filename, Users, CurrentSettings, CurrentTopology, CurrentNode);
                if(currObj == null) {
                    ConsoleLogger.Warn("RewriteObject unable to build disk obj from file " + filename);
                    return false;
                }

                #endregion

                #region Generate-Random-String

                random = Common.RandomString(8);

                #endregion

                #region Rename-Original

                if(!Common.RenameFile(filename, filename + "." + random)) {
                    ConsoleLogger.Warn("RewriteObject unable to rename " + filename + " to temporary filename " + filename + "." + random);
                    return false;
                }

                #endregion

                #region Delete-File

                if(!Common.DeleteFile(filename)) {
                    ConsoleLogger.Warn("RewriteObject unable to delete file " + filename);
                    return false;
                }

                #endregion

                #region Rewrite-File

                if(!Common.IsTrue(currObj.GatewayMode)) {
                    writeSuccess = Common.WriteFile(filename, Common.SerializeJson(currObj), false);
                    if(!writeSuccess) {
                        ConsoleLogger.Warn("RewriteObject unable to write object to " + filename);
                        return false;
                    }
                }
                else {
                    writeSuccess = Common.WriteFile(filename, currObj.Value);
                    if(!writeSuccess) {
                        ConsoleLogger.Warn("RewriteObject unable to write raw bytes to " + filename);
                        return false;
                    }
                }

                #endregion

                #region Delete-Temporary-File-and-Return

                if(!Common.DeleteFile(filename + "." + random)) {
                    ConsoleLogger.Warn("RewriteObject " + filename + " was successfully rerwritten but temporary file " + filename + "." + random + " was unable to be deleted");
                    return true;
                }
                else {
                    ConsoleLogger.Warn("RewriteObject successfully rewrote object " + filename);
                    return true;
                }

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("RewriteObject Outer exception" + e.ToString());
                return false;
            }
        }

        #endregion

        #endregion
    }
}
