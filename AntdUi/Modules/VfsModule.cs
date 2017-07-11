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

using antdlib.models;
using anthilla.core;
using Nancy;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntdUi.Modules {
    public class VfsModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public VfsModule() : base("/vfs") {

            #region [    Config    ]
            Get["/"] = x => {
                var model = _api.Get<PageVfsModel>($"http://127.0.0.1:{Application.ServerPort}{Request.Path}");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/save/system"] = x => {
                string config = Request.Form.Config;
                var dict = new Dictionary<string, string> {
                    { "Config", config }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            Post["/save/topology"] = x => {
                string config = Request.Form.Config;
                var dict = new Dictionary<string, string> {
                    { "Config", config }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            Post["/save/apikeys"] = x => {
                string config = Request.Form.Config;
                var dict = new Dictionary<string, string> {
                    { "Config", config }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            Post["/save/apikeypermissions"] = x => {
                string config = Request.Form.Config;
                var dict = new Dictionary<string, string> {
                    { "Config", config }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            Post["/save/users"] = x => {
                string config = Request.Form.Config;
                var dict = new Dictionary<string, string> {
                    { "Config", config }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };
            #endregion

            #region [    Object Actions    ]
            ///VfsClient.CreateObject(Client kvp, string containerPath, string fileType, byte[] data, string fileName = "")
            Post["/client/{userguid}/{server}/{port}/0/0"] = x => {
                var file = Request.Files.FirstOrDefault();
                if(file == null) {
                    return HttpStatusCode.BadRequest;
                }
                string containerPath = Request.Form.ContainerPath;
                string fileName = file.Name;
                var fileType = Kvpbase.MimeTypes.GetFromExtension(Path.GetExtension(fileName));

                var client = new RestClient($"http://127.0.0.1:{Application.ServerPort}{Request.Path}");
                var request = new RestRequest("/", Method.POST);
                request.AddParameter("ContainerPath", containerPath);
                request.AddParameter("FileType", fileType);
                request.AddParameter("FileName", fileName);
                byte[] array;
                byte[] buffer = new byte[16 * 1024];
                using(MemoryStream ms = new MemoryStream()) {
                    int read;
                    while((read = file.Value.Read(buffer, 0, buffer.Length)) > 0) {
                        ms.Write(buffer, 0, read);
                    }
                    array = ms.ToArray();
                    request.AddFile(file.Name, array, file.Name, file.ContentType);
                }
                var response = client.Execute(request);
                var result = response.StatusCode;
                return (HttpStatusCode)result;
            };

            ///VfsClient.RetrieveObject(Client kvp, string objectPath)
            Get["/client/{userguid}/{server}/{port}/0/1"] = x => {
                string objectPath = Request.Query.path;
                string file = Request.Query.file;
                var fileType = Kvpbase.MimeTypes.GetFromExtension(Path.GetExtension(file));
                var response = new Response();
                response.Headers.Add("Content-Disposition", "attachment; filename=" + file);
                response.ContentType = fileType;
                response.Contents = stream => {
                    using(var memoryStream = new MemoryStream()) {
                        var client = new RestClient($"http://127.0.0.1:{Application.ServerPort}{Request.Path}");
                        var request = new RestRequest("/", Method.GET);
                        request.AddParameter("ObjectPath", objectPath);
                        request.ResponseWriter = (responseStream) => responseStream.CopyTo(stream);
                        client.DownloadData(request);
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
                string objectPath = Request.Form.ObjectPath;
                var dict = new Dictionary<string, string> {
                    { "ObjectPath", objectPath }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            ///VfsClient.MoveObject(Client kvp, string objectPath, string newContainer)
            Post["/client/{userguid}/{server}/{port}/0/3"] = x => {
                string objectPath = Request.Form.ObjectPath;
                string newContainer = Request.Form.NewContainer;
                var dict = new Dictionary<string, string> {
                    { "ObjectPath", objectPath },
                    { "NewContainer", newContainer }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            ///VfsClient.RenameObject(Client kvp, string objectPath, string newName)
            Post["/client/{userguid}/{server}/{port}/0/4"] = x => {
                string objectPath = Request.Form.ObjectPath;
                string newName = Request.Form.NewName;
                var dict = new Dictionary<string, string> {
                    { "ObjectPath", objectPath },
                    { "NewName", newName }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            ///VfsClient.DeleteObject(Client kvp, string objectPath)
            Post["/client/{userguid}/{server}/{port}/0/5"] = x => {
                string objectPath = Request.Form.ObjectPath;
                var dict = new Dictionary<string, string> {
                    { "ObjectPath", objectPath }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };
            #endregion

            #region [    Container Actions    ]
            ///VfsClient.CreateContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/0"] = x => {
                string containerPath = Request.Form.ContainerPath;
                containerPath = containerPath.Replace("//", "/");
                var dict = new Dictionary<string, string> {
                    { "ContainerPath", containerPath }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            ///VfsClient.GetContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/1"] = x => {
                string containerPath = Request.Form.ContainerPath;
                var dict = new Dictionary<string, string> {
                    { "ContainerPath", containerPath }
                };
                var result = _api.Post<VfsContainerInfo>($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
                var json = JsonConvert.SerializeObject(result);
                return json;
            };

            ///VfsClient.VerifyContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/2"] = x => {
                string containerPath = Request.Form.ContainerPath;
                var dict = new Dictionary<string, string> {
                    { "ContainerPath", containerPath }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            ///VfsClient.MoveContainer(Client kvp, string containerPath, string newContainer)
            Post["/client/{userguid}/{server}/{port}/1/3"] = x => {
                string containerPath = Request.Form.ContainerPath;
                string newContainer = Request.Form.NewContainer;
                var dict = new Dictionary<string, string> {
                    { "ContainerPath", containerPath },
                    { "NewContainer", newContainer }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            ///VfsClient.RenameContainer(Client kvp, string containerPath, string newName)
            Post["/client/{userguid}/{server}/{port}/1/4"] = x => {
                string containerPath = Request.Form.ContainerPath;
                string newName = Request.Form.NewName;
                var dict = new Dictionary<string, string> {
                    { "ContainerPath", containerPath },
                    { "NewName", newName }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };

            ///VfsClient.DeleteContainer(Client kvp, string containerPath)
            Post["/client/{userguid}/{server}/{port}/1/5"] = x => {
                string containerPath = Request.Form.ContainerPath;
                var dict = new Dictionary<string, string> {
                    { "ContainerPath", containerPath }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}{Request.Path}", dict);
            };
            #endregion
        }
    }
}