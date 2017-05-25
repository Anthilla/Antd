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
using Nancy;
using Newtonsoft.Json;

namespace Antd.Modules {
    public class AssetGlusterModule : NancyModule {

        public AssetGlusterModule() {
            Get["/gluster"] = x => {
                var model = new PageGlusterModel {
                    Gluster = GlusterConfiguration.Get()
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/gluster/set"] = x => {
                GlusterConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/gluster/restart"] = x => {
                GlusterConfiguration.Launch();
                return HttpStatusCode.OK;
            };

            Post["/gluster/stop"] = x => {
                GlusterConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/gluster/enable"] = x => {
                GlusterConfiguration.Enable();
                GlusterConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/gluster/disable"] = x => {
                GlusterConfiguration.Disable();
                GlusterConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/gluster/save"] = x => {
                string config = Request.Form.Config;
                var model = JsonConvert.DeserializeObject<GlusterConfigurationModel>(config);
                GlusterConfiguration.Save(model);
                return HttpStatusCode.OK;
            };
        }
    }
}