using anthilla.core;
using System;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse PostOwner(RequestMetadata md) {
            #region Variables

            Find req = new Find();
            Node owner = new Node();
            string url = "";

            #endregion

            #region Deserialize-and-Initialize

            try {
                req = Common.DeserializeJson<Find>(md.CurrentHttpRequest.Data);
                if(req == null) {
                    ConsoleLogger.Warn("PostOwner null request after deserialization, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                        true);
                }
            }
            catch(Exception) {
                ConsoleLogger.Warn("PostOwner unable to deserialize request body");
                return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                    new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                    true);
            }

            #endregion

            #region Validate-Content

            if(String.IsNullOrEmpty(req.UserGuid)) {
                ConsoleLogger.Warn("PostOwner null GUID after deserialization, returning 400");
                return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                    new ErrorResponse(2, 400, "Unable to validate request body.  GUID is null.", null).ToJson(),
                    true);
            }

            #endregion

            #region Retrieve-Owner

            owner = Node.DetermineOwner(req.UserGuid, Users, CurrentTopology, CurrentNode);
            if(owner == null) {
                ConsoleLogger.Warn( "PostOwner primary for GUID " + req.UserGuid + " could not be discerned, returning 500");
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to determine primary node.", null).ToJson(),
                    true);
            }

            #endregion

            #region Generate-URLs

            if(Common.IsTrue(owner.Ssl)) {
                url = "https://" + owner.DnsHostname + ":" + owner.Port + "/" + req.UserGuid;
            }
            else {
                url = "http://" + owner.DnsHostname + ":" + owner.Port + "/" + req.UserGuid;
            }

            #endregion

            #region Respond

            ConsoleLogger.Warn("PostOwner GUID is mapped to " + url);
            return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "text/plain", url, true);

            #endregion
        }
    }
}