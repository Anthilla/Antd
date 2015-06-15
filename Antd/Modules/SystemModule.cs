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

using Antd.Common;
using Antd.MachineStatus;
using Antd.Status;
using Antd.ViewHelpers;
using Nancy;
using Nancy.Security;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Antd {

    public class SystemModule : NancyModule {

        public SystemModule()
            : base("/system") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Hostname = Command.Launch("hostname", "").output;
                vmod.Domainname = Command.Launch("hostname", "-f").output;
                vmod.Timezone = Command.Launch("timedatectl", "").output;
                vmod.Timeserver = "time.server.net";
                vmod.Language = "English";
                vmod.TCPport = "";
                vmod.MaxProcesses = "2";
                vmod.AlternateHostnames = "";
                vmod.SSHPort = "22";
                return View["_page-system", vmod];
            };

            Get["/mounts"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.MountRunning = Mount.Running;
                vmod.MountAntd = Mount.Antd;
                return View["_page-system-mounts", vmod];
            };

            Get["/sysctl"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Sysctl = VHStatus.Sysctl(Sysctl.Stock, Sysctl.Running, Sysctl.Antd);
                return View["_page-system-sysctl", vmod];
            };

            Get["/conf"] = x => {
                dynamic vmod = new ExpandoObject();
                HashSet<DirItemModel> etcList = new DirectoryLister("/etc", true).FullList2;
                HashSet<DirItemModel> cfgList = new DirectoryLister("/antd/etc", true).FullList2;
                List<dynamic> nl = new List<dynamic>() { };
                foreach (DirItemModel dir in etcList) {
                    dynamic imod = new ExpandoObject();
                    imod.isFile = dir.isFile;
                    imod.etcPath = dir.path;
                    bool hasCfg;
                    string cfgPath;
                    string cfgName;
                    string p = dir.path.ConvertPathToFileName().Replace("D:", "");
                    string c = (from i in cfgList
                                where i.name == p
                                select i.path).FirstOrDefault();
                    if (c == null) {
                        hasCfg = false;
                        cfgPath = "";
                        cfgName = "";
                    }
                    else {
                        hasCfg = true;
                        cfgPath = c;
                        cfgName = Path.GetFileName(c);
                    }
                    imod.hasCfg = hasCfg;
                    imod.cfgPath = cfgPath;
                    imod.cfgName = cfgName;
                    nl.Add(imod);
                }
                vmod.Conf = nl;
                return View["_page-system-conf", vmod];
            };

            Post["/export/file/{path*}"] = x => {
                string path = x.path;
                ConfigEtc.Export(path);
                return Response.AsJson("done");
            };

            Get["/read/file/{path*}"] = x => {
                string path = x.path;
                string text = FileSystem.ReadFile(path.RemoveDriveLetter());
                return Response.AsJson(text);
            };

            Post["/file"] = x => {
                string path = this.Request.Form.FilePath;
                string content = this.Request.Form.FileContent;
                ConfigEtc.EditFile(path, content);
                return Response.AsRedirect("/system");
            };

            Post["/sysctl/{param}/{value}"] = x => {
                string param = x.param;
                string value = x.value;
                var output = Sysctl.Config(param, value);
                return Response.AsJson(output);
            };

            Get["/wizard"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["page-wizard", vmod];
            };
        }
    }
}