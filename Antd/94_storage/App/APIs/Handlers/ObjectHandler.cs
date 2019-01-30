using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Kvpbase {
    public class ObjectHandler {
        #region Public-Members

        #endregion

        #region Private-Members

        private Settings CurrentSettings;
        private Topology CurrentTopology;
        private Node CurrentNode;
        private UserManager Users;
        private UrlLockManager LockManager;
        private MaintenanceManager Maintenance;
        private EncryptionModule EncryptionManager;
        private LoggerManager Logger;

        #endregion

        #region Constructors-and-Factories

        public ObjectHandler(
            Settings settings,
            Topology topology,
            Node node,
            UserManager users,
            UrlLockManager locks,
            MaintenanceManager maintenance,
            EncryptionModule encryption,
            LoggerManager logger) {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));
            if(node == null)
                throw new ArgumentNullException(nameof(node));
            if(users == null)
                throw new ArgumentNullException(nameof(users));
            if(locks == null)
                throw new ArgumentNullException(nameof(locks));
            if(maintenance == null)
                throw new ArgumentNullException(nameof(maintenance));
            if(encryption == null)
                throw new ArgumentNullException(nameof(encryption));
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            CurrentSettings = settings;
            CurrentTopology = topology;
            CurrentNode = node;
            Users = users;
            LockManager = locks;
            Maintenance = maintenance;
            EncryptionManager = encryption;
            Logger = logger;
        }

        #endregion

        #region Public-Methods

        public HttpResponse ObjectDelete(RequestMetadata md) {
            DateTime startTime = DateTime.Now;
            bool locked = false;

            try {
                #region try

                #region Variables

                RestResponse proxyResponse = new RestResponse();
                Dictionary<string, string> restHeaders = new Dictionary<string, string>();
                ObjInfo currObjInfo = new ObjInfo();
                bool deleteSuccess = false;
                string containerLogFile = "";
                string containerPropertiesFile = "";
                ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();
                string objectLogFile = "";
                string objectPropertiesFile = "";
                ObjectPropertiesFile currObjectPropertiesFile = new ObjectPropertiesFile();

                #endregion

                #region Check-Permissions

                if(md.CurrentApiKeyPermission == null) {
                    ConsoleLogger.Warn("ObjectDelete null ApiKeyPermission object supplied");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowDeleteObject)) {
                    ConsoleLogger.Warn("ObjectDelete AllowDeleteObject operation not authorized per permissions");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                #endregion

                #region Process-Owner

                if(md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId) {
                    #region Local-Owner

                    #region Add-Lock

                    locked = LockManager.LockUrl(md);
                    if(!locked) {
                        ConsoleLogger.Warn("ObjectDelete " + md.CurrentObj.DiskPath + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Check-Container-Permissions-and-Logging

                    currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                    if(currContainerPropertiesFile != null) {
                        if(currContainerPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.DeleteObject)) {
                                    #region Process-Logging

                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "DeleteObject", null));

                                    #endregion
                                }
                            }
                        }

                        if(currContainerPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ContainerPermission.GetPermission("DeleteObject", md, currContainerPropertiesFile)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "DeleteObject", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectDelete AllowDeleteObject operation not authorized per container permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Check-Object-Permissions-and-Logging

                    currObjectPropertiesFile = ObjectPropertiesFile.FromObject(md.CurrentObj, out objectLogFile, out objectPropertiesFile);
                    if(currObjectPropertiesFile != null) {
                        if(currObjectPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.DeleteObject)) {
                                    #region Process-Logging

                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectDelete", null));

                                    #endregion
                                }
                            }
                        }

                        if(currObjectPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ObjectPermission.GetPermission("ObjectDelete", md, currObjectPropertiesFile)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectDelete", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectDelete AllowDeleteObject operation not authorized per object permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Retrieve-Object-Metadata

                    var fileCurrObjInfo = "/" + string.Join("/", md.CurrentObj.ContainerPath) + "/" + md.CurrentObj.Key;
                    currObjInfo = ObjInfo.FromFile(fileCurrObjInfo);
                    //currObjInfo = ObjInfo.FromFile(md.CurrentObj.DiskPath);
                    if(currObjInfo == null) {
                        ConsoleLogger.Warn("ObjectDelete null file info returned for " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                            new ErrorResponse(5, 404, "Object does not exist.", null).ToJson(), true);
                    }

                    #endregion

                    #region Process-Replication

                    ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                    if(!rh.ObjectDelete(md.CurrentObj, md.CurrentObj.Replicas)) {
                        ConsoleLogger.Warn("ObjectDelete negative response from replication, returning 500");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                    }

                    BunkerHandler bh = new BunkerHandler(CurrentSettings);
                    bh.ObjectDelete(md.CurrentObj);

                    #endregion

                    #region Delete-and-Respond

                    deleteSuccess = Common.DeleteFile(fileCurrObjInfo);
                    //deleteSuccess = Common.DeleteFile(md.CurrentObj.DiskPath);
                    if(deleteSuccess) {
                        ConsoleLogger.Warn("ObjectDelete successfully deleted " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                    }
                    else {
                        ConsoleLogger.Warn("ObjectDelete could not delete " + md.CurrentUserMaster.Guid + " " + md.CurrentObj.Key);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to delete object.", null).ToJson(), true);
                    }

                    #endregion

                    #endregion
                }
                else {
                    #region Remote-Owner

                    switch(CurrentSettings.Redirection.DeleteRedirectionMode) {
                        case "none":
                            #region none

                            ConsoleLogger.Warn("ObjectDelete object is destined for a different machine but DeleteRedirectionModee is none");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                        #endregion

                        case "proxy":
                            #region proxy

                            ConsoleLogger.Warn("ObjectDelete proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs + " for object key " + md.CurrentObj.Key);

                            proxyResponse = RestRequest.SendRequestSafe(
                                md.CurrentObj.PrimaryUrlWithQs,
                                md.CurrentHttpRequest.ContentType,
                                md.CurrentHttpRequest.Method,
                                null, null, false,
                                Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                                GetCustomHeaders(md.CurrentHttpRequest.Headers),
                                md.CurrentHttpRequest.Data);

                            if(proxyResponse == null) {
                                ConsoleLogger.Warn("ObjectDelete null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectDelete server response to proxy REST request: " + proxyResponse.StatusCode);
                            return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                        #endregion

                        case "redirect":
                            #region redirect

                            ConsoleLogger.Warn("ObjectDelete redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs +
                                " using status " + CurrentSettings.Redirection.DeleteRedirectHttpStatus +
                                " for object key " + md.CurrentObj.Key);
                            Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                            redirectHeader.Add("location", md.CurrentObj.PrimaryUrlWithQs);
                            return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.DeleteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.DeleteRedirectString, true);

                        #endregion

                        default:
                            #region unknown

                            ConsoleLogger.Warn("ObjectDelete unknown DeleteRedirectionModee in redirection settings: " + CurrentSettings.Redirection.DeleteRedirectionMode);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                            #endregion
                    }

                    #endregion
                }

                #endregion

                #endregion
            }
            finally {
                #region finally

                #region unlock

                if(locked) {
                    if(!LockManager.UnlockUrl(md)) {
                        ConsoleLogger.Warn("ObjectDelete unable to unlock " + md.CurrentObj.DiskPath);
                    }
                }

                #endregion

                #endregion
            }
        }

        public HttpResponse ObjectHead(RequestMetadata md) {
            DateTime startTime = DateTime.Now;
            bool locked = false;

            try {
                #region try

                #region Variables

                RestResponse proxyResponse = new RestResponse();
                Dictionary<string, string> restHeaders = new Dictionary<string, string>();
                List<string> urls = new List<string>();
                string redirectUrls = "";
                string proxiedVal = "";
                bool proxied = false;
                ObjInfo currObjInfo = new ObjInfo();
                string containerLogFile = "";
                string containerPropertiesFile = "";
                ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();
                string objectLogFile = "";
                string objectPropertiesFile = "";
                ObjectPropertiesFile currObjectPropertiesFile = new ObjectPropertiesFile();

                #endregion

                #region Check-Permissions

                if(md.CurrentApiKeyPermission == null) {
                    ConsoleLogger.Warn("ObjectHead null ApiKeyPermission object supplied");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, null, null).ToJson(), true);
                }

                if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowReadObject)) {
                    ConsoleLogger.Warn("ObjectHead AllowReadObject operation not authorized per permissions");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, null, null).ToJson(), true);
                }

                #endregion

                #region Check-for-Key-in-URL

                if(String.IsNullOrEmpty(md.CurrentObj.Key)) {
                    ConsoleLogger.Warn("ObjectHead unable to find object key in URL");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(2, 400, null, null).ToJson(), true);

                }

                #endregion

                #region Get-Values-from-Querystring

                proxiedVal = md.CurrentHttpRequest.RetrieveHeaderValue("proxied");
                if(!String.IsNullOrEmpty(proxiedVal)) {
                    proxied = Common.IsTrue(proxiedVal);
                }

                #endregion

                #region Retrieve-Specific-Object

                /*
                 * Nodes that proxy requests will append ?proxied=true to the request URL
                 * to notify the next recipient node to not proxy it further (otherwise
                 * an infinite loop of REST requests will be generated)
                 * 
                 */

                if(proxied || (md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId)) {
                    #region Local-Owner

                    #region Add-Lock

                    locked = LockManager.AddReadResource(md.CurrentObj.DiskPath);
                    if(!locked) {
                        ConsoleLogger.Warn("ObjectHead " + md.CurrentObj.DiskPath + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Check-Container-Permissions-and-Logging

                    currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                    if(currContainerPropertiesFile != null) {
                        if(currContainerPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.ReadObject)) {
                                    #region Process-Logging

                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadObject", null));

                                    #endregion
                                }
                            }
                        }

                        if(currContainerPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ContainerPermission.GetPermission("ReadObject", md, currContainerPropertiesFile)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadObject", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectHead AllowReadObject operation not authorized per container permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Check-Object-Permissions-and-Logging

                    currObjectPropertiesFile = ObjectPropertiesFile.FromObject(md.CurrentObj, out objectLogFile, out objectPropertiesFile);
                    if(currObjectPropertiesFile != null) {
                        if(currObjectPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.ReadObject)) {
                                    #region Process-Logging

                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectHead", null));

                                    #endregion
                                }
                            }
                        }

                        if(currObjectPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ObjectPermission.GetPermission("ReadObject", md, currObjectPropertiesFile)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectHead", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectHead AllowReadObject operation not authorized per object permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Process

                    currObjInfo = ObjInfo.FromFile(md.CurrentObj.DiskPath);
                    if(currObjInfo == null) {
                        ConsoleLogger.Warn("ObjectHead null file info returned for " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                            new ErrorResponse(5, 404, "Object does not exist.", null).ToJson(), true);
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

                            ConsoleLogger.Warn("ObjectHead object is stored on a different machine but ReadRedirectionMode is none");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                        #endregion

                        case "proxy":
                            #region proxy

                            if(Maintenance.IsEnabled()) {
                                urls = Obj.BuildMaintReadUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                            }
                            else {
                                urls = Obj.BuildReplicaUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                            }

                            if(urls == null || urls.Count < 1) {
                                ConsoleLogger.Warn("ObjectHead unable to build replica URL list (null response)");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to build proxy URL.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectHead proxying request to " + urls.Count + " URLs for user GUID " + md.CurrentUserMaster.Guid);

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
                                ConsoleLogger.Warn("ObjectHead null response from proxy REST request to " + urls.Count + " URLs");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectHead server response to proxy REST request: " + proxyResponse.StatusCode);
                            return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                        #endregion

                        case "redirect":
                            #region redirect

                            redirectUrls = Obj.BuildRedirectUrl(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                            if(String.IsNullOrEmpty(redirectUrls)) {
                                ConsoleLogger.Warn("ObjectHead unable to generate redirect URL, returning 500");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to build redirect URL.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectHead redirecting request using status " + CurrentSettings.Redirection.ReadRedirectHttpStatus + " for user GUID " + md.CurrentUserMaster.Guid);
                            Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                            redirectHeader.Add("location", redirectUrls);
                            return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.ReadRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.ReadRedirectString, true);

                        #endregion

                        default:
                            #region unknown

                            ConsoleLogger.Warn("ObjectHead unknown ReadRedirectionMode in redirection settings: " + CurrentSettings.Redirection.ReadRedirectionMode);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                            #endregion
                    }

                    #endregion
                }

                #endregion

                #endregion
            }
            finally {
                #region finally

                #region unlock

                if(locked) {
                    if(!LockManager.RemoveReadResource(md.CurrentObj.DiskPath)) {
                        ConsoleLogger.Warn("ObjectHead unable to unlock " + md.CurrentObj.DiskPath);
                    }
                }

                #endregion

                #endregion
            }
        }

        public HttpResponse ObjectMove(RequestMetadata md) {
            DateTime startTime = DateTime.Now;
            bool lockedOriginal = false;
            bool lockedTarget = false;
            string diskPathOriginal = "";
            string diskPathTarget = "";

            try {
                #region try

                #region Variables

                MoveRequest req = new MoveRequest();
                RestResponse proxyResponse = new RestResponse();
                Dictionary<string, string> restHeaders = new Dictionary<string, string>();
                Obj currObj = new Obj();
                bool userGatewayMode = false;
                List<Node> successfulReplicas = new List<Node>();
                string containerLogFile = "";
                string containerPropertiesFile = "";
                ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();
                string objectLogFile = "";
                string objectPropertiesFile = "";
                ObjectPropertiesFile currObjectPropertiesFile = new ObjectPropertiesFile();

                #endregion

                #region Check-Permissions

                if(md.CurrentApiKeyPermission == null) {
                    ConsoleLogger.Warn("ObjectMove null ApiKeyPermission object supplied");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowWriteObject)) {
                    ConsoleLogger.Warn("ObjectMove AllowWriteObject operation not authorized per permissions");
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
                        if(currContainerPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.CreateObject)) {
                                    #region Process-Logging

                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteObject-Move", null));

                                    #endregion
                                }
                            }
                        }

                        if(currContainerPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ContainerPermission.GetPermission("WriteObject", md, currContainerPropertiesFile)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteObject-Move", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectMove AllowWriteObject operation not authorized per container permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Check-Object-Permissions-and-Logging

                    currObjectPropertiesFile = ObjectPropertiesFile.FromObject(md.CurrentObj, out objectLogFile, out objectPropertiesFile);
                    if(currObjectPropertiesFile != null) {
                        if(currObjectPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.CreateObject)) {
                                    #region Process-Logging

                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectMove", null));

                                    #endregion
                                }
                            }
                        }

                        if(currObjectPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ObjectPermission.GetPermission("WriteObject", md, currObjectPropertiesFile)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectMove", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectMove AllowWriteObject operation not authorized per object permissions");
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
                            ConsoleLogger.Warn("ObjectMove null request after deserialization, returning 400");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                        }
                    }
                    catch(Exception) {
                        ConsoleLogger.Warn("ObjectMove unable to deserialize request body");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }

                    req.UserGuid = String.Copy(md.CurrentUserMaster.Guid);

                    #endregion

                    #region Validate-Request-Body

                    if(req.FromContainer == null) {
                        req.FromContainer = new List<string>();
                    }

                    if(req.ToContainer == null) {
                        req.ToContainer = new List<string>();
                    }

                    if(String.IsNullOrEmpty(req.MoveFrom)) {
                        ConsoleLogger.Warn("ObjectMove null value supplied for MoveFrom, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for MoveFrom.", null).ToJson(), true);
                    }

                    if(String.IsNullOrEmpty(req.MoveTo)) {
                        ConsoleLogger.Warn("ObjectMove null value supplied for MoveTo, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for MoveTo.", null).ToJson(), true);
                    }

                    if(MoveRequest.UnsafeFsChars(req)) {
                        ConsoleLogger.Warn("ObjectMove unsafe characters detected in request, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(2, 400, "Unsafe characters detected.", null).ToJson(), true);

                    }

                    req.UserGuid = md.CurrentUserMaster.Guid;

                    #endregion

                    #region Check-if-Original-Object-Exists

                    diskPathOriginal = MoveRequest.BuildDiskPath(req, true, true, Users, CurrentSettings);
                    if(String.IsNullOrEmpty(diskPathOriginal)) {
                        ConsoleLogger.Warn("ObjectMove unable to build disk path for original object");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                    }

                    if(!Common.FileExists(diskPathOriginal)) {
                        ConsoleLogger.Warn("ObjectMove from object does not exist: " + diskPathOriginal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                            new ErrorResponse(5, 404, "Object does not exist.", null).ToJson(), true);
                    }

                    #endregion

                    #region Lock-Original

                    lockedOriginal = LockManager.LockResource(md, diskPathOriginal);
                    if(!lockedOriginal) {
                        ConsoleLogger.Warn("ObjectMove source object " + diskPathOriginal + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Check-if-Target-Container-Exists

                    string diskPathTargetContainer = MoveRequest.BuildDiskPath(req, false, false, Users, CurrentSettings);
                    if(String.IsNullOrEmpty(diskPathTargetContainer)) {
                        ConsoleLogger.Warn("ObjectMove unable to build disk path for target container");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to build disk path from target.", null).ToJson(), true);
                    }

                    if(!Common.DirectoryExists(diskPathTargetContainer)) {
                        ConsoleLogger.Warn("ObjectMove target container does not exist: " + diskPathOriginal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                            new ErrorResponse(5, 404, "Container does not exist.", null).ToJson(), true);
                    }

                    #endregion

                    #region Check-if-Target-Object-Exists

                    diskPathTarget = MoveRequest.BuildDiskPath(req, false, true, Users, CurrentSettings);
                    if(String.IsNullOrEmpty(diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectMove unable to build disk path for target object");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                    }

                    if(Common.FileExists(diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectMove target object already exists: " + diskPathOriginal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(2, 400, "Object already exists.", null).ToJson(), true);
                    }

                    #endregion

                    #region Lock-Target

                    lockedTarget = LockManager.LockResource(md, diskPathTarget);
                    if(!lockedTarget) {
                        ConsoleLogger.Warn("ObjectMove target object " + diskPathTarget + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Set-Gateway-Mode

                    userGatewayMode = md.CurrentUserMaster.GetGatewayMode(CurrentSettings);

                    #endregion

                    #region Read-Original-Object

                    currObj = Obj.BuildObjFromDisk(diskPathOriginal, Users, CurrentSettings, CurrentTopology, CurrentNode);
                    if(currObj == null) {
                        ConsoleLogger.Warn("ObjectMove unable to retrieve obj for " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to read object.", null).ToJson(), true);
                    }

                    #endregion

                    #region Process-Replication

                    ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                    if(!rh.ObjectMove(req, currObj)) {
                        ConsoleLogger.Warn("ObjectMove negative response from replication, returning 500");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                    }

                    BunkerHandler bh = new BunkerHandler(CurrentSettings);
                    bh.ObjectMove(req);

                    #endregion

                    #region Perform-Move

                    if(!Common.MoveFile(diskPathOriginal, diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectMove unable to move file from " + diskPathOriginal + " to " + diskPathTarget);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Object already exists.", null).ToJson(), true);
                    }

                    #endregion

                    #region Perform-Background-Rewrite

                    if(!userGatewayMode) {
                        ConsoleLogger.Warn("ObjectMove spawning background task to rewrite object with correct metadata");
                        Task.Run(() => RewriteObject(diskPathTarget));
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

                            ConsoleLogger.Warn("ObjectMove object is destined for a different machine but WriteRedirectionMode is none");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                        #endregion

                        case "proxy":
                            #region proxy

                            ConsoleLogger.Warn("ObjectMove proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs);

                            proxyResponse = RestRequest.SendRequestSafe(
                                md.CurrentObj.PrimaryUrlWithQs,
                                md.CurrentHttpRequest.ContentType,
                                md.CurrentHttpRequest.Method,
                                null, null, false,
                                Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                                GetCustomHeaders(md.CurrentHttpRequest.Headers),
                                md.CurrentHttpRequest.Data);

                            if(proxyResponse == null) {
                                ConsoleLogger.Warn("ObjectMove null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectMove server response to proxy REST request: " + proxyResponse.StatusCode);
                            return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                        #endregion

                        case "redirect":
                            #region redirect

                            ConsoleLogger.Warn("ObjectMove redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs + " using status " + CurrentSettings.Redirection.WriteRedirectHttpStatus);
                            Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                            redirectHeader.Add("location", md.CurrentObj.PrimaryUrlWithQs);
                            return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.WriteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.WriteRedirectString, true);

                        #endregion

                        default:
                            #region unknown

                            ConsoleLogger.Warn("ObjectMove unknown WriteRedirectionMode in redirection settings: " + CurrentSettings.Redirection.WriteRedirectionMode);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                            #endregion
                    }

                    #endregion
                }

                #endregion

                #endregion
            }
            finally {
                #region finally

                #region unlock

                if(lockedOriginal) {
                    if(!LockManager.UnlockResource(md, diskPathOriginal)) {
                        ConsoleLogger.Warn("ObjectMove unable to unlock source path " + diskPathOriginal);
                    }
                }

                if(lockedTarget) {
                    if(!LockManager.UnlockResource(md, diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectMove unable to unlock target path " + diskPathTarget);
                    }
                }

                #endregion

                #endregion
            }
        }

        public HttpResponse ObjectRead(RequestMetadata md) {
            DateTime startTime = DateTime.Now;
            bool locked = false;

            try {
                #region try

                #region Variables

                RestResponse proxyResponse = new RestResponse();
                Dictionary<string, string> restHeaders = new Dictionary<string, string>();
                List<string> urls = new List<string>();
                string redirectUrl = "";
                string metadataVal = "";
                bool metadataOnly = false;
                string proxiedVal = "";
                bool proxied = false;
                string readFromVal = "";
                int readFrom = 0;
                string countVal = "";
                int count = 0;
                string imsVal = "";
                DateTime imsDt = new DateTime(1970, 1, 1);
                bool ims = false;
                string publicUrl = "";
                PublicObj currPubfile = new PublicObj();
                ObjInfo currObjInfo = new ObjInfo();
                Obj currObj = new Obj();
                byte[] clear;
                string maxResultsStr = "";
                int maxResults = 0;
                string ContentType = "application/octet-stream";
                string containerLogFile = "";
                string containerPropertiesFile = "";
                ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();
                string objectLogFile = "";
                string objectPropertiesFile = "";
                ObjectPropertiesFile currObjectPropertiesFile = new ObjectPropertiesFile();
                bool resize = false;
                int resizeWdith = 0;
                int resizeHeight = 0;

                #endregion

                #region Check-Permissions

                if(md.CurrentApiKeyPermission == null) {
                    ConsoleLogger.Warn("ObjectRead null ApiKeyPermission object supplied");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowReadObject)) {
                    ConsoleLogger.Warn("ObjectRead AllowReadObject operation not authorized per permissions");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                #endregion

                #region Check-for-Key-in-URL

                if(String.IsNullOrEmpty(md.CurrentObj.Key)) {
                    ConsoleLogger.Warn("ObjectRead unable to find object key in URL");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to find object key URL.", null).ToJson(), true);
                }

                #endregion

                #region Get-Values-from-Querystring

                maxResultsStr = md.CurrentHttpRequest.RetrieveHeaderValue("max_results");
                if(!String.IsNullOrEmpty(maxResultsStr)) {
                    if(!Int32.TryParse(maxResultsStr, out maxResults)) {
                        ConsoleLogger.Warn("ObjectRead invalid value for max_results in querystring: " + maxResultsStr);
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

                readFromVal = md.CurrentHttpRequest.RetrieveHeaderValue("read_from");
                if(!String.IsNullOrEmpty(readFromVal)) {
                    if(!Int32.TryParse(readFromVal, out readFrom)) {
                        ConsoleLogger.Warn("ObjectRead invalid value for read_from in querystring: " + readFromVal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for read_from.", null).ToJson(), true);
                    }

                    if(readFrom < 0) {
                        ConsoleLogger.Warn("ObjectRead invalid value for read_from (must be zero or greater): " + readFrom);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for read_from.", null).ToJson(), true);
                    }
                }

                countVal = md.CurrentHttpRequest.RetrieveHeaderValue("count");
                if(!String.IsNullOrEmpty(countVal)) {
                    if(!Int32.TryParse(countVal, out count)) {
                        ConsoleLogger.Warn("ObjectRead invalid value for count in querystring: " + countVal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for count.", null).ToJson(), true);
                    }

                    if(count < 1) {
                        ConsoleLogger.Warn("ObjectRead invalid value for count (must be greater than zero): " + count);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for count.", null).ToJson(), true);
                    }
                }

                publicUrl = md.CurrentHttpRequest.RetrieveHeaderValue("public_url");

                resize = Common.IsTrue(md.CurrentHttpRequest.RetrieveHeaderValue("resize"));

                if(resize) {
                    if(!Int32.TryParse(md.CurrentHttpRequest.RetrieveHeaderValue("width"), out resizeWdith)) {
                        ConsoleLogger.Warn("ObjectRead invalid value for width in querystring");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for width.", null).ToJson(), true);
                    }

                    if(!Int32.TryParse(md.CurrentHttpRequest.RetrieveHeaderValue("height"), out resizeHeight)) {
                        ConsoleLogger.Warn("ObjectRead invalid value for height in querystring");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for height.", null).ToJson(), true);
                    }
                }

                #endregion

                #region Get-Values-from-Headers

                imsVal = md.CurrentHttpRequest.RetrieveHeaderValue("if-modified-since");
                if(!String.IsNullOrEmpty(imsVal)) {
                    if(!DateTime.TryParse(imsVal, out imsDt)) {
                        ConsoleLogger.Warn("ObjectRead invalid value for If-Modified-Since header: " + imsVal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for If-Modified-Since header.  Use MM/dd/yyyy hh:mm:ss.", null).ToJson(), true);
                    }

                    ims = true;
                }

                #endregion

                #region Retrieve-Specific-Object

                /*
                 * Nodes that proxy requests will append ?proxied=true to the request URL
                 * to notify the next recipient node to not proxy it further (otherwise
                 * an infinite loop of REST requests will be generated)
                 * 
                 */

                if(proxied || (md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId)) {
                    #region Local-Owner

                    #region Add-Lock

                    locked = LockManager.AddReadResource(md.CurrentObj.DiskPath);
                    if(!locked) {
                        ConsoleLogger.Warn("ObjectRead " + md.CurrentObj.DiskPath + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Check-Container-Permissions-and-Logging

                    currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                    if(currContainerPropertiesFile != null) {
                        if(currContainerPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.ReadObject)) {
                                    #region Process-Logging

                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadObject", null));

                                    #endregion
                                }
                            }
                        }

                        if(currContainerPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ContainerPermission.GetPermission("ReadObject", md, currContainerPropertiesFile)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "ReadObject", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectRead AllowReadObject operation not authorized per container permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Check-Object-Permissions-and-Logging

                    currObjectPropertiesFile = ObjectPropertiesFile.FromObject(md.CurrentObj, out objectLogFile, out objectPropertiesFile);
                    if(currObjectPropertiesFile != null) {
                        if(currObjectPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.ReadObject)) {
                                    #region Process-Logging

                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectRead", null));

                                    #endregion
                                }
                            }
                        }

                        if(currObjectPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ObjectPermission.GetPermission("ReadObject", md, currObjectPropertiesFile)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectRead", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectRead AllowReadObject operation not authorized per object permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Retrieve-Object-Metadata
                    var pre = md.CurrentObj.ContainerPath.Count == 0 ? "" : "/";
                    var fileCurrObjInfo = pre + string.Join("/", md.CurrentObj.ContainerPath) + "/" + md.CurrentObj.Key;
                    currObjInfo = ObjInfo.FromFile(fileCurrObjInfo);
                    //currObjInfo = ObjInfo.FromFile(md.CurrentObj.DiskPath);
                    if(currObjInfo == null) {
                        // ConsoleLogger.Warn("ObjectRead null file info returned for " + md.currObj.disk path);
                        return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                            new ErrorResponse(5, 404, "Object does not exist.", null).ToJson(), true);
                    }

                    #endregion

                    #region If-Modified-Since

                    if(ims) {
                        if(currObjInfo.LastUpdate != null) {
                            if(Convert.ToDateTime(currObjInfo.LastUpdate) < imsDt) {
                                ConsoleLogger.Warn("ObjectRead not 304 modified for " + md.CurrentObj.DiskPath);
                                return new HttpResponse(md.CurrentHttpRequest, true, 304, Common.AddToDictionary("Date", currObjInfo.LastUpdate.ToString(), null), null, null, true);
                            }
                        }
                    }

                    #endregion

                    #region Retrieve-Object

                    currObj = Obj.BuildObjFromDisk(fileCurrObjInfo, Users, CurrentSettings, CurrentTopology, CurrentNode);
                    //currObj = Obj.BuildObjFromDisk(md.CurrentObj.DiskPath, Users, CurrentSettings, CurrentTopology, CurrentNode);
                    if(currObj == null) {
                        ConsoleLogger.Warn("ObjectRead unable to retrieve obj for " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to read object.", null).ToJson(), true);
                    }

                    md.CurrentObj = currObj;

                    #endregion

                    #region Process-Pubfile

                    if(Common.IsTrue(publicUrl)) {
                        ConsoleLogger.Warn("ObjectRead generating public URL handle for obj " + md.CurrentObj.DiskPath);
                        currPubfile = new PublicObj();
                        currPubfile.Guid = Guid.NewGuid().ToString();
                        currPubfile.Url = PublicObj.BuildUrl(currPubfile.Guid, CurrentNode);
                        currPubfile.DiskPath = md.CurrentObj.DiskPath;
                        currPubfile.IsObject = 1;
                        currPubfile.IsContainer = 0;
                        currPubfile.Created = DateTime.Now.ToUniversalTime();
                        currPubfile.Expiration = currPubfile.Created.AddSeconds(CurrentSettings.PublicObj.DefaultExpirationSec);
                        currPubfile.UserGuid = md.CurrentUserMaster.Guid;

                        if(!Common.WriteFile(CurrentSettings.PublicObj.Directory + currPubfile.Guid, Encoding.UTF8.GetBytes(Common.SerializeJson(currPubfile)))) {
                            ConsoleLogger.Warn("ObjectRead unable to create pubfile record for " + md.CurrentObj.DiskPath);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to create public link.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ObjectRead created pubfile record for " + currPubfile.DiskPath + " expiring " + currPubfile.Expiration.ToString("MM/dd/yyyy HH:mm:ss"));
                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "text/plain", currPubfile.Url, true);
                    }

                    #endregion

                    #region Decrypt

                    if(Common.IsTrue(md.CurrentObj.IsEncrypted)) {
                        if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption))
                            ConsoleLogger.Warn("ObjectRead before decryption: " + Common.BytesToBase64(md.CurrentObj.Value));

                        if(String.IsNullOrEmpty(md.CurrentObj.EncryptionKsn)) {
                            md.CurrentObj.Value = EncryptionManager.LocalDecrypt(md.CurrentObj.Value);
                        }
                        else {
                            if(!EncryptionManager.ServerDecrypt(md.CurrentObj.Value, md.CurrentObj.EncryptionKsn, out clear)) {
                                ConsoleLogger.Warn("ObjectRead unable to decrypt object using server-based decryption");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to decrypt using crypto server.", null).ToJson(), true);
                            }

                            md.CurrentObj.Value = clear;
                        }

                        if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption))
                            ConsoleLogger.Warn("ObjectRead after decryption: " + Common.BytesToBase64(md.CurrentObj.Value));
                    }

                    #endregion

                    #region Decompress

                    if(Common.IsTrue(md.CurrentObj.IsCompressed)) {
                        if(Common.IsTrue(CurrentSettings.Debug.DebugCompression))
                            ConsoleLogger.Warn("ObjectRead before decompression: " + Common.BytesToBase64(md.CurrentObj.Value));
                        md.CurrentObj.Value = Common.GzipDecompress(md.CurrentObj.Value);
                        if(Common.IsTrue(CurrentSettings.Debug.DebugCompression))
                            ConsoleLogger.Warn("ObjectRead after decompression: " + Common.BytesToBase64(md.CurrentObj.Value));
                    }

                    #endregion

                    #region Set-Content-Type

                    if(!String.IsNullOrEmpty(md.CurrentObj.ContentType))
                        ContentType = md.CurrentObj.ContentType;
                    else
                        ContentType = MimeTypes.GetFromExtension(Common.GetFileExtension(md.CurrentObj.DiskPath));

                    #endregion

                    #region Validate-Range-Read

                    if(count > 0) {
                        if(readFrom + count > md.CurrentObj.Value.Length) {
                            ConsoleLogger.Warn("ObjectRead range exceeds object length (" + md.CurrentObj.Value.Length + "): read_from " + readFrom + " count " + count);
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Range exceeds object length.", null).ToJson(), true);
                        }
                    }

                    #endregion

                    #region Respond

                    if(!metadataOnly) {
                        #region Respond-with-Data

                        if(resize) {
                            ConsoleLogger.Warn("ObjectRead attempting to resize object to width " + resizeWdith + " height " + resizeHeight);

                            if(Common.IsImage(md.CurrentObj.Value)) {
                                Image original = Common.BytesToImage(md.CurrentObj.Value);
                                if(original == null) {
                                    ConsoleLogger.Warn("ObjectRead unable to convert bytes to image");
                                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                        new ErrorResponse(1, 500, "Unable to convert BLOB to image.", null).ToJson(), true);
                                }

                                Image resized = Common.ResizeImage(original, resizeWdith, resizeHeight);
                                if(resized == null) {
                                    ConsoleLogger.Warn("ObjectRead unable to resize image");
                                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                        new ErrorResponse(1, 500, "Unable to resize image.", null).ToJson(), true);
                                }

                                md.CurrentObj.Value = Common.ImageToBytes(resized);
                            }
                            else {
                                ConsoleLogger.Warn("ObjectRead byte data is not an image, returning original data");
                            }
                        }

                        count = (int)new FileInfo(currObj.DiskPath).Length;

                        if(count > 0) {
                            byte[] ret = new byte[count];
                            Buffer.BlockCopy(md.CurrentObj.Value, readFrom, ret, 0, count);

                            return new HttpResponse(md.CurrentHttpRequest, true, 200, null, ContentType, ret, true);
                        }
                        else {
                            return new HttpResponse(md.CurrentHttpRequest, true, 200, null, ContentType, md.CurrentObj.Value, true);
                        }

                        #endregion
                    }
                    else {
                        #region Respond-with-Metadata

                        md.CurrentObj.Value = null;
                        md.CurrentObj.Created = currObjInfo.Created;
                        md.CurrentObj.LastUpdate = currObjInfo.LastUpdate;
                        md.CurrentObj.LastAccess = currObjInfo.LastAccess;
                        return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(md.CurrentObj), true);

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

                            ConsoleLogger.Warn("ObjectRead object is stored on a different machine but ReadRedirectionMode is none");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request proxying disabled by configuration  Please direct this request to the appropriate node.", null).ToJson(), true);

                        #endregion

                        case "proxy":
                            #region proxy

                            if(Maintenance.IsEnabled()) {
                                urls = Obj.BuildMaintReadUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                            }
                            else {
                                urls = Obj.BuildReplicaUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                            }

                            if(urls == null || urls.Count < 1) {
                                ConsoleLogger.Warn("ObjectRead unable to build replica URL list (null response)");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to build proxy URL.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectRead proxying request to " + urls.Count + " URLs for user GUID " + md.CurrentUserMaster.Guid);

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
                                ConsoleLogger.Warn("ObjectRead null response from proxy REST request to " + urls.Count + " URLs");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectRead server response to proxy REST request: " + proxyResponse.StatusCode);
                            return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                        #endregion

                        case "redirect":
                            #region redirect

                            redirectUrl = Obj.BuildRedirectUrl(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                            if(String.IsNullOrEmpty(redirectUrl)) {
                                ConsoleLogger.Warn("ObjectRead unable to generate redirect_url, returning 500");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to build redirect URL.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectRead redirecting request using status " + CurrentSettings.Redirection.ReadRedirectHttpStatus + " for user GUID " + md.CurrentUserMaster.Guid);
                            Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                            redirectHeader.Add("location", redirectUrl);
                            return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.ReadRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.ReadRedirectString, true);

                        #endregion

                        default:
                            #region unknown

                            ConsoleLogger.Warn("ObjectRead unknown ReadRedirectionMode in redirection settings: " + CurrentSettings.Redirection.ReadRedirectionMode);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                            #endregion
                    }

                    #endregion
                }

                #endregion

                #endregion
            }
            finally {
                #region finally

                #region unlock

                if(locked) {
                    if(!LockManager.RemoveReadResource(md.CurrentObj.DiskPath)) {
                        ConsoleLogger.Warn("ObjectRead unable to unlock " + md.CurrentObj.DiskPath);
                    }
                }

                #endregion

                #endregion
            }
        }

        public HttpResponse ObjectRename(RequestMetadata md) {
            DateTime startTime = DateTime.Now;
            bool lockedOriginal = false;
            bool lockedTarget = false;
            string diskPathOriginal = "";
            string diskPathTarget = "";

            try {
                #region try

                #region Variables

                RenameRequest req = new RenameRequest();
                RestResponse proxyResponse = new RestResponse();
                Dictionary<string, string> restHeaders = new Dictionary<string, string>();
                bool userGatewayMode = false;
                Obj currObj = new Obj();
                List<Node> successfulReplicas = new List<Node>();
                string containerLogFile = "";
                string containerPropertiesFile = "";
                ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();
                string objectLogFile = "";
                string objectPropertiesFile = "";
                ObjectPropertiesFile currObjectPropertiesFile = new ObjectPropertiesFile();

                #endregion

                #region Check-Permissions

                if(md.CurrentApiKeyPermission == null) {
                    ConsoleLogger.Warn("ObjectRename null ApiKeyPermission object supplied");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowWriteObject)) {
                    ConsoleLogger.Warn("ObjectRename AllowWriteObject operation not authorized per permissions");
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
                        if(currContainerPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.CreateObject)) {
                                    #region Process-Logging

                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteObject-Rename", null));

                                    #endregion
                                }
                            }
                        }

                        if(currContainerPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ContainerPermission.GetPermission("WriteObject", md, currContainerPropertiesFile)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteObject-Rename", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectRename AllowWriteObject operation not authorized per container permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Check-Object-Permissions-and-Logging

                    currObjectPropertiesFile = ObjectPropertiesFile.FromObject(md.CurrentObj, out objectLogFile, out objectPropertiesFile);
                    if(currObjectPropertiesFile != null) {
                        if(currObjectPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.CreateObject)) {
                                    #region Process-Logging

                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectRename", null));

                                    #endregion
                                }
                            }
                        }

                        if(currObjectPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ObjectPermission.GetPermission("WriteObject", md, currObjectPropertiesFile)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectRename", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectRename AllowWriteObject operation not authorized per object permissions");
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
                            ConsoleLogger.Warn("ObjectRename null request after deserialization, returning 400");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                        }
                    }
                    catch(Exception) {
                        ConsoleLogger.Warn("ObjectRename unable to deserialize request body");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }

                    req.UserGuid = String.Copy(md.CurrentUserMaster.Guid);

                    #endregion

                    #region Validate-Request-Body

                    if(req.ContainerPath == null) {
                        req.ContainerPath = new List<string>();
                    }

                    if(String.IsNullOrEmpty(req.RenameFrom)) {
                        ConsoleLogger.Warn("ObjectRename null value supplied for RenameFrom, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for RenameFrom.", null).ToJson(), true);
                    }

                    if(String.IsNullOrEmpty(req.RenameTo)) {
                        ConsoleLogger.Warn("ObjectRename null value supplied for RenameTo, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Invalid value for RenameTo.", null).ToJson(), true);
                    }

                    if(RenameRequest.UnsafeFsChars(req)) {
                        ConsoleLogger.Warn("ObjectRename unsafe characters detected in request, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unsafe characters detected.", null).ToJson(), true);
                    }

                    req.UserGuid = md.CurrentUserMaster.Guid;

                    #endregion

                    #region Check-if-Original-Exists

                    diskPathOriginal = "/" + string.Join("/", req.ContainerPath) + "/" + req.RenameFrom;
                    //diskPathOriginal = RenameRequest.BuildDiskPath(req, true, Users, CurrentSettings, Logging);
                    if(String.IsNullOrEmpty(diskPathOriginal)) {
                        ConsoleLogger.Warn("ObjectRename unable to build disk path for original object");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                    }

                    if(!Common.FileExists(diskPathOriginal)) {
                        ConsoleLogger.Warn("ObjectRename from object does not exist: " + diskPathOriginal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                            new ErrorResponse(5, 404, "Object does not exist.", null).ToJson(), true);
                    }

                    #endregion

                    #region Lock-Original

                    lockedOriginal = LockManager.LockResource(md, diskPathOriginal);
                    if(!lockedOriginal) {
                        ConsoleLogger.Warn("ObjectRename source object " + diskPathOriginal + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Check-if-Target-Exists

                    diskPathTarget = "/" + string.Join("/", req.ContainerPath) + "/" + req.RenameTo;
                    //diskPathTarget = RenameRequest.BuildDiskPath(req, false, Users, CurrentSettings, Logging);
                    if(String.IsNullOrEmpty(diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectRename unable to build disk path for target object");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                    }

                    if(Common.FileExists(diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectRename target object already exists: " + diskPathOriginal);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Object already exists.", null).ToJson(), true);
                    }

                    #endregion

                    #region Lock-Target

                    lockedTarget = LockManager.LockResource(md, diskPathTarget);
                    if(!lockedTarget) {
                        ConsoleLogger.Warn("ObjectRename target object " + diskPathTarget + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Set-Gateway-Mode

                    userGatewayMode = md.CurrentUserMaster.GetGatewayMode(CurrentSettings);

                    #endregion

                    #region Read-Original-Object

                    currObj = Obj.BuildObjFromDisk(diskPathOriginal, Users, CurrentSettings, CurrentTopology, CurrentNode);
                    if(currObj == null) {
                        ConsoleLogger.Warn("ObjectRename unable to retrieve obj for " + md.CurrentObj.DiskPath);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to read object.", null).ToJson(), true);
                    }

                    #endregion

                    #region Process-Replication

                    ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                    if(!rh.ObjectRename(req, currObj)) {
                        ConsoleLogger.Warn("ObjectRename negative response from replication, returning 500");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                    }

                    BunkerHandler bh = new BunkerHandler(CurrentSettings);
                    bh.ObjectRename(req);

                    #endregion

                    #region Perform-Rename

                    if(!Common.RenameFile(diskPathOriginal, diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectRename unable to rename file from " + diskPathOriginal + " to " + diskPathTarget);
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Object already exists.", null).ToJson(), true);
                    }

                    #endregion

                    #region Perform-Background-Rewrite

                    if(!userGatewayMode) {
                        ConsoleLogger.Warn("ObjectRename spawning background task to rewrite object with correct metadata");
                        Task.Run(() => RewriteObject(diskPathTarget));
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

                            ConsoleLogger.Warn("ObjectRename object is destined for a different machine but WriteRedirectionMode is none");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                        #endregion

                        case "proxy":
                            #region proxy

                            ConsoleLogger.Warn("ObjectRename proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs);

                            proxyResponse = RestRequest.SendRequestSafe(
                                md.CurrentObj.PrimaryUrlWithQs,
                                md.CurrentHttpRequest.ContentType,
                                md.CurrentHttpRequest.Method,
                                null, null, false,
                                Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                                GetCustomHeaders(md.CurrentHttpRequest.Headers),
                                md.CurrentHttpRequest.Data);

                            if(proxyResponse == null) {
                                ConsoleLogger.Warn("ObjectRename null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectRename server response to proxy REST request: " + proxyResponse.StatusCode);
                            return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                        #endregion

                        case "redirect":
                            #region redirect

                            ConsoleLogger.Warn("ObjectRename redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs + " using status " + CurrentSettings.Redirection.WriteRedirectHttpStatus);
                            Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                            redirectHeader.Add("location", md.CurrentObj.PrimaryUrlWithQs);
                            return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.WriteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.WriteRedirectString, true);

                        #endregion

                        default:
                            #region unknown

                            ConsoleLogger.Warn("ObjectRename unknown WriteRedirectionMode in redirection settings: " + CurrentSettings.Redirection.WriteRedirectionMode);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                            #endregion
                    }

                    #endregion
                }

                #endregion

                #endregion
            }
            finally {
                #region finally

                #region unlock

                if(lockedOriginal) {
                    if(!LockManager.UnlockResource(md, diskPathOriginal)) {
                        ConsoleLogger.Warn("ObjectRename unable to unlock source path " + diskPathOriginal);
                    }
                }

                if(lockedTarget) {
                    if(!LockManager.UnlockResource(md, diskPathTarget)) {
                        ConsoleLogger.Warn("ObjectRename unable to unlock target path " + diskPathTarget);
                    }
                }

                #endregion

                #endregion
            }
        }

        public HttpResponse ObjectSearch(RequestMetadata md) {
            #region Variables

            string maxResultsStr = "";
            int maxResults = 0;
            string metadataVal = "";
            bool metadataOnly = false;
            Find req = new Find();
            string diskPath = "";
            RestResponse proxyResponse = new RestResponse();
            Dictionary<string, string> restHeaders = new Dictionary<string, string>();
            DirInfo ret = new DirInfo();
            List<DirInfo> retList = new List<DirInfo>();
            List<string> subdirList = new List<string>();
            List<string> urls = new List<string>();
            string redirectUrl = "";
            string proxiedVal = "";
            bool proxied = false;

            #endregion

            #region Check-Permissions

            if(md.CurrentApiKeyPermission == null) {
                ConsoleLogger.Warn("ObjectSearch null ApiKeyPermission object supplied");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowSearch)) {
                ConsoleLogger.Warn("ObjectSearch allow_search operation not authorized per permissions");
                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
            }

            #endregion

            #region Get-Values-from-Querystring

            maxResultsStr = md.CurrentHttpRequest.RetrieveHeaderValue("max_results");
            if(!String.IsNullOrEmpty(maxResultsStr)) {
                if(!Int32.TryParse(maxResultsStr, out maxResults)) {
                    ConsoleLogger.Warn("ObjectSearch invalid value for max_results in querystring: " + maxResultsStr);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Invalid value for max_results.", null).ToJson(), true);
                }
            }
            else {
                maxResults = 1;
            }

            metadataVal = md.CurrentHttpRequest.RetrieveHeaderValue("metadata_only");
            if(!String.IsNullOrEmpty(metadataVal)) {
                metadataOnly = Common.IsTrue(metadataVal);
            }

            proxiedVal = md.CurrentHttpRequest.RetrieveHeaderValue("proxied");
            if(!String.IsNullOrEmpty(proxiedVal)) {
                proxied = Common.IsTrue(proxiedVal);
            }

            #endregion

            #region Process-Owner

            /*
                * Nodes that proxy requests will append ?proxied=true to the request URL
                * to notify the next recipient node to not proxy it further (otherwise
                * an infinite loop of REST requests will be generated)
                * 
                */

            if(proxied || (md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId)) {
                #region Local-Owner

                #region Deserialize

                try {
                    req = Common.DeserializeJson<Find>(md.CurrentHttpRequest.Data);
                    if(req == null) {
                        ConsoleLogger.Warn("ObjectSearch null request after deserialization, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                    }
                }
                catch(Exception) {
                    ConsoleLogger.Warn("ObjectSearch unable to deserialize request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(), true);
                }

                req.UserGuid = md.CurrentObj.UserGuid;

                #endregion

                #region Process-and-Return

                diskPath = Find.BuildDiskPath(req, Users, CurrentSettings);
                if(String.IsNullOrEmpty(diskPath)) {
                    ConsoleLogger.Warn("ObjectSearch unable to build disk path from request body");
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                }

                if(req.Recursive) {
                    #region Recursive

                    #region Build-and-Process-Subdirectory-List

                    if(!String.IsNullOrEmpty(req.Key))
                        subdirList = Common.GetSubdirectoryList(diskPath.Replace(req.Key, ""), true);
                    else
                        subdirList = Common.GetSubdirectoryList(diskPath, true);

                    if(subdirList == null)
                        subdirList = new List<string>();

                    if(!String.IsNullOrEmpty(req.Key))
                        subdirList.Add(diskPath.Replace(req.Key, ""));
                    else
                        subdirList.Add(diskPath);

                    int resultsCount = maxResults;
                    foreach(string currSubdir in subdirList) {
                        DirInfo di = new DirInfo(CurrentSettings, Users);
                        ret = di.FromDirectory(currSubdir, md.CurrentObj.UserGuid, resultsCount, req.Filters, metadataOnly);
                        if(ret == null) {
                            ConsoleLogger.Warn("ObjectSearch null directory info for " + currSubdir);
                            return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                                new ErrorResponse(5, 404, "Container does not exist.", null).ToJson(), true);
                        }

                        // do not add to list if no objects are present
                        if(ret.NumObjects <= 0)
                            continue;

                        ret.UserGuid = md.CurrentUserMaster.Guid;
                        ret.ContainerPath = di.GetContainerList(currSubdir, md.CurrentUserMaster.Guid);

                        maxResults -= ret.NumObjects;
                        retList.Add(ret);

                        if(maxResults <= 0) {
                            break;
                        }
                    }

                    #endregion

                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(retList), true);

                    #endregion
                }
                else {
                    #region Non-Recursive

                    DirInfo di = new DirInfo(CurrentSettings, Users);
                    ret = di.FromDirectory(
                        Find.BuildDiskPath(
                            req, Users, CurrentSettings),
                        md.CurrentObj.UserGuid, maxResults, req.Filters, metadataOnly);

                    if(ret == null) {
                        ConsoleLogger.Warn("ObjectSearch null directory info for container");
                        return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                            new ErrorResponse(5, 404, "Container does not exist.", null).ToJson(), true);
                    }

                    ret.UserGuid = md.CurrentUserMaster.Guid;
                    ret.ContainerPath = req.ContainerPath;

                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(ret), true);

                    #endregion
                }

                #endregion

                #endregion
            }
            else {
                #region Remote-Owner

                switch(CurrentSettings.Redirection.SearchRedirectionMode) {
                    case "none":
                        #region none

                        ConsoleLogger.Warn("ObjectSearch object is destined for a different machine but SearchRedirectionMode is none");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                    #endregion

                    case "proxy":
                        #region proxy

                        urls = Obj.BuildReplicaUrls(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        if(urls == null || urls.Count < 1) {
                            ConsoleLogger.Warn("ObjectSearch unable to build replica URL list (null response)");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to build proxy URL.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ObjectSearch proxying request to " + urls.Count + " URLs for user GUID " + md.CurrentUserMaster.Guid);

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
                            ConsoleLogger.Warn("ObjectSearch null response from proxy REST request to " + urls.Count + " URLs");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ObjectSearch server response to proxy REST request: " + proxyResponse.StatusCode);
                        return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                    #endregion

                    case "redirect":
                        #region redirect

                        redirectUrl = Obj.BuildRedirectUrl(true, md.CurrentHttpRequest, md.CurrentObj, CurrentTopology);
                        if(String.IsNullOrEmpty(redirectUrl)) {
                            ConsoleLogger.Warn("ObjectSearch unable to generate redirect URL, returning 500");
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to build redirect URL.", null).ToJson(), true);
                        }

                        ConsoleLogger.Warn("ObjectSearch redirecting request using status " + CurrentSettings.Redirection.ReadRedirectHttpStatus + " for user GUID " + md.CurrentUserMaster.Guid);
                        Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                        redirectHeader.Add("location", redirectUrl);
                        return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.ReadRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.ReadRedirectString, true);

                    #endregion

                    default:
                        #region unknown

                        ConsoleLogger.Warn("ObjectSearch unknown SearchRedirectionMode in redirection settings: " + CurrentSettings.Redirection.SearchRedirectionMode);
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                        #endregion
                }

                #endregion
            }

            #endregion
        }

        public HttpResponse ObjectWrite(RequestMetadata md) {
            DateTime startTime = DateTime.Now;
            bool locked = false;

            try {
                #region try

                #region Variables

                ObjInfo currObjInfo = new ObjInfo();
                Obj currObj = new Obj();
                string rangeWriteVal = "";
                bool rangeWrite = false;
                string writeToVal = "";
                int writeTo = 0;
                string overwriteVal = "";
                //bool overwrite = false;
                bool overwrite = true;
                string homeDirectory = "";
                byte[] clear;
                byte[] cipher;
                string ksn = "";
                string baseDir = "";
                int guidAttempts = 0;
                RestResponse proxyResponse = new RestResponse();
                Dictionary<string, string> restHeaders = new Dictionary<string, string>();
                int expirationSeconds = 0;
                string expirationFilename = "";
                Obj expObj;
                bool localSuccess = false;
                List<Node> successfulReplicas = new List<Node>();
                string containerLogFile = "";
                string containerPropertiesFile = "";
                ContainerPropertiesFile currContainerPropertiesFile = new ContainerPropertiesFile();
                string objectLogFile = "";
                string objectPropertiesFile = "";
                ObjectPropertiesFile currObjectPropertiesFile = new ObjectPropertiesFile();

                #endregion

                #region Check-Permissions

                if(md.CurrentApiKeyPermission == null) {
                    ConsoleLogger.Warn("ObjectWrite null ApiKeyPermission object supplied");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                if(!Common.IsTrue(md.CurrentApiKeyPermission.AllowWriteObject)) {
                    ConsoleLogger.Warn("ObjectWrite AllowWriteObject operation not authorized per permissions");
                    return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                }

                #endregion

                #region Check-Size

                if(md.CurrentObj.Value == null || md.CurrentObj.Value.Length < 1) {
                    ConsoleLogger.Warn("ObjectWrite null object value detected");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "No request body.", null).ToJson(), true);
                }

                if(md.CurrentObj.Value.Length > CurrentSettings.Storage.MaxObjectSize) {
                    ConsoleLogger.Warn("ObjectWrite object size of " + md.CurrentObj.Value.Length + " exceeds configured max_object_size of " + CurrentSettings.Storage.MaxObjectSize);
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Request body is too large.", null).ToJson(), true);
                }

                #endregion

                #region Retrieve-User-Home-Directory

                homeDirectory = Users.GetHomeDirectory(md.CurrentUserMaster.Guid, CurrentSettings);
                if(String.IsNullOrEmpty(homeDirectory)) {
                    ConsoleLogger.Warn("ObjectWrite unable to retrieve home directory for user GUID " + md.CurrentUserMaster.Guid);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to find home directory for user.", null).ToJson(), true);
                }

                #endregion

                #region Process-Owner

                if(md.CurrentObj.PrimaryNode.NodeId == CurrentNode.NodeId) {
                    #region Local-Owner

                    #region Generate-New-Key-if-Needed

                    if(String.IsNullOrEmpty(md.CurrentObj.Key)) {
                        while(true) {
                            md.CurrentObj.Key = Guid.NewGuid().ToString();
                            if(!Common.FileExists(md.CurrentObj.DiskPath + md.CurrentObj.Key)) {
                                #region Amend-Path-Object-With-New-URL

                                md.CurrentObj.PrimaryUrlWithQs = Obj.BuildPrimaryUrl(true, md.CurrentHttpRequest, md.CurrentObj);
                                if(String.IsNullOrEmpty(md.CurrentObj.PrimaryUrlWithQs)) {
                                    ConsoleLogger.Warn("ObjectWrite unable to build primary URL for request (with querystring)");
                                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                        new ErrorResponse(4, 500, "Unable to build primary URL.", null).ToJson(), true);
                                }

                                md.CurrentObj.PrimaryUrlWithoutQs = Obj.BuildPrimaryUrl(false, md.CurrentHttpRequest, md.CurrentObj);
                                if(String.IsNullOrEmpty(md.CurrentObj.PrimaryUrlWithoutQs)) {
                                    ConsoleLogger.Warn("ObjectWrite unable to build primary URL for request (without querystring)");
                                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                        new ErrorResponse(4, 500, "Unable to build primary URL.", null).ToJson(), true);
                                }

                                md.CurrentObj.DiskPath = Obj.BuildDiskPath(md.CurrentObj, md.CurrentUserMaster, CurrentSettings);
                                if(String.IsNullOrEmpty(md.CurrentObj.DiskPath)) {
                                    ConsoleLogger.Warn("ObjectWrite unable to build disk path for request");
                                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                        new ErrorResponse(4, 500, "Unable to build disk path from request.", null).ToJson(), true);
                                }

                                ConsoleLogger.Warn("ObjectWrite overwriting path values (object had no key originally)");
                                ConsoleLogger.Warn("  Key         : " + md.CurrentObj.Key);
                                ConsoleLogger.Warn("  URL (no qs) : " + md.CurrentObj.PrimaryUrlWithoutQs);
                                ConsoleLogger.Warn("  URL (qs)    : " + md.CurrentObj.PrimaryUrlWithQs);
                                ConsoleLogger.Warn("  Disk Path   : " + md.CurrentObj.DiskPath);

                                #endregion

                                break;
                            }

                            guidAttempts++;
                            if(guidAttempts >= 8) {
                                ConsoleLogger.Warn("ObjectWrite cannot get a new GUID");
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to find unused GUID.", null).ToJson(), true);
                            }
                        }
                    }

                    #endregion

                    #region Add-Lock

                    locked = LockManager.LockUrl(md);
                    if(!locked) {
                        ConsoleLogger.Warn("ObjectWrite " + md.CurrentObj.DiskPath + " is unable to be locked");
                        return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                            new ErrorResponse(9, 423, "Resource in use.", null).ToJson(), true);
                    }

                    #endregion

                    #region Check-Container-Permissions-and-Logging

                    currContainerPropertiesFile = ContainerPropertiesFile.FromObject(md.CurrentObj, out containerLogFile, out containerPropertiesFile);
                    if(currContainerPropertiesFile != null) {
                        if(currContainerPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.CreateObject)) {
                                    #region Process-Logging

                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteObject", null));

                                    #endregion
                                }
                            }
                        }

                        if(currContainerPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ContainerPermission.GetPermission("WriteObject", md, currContainerPropertiesFile)) {
                                if(Common.IsTrue(currContainerPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(containerLogFile, LoggerManager.BuildMessage(md, "WriteObject", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectWrite AllowWriteObject operation not authorized per container permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Check-Object-Permissions-and-Logging

                    currObjectPropertiesFile = ObjectPropertiesFile.FromObject(md.CurrentObj, out objectLogFile, out objectPropertiesFile);
                    if(currObjectPropertiesFile != null) {
                        if(currObjectPropertiesFile.Logging != null) {
                            if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.CreateObject)) {
                                    #region Process-Logging

                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectWrite", null));

                                    #endregion
                                }
                            }
                        }

                        if(currObjectPropertiesFile.Permissions != null) {
                            #region Evaluate-Permissions

                            if(!ObjectPermission.GetPermission("WriteObject", md, currObjectPropertiesFile)) {
                                if(Common.IsTrue(currObjectPropertiesFile.Logging.Enabled)) {
                                    Logger.Add(objectLogFile, LoggerManager.BuildMessage(md, "ObjectWrite", "denied"));
                                }

                                ConsoleLogger.Warn("ObjectWrite AllowWriteObject operation not authorized per object permissions");
                                return new HttpResponse(md.CurrentHttpRequest, false, 401, null, "application/json",
                                    new ErrorResponse(3, 401, "Permission denied.", null).ToJson(), true);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    #region Validate-Request

                    if(Obj.UnsafeFsChars(md.CurrentObj)) {
                        ConsoleLogger.Warn("ObjectWrite unsafe characters detected in request, returning 400");
                        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unsafe characters detected.", null).ToJson(), true);
                    }

                    #endregion

                    #region Get-Values-from-Querystring

                    rangeWriteVal = md.CurrentHttpRequest.RetrieveHeaderValue("range_write");
                    if(!String.IsNullOrEmpty(rangeWriteVal)) {
                        rangeWrite = Common.IsTrue(rangeWriteVal);
                    }

                    writeToVal = md.CurrentHttpRequest.RetrieveHeaderValue("write_to");
                    if(!String.IsNullOrEmpty(writeToVal)) {
                        if(!Int32.TryParse(writeToVal, out writeTo)) {
                            ConsoleLogger.Warn("ObjectWrite invalid value for write_to in querystring: " + writeToVal);
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Invalid value for write_to.", null).ToJson(), true);
                        }

                        if(writeTo < 0) {
                            ConsoleLogger.Warn("ObjectWrite invalid value for write_to (must be zero or greater): " + writeTo);
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Invalid value for write_to.", null).ToJson(), true);
                        }
                    }

                    overwriteVal = md.CurrentHttpRequest.RetrieveHeaderValue("overwrite");
                    if(!String.IsNullOrEmpty(overwriteVal)) {
                        Boolean.TryParse(overwriteVal, out overwrite);
                    }

                    #endregion

                    #region Create-User-Folder-if-Needed

                    if(!Common.DirectoryExists(homeDirectory)) {
                        Common.CreateDirectory(homeDirectory);
                    }

                    #endregion

                    #region Create-Containers-if-Needed

                    baseDir = String.Copy(homeDirectory);

                    foreach(string currDirectory in md.CurrentObj.ContainerPath) {
                        baseDir += currDirectory;
                        //baseDir += Common.GetPathSeparator(CurrentSettings.Environment) + currDirectory;

                        if(!Common.DirectoryExists(baseDir)) {
                            ConsoleLogger.Warn("ObjectWrite directory " + baseDir + " does not exist, creating");
                            if(!Common.CreateDirectory(baseDir)) {
                                ConsoleLogger.Warn("ObjectWrite unable to create base directory " + baseDir);
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to write container.", null).ToJson(), true);
                            }
                        }
                    }

                    #endregion

                    #region Set-Content-Type-if-Needed

                    if(String.IsNullOrEmpty(md.CurrentObj.ContentType)) {
                        md.CurrentObj.ContentType = MimeTypes.GetFromExtension(Common.GetFileExtension(md.CurrentObj.DiskPath));
                    }

                    #endregion

                    #region Handle-Range-Write-and-Existing-File

                    if(rangeWrite) {
                        #region Update-Existing-File

                        #region Ensure-Body-Present

                        if(md.CurrentHttpRequest.Data.Length < 1) {
                            ConsoleLogger.Warn("ObjectWrite null request body supplied for range write");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request body not found.", null).ToJson(), true);
                        }

                        #endregion

                        #region Retrieve-Object-Metadata

                        currObjInfo = ObjInfo.FromFile(md.CurrentObj.DiskPath);
                        if(currObjInfo == null) {
                            // ConsoleLogger.Warn("ObjectWrite null file info returned for " + md.currObj.disk path);
                            return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                                new ErrorResponse(5, 404, "Object does not exist.", null).ToJson(), true);
                        }

                        #endregion

                        #region Read-Original

                        currObj = Obj.BuildObjFromDisk(md.CurrentObj.DiskPath, Users, CurrentSettings, CurrentTopology, CurrentNode);
                        if(currObj == null) {
                            ConsoleLogger.Warn("ObjectWrite unable to retrieve obj for " + md.CurrentObj.DiskPath);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to read object.", null).ToJson(), true);
                        }

                        md.CurrentObj = currObj;

                        #endregion

                        #region Decrypt

                        if(Common.IsTrue(md.CurrentObj.IsEncrypted)) {
                            if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption))
                                ConsoleLogger.Warn("ObjectWrite before decryption: " + Common.BytesToBase64(md.CurrentObj.Value));

                            if(String.IsNullOrEmpty(md.CurrentObj.EncryptionKsn)) {
                                md.CurrentObj.Value = EncryptionManager.LocalDecrypt(md.CurrentObj.Value);
                            }
                            else {
                                if(!EncryptionManager.ServerDecrypt(md.CurrentObj.Value, md.CurrentObj.EncryptionKsn, out clear)) {
                                    ConsoleLogger.Warn("ObjectWrite unable to decrypt object using server-based decryption");
                                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                        new ErrorResponse(4, 500, "Unable to decrypt object using crypto server.", null).ToJson(), true);
                                }

                                md.CurrentObj.Value = clear;
                            }

                            if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption))
                                ConsoleLogger.Warn("ObjectWrite after decryption: " + Common.BytesToBase64(md.CurrentObj.Value));
                        }

                        #endregion

                        #region Decompress

                        if(Common.IsTrue(md.CurrentObj.IsCompressed)) {
                            if(Common.IsTrue(CurrentSettings.Debug.DebugCompression))
                                ConsoleLogger.Warn("ObjectWrite before decompression: " + Common.BytesToBase64(md.CurrentObj.Value));
                            md.CurrentObj.Value = Common.GzipDecompress(md.CurrentObj.Value);
                            if(Common.IsTrue(CurrentSettings.Debug.DebugCompression))
                                ConsoleLogger.Warn("ObjectWrite after decompression: " + Common.BytesToBase64(md.CurrentObj.Value));
                        }

                        #endregion

                        #region Set-Content-Type

                        if(String.IsNullOrEmpty(md.CurrentObj.ContentType)) {
                            md.CurrentObj.ContentType = MimeTypes.GetFromExtension(Common.GetFileExtension(md.CurrentObj.DiskPath));
                        }

                        #endregion

                        #region Check-if-in-Range

                        if(writeTo < md.CurrentObj.Value.Length) {
                            if(writeTo + md.CurrentHttpRequest.Data.Length > md.CurrentObj.Value.Length) {
                                #region Replace-then-Append

                                // copy original from obj.value to a bigger byte array
                                byte[] newObjBytes = new byte[(writeTo + md.CurrentHttpRequest.Data.Length)];
                                Buffer.BlockCopy(md.CurrentObj.Value, 0, newObjBytes, 0, md.CurrentObj.Value.Length);

                                // add new data from http rqeuest
                                Buffer.BlockCopy(md.CurrentHttpRequest.Data, 0, newObjBytes, writeTo, md.CurrentHttpRequest.Data.Length);

                                // copy from bigger byte array back to obj.value
                                md.CurrentObj.Value = new byte[newObjBytes.Length];
                                Buffer.BlockCopy(newObjBytes, 0, md.CurrentObj.Value, 0, newObjBytes.Length);

                                #endregion
                            }
                            else {
                                #region Replace

                                // in-place replacement
                                Buffer.BlockCopy(md.CurrentHttpRequest.Data, 0, md.CurrentObj.Value, writeTo, md.CurrentHttpRequest.Data.Length);

                                #endregion
                            }
                        }
                        else if(writeTo >= md.CurrentObj.Value.Length) {
                            #region Append

                            // copy original from obj.value to a bigger byte array
                            byte[] newObjBytes = new byte[(writeTo + md.CurrentHttpRequest.Data.Length)];
                            Buffer.BlockCopy(md.CurrentObj.Value, 0, newObjBytes, 0, md.CurrentObj.Value.Length);

                            // add new data from http rqeuest
                            Buffer.BlockCopy(md.CurrentHttpRequest.Data, 0, newObjBytes, writeTo, md.CurrentHttpRequest.Data.Length);

                            // copy from bigger byte array back to obj.value
                            md.CurrentObj.Value = new byte[newObjBytes.Length];
                            Buffer.BlockCopy(newObjBytes, 0, md.CurrentObj.Value, 0, newObjBytes.Length);

                            #endregion
                        }

                        #endregion

                        #endregion
                    }
                    else {
                        #region Check-for-Existing-File

                        //if(!overwrite) {
                        //    if(Common.FileExists(md.CurrentObj.DiskPath)) {
                        //        ConsoleLogger.Warn( "ObjectWrite file already exists at " + md.CurrentObj.DiskPath);
                        //        return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        //            new ErrorResponse(2, 400, "Object already exists.", null).ToJson(), true);
                        //    }
                        //}

                        #endregion
                    }

                    #endregion

                    #region Write-Expiration-Object

                    expirationSeconds = md.CurrentUserMaster.GetExpirationSeconds(CurrentSettings, md.CurrentApiKey);
                    if(expirationSeconds > 0) {
                        expObj = Common.CopyObject<Obj>(md.CurrentObj);
                        expObj.Value = null;
                        expObj.Expiration = DateTime.Now.AddSeconds(expirationSeconds);
                        md.CurrentObj.Expiration = expObj.Expiration;

                        expirationFilename =
                            Convert.ToDateTime(expObj.Expiration).ToString("MMddyyyy-hhmmss") +
                            "-" + Common.RandomString(8) + "-" + expObj.Key;

                        if(!Common.WriteFile(CurrentSettings.Expiration.Directory + expirationFilename, Common.SerializeJson(expObj), false)) {
                            ConsoleLogger.Warn("ObjectWrite unable to create expiration object " + expirationFilename);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to create expiration object.", null).ToJson(), true);
                        }
                    }

                    #endregion

                    #region Compress

                    if(!Common.IsTrue(md.CurrentObj.GatewayMode)) {
                        if(Common.IsTrue(md.CurrentObj.IsCompressed)) {
                            if(Common.IsTrue(CurrentSettings.Debug.DebugCompression))
                                ConsoleLogger.Warn("ObjectWrite before compression: " + Common.BytesToBase64(md.CurrentObj.Value));
                            md.CurrentObj.Value = Common.GzipCompress(md.CurrentObj.Value);
                            if(Common.IsTrue(CurrentSettings.Debug.DebugCompression))
                                ConsoleLogger.Warn("ObjectWrite after compression: " + Common.BytesToBase64(md.CurrentObj.Value));
                        }
                    }
                    else {
                        md.CurrentObj.IsCompressed = 0;
                    }

                    #endregion

                    #region Encrypt

                    if(!Common.IsTrue(md.CurrentObj.GatewayMode)) {
                        if(Common.IsTrue(md.CurrentObj.IsEncrypted)) {
                            if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption))
                                ConsoleLogger.Warn("ObjectWrite before encryption: " + Common.BytesToBase64(md.CurrentObj.Value));

                            switch(CurrentSettings.Encryption.Mode) {
                                case "local":
                                    #region local

                                    md.CurrentObj.Value = EncryptionManager.LocalEncrypt(md.CurrentObj.Value);
                                    break;

                                #endregion

                                case "server":
                                    #region server

                                    if(!EncryptionManager.ServerEncrypt(md.CurrentObj.Value, out cipher, out ksn)) {
                                        ConsoleLogger.Warn("ObjectWrite unable to encrypt object using server-based encryption");
                                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                            new ErrorResponse(4, 500, "Unable to encrypt object using crypto server.", null).ToJson(), true);
                                    }

                                    md.CurrentObj.Value = cipher;
                                    md.CurrentObj.EncryptionKsn = ksn;
                                    break;

                                #endregion

                                default:
                                    #region default

                                    ConsoleLogger.Warn("ObjectWrite unknown encryption mode: " + CurrentSettings.Encryption.Mode);
                                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                        new ErrorResponse(4, 500, "Server has incorrect encryption configuration.", null).ToJson(), true);

                                    #endregion
                            }

                            if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption))
                                ConsoleLogger.Warn("ObjectWrite after encryption: " + Common.BytesToBase64(md.CurrentObj.Value));
                        }
                    }
                    else {
                        md.CurrentObj.IsEncrypted = 0;
                    }

                    #endregion

                    #region Process-Replication

                    ReplicationHandler rh = new ReplicationHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                    if(!rh.ObjectWrite(md.CurrentObj, out successfulReplicas)) {
                        ConsoleLogger.Warn("ObjectWrite negative response from replication, returning 500");
                        return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                            new ErrorResponse(4, 500, "Unable to process replication.", null).ToJson(), true);
                    }

                    BunkerHandler bh = new BunkerHandler(CurrentSettings);
                    bh.ObjectWrite(md.CurrentObj);

                    #endregion

                    #region Write-Locally

                    if(!Common.IsTrue(md.CurrentObj.GatewayMode)) {
                        //non è gateway
                        localSuccess = Common.WriteFile(md.CurrentObj.DiskPath, Common.SerializeJson(md.CurrentObj), false);
                        if(!localSuccess) {
                            ConsoleLogger.Warn("ObjectWrite unable to write object to " + md.CurrentUserMaster.Guid + " " + md.CurrentObj.Key);
                            Task.Run(() => {
                                rh.ObjectDelete(md.CurrentObj, successfulReplicas);
                            });

                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to write object.", null).ToJson(), true);
                        }
                    }
                    else {
                        //    è gateway
                        var realDestinationPath = "/" + string.Join("/", md.CurrentObj.ContainerPath) + "/" + md.CurrentObj.Key;
                        var tmpDestinationPath = $"{realDestinationPath}.tmp";
                        localSuccess = Common.WriteFile(tmpDestinationPath, md.CurrentObj.Value);
                        if(!localSuccess) {
                            ConsoleLogger.Warn("ObjectWrite unable to write raw bytes to " + md.CurrentUserMaster.Guid + " " + md.CurrentObj.Key);
                            Task.Run(() => {
                                rh.ObjectDelete(md.CurrentObj, successfulReplicas);
                            });

                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to write object.", null).ToJson(), true);
                        }
                        var syncSuccess = anthilla.sync.Tsync.SyncFile(tmpDestinationPath, realDestinationPath);
                        Common.DeleteFile(tmpDestinationPath);
                        if(!localSuccess) {
                            ConsoleLogger.Warn("ObjectWrite unable to sync data to " + md.CurrentUserMaster.Guid + " " + md.CurrentObj.Key);
                            Task.Run(() => {
                                rh.ObjectDelete(md.CurrentObj, successfulReplicas);
                            });

                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Unable to sync object.", null).ToJson(), true);
                        }
                    }

                    ConsoleLogger.Warn("ObjectWrite successfully wrote " + md.CurrentObj.Key + " for " + md.CurrentUserMaster.Guid);

                    #endregion

                    return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "text/plain", md.CurrentObj.PrimaryUrlWithoutQs, true);

                    #endregion
                }
                else {
                    #region Remote-Owner

                    switch(CurrentSettings.Redirection.WriteRedirectionMode) {
                        case "none":
                            #region none

                            ConsoleLogger.Warn("ObjectWrite object is destined for a different machine but WriteRedirectionMode is none");
                            return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                                new ErrorResponse(2, 400, "Request proxying disabled by configuration.  Please direct this request to the appropriate node.", null).ToJson(), true);

                        #endregion

                        case "proxy":
                            #region proxy

                            ConsoleLogger.Warn("ObjectWrite proxying request to " + md.CurrentObj.PrimaryUrlWithoutQs + " for object key " + md.CurrentObj.Key);

                            proxyResponse = RestRequest.SendRequestSafe(
                                md.CurrentObj.PrimaryUrlWithQs,
                                md.CurrentHttpRequest.ContentType,
                                md.CurrentHttpRequest.Method,
                                null, null, false,
                                Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                                GetCustomHeaders(md.CurrentHttpRequest.Headers),
                                md.CurrentHttpRequest.Data);

                            if(proxyResponse == null) {
                                ConsoleLogger.Warn("ObjectWrite null response from proxy REST request to " + md.CurrentObj.PrimaryUrlWithoutQs);
                                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                    new ErrorResponse(4, 500, "Unable to communicate with the appropriate node for this request.", null).ToJson(), true);
                            }

                            ConsoleLogger.Warn("ObjectWrite server response to proxy REST request: " + proxyResponse.StatusCode);
                            return new HttpResponse(md.CurrentHttpRequest, true, proxyResponse.StatusCode, proxyResponse.Headers, null, proxyResponse.Data, true);

                        #endregion

                        case "redirect":
                            #region redirect

                            ConsoleLogger.Warn("ObjectWrite redirecting request to " + md.CurrentObj.PrimaryUrlWithoutQs +
                                " using status " + CurrentSettings.Redirection.WriteRedirectHttpStatus + " for object key " + md.CurrentObj.Key);
                            Dictionary<string, string> redirectHeader = new Dictionary<string, string>();
                            redirectHeader.Add("location", md.CurrentObj.PrimaryUrlWithQs);
                            return new HttpResponse(md.CurrentHttpRequest, true, CurrentSettings.Redirection.WriteRedirectHttpStatus, redirectHeader, null, CurrentSettings.Redirection.WriteRedirectString, true);

                        #endregion

                        default:
                            #region unknown

                            ConsoleLogger.Warn("ObjectWrite unknown WriteRedirectionMode in redirection settings: " + CurrentSettings.Redirection.WriteRedirectionMode);
                            return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                                new ErrorResponse(4, 500, "Server has incorrect proxy configuration.", null).ToJson(), true);

                            #endregion
                    }

                    #endregion
                }

                #endregion

                #endregion
            }
            catch(IOException ioeOuter) {
                #region disk-full

                if(
                    ((ioeOuter.HResult & 0xFFFF) == 0x27)
                    || ((ioeOuter.HResult & 0xFFFF) == 0x70)
                   ) {
                    ConsoleLogger.Warn( "ObjectWrite disk full detected during write operation for " + md.CurrentUserMaster.Guid + " " + md.CurrentObj.Key);
                    return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                        new ErrorResponse(1, 500, "Disk is full.", null).ToJson(), true);
                }

                #endregion

                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json", null, true);
            }
            finally {
                #region finally

                #region unlock

                if(locked) {
                    if(!LockManager.UnlockUrl(md)) {
                        ConsoleLogger.Warn("ObjectWrite unable to unlock " + md.CurrentObj.DiskPath);
                    }
                }

                #endregion

                #endregion
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
