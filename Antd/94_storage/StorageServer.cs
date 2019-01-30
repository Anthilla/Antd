using anthilla.core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {

        public StorageServer(Settings settings) {
            #region Initialize-Settings
            CurrentSettings = settings;
            #endregion
        }

        public void Start() {
            #region Create Working Directories
            Common.CreateDirectory(CurrentSettings.Storage.Directory);
            Common.CreateDirectory(CurrentSettings.Storage.Directory + "/default");
            Common.CreateDirectory(CurrentSettings.Messages.Directory);
            Common.CreateDirectory(CurrentSettings.Expiration.Directory);
            Common.CreateDirectory(CurrentSettings.Replication.Directory);
            Common.CreateDirectory(CurrentSettings.Bunker.Directory);
            Common.CreateDirectory(CurrentSettings.PublicObj.Directory);
            Common.CreateDirectory(CurrentSettings.Tasks.Directory);
            #endregion

            //#region Verify-Storage-Directory-Access
            //if(!Common.VerifyDirectoryAccess(CurrentSettings.Environment, CurrentSettings.Storage.Directory)) {
            //    Common.ExitApplication("StorageServer", "Unable to verify storage directory access", -1);
            //    return;
            //}
            //#endregion

            #region Initialize-Global-Variables
            Users = new UserManager(UserMaster.FromFile(CurrentSettings.Files.UserMaster));
            ApiKeys = new ApiKeyManager(ApiKey.FromFile(CurrentSettings.Files.ApiKey), ApiKeyPermission.FromFile(CurrentSettings.Files.Permission));
            CurrentTopology = Topology.FromFile(CurrentSettings.Files.Topology);
            ConnManager = new ConnectionManager();
            EncryptionManager = new EncryptionModule(CurrentSettings);
            LockManager = new UrlLockManager();
            Logger = new LoggerManager(CurrentSettings);
            Maintenance = new MaintenanceManager();
            #endregion

            #region Verify-Topology
            if(!CurrentTopology.ValidateTopology(out CurrentNode)) {
                Common.ExitApplication("StorageServer", "Topology errors detected", -1);
                return;
            }

            CurrentTopology.PopulateReplicas(CurrentNode);
            ConsoleLogger.Log("Populated " + CurrentTopology.Replicas.Count + " replica nodes in topology");
            #endregion

            #region Check-for-HTTP-Listener
            if(!HttpListener.IsSupported) {
                Common.ExitApplication("StorageServer", "Your OS does not support HttpListener", -1);
                return;
            }
            #endregion

            #region Start-Threads
            new PublicObjThread(CurrentSettings);
            new FailedRequestsThread(CurrentSettings, FailedRequests);
            new PeerManagerThread(CurrentSettings, CurrentTopology, CurrentNode);
            new MessengerThread(CurrentSettings, CurrentTopology, CurrentNode);
            new ExpirationThread(CurrentSettings);
            new ReplicationThread(CurrentSettings, CurrentTopology, CurrentNode);
            new TasksThread(CurrentSettings);
            #endregion

            #region Start-Server
            Server watson = new Server(CurrentNode.DnsHostname, CurrentNode.Port, Common.IsTrue(CurrentNode.Ssl), RequestReceived, true) {
                //DebugRestRequests = false,
                //DebugRestResponses = false,
                //ConsoleLogging = false
            };
            #endregion

            #region Wait-for-Server-Thread
            EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, Guid.NewGuid().ToString());
            bool waitHandleSignal = false;
            do {
                waitHandleSignal = waitHandle.WaitOne(1000);
            } while(!waitHandleSignal);

            ConsoleLogger.Warn("StorageServer exiting");
            #endregion
        }

        public HttpResponse RequestReceived(HttpRequest req) {
            try {
                #region Variables
                DateTime startTime = DateTime.Now;
                RequestMetadata md = new RequestMetadata();

                string apiKey = "";
                string email = "";
                string password = "";
                string token = "";
                string version = "";

                UserMaster currUserMaster = null;
                ApiKey currApiKey = null;
                ApiKeyPermission currApiKeyPermission = null;

                md.CurrentHttpRequest = req;
                md.CurrentNode = CurrentNode;

                if(Common.IsTrue(CurrentSettings.Syslog.LogHttpRequests)) {
                    ConsoleLogger.Warn("RequestReceived request received: " + Environment.NewLine + md.CurrentHttpRequest.ToString());
                }
                #endregion

                #region Options-Handler
                if(req.Method.ToLower().Trim().Contains("option")) {
                    ConsoleLogger.Warn("RequestReceived " + Thread.CurrentThread.ManagedThreadId + ": OPTIONS request received");
                    return OptionsHandler(req);
                }
                #endregion

                #region Favicon-Robots-Root
                if(req.RawUrlEntries != null && req.RawUrlEntries.Count > 0) {
                    if(String.Compare(req.RawUrlEntries[0].ToLower(), "favicon.ico") == 0) {
                        return new HttpResponse(req, true, 200, null, null, null, true);
                    }
                }

                if(req.RawUrlEntries != null && req.RawUrlEntries.Count > 0) {
                    if(String.Compare(req.RawUrlEntries[0].ToLower(), "robots.txt") == 0) {
                        return new HttpResponse(req, true, 200, null, "text/plain", "User-Agent: *\r\nDisallow:\r\n", true);
                    }
                }

                if(req.RawUrlEntries == null || req.RawUrlEntries.Count == 0) {
                    ConsoleLogger.Log("RequestReceived null raw URL list detected, redirecting to documentation page");
                    return new HttpResponse(req, true, 301,
                        Common.AddToDictionary("location", CurrentSettings.DocumentationUrl, null),
                        "text/plain",
                        "Moved Permanently",
                        true);
                }
                #endregion

                #region Add-Connection
                ConnManager.Add(Thread.CurrentThread.ManagedThreadId, req);
                #endregion

                #region Unauthenticated-API

                switch(req.Method.ToLower()) {
                    case "get":
                        #region get

                        #region loopback

                        if(WatsonCommon.UrlEqual(req.RawUrlWithoutQuery, "/loopback", false)) {
                            return new HttpResponse(req, true, 200, null, "text/plain", "Hello from kvpbase!", true);
                        }

                        #endregion

                        #region public

                        if(req.RawUrlWithoutQuery.StartsWith("/public/")) {
                            return StorageServer.GetPublicObject(md);
                        }

                        #endregion

                        #region version

                        if(WatsonCommon.UrlEqual(req.RawUrlWithoutQuery, "/version", false)) {
                            return new HttpResponse(req, true, 200, null, "text/plain", CurrentSettings.ProductVersion, true);
                        }

                        #endregion

                        break;

                    #endregion

                    case "put":
                        break;

                    case "post":
                        break;

                    case "delete":
                        break;

                    default:
                        break;
                }

                #endregion

                #region Retrieve-Auth-Parameters

                apiKey = req.RetrieveHeaderValue(CurrentSettings.Server.HeaderApiKey);
                email = req.RetrieveHeaderValue(CurrentSettings.Server.HeaderEmail);
                password = req.RetrieveHeaderValue(CurrentSettings.Server.HeaderPassword);
                token = req.RetrieveHeaderValue(CurrentSettings.Server.HeaderToken);
                version = req.RetrieveHeaderValue(CurrentSettings.Server.HeaderVersion);

                #endregion

                #region Admin-API

                if(req.RawUrlEntries != null && req.RawUrlEntries.Count > 0) {
                    if(String.Compare(req.RawUrlEntries[0], "admin") == 0) {
                        if(String.IsNullOrEmpty(apiKey)) {
                            ConsoleLogger.Warn("RequestReceived admin API requested but no API key specified");
                            return new HttpResponse(req, false, 401, null, "application/json",
                                new ErrorResponse(3, 401, "No API key specified.", null).ToJson(),
                                true);
                        }

                        if(String.Compare(CurrentSettings.Server.AdminApiKey, apiKey) != 0) {
                            ConsoleLogger.Warn("RequestReceived admin API requested but invalid API key specified");
                            return new HttpResponse(req, false, 401, null, "application/json",
                                new ErrorResponse(3, 401, null, null).ToJson(),
                                true);
                        }

                        return StorageServer.AdminApiHandler(md);
                    }
                }

                #endregion

                #region Authenticate-Request-and-Update-Connection

                if(!String.IsNullOrEmpty(token)) {
                    if(!Token.VerifyToken(token, Users, EncryptionManager, out currUserMaster, out currApiKeyPermission)) {
                        ConsoleLogger.Warn("RequestReceived unable to verify token");
                        return new HttpResponse(req, false, 401, null, "application/json",
                            new ErrorResponse(3, 401, null, null).ToJson(),
                            true);
                    }
                }
                else if(!String.IsNullOrEmpty(apiKey)) {
                    if(!ApiKeys.VerifyApiKey(apiKey, Users, out currUserMaster, out currApiKey, out currApiKeyPermission)) {
                        ConsoleLogger.Warn("RequestReceived unable to verify API key " + apiKey);
                        return new HttpResponse(req, false, 401, null, "application/json",
                           new ErrorResponse(3, 401, null, null).ToJson(),
                           true);
                    }
                }
                else if((!String.IsNullOrEmpty(email)) && (!String.IsNullOrEmpty(password))) {
                    if(!Users.AuthenticateCredentials(email, password, out currUserMaster)) {
                        ConsoleLogger.Warn("RequestReceived unable to verify credentials for email " + email);
                        return new HttpResponse(req, false, 401, null, "application/json",
                            new ErrorResponse(3, 401, null, null).ToJson(),
                            true);
                    }

                    currApiKeyPermission = ApiKeyPermission.DefaultPermit(currUserMaster);
                }
                else {
                    ConsoleLogger.Warn("RequestReceived user API requested but no authentication material supplied");
                    return new HttpResponse(req, false, 401, null, "application/json",
                        new ErrorResponse(3, 401, "No authentication material.", null).ToJson(),
                        true);
                }

                md.CurrentUserMaster = currUserMaster;
                md.CurrentApiKey = currApiKey;
                md.CurrentApiKeyPermission = currApiKeyPermission;

                ConnManager.Update(Thread.CurrentThread.ManagedThreadId, Convert.ToInt32(currUserMaster.UserMasterId), currUserMaster.Email);

                #endregion

                #region User-Administrative-APIs

                switch(md.CurrentHttpRequest.Method.ToLower()) {
                    case "get":
                        #region get

                        if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/token", false)) {
                            return new HttpResponse(req, true, 200, null, "text/plain", Token.FromUser(md.CurrentUserMaster, CurrentSettings, EncryptionManager), true);
                        }

                        if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/replicas", false)) {
                            return StorageServer.GetReplicas(md);
                        }

                        if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/user_master", false)) {
                            return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(md.CurrentUserMaster), true);
                        }

                        if(WatsonCommon.UrlEqual(md.CurrentHttpRequest.RawUrlWithoutQuery, "/wait", false)) {
                            Thread.Sleep(10000);
                            return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
                        }

                        break;

                    #endregion

                    case "put":
                        #region put

                        break;

                    #endregion

                    case "post":
                        #region post

                        break;

                    #endregion

                    case "delete":
                        #region delete

                        break;

                    #endregion

                    case "head":
                        #region head

                        break;

                    #endregion

                    default:
                        #region default

                        ConsoleLogger.Warn("RequestReceived unknown http method: " + md.CurrentHttpRequest.Method);
                        return new HttpResponse(req, false, 400, null, "application/json",
                            new ErrorResponse(2, 400, "Unknown method.", null).ToJson(),
                            true);

                        #endregion
                }

                #endregion

                #region Build-Object

                md.CurrentObj = Obj.BuildObj(md, Users, CurrentSettings, CurrentTopology, CurrentNode);
                if(md.CurrentObj == null) {
                    ConsoleLogger.Warn("RequestReceived unable to build payload object from request");
                    return new HttpResponse(req, false, 500, null, "application/json",
                        new ErrorResponse(4, 500, "Unable to build object from request.", null).ToJson(),
                        true);
                }

                #endregion

                #region Call-User-API

                HttpResponse resp = StorageServer.UserApiHandler(md);

                if(Common.IsTrue(CurrentSettings.Syslog.LogHttpRequests)) {
                    ConsoleLogger.Warn("RequestReceived sending response: " + Environment.NewLine + resp.ToString());
                }

                return resp;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Error("RequestReceived Outer exception" + e.ToString());
                return new HttpResponse(req, false, 500, null, "application/json",
                    new ErrorResponse(1, 500, "Outer exception.", null).ToJson(),
                    true);
            }
            finally {
                ConnManager.Close(Thread.CurrentThread.ManagedThreadId);
            }
        }

        private HttpResponse OptionsHandler(HttpRequest req) {
            ConsoleLogger.Warn("OptionsHandler " + Thread.CurrentThread.ManagedThreadId + ": processing options request");

            Dictionary<string, string> responseHeaders = new Dictionary<string, string>();

            string[] requestedHeaders = null;
            if(req.Headers != null) {
                foreach(KeyValuePair<string, string> curr in req.Headers) {
                    if(String.IsNullOrEmpty(curr.Key))
                        continue;
                    if(String.IsNullOrEmpty(curr.Value))
                        continue;
                    if(String.Compare(curr.Key.ToLower(), "access-control-request-headers") == 0) {
                        requestedHeaders = curr.Value.Split(',');
                        break;
                    }
                }
            }

            string headers =
                CurrentSettings.Server.HeaderApiKey + ", " +
                CurrentSettings.Server.HeaderEmail + ", " +
                CurrentSettings.Server.HeaderPassword + ", " +
                CurrentSettings.Server.HeaderToken + ", " +
                CurrentSettings.Server.HeaderVersion;

            if(requestedHeaders != null) {
                foreach(string curr in requestedHeaders) {
                    headers += ", " + curr;
                }
            }

            responseHeaders.Add("Access-Control-Allow-Methods", "OPTIONS, HEAD, GET, PUT, POST, DELETE");
            responseHeaders.Add("Access-Control-Allow-Headers", "*, Content-Type, X-Requested-With, " + headers);
            responseHeaders.Add("Access-Control-Expose-Headers", "Content-Type, X-Requested-With, " + headers);
            responseHeaders.Add("Access-Control-Allow-Origin", "*");
            responseHeaders.Add("Accept", "*/*");
            responseHeaders.Add("Accept-Language", "en-US, en");
            responseHeaders.Add("Accept-Charset", "ISO-8859-1, utf-8");
            responseHeaders.Add("Connection", "keep-alive");

            if(Common.IsTrue(CurrentNode.Ssl)) {
                responseHeaders.Add("Host", "https://" + CurrentNode.DnsHostname + ":" + CurrentNode.Port);
            }
            else {
                responseHeaders.Add("Host", "http://" + CurrentNode.DnsHostname + ":" + CurrentNode.Port);
            }

            ConsoleLogger.Warn("OptionsHandler " + Thread.CurrentThread.ManagedThreadId + ": exiting successfully from OptionsHandler");
            return new HttpResponse(req, true, 200, responseHeaders, null, null, true);
        }
    }
}
