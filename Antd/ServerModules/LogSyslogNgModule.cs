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

namespace Antd.ServerModules {
    public class LogSyslogNgModule : NancyModule {

        public LogSyslogNgModule() {
            Get["/syslogng"] = x => {
                var syslogngConfiguration = new SyslogNgConfiguration();
                var syslogngIsActive = syslogngConfiguration.IsActive();
                var model = new PageSyslogNgModel {
                    SyslogNgIsActive = syslogngIsActive,
                    SyslogNgOptions = syslogngConfiguration.Get() ?? new SyslogNgConfigurationModel()
                };
                return JsonConvert.SerializeObject(model);
            };

            Post["/syslogng/set"] = x => {
                var syslogngConfiguration = new SyslogNgConfiguration();
                syslogngConfiguration.Set();
                return HttpStatusCode.OK;
            };

            Post["/syslogng/restart"] = x => {
                var syslogngConfiguration = new SyslogNgConfiguration();
                syslogngConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/syslogng/stop"] = x => {
                var syslogngConfiguration = new SyslogNgConfiguration();
                syslogngConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/syslogng/enable"] = x => {
                var dhcpdConfiguration = new SyslogNgConfiguration();
                dhcpdConfiguration.Enable();
                dhcpdConfiguration.Start();
                return HttpStatusCode.OK;
            };

            Post["/syslogng/disable"] = x => {
                var dhcpdConfiguration = new SyslogNgConfiguration();
                dhcpdConfiguration.Disable();
                dhcpdConfiguration.Stop();
                return HttpStatusCode.OK;
            };

            Post["/syslogng/options"] = x => {
                string threaded = Request.Form.Threaded;
                string chainHostname = Request.Form.ChainHostname;
                string statsFrequency = Request.Form.StatsFrequency;
                string markFrequency = Request.Form.MarkFrequency;
                string checkHostname = Request.Form.CheckHostname;
                string createDirectories = Request.Form.CreateDirectories;
                string dnsCache = Request.Form.DnsCache;
                string keepHostname = Request.Form.KeepHostname;
                string dirAcl = Request.Form.DirAcl;
                string acl = Request.Form.Acl;
                string useDns = Request.Form.UseDns;
                string useFqdn = Request.Form.UseFqdn;
                string rootPath = Request.Form.RootPath;
                string portLevelApplication = Request.Form.PortLevelApplication;
                string portLevelSecurity = Request.Form.PortLevelSecurity;
                string portLevelSystem = Request.Form.PortLevelSystem;
                var model = new SyslogNgConfigurationModel {
                    Threaded = threaded,
                    ChainHostname = chainHostname,
                    StatsFrequency = statsFrequency,
                    MarkFrequency = markFrequency,
                    CheckHostname = checkHostname,
                    CreateDirectories = createDirectories,
                    DnsCache = dnsCache,
                    KeepHostname = keepHostname,
                    DirAcl = dirAcl,
                    Acl = acl,
                    UseDns = useDns,
                    UseFqdn = useFqdn,
                    RootPath = rootPath,
                    PortLevelApplication = portLevelApplication,
                    PortLevelSecurity = portLevelSecurity,
                    PortLevelSystem = portLevelSystem

                };
                var syslogngConfiguration = new SyslogNgConfiguration();
                syslogngConfiguration.Save(model);
                return Response.AsRedirect("/");
            };
        }
    }
}