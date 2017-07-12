//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using antdlib.config;
using antdlib.models;
using anthilla.core;
using Kvpbase;
using KvpbaseSDK;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.Modules {
    public class VfsModule : NancyModule {

        public VfsModule() : base("/vfs") {
            Get["/"] = x => {
                var model = new PageVfsModel {
                    Settings = VfsConfiguration.GetSystemConfiguration(),
                    Topology = VfsConfiguration.GetTopologyConfiguration(),
                    ApiKeys = VfsConfiguration.GetApiKeyConfiguration(),
                    ApiKeyPermissions = VfsConfiguration.GetApiKeyPermissionConfiguration(),
                    UserMasters = VfsConfiguration.GetUserMasterConfiguration(),
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/save/system"] = x => {
                string config = Request.Form.Config;
                var model = JsonConvert.DeserializeObject<Settings>(config);
                VfsConfiguration.SaveSystemConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/save/topology"] = x => {
                string config = Request.Form.Config;
                var model = JsonConvert.DeserializeObject<Kvpbase.Topology>(config);
                VfsConfiguration.SaveTopologyConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/save/apikeys"] = x => {
                string config = Request.Form.Config;
                var model = JsonConvert.DeserializeObject<List<ApiKey>>(config);
                VfsConfiguration.SaveApiKeyConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/save/apikeypermissions"] = x => {
                string config = Request.Form.Config;
                var model = JsonConvert.DeserializeObject<List<ApiKeyPermission>>(config);
                VfsConfiguration.SaveApiKeyPermissionConfiguration(model);
                return HttpStatusCode.OK;
            };

            Post["/save/users"] = x => {
                string config = Request.Form.Config;
                var model = JsonConvert.DeserializeObject<List<UserMaster>>(config);
                VfsConfiguration.SaveUserMasterConfiguration(model);
                return HttpStatusCode.OK;
            };

            #region [    Object Actions    ]
            ///VfsClient.CreateObject(Client kvp, string containerPath, string fileType, byte[] data, string fileName = "")
            Post["/client/{userguid}/{server}/{port}/0/0"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                string fileType = Request.Form.FileType;
                string fileName = Request.Form.FileName;
                var file = Request.Files.FirstOrDefault();
                if(file == null) {
                    return HttpStatusCode.BadRequest;
                }
                var data = file.Value.ReadAllBytes();
                var result = VFS.VfsClient.CreateObject(client, containerPath, fileType, data, fileName);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.RetrieveObject(Client kvp, string objectPath)
            Get["/client/{userguid}/{server}/{port}/0/1"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string objectPath = Request.Form.ObjectPath;
                objectPath = Request.Query.file;

                var result = VFS.VfsClient.RetrieveObject(client, objectPath);
                var fileName = objectPath.Split('/').LastOrDefault();
                var fileType = Kvpbase.MimeTypes.GetFromExtension(Path.GetExtension(fileName));
                var response = new Response();
                response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
                response.ContentType = fileType;
                response.Contents = stream => {
                    using(var memoryStream = new MemoryStream(result)) {
                        int data;
                        while((data = memoryStream.ReadByte()) != -1) {
                            memoryStream.WriteByte((byte)data);
                        }
                    }
                };
                return response;
            };

            ///VfsClient.VerifyObject(Client kvp, string objectPath)
            Post["/client/{userguid}/{server}/{port}/0/2"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string objectPath = Request.Form.ObjectPath;
                var result = VFS.VfsClient.VerifyObject(client, objectPath);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.MoveObject(Client kvp, string objectPath, string newContainer)
            Post["/client/{userguid}/{server}/{port}/0/3"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string objectPath = Request.Form.ObjectPath;
                string newContainer = Request.Form.NewContainer;
                var result = VFS.VfsClient.MoveObject(client, objectPath, newContainer);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.RenameObject(Client kvp, string objectPath, string newName)
            Post["/client/{userguid}/{server}/{port}/0/4"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string objectPath = Request.Form.ObjectPath;
                string newName = Request.Form.NewName;
                var result = VFS.VfsClient.RenameObject(client, objectPath, newName);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.DeleteObject(Client kvp, string objectPath)
            Post["/client/{userguid}/{server}/{port}/0/5"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string objectPath = Request.Form.ObjectPath;
                var result = VFS.VfsClient.DeleteObject(client, objectPath);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };
            #endregion

            #region [    Container Actions    ]
            ///VfsClient.CreateContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/0"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                var result = VFS.VfsClient.CreateContainer(client, containerPath);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.GetContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/1"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                var result = VFS.VfsClient.GetContainer(client, containerPath);
                var json = JsonConvert.SerializeObject(result);
                return json;
            };

            ///VfsClient.VerifyContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/2"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                var result = VFS.VfsClient.VerifyContainer(client, containerPath);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.MoveContainer(Client kvp, string containerPath, string newContainer)
            Post["/client/{userguid}/{server}/{port}/1/3"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                string newContainer = Request.Form.NewContainer;
                var result = VFS.VfsClient.MoveContainer(client, containerPath, newContainer);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.RenameContainer(Client kvp, string containerPath, string newName)
            Post["/client/{userguid}/{server}/{port}/1/4"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                string newName = Request.Form.NewName;
                var result = VFS.VfsClient.RenameContainer(client, containerPath, newName);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };

            ///VfsClient.DeleteContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/5"] = x => {
                string userguid = x.userguid;
                string server = x.server;
                string port = x.port;
                if(string.IsNullOrEmpty(userguid) || string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port)) {
                    return HttpStatusCode.BadRequest;
                }
                var client = new Client(userguid, "default", $"http://{server}:{port}");
                if(client == null) {
                    return HttpStatusCode.InternalServerError;
                }
                string containerPath = Request.Form.ContainerPath;
                var result = VFS.VfsClient.DeleteContainer(client, containerPath);
                return result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            };
            #endregion
        }
    }
}