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

using System.Collections.Generic;
using antdlib.common;
using antdlib.models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.Modules {
    public class AntdSambaModule : NancyModule {

        private readonly ApiConsumer _api = new ApiConsumer();

        public AntdSambaModule() {
            Get["/samba"] = x => {
                var model = _api.Get<PageSambaModel>($"http://127.0.0.1:{Application.ServerPort}/samba");
                var json = JsonConvert.SerializeObject(model);
                return json;
            };

            Post["/samba/set"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/samba/set");
            };

            Post["/samba/restart"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/samba/restart");
            };

            Post["/samba/stop"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/samba/stop");
            };

            Post["/samba/enable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/samba/enable");
            };

            Post["/samba/disable"] = x => {
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/samba/disable");
            };

            Post["/samba/options"] = x => {
                string dosCharset = Request.Form.DosCharset;
                string workgroup = Request.Form.Workgroup;
                string serverString = Request.Form.ServerString;
                string mapToGuest = Request.Form.MapToGuest;
                string obeyPamRestrictions = Request.Form.ObeyPamRestrictions;
                string guestAccount = Request.Form.GuestAccount;
                string pamPasswordChange = Request.Form.PamPasswordChange;
                string passwdProgram = Request.Form.PasswdProgram;
                string unixPasswordSync = Request.Form.UnixPasswordSync;
                string resetOnZeroVc = Request.Form.ResetOnZeroVc;
                string hostnameLookups = Request.Form.HostnameLookups;
                string loadPrinters = Request.Form.LoadPrinters;
                string printcapName = Request.Form.PrintcapName;
                string disableSpoolss = Request.Form.DisableSpoolss;
                string templateShell = Request.Form.TemplateShell;
                string winbindEnumUsers = Request.Form.WinbindEnumUsers;
                string winbindEnumGroups = Request.Form.WinbindEnumGroups;
                string winbindUseDefaultDomain = Request.Form.WinbindUseDefaultDomain;
                string winbindNssInfo = Request.Form.WinbindNssInfo;
                string winbindRefreshTickets = Request.Form.WinbindRefreshTickets;
                string winbindNormalizeNames = Request.Form.WinbindNormalizeNames;
                string recycleTouch = Request.Form.RecycleTouch;
                string recycleKeeptree = Request.Form.RecycleKeeptree;
                string recycleRepository = Request.Form.RecycleRepository;
                string nfs4Chown = Request.Form.Nfs4Chown;
                string nfs4Acedup = Request.Form.Nfs4Acedup;
                string nfs4Mode = Request.Form.Nfs4Mode;
                string shadowFormat = Request.Form.ShadowFormat;
                string shadowLocaltime = Request.Form.ShadowLocaltime;
                string shadowSort = Request.Form.ShadowSort;
                string shadowSnapdir = Request.Form.ShadowSnapdir;
                string rpcServerDefault = Request.Form.RpcServerDefault;
                string rpcServerSvcctl = Request.Form.RpcServerSvcctl;
                string rpcServerSrvsvc = Request.Form.RpcServerSrvsvc;
                string rpcServerEventlog = Request.Form.RpcServerEventlog;
                string rpcServerNtsvcs = Request.Form.RpcServerNtsvcs;
                string rpcServerWinreg = Request.Form.RpcServerWinreg;
                string rpcServerSpoolss = Request.Form.RpcServerSpoolss;
                string rpcDaemonSpoolssd = Request.Form.RpcDaemonSpoolssd;
                string rpcServerTcpip = Request.Form.RpcServerTcpip;
                string idmapConfigBackend = Request.Form.IdmapConfigBackend;
                string readOnly = Request.Form.ReadOnly;
                string guestOk = Request.Form.GuestOk;
                string aioReadSize = Request.Form.AioReadSize;
                string aioWriteSize = Request.Form.AioWriteSize;
                string eaSupport = Request.Form.EaSupport;
                string directoryNameCacheSize = Request.Form.DirectoryNameCacheSize;
                string caseSensitive = Request.Form.CaseSensitive;
                string mapReadonly = Request.Form.MapReadonly;
                string storeDosAttributes = Request.Form.StoreDosAttributes;
                string wideLinks = Request.Form.WideLinks;
                string dosFiletimeResolution = Request.Form.DosFiletimeResolution;
                string vfsObjects = Request.Form.VfsObjects;
                var dict = new Dictionary<string, string> {
                    { "DosCharset", dosCharset },
                    { "Workgroup", workgroup },
                    { "ServerString", serverString },
                    { "MapToGuest", mapToGuest },
                    { "ObeyPamRestrictions", obeyPamRestrictions },
                    { "GuestAccount", guestAccount },
                    { "PamPasswordChange", pamPasswordChange },
                    { "PasswdProgram", passwdProgram },
                    { "UnixPasswordSync", unixPasswordSync },
                    { "ResetOnZeroVc", resetOnZeroVc },
                    { "HostnameLookups", hostnameLookups },
                    { "LoadPrinters", loadPrinters },
                    { "PrintcapName", printcapName },
                    { "DisableSpoolss", disableSpoolss },
                    { "TemplateShell", templateShell },
                    { "WinbindEnumUsers", winbindEnumUsers },
                    { "WinbindEnumGroups", winbindEnumGroups },
                    { "WinbindUseDefaultDomain", winbindUseDefaultDomain },
                    { "WinbindNssInfo", winbindNssInfo },
                    { "WinbindRefreshTickets", winbindRefreshTickets },
                    { "WinbindNormalizeNames", winbindNormalizeNames },
                    { "RecycleTouch", recycleTouch },
                    { "RecycleKeeptree", recycleKeeptree },
                    { "RecycleRepository", recycleRepository },
                    { "Nfs4Chown", nfs4Chown },
                    { "Nfs4Acedup", nfs4Acedup },
                    { "Nfs4Mode", nfs4Mode },
                    { "ShadowFormat", shadowFormat },
                    { "ShadowLocaltime", shadowLocaltime },
                    { "ShadowSort", shadowSort },
                    { "ShadowSnapdir", shadowSnapdir },
                    { "RpcServerDefault", rpcServerDefault },
                    { "RpcServerSvcctl", rpcServerSvcctl },
                    { "RpcServerSrvsvc", rpcServerSrvsvc },
                    { "RpcServerEventlog", rpcServerEventlog },
                    { "RpcServerNtsvcs", rpcServerNtsvcs },
                    { "RpcServerWinreg", rpcServerWinreg },
                    { "RpcServerSpoolss", rpcServerSpoolss },
                    { "RpcDaemonSpoolssd", rpcDaemonSpoolssd },
                    { "RpcServerTcpip", rpcServerTcpip },
                    { "IdmapConfigBackend", idmapConfigBackend },
                    { "ReadOnly", readOnly },
                    { "GuestOk", guestOk },
                    { "AioReadSize", aioReadSize },
                    { "AioWriteSize", aioWriteSize },
                    { "EaSupport", eaSupport },
                    { "DirectoryNameCacheSize", directoryNameCacheSize },
                    { "CaseSensitive", caseSensitive },
                    { "MapReadonly", mapReadonly },
                    { "StoreDosAttributes", storeDosAttributes },
                    { "WideLinks", wideLinks },
                    { "DosFiletimeResolution", dosFiletimeResolution },
                    { "VfsObjects", vfsObjects },
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/ca/options", dict);
            };

            Post["/samba/resource"] = x => {
                string name = Request.Form.Name;
                string path = Request.Form.Path;
                string comment = Request.Form.Comment;
                var dict = new Dictionary<string, string> {
                    { "Name", name },
                    { "Path", path },
                    { "Comment", comment }
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/samba/resource", dict);
            };

            Post["/samba/resource/del"] = x => {
                string guid = Request.Form.Guid;
                var dict = new Dictionary<string, string> {
                    { "Guid", guid },
                };
                return _api.Post($"http://127.0.0.1:{Application.ServerPort}/samba/resource/del", dict);
            };
        }
    }
}