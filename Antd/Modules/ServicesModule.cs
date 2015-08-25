///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using antdlib.CCTable;
using antdlib.Svcs.Bind;
using antdlib.Svcs.Samba;
using antdlib.ViewBinds;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;

namespace Antd {

    public class ServicesModule : NancyModule {

        public ServicesModule()
            : base("/services") {
            //this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.CurrentContext = Request.Path;
                vmod.CCTable = CCTableRepository.GetAllByContext(Request.Path);
                vmod.Count = CCTableRepository.GetAllByContext(Request.Path).ToArray().Length;
                return View["_page-services", vmod];
            };

            #region SAMBA
            Post["/activate/samba"] = x => {
                SambaConfig.SetReady();
                SambaConfig.MapFile.Render();
                return Response.AsJson(true);
            };

            Post["/refresh/samba"] = x => {
                SambaConfig.MapFile.Render();
                return Response.AsJson(true);
            };

            Post["/reloadconfig/samba"] = x => {
                SambaConfig.ReloadConfig();
                return Response.AsJson(true);
            };

            Post["/update/samba"] = x => {
                var parameters = this.Bind<List<ServiceSamba>>();
                SambaConfig.WriteFile.SaveGlobalConfig(parameters);
                Thread.Sleep(1000);
                SambaConfig.WriteFile.DumpGlobalConfig();
                Thread.Sleep(1000);
                SambaConfig.WriteFile.RewriteSMBCONF();
                return Response.AsRedirect("/services");
            };

            Post["/update/sambashares"] = x => {
                var parameters = this.Bind<List<ServiceSamba>>();
                string file = Request.Form.ShareFile;
                string name = Request.Form.ShareName;
                string query = Request.Form.ShareQueryName;
                SambaConfig.WriteFile.SaveShareConfig(file, name, query, parameters);
                Thread.Sleep(1000);
                SambaConfig.WriteFile.DumpShare(name);
                Thread.Sleep(1000);
                SambaConfig.WriteFile.RewriteSMBCONF();
                return Response.AsRedirect("/services");
            };

            Post["/samba/addparam"] = x => {
                string key = Request.Form.NewParameterKey;
                string value = Request.Form.NewParameterValue;
                SambaConfig.WriteFile.AddParameterToGlobal(key, value);
                Thread.Sleep(1000);
                SambaConfig.WriteFile.RewriteSMBCONF();
                return Response.AsRedirect("/services");
            };

            Post["/samba/addshare"] = x => {
                string name = Request.Form.NewShareName;
                string directory = Request.Form.NewShareDirectory;
                SambaConfig.WriteFile.AddShare(name, directory);
                Thread.Sleep(1000);
                SambaConfig.WriteFile.RewriteSMBCONF();
                return Response.AsRedirect("/services");
            };
            #endregion SAMBA

            #region BIND
            Post["/activate/bind"] = x => {
                BindConfig.SetReady();
                BindConfig.MapFile.Render();
                return Response.AsJson(true);
            };

            Post["/refresh/bind"] = x => {
                BindConfig.MapFile.Render();
                return Response.AsJson(true);
            };

            Post["/reloadconfig/bind"] = x => {
                BindConfig.ReloadConfig();
                return Response.AsJson(true);
            };

            Post["/update/bind/{section}"] = x => {
                var section = (string)x.section;
                var parameters = this.Bind<List<ServiceBind>>();
                BindConfig.WriteFile.SaveGlobalConfig(section, parameters);
                Thread.Sleep(1000);
                BindConfig.WriteFile.DumpGlobalConfig();
                return Response.AsRedirect("/services");
            };

            #endregion BIND

        }
    }
}