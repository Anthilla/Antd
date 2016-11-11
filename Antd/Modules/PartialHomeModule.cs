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
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using antd.commands;
using antdlib.common;
using antdlib.common.Tool;
using antdlib.views;
using antdlib.Virsh;
using Antd.Database;
using Antd.Firewall;
using Antd.Gluster;
using Antd.Info;
using Antd.Storage;
using Antd.SystemdTimer;
using Nancy.Security;

namespace Antd.Modules {
    public class PartialHomeModule : CoreModule {

        private static readonly DhcpServerOptionsRepository DhcpServerOptionsRepository = new DhcpServerOptionsRepository();
        private static readonly DhcpServerSubnetRepository DhcpServerSubnetRepository = new DhcpServerSubnetRepository();
        private static readonly DhcpServerClassRepository DhcpServerClassRepository = new DhcpServerClassRepository();
        private static readonly DhcpServerPoolRepository DhcpServerPoolRepository = new DhcpServerPoolRepository();
        private static readonly DhcpServerReservationRepository DhcpServerReservationRepository = new DhcpServerReservationRepository();
        private static readonly UserRepository UserRepository = new UserRepository();
        private readonly Bash _bash = new Bash();
        private readonly CommandLauncher _launcher = new CommandLauncher();

        public PartialHomeModule() {
            this.RequiresAuthentication();

            Get["/part/load/info"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.VersionOS = _bash.Execute("uname -a");
                viewModel.Meminfo = MachineInfo.GetMeminfo();
                viewModel.Cpuinfo = MachineInfo.GetCpuinfo();
                viewModel.AosInfo = MachineInfo.GetAosrelease();
                viewModel.Uptime = MachineInfo.GetUptime();
                viewModel.Free = MachineInfo.GetFree();
                viewModel.GentooRelease = _launcher.Launch("cat-etc-gentoorel");
                viewModel.LsbRelease = _launcher.Launch("cat-etc-lsbrel");
                viewModel.OsRelease = _launcher.Launch("cat-etc-osrel");
                return View["antd/page-antd-info", viewModel];
            };

            Get["/part/load/system"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.SystemComponents = MachineInfo.GetSystemComponentModels();
                viewModel.LosetupInfo = MachineInfo.GetLosetup();
                viewModel.AntdUpdateCheck = _launcher.Launch("mono-antdsh-update-check");
                return View["antd/page-antd-system", viewModel];
            };

            Get["/part/load/host"] = x => {
                dynamic viewModel = new ExpandoObject();

                var hostnamectl = _launcher.Launch("hostnamectl");
                ConsoleLogger.Log(hostnamectl);
                ConsoleLogger.Log(hostnamectl.Count);

                viewModel.StaticHostname = _launcher.Launch("hostnamectl-get-hostname");
                viewModel.IconName = _launcher.Launch("hostnamectl-get-iconname");
                viewModel.Chassis = _launcher.Launch("hostnamectl-get-chassis");
                viewModel.Deployment = _launcher.Launch("hostnamectl-get-deployment");
                viewModel.Location = _launcher.Launch("hostnamectl-get-location");
                viewModel.MachineID = _launcher.Launch("hostnamectl-get-machineid");
                viewModel.BootID = _launcher.Launch("hostnamectl-get-bootid");
                viewModel.Virtualization = _launcher.Launch("hostnamectl-get-virtualization");
                viewModel.OS = _launcher.Launch("hostnamectl-get-os");
                viewModel.Kernel = _launcher.Launch("hostnamectl-get-kernel");
                viewModel.Architecture = _launcher.Launch("hostnamectl-get-arch");
                return View["antd/page-antd-host", viewModel];
            };

            Get["/part/load/time"] = x => {
                dynamic viewModel = new ExpandoObject();
                var timezones = _bash.Execute("timedatectl list-timezones --no-pager").SplitBash();
                viewModel.Timezones = timezones;

                var timedatectl = _launcher.Launch("timedatectl");
                ConsoleLogger.Log(timedatectl);

                viewModel.LocalTime = _launcher.Launch("timedatectl-get-localtime");
                viewModel.UnivTime = _launcher.Launch("timedatectl-get-univtime");
                viewModel.RTCTime = _launcher.Launch("timedatectl-get-rtctime");
                viewModel.Timezone = _launcher.Launch("timedatectl-get-timezone");
                viewModel.Nettimeon = _launcher.Launch("timedatectl-get-nettimeon");
                viewModel.Ntpsync = _launcher.Launch("timedatectl-get-ntpsync");
                viewModel.Rtcintz = _launcher.Launch("timedatectl-get-rtcintz");
                return View["antd/page-antd-time", viewModel];
            };

            Get["/part/load/ns"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Resolv = _launcher.Launch("cat-etc-resolv");
                viewModel.Hostname = _launcher.Launch("cat-etc-hostname");
                viewModel.Hosts = _launcher.Launch("cat-etc-hosts");
                viewModel.ResolvNetworks = _launcher.Launch("cat-etc-networks");
                return View["antd/page-antd-dns-client", viewModel];
            };

            Get["/part/load/dhcp"] = x => {
                dynamic viewModel = new ExpandoObject();
                var dhcpdIsActive = DhcpServerOptionsRepository.Get() != null && DhcpServerSubnetRepository.Get() != null;
                viewModel.DhcpdIsActive = dhcpdIsActive;
                viewModel.DhcpdOptions = DhcpServerOptionsRepository.Get();
                viewModel.DhcpdSubnet = DhcpServerSubnetRepository.Get();
                viewModel.DhcpdClass = DhcpServerClassRepository.GetAll();
                viewModel.DhcpdPools = DhcpServerPoolRepository.GetAll();
                viewModel.DhcpdReservation = DhcpServerReservationRepository.GetAll();
                return View["antd/page-antd-dhcp", viewModel];
            };

            Get["/part/load/net"] = x => {
                dynamic viewModel = new ExpandoObject();
                var networkInterfaces = new NetworkInterfaceRepository().GetAll().ToList();
                var phyIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Physical.ToString()).OrderBy(_ => _.Name);
                viewModel.NetworkPhysicalIf = phyIf;
                var brgIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Bridge.ToString()).OrderBy(_ => _.Name);
                viewModel.NetworkBridgeIf = brgIf;
                var bndIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Bond.ToString()).OrderBy(_ => _.Name);
                viewModel.NetworkBondIf = bndIf;
                var vrtIf = networkInterfaces.Where(_ => _.Type == NetworkInterfaceType.Virtual.ToString()).OrderBy(_ => _.Name).ToList();
                foreach(var v in vrtIf) {
                    if(phyIf.Contains(v) || brgIf.Contains(v) || bndIf.Contains(v)) {
                        vrtIf.Remove(v);
                    }
                }
                viewModel.NetworkVirtualIf = vrtIf;
                return View["antd/page-antd-network", viewModel];
            };

            Get["/part/load/fw"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.NftTables = NfTables.Tables();
                viewModel.MacAddressList = new MacAddressRepository().GetAll();
                return View["antd/page-antd-firewall", viewModel];
            };

            Get["/part/load/cron"] = x => {
                dynamic viewModel = new ExpandoObject();
                var scheduledJobs = Timers.GetAll();
                viewModel.Jobs = scheduledJobs?.ToList().OrderBy(_ => _.Alias);
                return View["_partial/part-scheduler", viewModel];
            };

            Get["/part/load/storage"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Mounts = new MountRepository().GetAll();
                viewModel.Overlay = OverlayWatcher.ChangedDirectories;
                viewModel.DisksList = Disks.List();
                viewModel.ZpoolList = Zpool.List();
                viewModel.ZfsList = Zfs.List();
                viewModel.ZfsSnap = ZfsSnap.List();
                viewModel.ZpoolHistory = Zpool.History();
                return View["_partial/part-antd-storage", viewModel];
            };

            Get["/part/load/sync"] = x => {
                dynamic viewModel = new ExpandoObject();
                var glusterConfig = GlusterConfiguration.Get();
                viewModel.GlusterName = glusterConfig.Name;
                viewModel.GlusterPath = glusterConfig.Path;
                viewModel.GlusterNodes = glusterConfig.Nodes;
                viewModel.GlusterVolumes = glusterConfig.Volumes;
                return View["antd/page-antd-gluster", viewModel];
            };

            Get["/part/load/vm"] = x => {
                dynamic viewModel = new ExpandoObject();
                var vmList = Virsh.GetVmList();
                viewModel.VMListAny = vmList.Any();
                viewModel.VMList = vmList;
                return View["antd/page-antd-vm", viewModel];
            };

            Get["/part/load/users"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Users = UserRepository.GetAll().OrderBy(_ => _.Alias);
                return View["antd/page-antd-users", viewModel];
            };

            Get["/part/load/ssh"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Keys = null;
                return View["_partial/part-ssh", viewModel];
            };
        }
    }
}