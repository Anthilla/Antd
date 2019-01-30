﻿using anthilla.core;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse AdminApiHandler(RequestMetadata md) {
            #region Enumerate

            ConsoleLogger.Warn(
                "AdminApiHandler admin API requested by " +
                md.CurrentHttpRequest.SourceIp + ":" + md.CurrentHttpRequest.SourcePort + " " +
                md.CurrentHttpRequest.Method + " " + md.CurrentHttpRequest.RawUrlWithoutQuery);

            #endregion

            #region Variables

            string reqMetadataVal = "";
            bool reqMetadata = false;

            #endregion

            #region Check-for-Metadata-Request

            reqMetadataVal = md.CurrentHttpRequest.RetrieveHeaderValue("request_metadata");
            reqMetadata = Common.IsTrue(reqMetadataVal);

            if(reqMetadata) {
                return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(md), true);
            }

            #endregion

            #region Process-Request

            switch(md.CurrentHttpRequest.Method.ToLower()) {
                case "get":
                    #region get

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/cleanup", false)) {
                        return GetCleanup(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/connections", false)) {
                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(ConnManager.GetActiveConnections()), true);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/disks", false)) {
                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(DiskInfo.GetAllDisks()), true);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/heartbeat", false)) {
                        return GetHeartbeat(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/messages/count", false)) {
                        return GetMessagesCount(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/neighbors", false)) {
                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(CurrentTopology.Replicas), true);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/count", false)) {
                        return GetReplicationCount(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/tasks/count", false)) {
                        return GetTasksCount(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/topology", false)) {
                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(CurrentTopology), true);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/login", false)) {
                        return GetLogin(md);
                    }

                    break;

                #endregion

                case "put":
                    #region put

                    break;

                #endregion

                case "post":
                    #region post

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/find", false)) {
                        return PostFind(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/kill", false)) {
                        return PostKill(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/message", false)) {
                        return PostMessage(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/owner", false)) {
                        return PostOwner(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/container", false)) {
                        ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                        return rh.ServerContainerReceive(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/object", false)) {
                        ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                        return rh.ServerObjectReceive(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/move/container", false)) {
                        ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                        return rh.ServerContainerMove(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/move/object", false)) {
                        ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                        return rh.ServerObjectMove(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/rename/container", false)) {
                        ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                        return rh.ServerContainerRename(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/rename/object", false)) {
                        ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                        return rh.ServerObjectRename(md);
                    }

                    break;

                #endregion

                case "delete":
                    #region delete

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/container", false)) {
                        return DeleteReplicationContainer(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/replication/object", false)) {
                        return DeleteReplicationObject(md);
                    }

                    if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/admin/user_guid", false)) {
                        return DeleteUserGuid(md);
                    }

                    break;

                #endregion

                case "head":
                    #region head

                    break;

                #endregion

                default:
                    ConsoleLogger.Warn("AdminApiHandler unknown http method: " + md.CurrentHttpRequest.Method);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unsupported HTTP method.", null).ToJson(),
                        true);
            }

            ConsoleLogger.Warn("AdminApiHandler unknown endpoint URL: " + md.CurrentHttpRequest.RawUrlWithoutQuery);
            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                new ErrorResponse(2, 400, "Unknown endpoint.", null), true);

            #endregion
        }
    }
}