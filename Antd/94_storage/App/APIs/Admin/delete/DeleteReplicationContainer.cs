using anthilla.core;
using System;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse DeleteReplicationContainer(RequestMetadata md) {
            #region Deserialize

            Obj req = new Obj();
            try {
                req = Common.DeserializeJson<Obj>(md.CurrentHttpRequest.Data);
                if(req == null) {
                    ConsoleLogger.Warn("DeleteReplicationContainer null request after deserialization, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                }
            }
            catch(Exception) {
                ConsoleLogger.Warn("DeleteReplicationContainer unable to deserialize request body");
                return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                    new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
            }

            #endregion

            #region Build-Disk-Path

            req.DiskPath = Obj.BuildDiskPath(req, md.CurrentUserMaster, CurrentSettings);
            if(String.IsNullOrEmpty(req.DiskPath)) {
                ConsoleLogger.Warn("DeleteReplicationContainer unable to build disk path from request");
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
            }

            #endregion

            #region Process

            if(Common.DirectoryExists(req.DiskPath)) {
                if(Common.DeleteDirectory(req.DiskPath, true)) {
                    ConsoleLogger.Warn("DeleteReplicationContainer successfully deleted " + req.DiskPath);
                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                }

                ConsoleLogger.Warn("DeleteReplicationContainer unable to delete " + req.DiskPath);
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to delete container.", null).ToJson(), true);
            }
            else {
                ConsoleLogger.Warn("DeleteReplicationContainer unable to find " + req.DiskPath);
                return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                    new ErrorResponse(5, 404, "Container does not exist.", null).ToJson(), true);
            }

            #endregion
        }
    }
}