using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Kvpbase {
    public class ContainerHandler {
        #region Public-Members

        #endregion

        #region Private-Members

        private Settings CurrentSettings;
        private Topology CurrentTopology;
        private Node CurrentNode;
        private UserManager Users;

        #endregion

        #region Constructors-and-Factories

        public ContainerHandler(Settings settings, Topology topology, Node node, UserManager users) {
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

        public HttpResponse ContainerDelete(RequestMetadata md, bool recursive) {
            #region Variables

            bool deleteSuccess = false;
            RestResponse proxyResponse = new RestResponse();
            Dictionary<string, string> restHeaders = new Dictionary<string, string>();
            string containerLogFile = "";
            string containerPropertiesFile = "";
            ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();

            #endregion

            //md.CurrentObj.DiskPath = @"/";

            #region Check-Permissions

            if(md.CurrentApiKeyPermission == null) {
                ConsoleLogger.Warn("ContainerDelete null ApiKeyPermission object supplied");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowDeleteContainer)) {
                ConsoleLogger.Warn("ContainerDelete AllowDeleteContainer operation not authorized per permissions");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            #endregion

            #region Process-Owner

            if(md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId) {
                #region Local-Owner

                #region Check-Container-Permissions-and-Logging

                currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                if(currContainerPropertiesFile != null) {
                    //if(currContainerPropertiesFile.Logging != null) {
                    //    if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                    //        if(Common.IsTrue(currContainerPropertiesFile.Logging.DeleteContainer)) {
                    //            #region Process-Logging

                    //            Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "DeleteContainer", null));

                    //            #endregion
                    //        }
                    //    }
                    //}

                    if(currContainerPropertiesFile.Permissions != null) {
                        #region Evaluate-Permissions

                        if(!ContainerPermission.GetPermission("DeleteContainer", md, currContainerPropertiesFile)) {
                            //if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                            //    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "DeleteContainer", "denied"));
                            //}

                            ConsoleLogger.Warn("ContainerDelete AllowDeleteContainer operation not authorized per container permissions");
                            return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                        }

                        #endregion
                    }
                }

                #endregion

                #region Process-Replication

                ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                if(!rh.ContainerDelete(md.CurrentObj, CurrentTopology.Replicas)) {
                    ConsoleLogger.Warn("ContainerDelete negative response from replication, returning 500");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                }

                BunkerHandler bh = new BunkerHandler(CurrentSettings);
                bh.ContainerDelete(md.CurrentObj, recursive);

                #endregion

                #region Delete-Directory-and-Respond

                deleteSuccess = DirectoryUtil.DeleteDirectory(md.CurrentObj.ContainerPath.ToArray(), recursive);

                #endregion

                #region Respond

                if(deleteSuccess) {
                    ConsoleLogger.Warn("ContainerDelete successfully deleted " + md.CurrentObj.DiskPath);
                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                }
                else {
                    ConsoleLogger.Warn("ContainerDelete could not delete " + md.CurrentObj.DiskPath);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to delete container.", null).ToJson(), true);
                }

                #endregion

                #endregion
            }
            else {
                #region Remote-Owner

                switch(CurrentSettings.Redirection.DeleteRedirectionMode) {
                    case "none":
                        #region none

                        ConsoleLogger.Warn("ContainerDelete object is destined for a different machine but DeleteRedirectionModee is none");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                    #endregion

                    case "proxy":
                        #region proxy

                        ConsoleLogger.Warn("ContainerDelete proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs + " for container deletion");

                        proxyResponse = RestRequest.SendRequestSafe(
                            md.CurrentObj.PrimaryUrlWithQs,
                            md.CurrentHttpRequest.ContentType,
                            md.CurrentHttpRequest.Method,
                            null, null, false,
                            Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                            GetCustomHeaders(md.CurrentHttpRequest.Headers),
                            md.CurrentHttpRequest.Data);

                        if(proxyResponse == null) {
                            ConsoleLogger.Warn("ContainerDelete null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerDelete server response to proxy REST request: " + proxyResponse.StatusCode);
                        return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                    #endregion

                    case "redirect":
                        #region redirect

                        ConsoleLogger.Warn("ContainerDelete redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs + " using status " + CurrentSettings.Redirection.DeleteRedirectHttpStatus + " for container deletion");
                        Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                        redirectHeader.Add("location", md.CurrentObj.PrimaryUrlWithQs);
                        return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.DeleteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.DeleteRedirectString, true);

                    #endregion

                    default:
                        #region unknown

                        ConsoleLogger.Warn("ContainerDelete unknown DeleteRedirectionModee in redirection settings: " + CurrentSettings.Redirection.DeleteRedirectionMode);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                        #endregion
                }

                #endregion
            }

            #endregion
        }

        public HttpResponse ContainerHead(RequestMetadata md) {
            #region Variables

            string homeDirectory = "";
            RestResponse proxyResponse = new RestResponse();
            Dictionary<string, string> restHeaders = new Dictionary<string, string>();
            List<string> urls = new List<string>();
            string redirectUrl = "";
            string proxiedVal = "";
            bool proxied = false;
            string containerLogFile = "";
            string containerPropertiesFile = "";
            ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();

            #endregion

            #region Check-Permissions

            if(md.CurrentApiKeyPermission == null) {
                ConsoleLogger.Warn("ContainerHead null ApiKeyPermission object supplied");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json", null, true);
            }

            if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowReadContainer)) {
                ConsoleLogger.Warn("ContainerHead AllowReadContainer operation not authorized per permissions");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json", null, true);
            }

            #endregion

            #region Get-Values-from-Querystring

            proxiedVal = md.CurrentHttpRequest.RetrieveHeaderValue("proxied");
            if(!String.IsNullOrEmpty(proxiedVal)) {
                proxied = Common.IsTrue(proxiedVal);
            }

            #endregion

            #region Retrieve-User-Home-Directory

            homeDirectory = Users.GetHomeDirectory(md.CurrentUserMaster.Guid, CurrentSettings);
            if(String.IsNullOrEmpty(homeDirectory)) {
                ConsoleLogger.Warn("ContainerHead unable to retrieve home directory for user GUID " + md.CurrentUserMaster.Guid);
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json", null, true);
            }

            #endregion

            #region Retrieve-Directory

            /*
                * Nodes that proxy requests will append ?proxied=true to the request URL
                * to notify the next recipient node to not proxy it further (otherwise
                * an infinite loop of REST requests will be generated)
                * 
                */

            if(proxied || (md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId)) {
                #region Local-Owner

                #region Check-Container-Permissions-and-Logging

                currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                if(currContainerPropertiesFile != null) {
                    //if(currContainerPropertiesFile.Logging != null) {
                    //    if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                    //        if(Common.IsTrue(currContainerPropertiesFile.Logging.ReadContainer)) {
                    //            #region Process-Logging

                    //            Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadContainer", null));

                    //            #endregion
                    //        }
                    //    }
                    //}

                    if(currContainerPropertiesFile.Permissions != null) {
                        #region Evaluate-Permissions

                        if(!ContainerPermission.GetPermission("ReadContainer", md, currContainerPropertiesFile)) {
                            //if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                            //    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadContainer", "denied"));
                            //}

                            ConsoleLogger.Warn("ContainerHead AllowReadContainer operation not authorized per container permissions");
                            return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                        }

                        #endregion
                    }
                }

                #endregion

                #region Process

                if(!Common.DirectoryExists(homeDirectory))
                    Common.CreateDirectory(homeDirectory);

                DirInfo di = new DirInfo(CurrentSettings, Users);
                di = di.FromDirectory(md.CurrentObj.DiskPath, md.CurrentObj.UserGuid, 1, null, true);
                if(di == null) {
                    ConsoleLogger.Warn("ContainerHead null info returned from " + md.CurrentObj.DiskPath);
                    return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json", null, true);
                }

                return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);

                #endregion

                #endregion
            }
            else {
                #region Remote-Owner

                switch(CurrentSettings.Redirection.ReadRedirectionMode) {
                    case "none":
                        #region none

                        ConsoleLogger.Warn("ContainerHead object is stored on a different machine but ReadRedirectionMode is none");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                    #endregion

                    case "proxy":
                        #region proxy

                        //if(Maintenance.IsEnabled()) {
                        //    urls = Obj.BuildMaintReadUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        //}
                        //else {
                        //    urls = Obj.BuildReplicaUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        //}

                        if(urls == null || urls.Count < 1) {
                            ConsoleLogger.Warn("ContainerHead unable to build replica URL list (null response)");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to build proxy URL.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerHead proxying request to " + urls.Count + " URLs for user GUID " + md.CurrentUserMaster.Guid);

                        proxyResponse = FirstResponder.SendRequest(
                            CurrentSettings,
                            md,
                            urls,
                            md.CurrentHttpRequest.ContentType,
                            md.CurrentHttpRequest.Method,
                            null, null, false,
                            GetCustomHeaders(md.CurrentHttpRequest.Headers),
                            md.CurrentHttpRequest.Data);

                        if(proxyResponse == null) {
                            ConsoleLogger.Warn("ContainerHead null response from REST request to " + urls.Count + " URLs");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerHead server response to proxy REST request: " + proxyResponse.StatusCode);
                        return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                    #endregion

                    case "redirect":
                        #region redirect

                        redirectUrl = Obj.BuildRedirectUrl(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        if(String.IsNullOrEmpty(redirectUrl)) {
                            ConsoleLogger.Warn("ContainerHead unable to generate redirect URL, returning 500");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to build redirect URL.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerHead redirecting request using status " + CurrentSettings.Redirection.ReadRedirectHttpStatus + " for user GUID " + md.CurrentUserMaster.Guid);
                        Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                        redirectHeader.Add("location", redirectUrl);
                        return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.ReadRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.ReadRedirectString, true);

                    #endregion

                    default:
                        #region unknown

                        ConsoleLogger.Warn("ContainerHead unknown ReadRedirectionMode in redirection settings: " + CurrentSettings.Redirection.ReadRedirectionMode);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                        #endregion
                }

                #endregion
            }

            #endregion
        }

        public HttpResponse ContainerMove(RequestMetadata md) {
            #region Variables

            MoveRequest req = new MoveRequest();
            RestResponse proxyResponse = new RestResponse();
            Dictionary<string, string> restHeaders = new Dictionary<string, string>();
            bool userGatewayMode = false;
            string containerLogFile = "";
            string containerPropertiesFile = "";
            ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();

            #endregion

            #region Check-Permissions

            if(md.CurrentApiKeyPermission == null) {
                ConsoleLogger.Warn("ContainerMove null ApiKeyPermission object supplied");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowWriteContainer)) {
                ConsoleLogger.Warn("ContainerMove AllowWriteContainer operation not authorized per permissions");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            #endregion

            #region Process-Owner

            if(md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId) {
                #region Local-Owner

                #region Check-Container-Permissions-and-Logging

                currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                if(currContainerPropertiesFile != null) {
                    //if(currContainerPropertiesFile.Logging != null) {
                    //    if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                    //        if(Common.IsTrue(currContainerPropertiesFile.Logging.ReadContainer)) {
                    //            #region Process-Logging

                    //            Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteContainer-Move", null));

                    //            #endregion
                    //        }
                    //    }
                    //}

                    if(currContainerPropertiesFile.Permissions != null) {
                        #region Evaluate-Permissions

                        if(!ContainerPermission.GetPermission("WriteContainer", md, currContainerPropertiesFile)) {
                            //if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                            //    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteContainer-Move", "denied"));
                            //}

                            ConsoleLogger.Warn("ContainerMove AllowWriteContainer operation not authorized per container permissions");
                            return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                        }

                        #endregion
                    }
                }

                #endregion

                #region Deserialize

                try {
                    req = Common.DeserializeJson<MoveRequest>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ContainerMove null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ContainerMove unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                }

                req.UserGuid = String.Copy(md.CurrentUserMaster.Guid);

                #endregion

                #region Validate-Request-Body

                if(req.FromContainer == null) {
                    ConsoleLogger.Warn("ContainerMove null value supplied for FromContainer, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for FromContainer.", null).ToJson(), true);
                }

                if(req.ToContainer == null) {
                    ConsoleLogger.Warn("ContainerMove null value supplied for ToContainer, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for ToContainer.", null).ToJson(), true);
                }

                if(String.IsNullOrEmpty(req.MoveFrom)) {
                    ConsoleLogger.Warn("ContainerMove null value supplied for MoveFrom, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for MoveFrom.", null).ToJson(), true);
                }

                if(String.IsNullOrEmpty(req.MoveTo)) {
                    ConsoleLogger.Warn("ContainerMove null value supplied for MoveTo, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for MoveTo.", null).ToJson(), true);
                }

                if(MoveRequest.UnsafeFsChars(req)) {
                    ConsoleLogger.Warn("ContainerMove unsafe characters detected in request, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unsafe characters detected.", null).ToJson(), true);
                }

                req.UserGuid = md.CurrentUserMaster.Guid;

                #endregion

                #region Check-if-Original-Exists

                string diskPathOriginal = MoveRequest.BuildDiskPath(req, true, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathOriginal)) {
                    ConsoleLogger.Warn("ContainerMove unable to build disk path for original container");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                }

                if(!Common.DirectoryExists(diskPathOriginal)) {
                    ConsoleLogger.Warn("ContainerMove original container does not exist: " + diskPathOriginal);
                    return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                        new ErrorResponse(5, 404, "Source container does not exist.", null).ToJson(), true);
                }

                #endregion

                #region Check-if-Target-Parent-Exists

                string diskPathTargetParent = MoveRequest.BuildDiskPath(req, false, false, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTargetParent)) {
                    ConsoleLogger.Warn("ContainerMove unable to build disk path for target container");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                }

                if(!Common.DirectoryExists(diskPathTargetParent)) {
                    ConsoleLogger.Warn("ContainerMove target parent container does not exist: " + diskPathOriginal);
                    return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                        new ErrorResponse(5, 404, "Target container does not exist.", null).ToJson(), true);
                }

                #endregion

                #region Check-if-Target-Child-Exists

                string diskPathTargetChild = MoveRequest.BuildDiskPath(req, false, true, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPathTargetChild)) {
                    ConsoleLogger.Warn("ContainerMove unable to build disk path for target container");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                }

                if(Common.DirectoryExists(diskPathTargetChild)) {
                    ConsoleLogger.Warn("ContainerMove target container already exists: " + diskPathOriginal);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Container already exists.", null).ToJson(), true);
                }

                #endregion

                #region Set-Gateway-Mode

                userGatewayMode = md.CurrentUserMaster.GetGatewayMode(CurrentSettings);

                #endregion

                #region Process-Replication

                ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                if(!rh.ContainerMove(req)) {
                    ConsoleLogger.Warn("ContainerMove negative response from replication, returning 500");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                }

                BunkerHandler bh = new BunkerHandler(CurrentSettings);
                bh.ContainerMove(req);

                #endregion

                #region Move-Directory

                if(!Common.MoveDirectory(diskPathOriginal, diskPathTargetChild)) {
                    ConsoleLogger.Warn("ContainerMove unable to move container from " + diskPathOriginal + " to " + diskPathTargetChild);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Container already exists.", null).ToJson(), true);
                }

                #endregion

                #region Perform-Background-Rewrite

                if(!userGatewayMode) {
                    ConsoleLogger.Warn("ContainerMove spawning background task to rewrite objects with correct metadata");
                    Task.Run(() => RewriteTree(diskPathTargetChild));
                }

                #endregion

                return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);

                #endregion
            }
            else {
                #region Remote-Owner

                switch(CurrentSettings.Redirection.WriteRedirectionMode) {
                    case "none":
                        #region none

                        ConsoleLogger.Warn("ContainerMove object is destined for a different machine but WriteRedirectionMode is none");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                    #endregion

                    case "proxy":
                        #region proxy

                        ConsoleLogger.Warn("ContainerMove proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs);

                        proxyResponse = RestRequest.SendRequestSafe(
                            md.CurrentObj.PrimaryUrlWithQs,
                            md.CurrentHttpRequest.ContentType,
                            md.CurrentHttpRequest.Method,
                            null, null, false,
                            Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                            GetCustomHeaders(md.CurrentHttpRequest.Headers),
                            md.CurrentHttpRequest.Data);

                        if(proxyResponse == null) {
                            ConsoleLogger.Warn("ContainerMove null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable tocommunicate with the appropriate node for this request.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerMove server response to proxy REST request: " + proxyResponse.StatusCode);
                        return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                    #endregion

                    case "redirect":
                        #region redirect

                        ConsoleLogger.Warn("ContainerMove redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs + " using status " + CurrentSettings.Redirection.WriteRedirectHttpStatus);
                        Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                        redirectHeader.Add("location", md.CurrentObj.PrimaryUrlWithQs);
                        return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.WriteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.WriteRedirectString, true);

                    #endregion

                    default:
                        #region unknown

                        ConsoleLogger.Warn("ContainerMove unknown WriteRedirectionMode in redirection settings: " + CurrentSettings.Redirection.WriteRedirectionMode);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                        #endregion
                }

                #endregion
            }

            #endregion
        }

        public HttpResponse ContainerRead(RequestMetadata md) {
            #region Variables

            string homeDirectory = "";
            RestResponse proxyResponse = new RestResponse();
            Dictionary<string, string> restHeaders = new Dictionary<string, string>();
            List<string> urlList = new List<string>();
            string redirectUrl = "";

            string metadataVal = "";
            bool metadataOnly = false;
            string proxiedVal = "";
            bool proxied = false;
            string recursiveVal = "";
            bool recursive = false;
            string statsVal = "";
            bool stats = false;
            string maxResultsStr = "";
            int maxResults = 0;
            string walkVal = "";
            bool walk = false;
            string debugVal = "";
            bool debug = false;
            string propertiesVal = "";
            bool properties = false;

            string containerLogFile = "";
            string containerPropertiesFile = "";
            ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();

            #endregion

            #region Check-Permissions

            if(md.CurrentApiKeyPermission == null) {
                ConsoleLogger.Warn("ContainerRead null ApiKeyPermission object supplied");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowReadContainer)) {
                ConsoleLogger.Warn("ContainerRead AllowReadContainer operation not authorized per permissions");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            #endregion

            #region Get-Values-from-Querystring

            maxResultsStr = md.CurrentHttpRequest.RetrieveHeaderValue("max_results");
            if(!String.IsNullOrEmpty(maxResultsStr)) {
                if(!Int32.TryParse(maxResultsStr, out maxResults)) {
                    ConsoleLogger.Warn("ContainerRead invalid value for max_results in querystring: " + maxResultsStr);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for max_results.", null).ToJson(), true);
                }
            }

            metadataVal = md.CurrentHttpRequest.RetrieveHeaderValue("metadata");
            if(!String.IsNullOrEmpty(metadataVal)) {
                metadataOnly = Common.IsTrue(metadataVal);
            }

            proxiedVal = md.CurrentHttpRequest.RetrieveHeaderValue("proxied");
            if(!String.IsNullOrEmpty(proxiedVal)) {
                proxied = Common.IsTrue(proxiedVal);
            }

            statsVal = md.CurrentHttpRequest.RetrieveHeaderValue("stats");
            if(!String.IsNullOrEmpty(statsVal)) {
                stats = Common.IsTrue(statsVal);
            }

            recursiveVal = md.CurrentHttpRequest.RetrieveHeaderValue("recursive");
            if(!String.IsNullOrEmpty(recursiveVal)) {
                recursive = Common.IsTrue(recursiveVal);
            }

            walkVal = md.CurrentHttpRequest.RetrieveHeaderValue("walk");
            if(!String.IsNullOrEmpty(walkVal)) {
                walk = Common.IsTrue(walkVal);
            }

            debugVal = md.CurrentHttpRequest.RetrieveHeaderValue("debug");
            if(!String.IsNullOrEmpty(debugVal)) {
                debug = Common.IsTrue(debugVal);
            }

            propertiesVal = md.CurrentHttpRequest.RetrieveHeaderValue("properties");
            if(!String.IsNullOrEmpty(propertiesVal)) {
                properties = Common.IsTrue(propertiesVal);
            }

            #endregion

            #region Retrieve-User-Home-Directory

            //qui recupera la home dell'utente
            homeDirectory = Users.GetHomeDirectory(md.CurrentUserMaster.Guid, CurrentSettings);
            if(String.IsNullOrEmpty(homeDirectory)) {
                ConsoleLogger.Warn("ContainerRead unable to retrieve home directory for user GUID " + md.CurrentUserMaster.Guid);
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to find home directory for user.", null).ToJson(), true);
            }

            #endregion

            #region Retrieve-Directory

            /*
             * Nodes that proxy requests will append ?proxied=true to the request URL
             * to notify the next recipient node to not proxy it further (otherwise
             * an infinite loop of REST requests will be generated)
             * 
             */

            if(proxied || (md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId)) {
                #region Local-Owner

                #region Check-Container-Permissions-and-Logging

                currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                if(currContainerPropertiesFile != null) {
                    //if(currContainerPropertiesFile.Logging != null) {
                    //    if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                    //        if(Common.IsTrue(currContainerPropertiesFile.Logging.ReadContainer)) {
                    //            #region Process-Logging

                    //            Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadContainer", null));

                    //            #endregion
                    //        }
                    //    }
                    //}

                    if(currContainerPropertiesFile.Permissions != null) {
                        #region Evaluate-Permissions

                        if(!ContainerPermission.GetPermission("ReadContainer", md, currContainerPropertiesFile)) {
                            //if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                            //    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadContainer", "denied"));
                            //}

                            ConsoleLogger.Warn("ContainerRead AllowReadContainer operation not authorized per container permissions");
                            return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                        }

                        #endregion
                    }
                }

                #endregion

                #region Create-Directory-if-Needed

                if(!Common.DirectoryExists(homeDirectory))
                    Common.CreateDirectory(homeDirectory);

                #endregion

                #region Gather-Directory-Info

                DirInfo di = new DirInfo(CurrentSettings, Users);

                if(md.CurrentObj.ContainerPath.Count == 0) {
                    md.CurrentObj.DiskPath = "/";
                }
                else {
                    var requestedPath = DirectoryUtil.SearchDirectory(md.CurrentObj.ContainerPath.ToArray());
                    md.CurrentObj.DiskPath = requestedPath;
                }

                di = di.FromDirectory(md.CurrentObj.DiskPath, md.CurrentObj.UserGuid, maxResults, null, metadataOnly);
                if(di == null) {
                    ConsoleLogger.Warn("ContainerRead null info returned from " + md.CurrentObj.DiskPath);
                    return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                        new ErrorResponse(5, 404, "Container does not exist.", null).ToJson(), true);
                }

                di.UserGuid = md.CurrentObj.UserGuid;
                di.Url = md.CurrentObj.PrimaryUrlWithoutQs;
                di.ContainerPath = md.CurrentObj.ContainerPath;

                #endregion

                #region Respond

                if(stats) {
                    #region stats

                    long bytes = 0;
                    int files = 0;
                    int subdirs = 0;
                    DirectoryInfo dirinfo = new DirectoryInfo(md.CurrentObj.DiskPath);

                    if(Common.DirectoryStatistics(dirinfo, recursive, out bytes, out files, out subdirs)) {
                        #region stats-success

                        Dictionary<string, object> ret = new Dictionary<string, object>();
                        ret.Add("UserGuid", di.UserGuid);
                        ret.Add("Url", di.Url);
                        if(debug)
                            ret.Add("DiskPath", md.CurrentObj.DiskPath);
                        if(debug)
                            ret.Add("PrimaryUrl", md.CurrentObj.PrimaryUrlWithoutQs);
                        ret.Add("Recursive", recursive);
                        ret.Add("Bytes", bytes);
                        ret.Add("Objects", files);
                        ret.Add("Containers", subdirs);

                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(ret), true);

                        #endregion
                    }
                    else {
                        #region stats-fail

                        ConsoleLogger.Warn("ContainerRead unable to execute Common.DirectoryStatistics on " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(1, 500, null, null).ToJson(), true);

                        #endregion
                    }

                    #endregion
                }
                else if(walk) {
                    #region walk

                    List<string> containers = new List<string>();
                    List<string> objects = new List<string>();
                    long bytes = 0;

                    if(Common.WalkDirectory(
                        CurrentSettings.Environment,
                        0,
                        md.CurrentObj.DiskPath,
                        true,
                        out containers,
                        out objects,
                        out bytes,
                        recursive)) {
                        #region walk-success

                        List<string> containersUpdated = new List<string>();
                        List<string> objectsUpdated = new List<string>();

                        foreach(string currContainer in containers) {
                            string tempContainer = String.Copy(currContainer);
                            tempContainer = tempContainer.Replace(md.CurrentObj.DiskPath, "");
                            tempContainer = tempContainer.Replace("\\", "/");
                            tempContainer = tempContainer.Replace("//", "/");
                            containersUpdated.Add(tempContainer);
                        }

                        foreach(string currObject in objects) {
                            string tempObject = String.Copy(currObject);
                            tempObject = tempObject.Replace(md.CurrentObj.DiskPath, "");
                            tempObject = tempObject.Replace("\\", "/");
                            tempObject = tempObject.Replace("//", "/");
                            while(tempObject.StartsWith("/"))
                                tempObject = tempObject.Substring(1, tempObject.Length - 1);
                            objectsUpdated.Add(tempObject);
                        }

                        Dictionary<string, object> ret = new Dictionary<string, object>();
                        ret.Add("UserGuid", di.UserGuid);
                        ret.Add("Url", di.Url);
                        if(debug)
                            ret.Add("DiskPath", md.CurrentObj.DiskPath);
                        if(debug)
                            ret.Add("PrimaryUrl", md.CurrentObj.PrimaryUrlWithoutQs);
                        ret.Add("Recursive", recursive);
                        ret.Add("Bytes", bytes);
                        ret.Add("Objects", objects.Count);
                        ret.Add("Containers", containers.Count);
                        ret.Add("ObjectList", objectsUpdated);
                        ret.Add("ContainerList", containersUpdated);

                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(ret), true);

                        #endregion
                    }
                    else {
                        #region walk-fail

                        ConsoleLogger.Warn("ContainerRead unable to execute walk on " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(1, 500, null, null).ToJson(), true);

                        #endregion
                    }

                    #endregion
                }
                else {
                    #region send-dir-info

                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(di), true);

                    #endregion
                }

                #endregion

                #endregion
            }
            else {
                #region Remote-Owner

                switch(CurrentSettings.Redirection.ReadRedirectionMode) {
                    case "none":
                        #region none

                        ConsoleLogger.Warn("ContainerRead object is stored on a different machine but ReadRedirectionMode is none");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                    #endregion

                    case "proxy":
                        #region proxy

                        //if(Maintenance.IsEnabled()) {
                        //    urlList = Obj.BuildMaintReadUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        //}
                        //else {
                        //    urlList = Obj.BuildReplicaUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        //}

                        if(urlList == null || urlList.Count < 1) {
                            ConsoleLogger.Warn("ContainerRead unable to build replica URL list (null response)");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to build proxy URL.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerRead proxying request to " + urlList.Count + " URLs for user GUID " + md.CurrentUserMaster.Guid);

                        proxyResponse = FirstResponder.SendRequest(
                            CurrentSettings,
                            md,
                            urlList,
                            md.CurrentHttpRequest.ContentType,
                            md.CurrentHttpRequest.Method,
                            null, null, false,
                            GetCustomHeaders(md.CurrentHttpRequest.Headers),
                            md.CurrentHttpRequest.Data);

                        if(proxyResponse == null) {
                            ConsoleLogger.Warn("ContainerRead null response from REST request to " + urlList.Count + " URLs");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerRead server response to proxy REST request: " + proxyResponse.StatusCode);
                        return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                    #endregion

                    case "redirect":
                        #region redirect

                        redirectUrl = Obj.BuildRedirectUrl(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        if(String.IsNullOrEmpty(redirectUrl)) {
                            ConsoleLogger.Warn("ContainerRead unable to generate redirect_url, returning 500");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to build redirect URL.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerRead redirecting request using status " + CurrentSettings.Redirection.ReadRedirectHttpStatus + " for user GUID " + md.CurrentUserMaster.Guid);
                        Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                        redirectHeader.Add("location", redirectUrl);
                        return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.ReadRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.ReadRedirectString, true);

                    #endregion

                    default:
                        #region unknown

                        ConsoleLogger.Warn("ContainerRead unknown ReadRedirectionMode in redirection settings: " + CurrentSettings.Redirection.ReadRedirectionMode);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                        #endregion
                }

                #endregion
            }

            #endregion
        }

        public HttpResponse ContainerRename(RequestMetadata md) {
            #region Variables

            RenameRequest req = new RenameRequest();
            RestResponse proxyResponse = new RestResponse();
            Dictionary<string, string> restHeaders = new Dictionary<string, string>();
            bool userGatewayMode = false;
            string containerLogFile = "";
            string containerPropertiesFile = "";
            ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();

            #endregion

            #region Check-Permissions

            if(md.CurrentApiKeyPermission == null) {
                ConsoleLogger.Warn("ContainerRename null ApiKeyPermission object supplied");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowWriteContainer)) {
                ConsoleLogger.Warn("ContainerRename AllowWriteContainer operation not authorized per permissions");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            #endregion

            #region Process-Owner

            if(md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId) {
                #region Local-Owner

                #region Check-Container-Permissions-and-Logging

                currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                if(currContainerPropertiesFile != null) {
                    //if(currContainerPropertiesFile.Logging != null) {
                    //    if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                    //        if(Common.IsTrue(currContainerPropertiesFile.Logging.ReadContainer)) {
                    //            #region Process-Logging

                    //            Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteContainer-Rename", null));

                    //            #endregion
                    //        }
                    //    }
                    //}

                    if(currContainerPropertiesFile.Permissions != null) {
                        #region Evaluate-Permissions

                        if(!ContainerPermission.GetPermission("WriteContainer", md, currContainerPropertiesFile)) {
                            //if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                            //    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteContainer-Rename", "denied"));
                            //}

                            ConsoleLogger.Warn("ContainerRename AllowWriteContainer operation not authorized per container permissions");
                            return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                        }

                        #endregion
                    }
                }

                #endregion

                #region Deserialize

                try {
                    req = Common.DeserializeJson<RenameRequest>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ContainerRename null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ContainerRename unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                }

                req.UserGuid = String.Copy(md.CurrentUserMaster.Guid);

                #endregion

                #region Validate-Request-Body

                if(req.ContainerPath == null) {
                    ConsoleLogger.Warn("ContainerRename null value supplied for ContainerPath, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for ContainerPath.", null).ToJson(), true);
                }

                if(String.IsNullOrEmpty(req.RenameFrom)) {
                    ConsoleLogger.Warn("ContainerRename null value supplied for RenameFrom, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for RenameFrom.", null).ToJson(), true);
                }

                if(String.IsNullOrEmpty(req.RenameTo)) {
                    ConsoleLogger.Warn("ContainerRename null value supplied for RenameTo, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for RenameTo.", null).ToJson(), true);

                }

                if(RenameRequest.UnsafeFsChars(req)) {
                    ConsoleLogger.Warn("ContainerRename unsafe characters detected in request, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unsafe characters detected.", null).ToJson(), true);

                }

                req.UserGuid = md.CurrentUserMaster.Guid;

                #endregion

                #region Check-if-Original-Exists

                string diskPathOriginal = "/" + string.Join("/", req.ContainerPath) + "/" + req.RenameFrom;
                //string diskPathOriginal = RenameRequest.BuildDiskPath(req, true, Users, CurrentSettings, Logging);
                if(String.IsNullOrEmpty(diskPathOriginal)) {
                    ConsoleLogger.Warn("ContainerRename unable to build disk path for original container");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                }

                if(!Common.DirectoryExists(diskPathOriginal)) {
                    ConsoleLogger.Warn("ContainerRename original container does not exist: " + diskPathOriginal);
                    return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                        new ErrorResponse(5, 404, "Container does not exist.", null).ToJson(), true);
                }

                #endregion

                #region Check-if-Target-Exists

                string diskPathTarget = "/" + string.Join("/", req.ContainerPath) + "/" + req.RenameTo;
                //string diskPathTarget = RenameRequest.BuildDiskPath(req, false, Users, CurrentSettings, Logging);
                if(String.IsNullOrEmpty(diskPathTarget)) {
                    ConsoleLogger.Warn("ContainerRename unable to build disk path for target container");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                }

                if(Common.DirectoryExists(diskPathTarget)) {
                    ConsoleLogger.Warn("ContainerRename target container already exists: " + diskPathOriginal);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Container already exists.", null).ToJson(), true);

                }

                #endregion

                #region Set-Gateway-Mode

                userGatewayMode = md.CurrentUserMaster.GetGatewayMode(CurrentSettings);

                #endregion

                #region Process-Replication

                ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                if(!rh.ContainerRename(req)) {
                    ConsoleLogger.Warn("ContainerRename negative response from replication, returning 500");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                }

                BunkerHandler bh = new BunkerHandler(CurrentSettings);
                bh.ContainerRename(req);

                #endregion

                #region Rename-Directory

                if(!Common.RenameDirectory(diskPathOriginal, diskPathTarget)) {
                    ConsoleLogger.Warn("ContainerRename unable to rename container from " + diskPathOriginal + " to " + diskPathTarget);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Container already exists.", null).ToJson(), true);
                }

                #endregion

                #region Perform-Background-Rewrite

                if(!userGatewayMode) {
                    ConsoleLogger.Warn("ContainerRename spawning background task to rewrite objects with correct metadata");
                    Task.Run(() => RewriteTree(diskPathTarget));
                }

                #endregion

                return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);

                #endregion
            }
            else {
                #region Remote-Owner

                switch(CurrentSettings.Redirection.WriteRedirectionMode) {
                    case "none":
                        #region none

                        ConsoleLogger.Warn("ContainerRename object is destined for a different machine but WriteRedirectionMode is none");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                    #endregion

                    case "proxy":
                        #region proxy

                        ConsoleLogger.Warn("ContainerRename proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs);

                        proxyResponse = RestRequest.SendRequestSafe(
                            md.CurrentObj.PrimaryUrlWithQs,
                            md.CurrentHttpRequest.ContentType,
                            md.CurrentHttpRequest.Method,
                            null, null, false,
                            Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                            GetCustomHeaders(md.CurrentHttpRequest.Headers),
                            md.CurrentHttpRequest.Data);

                        if(proxyResponse == null) {
                            ConsoleLogger.Warn("ContainerRename null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ContainerRename server response to proxy REST request: " + proxyResponse.StatusCode);
                        return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                    #endregion

                    case "redirect":
                        #region redirect

                        ConsoleLogger.Warn("ContainerRename redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs + " using status " + CurrentSettings.Redirection.WriteRedirectHttpStatus);
                        Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                        return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.WriteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.WriteRedirectString, true);

                    #endregion

                    default:
                        #region unknown

                        ConsoleLogger.Warn("ContainerRename unknown WriteRedirectionMode in redirection settings: " + CurrentSettings.Redirection.WriteRedirectionMode);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                        #endregion
                }

                #endregion
            }

            #endregion
        }

        public HttpResponse ContainerWrite(RequestMetadata md) {
            try {

                #region Variables

                string homeDirectory = "";
                RestResponse proxyResponse = new RestResponse();
                Dictionary<string, string> restHeaders = new Dictionary<string, string>();
                bool localSuccess = false;
                List<Node> successfulReplicas = new List<Node>();
                string containerLogFile = "";
                string containerPropertiesFile = "";
                ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();

                #endregion

                #region Check-Permissions

                if(md.CurrentApiKeyPermission == null) {
                    ConsoleLogger.Warn("ContainerWrite null ApiKeyPermission object supplied");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowWriteContainer)) {
                    ConsoleLogger.Warn("ContainerWrite AllowWriteContainer operation not authorized per permissions");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                #endregion

                #region Retrieve-User-Home-Directory

                homeDirectory = Users.GetHomeDirectory(md.CurrentUserMaster.Guid, CurrentSettings);
                if(String.IsNullOrEmpty(homeDirectory)) {
                    ConsoleLogger.Warn("ContainerWrite unable to retrieve home directory for user GUID " + md.CurrentUserMaster.Guid);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to find home directory for user.", null).ToJson(), true);
                }

                #endregion

                #region Process-Owner

                if(md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId) {
                    #region Local-Owner

                    #region Check-Container-Permissions-and-Logging

                    currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                    if(currContainerPropertiesFile != null) {
                        //if(currContainerPropertiesFile.Logging != null) {
                        //    if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                        //        if(Common.IsTrue(currContainerPropertiesFile.Logging.ReadContainer)) {
                        //            #region Process-Logging

                        //            Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteContainer", null));

                        //            #endregion
                        //        }
                        //    }
                        //}

                        if(currContainerPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ContainerPermission.GetPermission("WriteContainer", md, currContainerPropertiesFile)) {
                                //if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                //    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteContainer", "denied"));
                                //}

                                ConsoleLogger.Warn("ContainerWrite AllowWriteContainer operation not authorized per container permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Create-User-Folder-if-Needed

                    if(!Common.DirectoryExists(homeDirectory))
                        Common.CreateDirectory(homeDirectory);

                    #endregion

                    #region Process-Replication

                    ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                    if(!rh.ContainerWrite(md.CurrentObj, out successfulReplicas)) {
                        ConsoleLogger.Warn("ContainerWrite negative response from replication, returning 500");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                    }

                    BunkerHandler bh = new BunkerHandler(CurrentSettings);
                    bh.ContainerWrite(md.CurrentObj);

                    #endregion

                    #region Create-Directory-Locally

                    localSuccess = DirectoryUtil.CreateDirectory(md.CurrentObj.ContainerPath.ToArray());
                    //localSuccess = Common.CreateDirectory(md.CurrentObj.DiskPath);
                    if(!localSuccess) {
                        ConsoleLogger.Warn("ContainerWrite unable to write container to " + md.CurrentObj.DiskPath);
                        Task.Run(() => {
                            rh.ContainerDelete(md.CurrentObj, CurrentTopology.Replicas);
                        });

                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to store container.", null).ToJson(), true);
                    }

                    ConsoleLogger.Warn("ContainerWrite successfully created container " + md.CurrentObj.DiskPath);

                    #endregion

                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "text/plain", md.CurrentObj.PrimaryUrlWithoutQs, true);

                    #endregion
                }
                else {
                    #region Remote-Owner

                    switch(CurrentSettings.Redirection.WriteRedirectionMode) {
                        case "none":
                            #region none

                            ConsoleLogger.Warn("ContainerWrite object is destined for a different machine but WriteRedirectionMode is none");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                        #endregion

                        case "proxy":
                            #region proxy

                            ConsoleLogger.Warn("ContainerWrite proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs);

                            proxyResponse = RestRequest.SendRequestSafe(
                                md.CurrentObj.PrimaryUrlWithQs,
                                md.CurrentHttpRequest.ContentType,
                                md.CurrentHttpRequest.Method,
                                null, null, false,
                                Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                                GetCustomHeaders(md.CurrentHttpRequest.Headers),
                                md.CurrentHttpRequest.Data);

                            if(proxyResponse == null) {
                                ConsoleLogger.Warn("ContainerWrite null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ContainerWrite server response to proxy REST request: " + proxyResponse.StatusCode);
                            return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                        #endregion

                        case "redirect":
                            #region redirect

                            ConsoleLogger.Warn("ContainerWrite redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs + " using status " + CurrentSettings.Redirection.WriteRedirectHttpStatus);
                            Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                            redirectHeader.Add("location", md.CurrentObj.PrimaryUrlWithQs);
                            return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.WriteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.WriteRedirectString, true);

                        #endregion

                        default:
                            #region unknown

                            ConsoleLogger.Warn("ContainerWrite unknown WriteRedirectionMode in redirection settings: " + CurrentSettings.Redirection.WriteRedirectionMode);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                            #endregion
                    }

                    #endregion
                }

                #endregion
            }
            catch(IOException ioeOuter) {
                #region disk-full

                if(
                    ((ioeOuter.HResult & 0xFFFF) == 0x27)
                    || ((ioeOuter.HResult & 0xFFFF) == 0x70)
                   ) {
                    ConsoleLogger.Error("ContainerWrite disk full detected during write operation for " + md.CurrentUserMaster.Guid + " " + md.CurrentObj.Key);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(1, 500, "Disk is full.", null).ToJson(), true);
                }

                #endregion

                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json", null, true);
            }
        }

        #endregion

        #region Private-Methods

        private Dictionary<string, string> GetCustomHeaders(Dictionary<string, string> headers) {
            if(headers == null)
                return null;
            if(headers.Count == 0)
                return null;
            Dictionary<string, string> ret = new Dictionary<string, string>();

            foreach(KeyValuePair<string, string> curr in headers) {
                if(String.Compare(curr.Key, CurrentSettings.Server.HeaderApiKey) == 0
                    || String.Compare(curr.Key, CurrentSettings.Server.HeaderEmail) == 0
                    || String.Compare(curr.Key, CurrentSettings.Server.HeaderPassword) == 0
                    || String.Compare(curr.Key, CurrentSettings.Server.HeaderToken) == 0
                    || String.Compare(curr.Key, CurrentSettings.Server.HeaderVersion) == 0) {
                    if(ret.ContainsKey(curr.Key)) {
                        string tempVal = ret[curr.Key];
                        tempVal += "," + curr.Value;
                        ret.Remove(curr.Key);
                        ret.Add(curr.Key, tempVal);
                    }
                    else {
                        ret.Add(curr.Key, curr.Value);
                    }
                }
            }

            return ret;
        }

        private bool RewriteTree(string root) {
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

        private bool RewriteObject(string filename) {
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

        #endregion
    }
}
