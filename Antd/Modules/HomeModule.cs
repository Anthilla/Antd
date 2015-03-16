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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd {

    public class HomeModule : NancyModule {

        public HomeModule() {
            this.RequiresAuthentication();

            Get["/"] = x => {
                return Response.AsRedirect("/anthillasp");
            };

            Get["/directory/tree/{path*}"] = x => {
                var p = x.path;
                HashSet<string> directories = new DirectoryLister("/" + p).FullList;
                return View["page-directories", directories.ToList()];
            };

            //Get["/directory/watch/"] = x => {
            //    return View["page-directories-watch"];
            //};

            Get["/info"] = x => {
                return View["page-info"];
            };

            Get["/paramlist"] = x => {
                return View["page-paramlist"];
            };

            Get["/meminfo"] = x => {
                List<MeminfoModel> meminfo = Meminfo.GetModel();
                if (meminfo == null) {
                    return View["page-meminfo"];
                }
                return View["page-meminfo", meminfo];
            };

            Get["/meminfo/text"] = x => {
                var meminfo = Meminfo.GetText();
                return Response.AsJson(meminfo);
            };

            Get["/network"] = x => {
                NetworkModel network = NetworkInfo.GetModel();
                if (network == null) {
                    return View["page-network"];
                }
                return View["page-network", network];
            };

            Post["/network"] = x => {
                string hostname = (string)this.Request.Form.Hostname;
                NetworkModel network = Antd.NetworkInfo.GetModel(hostname);
                if (network == null) {
                    return View["page-network"];
                }
                return View["page-network", network];
            };

            Get["/cpuinfo"] = x => {
                List<CpuinfoModel> cpuinfo = Cpuinfo.GetModel();
                if (cpuinfo == null) {
                    return View["page-cpuinfo"];
                }
                return View["page-cpuinfo", cpuinfo];
            };

            Get["/cpuinfo/text"] = x => {
                var cpuinfo = Cpuinfo.GetText();
                return Response.AsJson(cpuinfo);
            };

            Get["/version"] = x => {
                VersionModel version = Version.GetModel();
                if (version == null) {
                    return View["page-version"];
                }
                return View["page-version", version];
            };

            Get["/version/text"] = x => {
                var version = Version.GetText();
                return Response.AsJson(version);
            };

            Get["/dmidecode"] = x => {
                CommandModel command = Command.Launch("dmidecode", "");
                return View["page-dmidecode", command];
            };

            Get["/dmidecode/uuid"] = x => {
                CommandModel command = Command.Launch("dmidecode", "");
                string uuid = Dmidecode.GetUUID(command.outputTable);
                return Response.AsJson(uuid);
            };

            Get["/dmidecode/text"] = x => {
                CommandModel command = Command.Launch("dmidecode", "");
                return JsonConvert.SerializeObject(command.output);
            };

            Get["/ifconfig"] = x => {
                CommandModel command = Command.Launch("ifconfig", "");
                return View["page-ifconfig", command];
            };

            Get["/ifconfig/ether"] = x => {
                return JsonConvert.SerializeObject(Ifconfig.GetEther());
            };

            Get["/ifconfig/text"] = x => {
                CommandModel command = Command.Launch("ifconfig", "");
                return JsonConvert.SerializeObject(command.output);
            };

            Get["/command"] = x => {
                return View["page-command"];
            };

            Post["/command"] = x => {
                string file = (string)this.Request.Form.File;
                string args = (string)this.Request.Form.Arguments;

                CommandModel command = Command.Launch(file, args);
                return View["page-command", command];
            };

            Get["/procs"] = x => {
                List<ProcModel> procs = Proc.All;
                return View["page-procs", procs];
            };

            Get["/procs/text"] = x => {
                CommandModel command = Command.Launch("ps", "-aef");
                return JsonConvert.SerializeObject(command.output.Replace("\"", ""));
            };

            Post["/procs/kill"] = x => {
                string pid = (string)this.Request.Form.data;
                CommandModel command = Command.Launch("kill", pid);
                return Response.AsRedirect("/procs");
            };
        }
    }
}
