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
using anthilla.core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class LogJournaldModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public LogJournaldModule() {
            Get["/journald"] = x => {
                var model = _api.Get<PageJournaldModel>($"http://127.0.0.1:{Application.ServerPort}/journald");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/journald/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/journald/set");
            };

            Post["/journald/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/journald/restart");
            };

            Post["/journald/stop"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/journald/stop");
            };

            Post["/journald/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/journald/enable");
            };

            Post["/journald/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/journald/disable");
            };

            Post["/journald/options"] = x => {
                string storage = Request.Form.Storage;
                string compress = Request.Form.Compress;
                string seal = Request.Form.Seal;
                string splitMode = Request.Form.SplitMode;
                string syncIntervalSec = Request.Form.SyncIntervalSec;
                string rateLimitInterval = Request.Form.RateLimitInterval;
                string rateLimitBurst = Request.Form.RateLimitBurst;
                string systemMaxUse = Request.Form.SystemMaxUse;
                string systemKeepFree = Request.Form.SystemKeepFree;
                string systemMaxFileSize = Request.Form.SystemMaxFileSize;
                string runtimeMaxUse = Request.Form.RuntimeMaxUse;
                string runtimeKeepFree = Request.Form.RuntimeKeepFree;
                string runtimeMaxFileSize = Request.Form.RuntimeMaxFileSize;
                string maxRetentionSec = Request.Form.MaxRetentionSec;
                string maxFileSec = Request.Form.MaxFileSec;
                string forwardToSyslog = Request.Form.ForwardToSyslog;
                string forwardToKMsg = Request.Form.ForwardToKMsg;
                string forwardToConsole = Request.Form.ForwardToConsole;
                string forwardToWall = Request.Form.ForwardToWall;
                string ttyPath = Request.Form.TTYPath;
                string maxLevelStore = Request.Form.MaxLevelStore;
                string maxLevelSyslog = Request.Form.MaxLevelSyslog;
                string maxLevelKMsg = Request.Form.MaxLevelKMsg;
                string maxLevelConsole = Request.Form.MaxLevelConsole;
                string maxLevelWall = Request.Form.MaxLevelWall;
                var dict = new Dictionary<string, string> {
                    { "Storage", storage },
                    { "Compress", compress },
                    { "Seal", seal },
                    { "SplitMode", splitMode },
                    { "SyncIntervalSec", syncIntervalSec },
                    { "RateLimitInterval", rateLimitInterval },
                    { "RateLimitBurst", rateLimitBurst },
                    { "SystemMaxUse", systemMaxUse },
                    { "SystemKeepFree", systemKeepFree },
                    { "SystemMaxFileSize", systemMaxFileSize },
                    { "RuntimeMaxUse", runtimeMaxUse },
                    { "RuntimeKeepFree", runtimeKeepFree },
                    { "RuntimeMaxFileSize", runtimeMaxFileSize },
                    { "MaxRetentionSec", maxRetentionSec },
                    { "MaxFileSec", maxFileSec },
                    { "ForwardToSyslog", forwardToSyslog },
                    { "ForwardToKMsg", forwardToKMsg },
                    { "ForwardToConsole", forwardToConsole },
                    { "ForwardToWall", forwardToWall },
                    { "TTYPath", ttyPath },
                    { "MaxLevelStore", maxLevelStore },
                    { "MaxLevelSyslog", maxLevelSyslog },
                    { "MaxLevelKMsg", maxLevelKMsg },
                    { "MaxLevelConsole", maxLevelConsole },
                    { "MaxLevelWall", maxLevelWall }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/journald/options", dict);
            };
        }
    }
}