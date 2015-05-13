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

using Nancy;
using Nancy.Security;
using Antd.Common;
using System.Dynamic;
using System.Linq;
using Antd.MachineStatus;
using System.Collections.Generic;
using System.IO;

namespace Antd {

    public class ConfigModule : NancyModule {

        public ConfigModule()
            : base("/config") {
            this.RequiresAuthentication();

            Get["/file"] = x => {
                dynamic vmod = new ExpandoObject();
                HashSet<DirItemModel> etcList = new DirectoryLister("/etc", true).FullList2;
                HashSet<DirItemModel> cfgList = new DirectoryLister("/cfg/etc", true).FullList2;
                List<dynamic> nl = new List<dynamic>() { };
                foreach (DirItemModel dir in etcList) {
                    dynamic imod = new ExpandoObject();
                    imod.isFile = dir.isFile;
                    imod.etcPath = dir.path;
                    bool hasCfg;
                    string cfgPath;
                    string cfgName;
                    //fix get file name
                    //string simplefilename = Path.GetFileName(dir.path);
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
                vmod.ALL = nl;
                return View["page-config-file", vmod];
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
                return Response.AsRedirect("/config/file");
            };
        }
    }
}
