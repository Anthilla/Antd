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

using antdlib.common;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AntdUi.Modules {
    public class AntdBindModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AntdBindModule() {
            Get["/bind"] = x => {
                var model = _api.Get<PageBindModel>($"http://127.0.0.1:{Application.ServerPort}/bind");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/bind/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/bind/set");
            };

            Post["/bind/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/bind/restart");
            };

            Post["/bind/stop"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/bind/stop");
            };

            Post["/bind/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/bind/enable");
            };

            Post["/bind/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/bind/disable");
            };

            Post["/bind/options"] = x => {
                string notify = Request.Form.Notify;
                string maxCacheSize = Request.Form.MaxCacheSize;
                string maxCacheTtl = Request.Form.MaxCacheTtl;
                string maxNcacheTtl = Request.Form.MaxNcacheTtl;
                string forwarders = Request.Form.Forwarders;
                string allowNotify = Request.Form.AllowNotify;
                string allowTransfer = Request.Form.AllowTransfer;
                string recursion = Request.Form.Recursion;
                string transferFormat = Request.Form.TransferFormat;
                string querySourceAddress = Request.Form.QuerySourceAddress;
                string querySourcePort = Request.Form.QuerySourcePort;
                string version = Request.Form.Version;
                string allowQuery = Request.Form.AllowQuery;
                string allowRecursion = Request.Form.AllowRecursion;
                string ixfrFromDifferences = Request.Form.IxfrFromDifferences;
                string listenOnV6 = Request.Form.ListenOnV6;
                string listenOnPort53 = Request.Form.ListenOnPort53;
                string dnssecEnabled = Request.Form.DnssecEnabled;
                string dnssecValidation = Request.Form.DnssecValidation;
                string dnssecLookaside = Request.Form.DnssecLookaside;
                string authNxdomain = Request.Form.AuthNxdomain;
                string keyName = Request.Form.KeyName;
                string keySecret = Request.Form.KeySecret;
                string controlAcl = Request.Form.ControlAcl;
                string controlIp = Request.Form.ControlIp;
                string controlPort = Request.Form.ControlPort;
                string controlAllow = Request.Form.ControlAllow;
                string loggingChannel = Request.Form.LoggingChannel;
                string loggingDaemon = Request.Form.LoggingDaemon;
                string loggingSeverity = Request.Form.LoggingSeverity;
                string loggingPrintCategory = Request.Form.LoggingPrintCategory;
                string loggingPrintSeverity = Request.Form.LoggingPrintSeverity;
                string loggingPrintTime = Request.Form.LoggingPrintTime;
                string trustedKeys = Request.Form.TrustedKey;
                string aclLocalInterfaces = Request.Form.AclLocalInterfaces;
                string aclInternalInterfaces = Request.Form.AclInternalInterfaces;
                string aclExternalInterfaces = Request.Form.AclExternalInterfaces;
                string aclLocalNetworks = Request.Form.AclLocalNetworks;
                string aclInternalNetworks = Request.Form.AclInternalNetworks;
                string aclExternalNetworks = Request.Form.AclExternalNetworks;
                var dict = new Dictionary<string, string> {
                    { "Notify", notify },
                    { "MaxCacheSize", maxCacheSize },
                    { "MaxCacheTtl", maxCacheTtl },
                    { "MaxNcacheTtl", maxNcacheTtl },
                    { "Forwarders", forwarders },
                    { "AllowNotify", allowNotify },
                    { "AllowTransfer", allowTransfer },
                    { "Recursion", recursion },
                    { "TransferFormat", transferFormat },
                    { "QuerySourceAddress", querySourceAddress },
                    { "QuerySourcePort", querySourcePort },
                    { "Version", version },
                    { "AllowQuery", allowQuery },
                    { "AllowRecursion", allowRecursion },
                    { "IxfrFromDifferences", ixfrFromDifferences },
                    { "ListenOnV6", listenOnV6 },
                    { "ListenOnPort53", listenOnPort53 },
                    { "DnssecEnabled", dnssecEnabled },
                    { "DnssecValidation", dnssecValidation },
                    { "DnssecLookaside", dnssecLookaside },
                    { "AuthNxdomain", authNxdomain },
                    { "KeyName", keyName },
                    { "KeySecret", keySecret },
                    { "ControlAcl", controlAcl },
                    { "ControlIp", controlIp },
                    { "ControlPort", controlPort },
                    { "ControlAllow", controlAllow },
                    { "LoggingChannel", loggingChannel },
                    { "LoggingDaemon", loggingDaemon },
                    { "LoggingSeverity", loggingSeverity },
                    { "LoggingPrintCategory", loggingPrintCategory },
                    { "LoggingPrintSeverity", loggingPrintSeverity },
                    { "LoggingPrintTime", loggingPrintTime },
                    { "TrustedKey", trustedKeys },
                    { "AclLocalInterfaces", aclLocalInterfaces },
                    { "AclInternalInterfaces", aclInternalInterfaces },
                    { "AclExternalInterfaces", aclExternalInterfaces },
                    { "AclLocalNetworks", aclLocalNetworks },
                    { "AclInternalNetworks", aclInternalNetworks },
                    { "AclExternalNetworks", aclExternalNetworks }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/options", dict);
            };

            Post["/bind/zone"] = x => {
                string name = Request.Form.Name;
                string type = Request.Form.Type;
                string file = Request.Form.File;
                string serialUpdateMethod = Request.Form.NameSerialUpdateMethod;
                string allowUpdate = Request.Form.AllowUpdate;
                string allowQuery = Request.Form.AllowQuery;
                string allowTransfer = Request.Form.AllowTransfer;
                var dict = new Dictionary<string, string> {
                    { "Name", name },
                    { "Type", type },
                    { "File", file },
                    { "NameSerialUpdateMethod", serialUpdateMethod },
                    { "AllowUpdate", allowUpdate },
                    { "AllowQuery", allowQuery },
                    { "AllowTransfer", allowTransfer },
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/bind/zone", dict);
            };

            Post["/bind/zone/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/bind/zone/del", dict);
            };
        }
    }
}