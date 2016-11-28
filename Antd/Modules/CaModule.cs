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
using System.Dynamic;
using antdlib.common;
using Nancy.Security;

namespace Antd.Modules {
    public class CaModule : CoreModule {

        public CaModule() {
            this.RequiresAuthentication();

            #region [    Home    ]
            Get["/ca"] = x => {
                dynamic viewModel = new ExpandoObject();
                return View["antd/page-ca", viewModel];
            };
            #endregion

            #region [    Partials    ]
            Get["/part/ca/dc"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    return View["antd/part/page-ca-dc", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/ca/dcusers"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    return View["antd/part/page-ca-dcusers", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/ca/setup"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    return View["antd/part/page-ca-setup", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/ca/cert"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    return View["antd/part/page-ca-cert", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/ca/certdc"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    return View["antd/part/page-ca-certdc", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/ca/certsc"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    return View["antd/part/page-ca-certsc", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };
            #endregion

            #region [    Hooks    ]
            After += ctx => {
                if(ctx.Response.ContentType == "text/html") {
                    ctx.Response.ContentType = "text/html; charset=utf-8";
                }
            };
            #endregion
        }
    }
}