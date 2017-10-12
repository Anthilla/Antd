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
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using anthilla.core;

namespace AntdUi.Modules {
    public class AssetSettingModule : NancyModule {

        public AssetSettingModule() {
            Get["/assetsetting"] = x => {
                var model = ApiConsumer.Get<PageAssetSettingModel>($"http://127.0.0.1:{Application.ServerPort}/assetsetting");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/netscan/setsubnet"] = x => {
                string subnet = Request.Form.Subnet;
                string label = Request.Form.Label;
                var dict = new Dictionary<string, string> {
                    {"Subnet", subnet},
                    {"Label", label}
                };
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/netscan/setsubnet", dict);
            };

            Post["/netscan/setlabel"] = x => {
                string letter = Request.Form.Letter;
                string number = Request.Form.Number;
                string label = Request.Form.Label;
                var dict = new Dictionary<string, string> {
                    {"Letter", letter},
                    {"Number", number},
                    {"Label", label}
                };
                return ApiConsumer.Post($"http://127.0.0.1:{Application.ServerPort}/netscan/setlabel", dict);
            };
        }
    }
}