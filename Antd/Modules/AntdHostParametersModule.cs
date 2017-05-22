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

using System;
using antdlib.config;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;
using anthilla.core;

namespace Antd.Modules {
    public class AntdHostParametersModule : NancyModule {

        public AntdHostParametersModule() {

            Get["/hostparam"] = x => {
                var hostparam = HostParametersConfiguration.Conf;
                return JsonConvert.SerializeObject(hostparam);
            };

            Post["/hostparam/set/modprobeslist"] = x => {
                string data = Request.Form.Data;
                HostParametersConfiguration.SetModprobesList(data.SplitToList(";"));
                new Do().SaveModprobes();
                return HttpStatusCode.OK;
            };

            Post["/hostparam/set/rmmodlist"] = x => {
                string data = Request.Form.Data;
                HostParametersConfiguration.SetRmmodList(data.SplitToList(";"));
                new Do().RemoveModules();
                return HttpStatusCode.OK;
            };

            Post["/hostparam/set/modulesblacklistlist"] = x => {
                string data = Request.Form.Data;
                HostParametersConfiguration.SetModulesBlacklistList(data.SplitToList(";"));
                new Do().BlacklistMudules();
                return HttpStatusCode.OK;
            };

            Post["/hostparam/set/osparameters"] = x => {
                string data = Request.Form.Data;
                HostParametersConfiguration.SetOsParametersList(data.SplitToList(";"));
                new Do().SaveOsParameters();
                return HttpStatusCode.OK;
            };

            Post["/hostparam/set/servicesstartlist"] = x => {
                string data = Request.Form.Data;
                HostParametersConfiguration.SetServicesStartList(data.SplitToList(";"));
                new Do().StartService();
                return HttpStatusCode.OK;
            };

            Post["/hostparam/set/servicesstoplist"] = x => {
                string data = Request.Form.Data;
                HostParametersConfiguration.SetServicesStopList(data.SplitToList(";"));
                new Do().StopService();
                return HttpStatusCode.OK;
            };

            Post["/hostparam/set/startcommandslist"] = x => {
                string data = Request.Form.Data;
                var list = new List<Control>();
                var arr1 = data.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for(var i = 0; i < arr1.Length; i++) {
                    var mo = new Control {
                        Index = i,
                        FirstCommand = arr1[i],
                        ControlCommand = "",
                        Check = ""
                    };
                    list.Add(mo);
                }
                HostParametersConfiguration.SetStartCommandsList(list);
                new Do().LaunchStart();
                return HttpStatusCode.OK;
            };

            Post["/hostparam/set/endcommandslist"] = x => {
                string data = Request.Form.Data;
                var list = new List<Control>();
                var arr1 = data.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for(var i = 0; i < arr1.Length; i++) {
                    var mo = new Control {
                        Index = i,
                        FirstCommand = arr1[i],
                        ControlCommand = "",
                        Check = ""
                    };
                    list.Add(mo);
                }
                HostParametersConfiguration.SetEndCommandsList(list);
                new Do().LaunchEnd();
                return HttpStatusCode.OK;
            };
        }
    }
}