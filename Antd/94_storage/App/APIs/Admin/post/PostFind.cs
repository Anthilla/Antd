﻿using anthilla.core;
using System;
using System.Collections.Generic;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse PostFind(RequestMetadata md) {
            #region Variables

            string homeDirectory = "";
            Find req = new Find();
            string diskPath = "";
            string url = "";

            #endregion

            #region Deserialize-and-Initialize

            try {
                req = Common.DeserializeJson<Find>(md.CurrentHttpRequest.Data);
                if(req == null) {
                    ConsoleLogger.Warn("PostFind null request after deserialization, returning 400");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                        true);
                }
            }
            catch(Exception) {
                ConsoleLogger.Warn("PostFind unable to deserialize request body");
                return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                    new ErrorResponse(2, 400, "Unable to deserialize request body.", null).ToJson(),
                    true);
            }

            req.Urls = new List<string>();

            #endregion

            #region Validate-Content

            if(String.IsNullOrEmpty(req.UserGuid)) {
                ConsoleLogger.Warn("PostFind null GUID after deserialization, returning 400");
                return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                    new ErrorResponse(2, 400, "Unable to validate request body.", null).ToJson(),
                    true);
            }

            #endregion

            #region Retrieve-User-Home-Directory

            homeDirectory = Users.GetHomeDirectory(req.UserGuid, CurrentSettings);
            if(String.IsNullOrEmpty(homeDirectory)) {
                ConsoleLogger.Warn("PostFind unable to retrieve home directory for user GUID " + md.CurrentUserMaster.Guid);
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json",
                    new ErrorResponse(4, 500, "Unable to find home directory for user.", null).ToJson(),
                    true);
            }

            #endregion

            #region Build-Disk-Path

            diskPath = Find.BuildDiskPath(req, Users, CurrentSettings);

            #endregion

            #region Process

            #region Check-Locally

            if(String.IsNullOrEmpty(req.Key)) {
                #region Check-for-Folder

                if(Common.VerifyDirectoryAccess(CurrentSettings.Environment, homeDirectory)) {
                    if(Common.IsTrue(CurrentNode.Ssl)) {
                        req.Urls.Add("https://" + CurrentNode.DnsHostname + ":" + CurrentNode.Port + "/" + req.UserGuid + "/");
                    }
                    else {
                        req.Urls.Add("http://" + CurrentNode.DnsHostname + ":" + CurrentNode.Port + "/" + req.UserGuid + "/");
                    }
                }

                #endregion
            }
            else {
                #region Check-for-Specific-File

                if(Common.FileExists(diskPath)) {
                    if(Common.IsTrue(CurrentNode.Ssl))
                        url = "https://";
                    else
                        url = "http://";
                    url += CurrentNode.DnsHostname + ":" + CurrentNode.Port + "/" + req.UserGuid + "/";
                    foreach(string currContainer in req.ContainerPath)
                        url += currContainer + "/";
                    if(!String.IsNullOrEmpty(req.Key))
                        url += req.Key;
                    req.Urls.Add(url);
                }

                #endregion
            }

            #endregion

            #region Check-Topology

            if(req.QueryTopology) {
                foreach(Node curr in CurrentTopology.Nodes) {
                    if(curr.NodeId == CurrentNode.NodeId)
                        continue;
                    if(Node.FindObject(CurrentSettings, curr, req)) {
                        if(Common.IsTrue(curr.Ssl))
                            url = "https://";
                        else
                            url = "http://";
                        url += CurrentNode.DnsHostname + ":" + CurrentNode.Port + "/" + req.UserGuid + "/";
                        foreach(string currContainer in req.ContainerPath)
                            url += currContainer + "/";
                        if(!String.IsNullOrEmpty(req.Key))
                            url += req.Key;
                        req.Urls.Add(url);
                    }
                }
            }

            #endregion

            #endregion

            #region Respond

            if(req.Urls.Count > 0) {
                return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", Common.SerializeJson(req), true);
            }
            else {
                return new HttpResponse(md.CurrentHttpRequest, false, 404, null, "application/json",
                   new ErrorResponse(5, 404, null, null).ToJson(),
                   true);
            }

            #endregion
        }
    }
}