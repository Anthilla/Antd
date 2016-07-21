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
using System.Dynamic;
using System.Linq;
using antdlib;
using antdlib.common;
using antdlib.Certificate;
using antdlib.Log;
using antdlib.views;
using Antd.Database;
using Antd.Info;
using Antd.Storage;
using Antd.Svcs.Dhcp;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class HomeModule : CoreModule {

        public HomeModule() {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic viewModel = new ExpandoObject();

                viewModel.AntdContext = new[] {
                    "Info",
                    "Host",
                    "Network",
                    "DnsClient",
                    "Firewall",
                    "DnsServer",
                    "Proxy",
                    "Acl",
                    "Cron",
                    "Storage",
                    "VM",
                    "Mount",
                    "Users",
                    "Samba",
                };

                var os = Terminal.Execute("uname -a");
                viewModel.VersionOS = os;

                var machineInfo = new MachineInfo();
                viewModel.Meminfo = machineInfo.GetMeminfo();
                viewModel.Cpuinfo = machineInfo.GetCpuinfo();
                viewModel.LosetupInfo = machineInfo.GetLosetup();
                viewModel.AosInfo = machineInfo.GetAosrelease();
                viewModel.SystemComponents = machineInfo.GetSystemComponentModels();

                var networkInterfaces = new NetworkInterfaceRepository().GetAll().ToList();
                viewModel.NetworkPhysicalIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Physical.ToString()).OrderBy(_ => _.Name);
                viewModel.NetworkVirtualIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Virtual.ToString()).OrderBy(_ => _.Name);
                viewModel.NetworkBondIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Bond.ToString()).OrderBy(_ => _.Name);
                viewModel.NetworkBridgeIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Bridge.ToString()).OrderBy(_ => _.Name);

                viewModel.FirewallCommands = new FirewallListRepository().GetAll();
                viewModel.DhcpdStatus = DhcpConfig.IsActive;
                //var dhcpdModel = DhcpConfig.MapFile.Get();
                //if (dhcpdModel != null) {
                //    viewModel.DhcpdGetGlobal = dhcpdModel.DhcpGlobal;
                //    viewModel.DhcpdGetPrefix6 = dhcpdModel.DhcpPrefix6;
                //    viewModel.DhcpdGetRange = dhcpdModel.DhcpRange;
                //    viewModel.DhcpdGetRange6 = dhcpdModel.DhcpRange6;
                //    viewModel.DhcpdGetKeys = dhcpdModel.DhcpKey;
                //    viewModel.DhcpdGetSubnet = dhcpdModel.DhcpSubnet;
                //    viewModel.DhcpdGetSubnet6 = dhcpdModel.DhcpSubnet6;
                //    viewModel.DhcpdGetHost = dhcpdModel.DhcpHost;
                //    viewModel.DhcpdGetFailover = dhcpdModel.DhcpFailover;
                //    viewModel.DhcpdGetSharedNetwork = dhcpdModel.DhcpSharedNetwork;
                //    viewModel.DhcpdGetGroup = dhcpdModel.DhcpGroup;
                //    viewModel.DhcpdGetClass = dhcpdModel.DhcpClass;
                //    viewModel.DhcpdGetSubclass = dhcpdModel.DhcpSubclass;
                //    viewModel.DhcpdGetLogging = dhcpdModel.DhcpLogging;
                //}

                viewModel.MacAddressList = new MacAddressRepository().GetAll();

                viewModel.Mounts = new MountRepository().GetAll();

                var scheduledJobs = new TimerRepository().GetAll();
                viewModel.Jobs = scheduledJobs?.ToList().OrderBy(_ => _.Alias);

                viewModel.DisksList = Disks.List();
                viewModel.ZpoolList = Zpool.List();
                viewModel.ZfsList = Zfs.List();
                viewModel.ZfsSnap = ZfsSnap.List();

                viewModel.VMList = antdlib.Virsh.Virsh.GetVmList();

                //todo check next parameters
                viewModel.SSHPort = "22";
                viewModel.AuthStatus = ApplicationSetting.TwoFactorAuth();

                return View["antd/page-antd", viewModel];
            };

            Get["/log"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.AntdContext = new[] {
                    "AntdLog",
                    "SystemLog",
                    "LogReport",
                };

                viewModel.Logs = ConsoleLogger.GetAll();
                viewModel.LogReports = Journalctl.Report.Get();
                return View["antd/page-log", viewModel];
            };

            Get["/ca"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.AntdContext = new[] {
                    "Manage",
                };

                viewModel.SslStatus = "Enabled";
                viewModel.SslStatusAction = "Disable";
                if (ApplicationSetting.Ssl() == "no") {
                    viewModel.SslStatus = "Disabled";
                    viewModel.SslStatusAction = "Enable";
                }
                viewModel.CertificatePath = ApplicationSetting.CertificatePath();
                viewModel.CaStatus = "Enabled";
                if (ApplicationSetting.CertificateAuthority() == "no") {
                    viewModel.CaStatus = "Disabled";
                }
                viewModel.CaIsActive = CertificateAuthority.IsActive;
                //viewModel.Certificates = CertificateRepository.GetAll();

                return View["antd/page-ca", viewModel];
            };

            Get["/vnc"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.Connections = new Dictionary<string, string>();
                return View["antd/page-vnc", vmod];
            };

            Post["/network/import/if"] = x => {
                new NetworkInterfaceRepository().Import();
                return HttpStatusCode.OK;
            };
        }
    }
}