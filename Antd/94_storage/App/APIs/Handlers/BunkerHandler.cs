using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kvpbase {
    public class BunkerHandler {
        #region Public-Members

        #endregion

        #region Private-Members

        private Settings CurrentSettings;

        #endregion

        #region Constructors-and-Factories

        public BunkerHandler(Settings settings) {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));

            CurrentSettings = settings;
        }

        #endregion

        #region Public-Methods

        public void ContainerDelete(Obj currObj, bool recursive) {
            Task.Run(() => BunkerContainerDeleteThread(currObj, recursive));
            return;
        }

        public void ContainerMove(MoveRequest currMove) {
            Task.Run(() => BunkerContainerMoveThread(currMove));
            return;
        }

        public void ContainerRename(RenameRequest currRename) {
            Task.Run(() => BunkerContainerRenameThread(currRename));
            return;
        }

        public void ContainerWrite(Obj currObj) {
            Task.Run(() => BunkerContainerWriteThread(currObj));
            return;
        }

        public void ObjectDelete(Obj currObj) {
            Task.Run(() => BunkerObjectDeleteThread(currObj));
            return;
        }

        public bool ObjectMove(MoveRequest currMove) {
            Task.Run(() => BunkerObjectMoveThread(currMove));
            return true;
        }

        public void ObjectRename(RenameRequest currRename) {
            Task.Run(() => BunkerObjectRenameThread(currRename));
            return;
        }

        public void ObjectWrite(Obj currObj) {
            Task.Run(() => BunkerObjectWriteThread(currObj));
            return;
        }

        #endregion

        #region Private-Methods

        private void BunkerContainerDeleteThread(Obj currObj, bool recursive) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currObj == null) {
                    ConsoleLogger.Warn("BunkerContainerDeleteThread null value for currObj");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerContainerDeleteThread processing bunker node " + curr.Name);

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url;
                    url += currObj.UserGuid + "/";

                    foreach(string currString in currObj.ContainerPath) {
                        url += currString + "/";
                    }

                    if(!String.IsNullOrEmpty(currObj.Key))
                        url += currObj.Key;
                    url += "?container=true";

                    if(recursive)
                        url += "&recursive=true";

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "DELETE", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        null);

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerContainerDeleteThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerContainerDeleteThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerContainerDeleteThread exception encountered");
                return;
            }
        }

        private void BunkerContainerMoveThread(MoveRequest currMove) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currMove == null) {
                    ConsoleLogger.Warn("BunkerContainerMoveThread null value for currMove");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";
                MoveRequest req = new MoveRequest();

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerContainerMoveThread processing bunker node " + curr.Name);

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url + "move";
                    url += "?container=true";

                    #endregion

                    #region Build-Request-Object

                    //
                    // The request body must be rebuilt, since all objects in the bunker will be stored
                    // under a single user_guid.  Therefore, the user_guid in the request should be the
                    // user_guid of the account, and the user_guid from the locally-received request
                    // should be inserted as the first container
                    //

                    req = new MoveRequest();
                    req.UserGuid = curr.UserGuid;

                    req.FromContainer = new List<string>();
                    req.FromContainer.Add(currMove.UserGuid);
                    foreach(string currFromContainer in currMove.FromContainer) {
                        req.FromContainer.Add(currFromContainer);
                    }
                    req.MoveFrom = currMove.MoveFrom;

                    req.ToContainer = new List<string>();
                    req.ToContainer.Add(currMove.UserGuid);
                    foreach(string currToContainer in currMove.ToContainer) {
                        req.ToContainer.Add(currToContainer);
                    }
                    req.MoveTo = currMove.MoveTo;

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "POST", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        Encoding.UTF8.GetBytes(Common.SerializeJson(req)));

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerContainerMoveThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerContainerMoveThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerContainerMoveThread exception encountered");
                ConsoleLogger.Error("BunkerContainerMoveThread Outer exception" + e.ToString());
                return;
            }
        }

        private void BunkerContainerRenameThread(RenameRequest currRename) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currRename == null) {
                    ConsoleLogger.Warn("BunkerContainerRenameThread null value for currRename");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";
                RenameRequest req = new RenameRequest();

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerContainerRenameThread processing bunker node " + curr.Name);

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url + "rename";
                    url += "?container=true";

                    #endregion

                    #region Build-Request-Object

                    //
                    // The request body must be rebuilt, since all objects in the bunker will be stored
                    // under a single user_guid.  Therefore, the user_guid in the request should be the
                    // user_guid of the account, and the user_guid from the locally-received request
                    // should be inserted as the first container
                    //

                    req = new RenameRequest();
                    req.UserGuid = curr.UserGuid;

                    req.ContainerPath = new List<string>();
                    req.ContainerPath.Add(currRename.UserGuid);
                    foreach(string currContainerPath in currRename.ContainerPath) {
                        req.ContainerPath.Add(currContainerPath);
                    }

                    req.RenameFrom = currRename.RenameFrom;
                    req.RenameTo = currRename.RenameTo;

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "POST", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        Encoding.UTF8.GetBytes(Common.SerializeJson(req)));

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerContainerRenameThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerContainerRenameThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerContainerRenameThread exception encountered");
                ConsoleLogger.Error("BunkerContainerRenameThread Outer exception" + e.ToString());
                return;
            }
        }

        private void BunkerContainerWriteThread(Obj currObj) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currObj == null) {
                    ConsoleLogger.Warn("BunkerContainerWriteThread null value for currObj");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerContainerWriteThread processing bunker node " + curr.Name);

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url;
                    url += currObj.UserGuid + "/";

                    foreach(string currContainer in currObj.ContainerPath) {
                        url += currContainer + "/";
                    }

                    if(!String.IsNullOrEmpty(currObj.Key))
                        url += currObj.Key;
                    url += "?container=true";

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "PUT", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        null);

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerContainerWriteThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerContainerWriteThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerContainerWriteThread exception encountered");
                ConsoleLogger.Error("BunkerContainerWriteThread Outer exception" + e.ToString());
                return;
            }
        }

        private void BunkerObjectDeleteThread(Obj currObj) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currObj == null) {
                    ConsoleLogger.Warn("BunkerObjectDeleteThread null value for currObj");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerObjectDeleteThread processing bunker node " + curr.Name + " (" + curr.Vendor + " v" + curr.Version + ")");

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url;
                    url += currObj.UserGuid + "/";

                    foreach(string currString in currObj.ContainerPath) {
                        url += currString + "/";
                    }

                    if(!String.IsNullOrEmpty(currObj.Key))
                        url += currObj.Key;

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "DELETE", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        null);

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerObjectDeleteThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerObjectDeleteThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerObjectDeleteThread exception encountered");
                ConsoleLogger.Error("BunkerObjectDeleteThread Outer exception" + e.ToString());
                return;
            }
        }

        private void BunkerObjectMoveThread(MoveRequest currMove) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currMove == null) {
                    ConsoleLogger.Warn("BunkerObjectMoveThread null value for currMove");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";
                MoveRequest req = new MoveRequest();

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerObjectMoveThread processing bunker node " + curr.Name + " (" + curr.Vendor + " v" + curr.Version + ")");

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url + "move";

                    #endregion

                    #region Build-Request-Object

                    //
                    // The request body must be rebuilt, since all objects in the bunker will be stored
                    // under a single user_guid.  Therefore, the user_guid in the request should be the
                    // user_guid of the account, and the user_guid from the locally-received request
                    // should be inserted as the first container
                    //

                    req = new MoveRequest();
                    req.UserGuid = curr.UserGuid;

                    req.FromContainer = new List<string>();
                    req.FromContainer.Add(currMove.UserGuid);
                    foreach(string currFromContainer in currMove.FromContainer) {
                        req.FromContainer.Add(currFromContainer);
                    }
                    req.MoveFrom = currMove.MoveFrom;

                    req.ToContainer = new List<string>();
                    req.ToContainer.Add(currMove.UserGuid);
                    foreach(string currToContainer in currMove.ToContainer) {
                        req.ToContainer.Add(currToContainer);
                    }
                    req.MoveTo = currMove.MoveTo;

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "POST", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        Encoding.UTF8.GetBytes(Common.SerializeJson(req)));

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerObjectMoveThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerObjectMoveThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerObjectMoveThread exception encountered");
                ConsoleLogger.Error("BunkerObjectMoveThread Outer exception" + e.ToString());
                return;
            }
        }

        private void BunkerObjectRenameThread(RenameRequest currRename) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currRename == null) {
                    ConsoleLogger.Warn("BunkerObjectRenameThread null value for currRename");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";
                RenameRequest req = new RenameRequest();

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerObjectRenameThread processing bunker node " + curr.Name + " (" + curr.Vendor + " v" + curr.Version + ")");

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url + "rename";

                    #endregion

                    #region Build-Request-Object

                    //
                    // The request body must be rebuilt, since all objects in the bunker will be stored
                    // under a single user_guid.  Therefore, the user_guid in the request should be the
                    // user_guid of the account, and the user_guid from the locally-received request
                    // should be inserted as the first container
                    //

                    req = new RenameRequest();
                    req.UserGuid = curr.UserGuid;

                    req.ContainerPath = new List<string>();
                    req.ContainerPath.Add(currRename.UserGuid);
                    foreach(string currContainer in currRename.ContainerPath) {
                        req.ContainerPath.Add(currContainer);
                    }

                    req.RenameFrom = currRename.RenameFrom;
                    req.RenameTo = currRename.RenameTo;

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "POST", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        Encoding.UTF8.GetBytes(Common.SerializeJson(req)));

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerObjectRenameThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerObjectRenameThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerObjectRenameThread exception encountered");
                ConsoleLogger.Error("BunkerObjectRenameThread Outer exception" + e.ToString());
                return;
            }
        }

        private void BunkerObjectWriteThread(Obj currObj) {
            try {
                #region Check-for-Null-Values-and-Configuration

                if(currObj == null) {
                    ConsoleLogger.Warn("BunkerObjectWriteThread null value for currObj");
                    return;
                }

                if(CurrentSettings.Bunker == null)
                    return;
                if(CurrentSettings.Bunker.Nodes == null)
                    return;
                if(CurrentSettings.Bunker.Nodes.Count < 1)
                    return;
                if(!Common.IsTrue(CurrentSettings.Bunker.Enable))
                    return;

                #endregion

                #region Variables

                Dictionary<string, string> headers = new Dictionary<string, string>();
                RestResponse resp = new RestResponse();
                string url = "";

                #endregion

                #region Process-Each-Node

                foreach(Settings.BunkerNode curr in CurrentSettings.Bunker.Nodes) {
                    ConsoleLogger.Warn("BunkerObjectWriteThread processing bunker node " + curr.Name + " (" + curr.Vendor + " v" + curr.Version + ")");

                    #region Reset-Variables

                    headers = new Dictionary<string, string>();
                    resp = new RestResponse();
                    url = "";

                    #endregion

                    #region Build-Header-Dictionary

                    headers.Add(CurrentSettings.Server.HeaderApiKey, curr.ApiKey);

                    #endregion

                    #region Generate-URL

                    url = curr.Url;
                    url += currObj.UserGuid + "/";

                    foreach(string currContainer in currObj.ContainerPath) {
                        url += currContainer + "/";
                    }

                    if(!String.IsNullOrEmpty(currObj.Key))
                        url += currObj.Key;

                    #endregion

                    #region Submit-Request

                    resp = RestRequest.SendRequestSafe(
                        url, "application/json", "PUT", null, null, false,
                        Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                        headers,
                        currObj.Value);

                    if(resp == null) {
                        ConsoleLogger.Warn("BunkerObjectWriteThread null REST response while writing to " + url);
                        continue;
                    }

                    if(resp.StatusCode != 200 && resp.StatusCode != 201) {
                        ConsoleLogger.Warn("BunkerObjectWriteThread non-200/201 REST response from " + url);
                        continue;
                    }

                    #endregion
                }

                return;

                #endregion
            }
            catch(Exception e) {
                ConsoleLogger.Warn("BunkerObjectWriteThread exception encountered");
                ConsoleLogger.Error("BunkerObjectWriteThread Outer exception" + e.ToString());
                return;
            }
        }

        #endregion
    }
}
