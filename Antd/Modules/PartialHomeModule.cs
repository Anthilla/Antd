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

using System.Dynamic;
using System.Linq;
using antd.commands;
using antdlib.common;
using antdlib.common.Tool;
using antdlib.views;
using Antd.Database;
using Antd.Firewall;
using Antd.Gluster;
using Antd.Info;
using Antd.Storage;
using Antd.SystemdTimer;
using Nancy.Security;

namespace Antd.Modules {
    public class PartialHomeModule : CoreModule {



        private readonly UserRepository _userRepository = new UserRepository();
        private readonly Bash _bash = new Bash();
        private readonly CommandLauncher _launcher = new CommandLauncher();
        private readonly Virsh.Virsh _virsh = new Virsh.Virsh();
        private readonly NfTables _nfTables = new NfTables();
        private readonly GlusterConfiguration _glusterConfiguration = new GlusterConfiguration();
        private readonly MachineInfo _machineInfo = new MachineInfo();
        private readonly ZfsSnap _zfsSnap = new ZfsSnap();
        private readonly Zfs _zfs = new Zfs();
        private readonly Timers _timers = new Timers();
        private readonly Zpool _zpool = new Zpool();


        public PartialHomeModule() {
            this.RequiresAuthentication();

            Get["/part/info"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.VersionOS = _bash.Execute("uname -a");
                viewModel.AosInfo = _machineInfo.GetAosrelease();
                viewModel.Uptime = _machineInfo.GetUptime();
                viewModel.GentooRelease = _launcher.Launch("cat-etc-gentoorel");
                viewModel.LsbRelease = _launcher.Launch("cat-etc-lsbrel");
                viewModel.OsRelease = _launcher.Launch("cat-etc-osrel");
                return View["antd/part/page-antd-info", viewModel];
            };

            Get["/part/info/memory"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Meminfo = _machineInfo.GetMeminfo();
                viewModel.Free = _machineInfo.GetFree();
                return View["antd/part/page-antd-info-memory", viewModel];
            };

            Get["/part/info/cpu"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Cpuinfo = _machineInfo.GetCpuinfo();
                return View["antd/part/page-antd-info-cpu", viewModel];
            };

            Get["/part/system"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.SystemComponents = _machineInfo.GetSystemComponentModels();
                return View["antd/part/page-antd-system", viewModel];
            };

            Get["/part/system/losetup"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.LosetupInfo = _machineInfo.GetLosetup();
                return View["antd/part/page-antd-system-losetup", viewModel];
            };

            Get["/part/system/update"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.AntdUpdateCheck = _launcher.Launch("mono-antdsh-update-check");
                return View["antd/part/page-antd-system-update", viewModel];
            };

            Get["/part/host"] = x => {
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
                return View["antd/part/page-antd-host", viewModel];
            };

            Get["/part/time"] = x => {
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
                return View["antd/part/page-antd-time", viewModel];
            };

            Get["/part/ns"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Resolv = _launcher.Launch("cat-etc-resolv");
                viewModel.Hostname = _launcher.Launch("cat-etc-hostname");
                viewModel.Hosts = _launcher.Launch("cat-etc-hosts");
                viewModel.Networks = _launcher.Launch("cat-etc-networks");
                return View["antd/part/page-antd-ns", viewModel];
            };

            Get["/part/named"] = x => {
                dynamic viewModel = new ExpandoObject();
                var bindServerOptionsRepository = new BindServerOptionsRepository();
                var bindServerZoneRepository = new BindServerZoneRepository();
                var bindIsActive = bindServerOptionsRepository.Get() != null;
                viewModel.BindIsActive = bindIsActive;
                var options = new BindServerOptionsModel(bindServerOptionsRepository.Get());
                viewModel.BindOptions = bindIsActive ? options : bindServerOptionsRepository.Default;
                viewModel.BindZones = bindServerZoneRepository.GetAll();
                return View["antd/part/page-antd-bind", viewModel];
            };

            Get["/part/dhcp"] = x => {
                dynamic viewModel = new ExpandoObject();
                var dhcpServerOptionsRepository = new DhcpServerOptionsRepository();
                var dhcpServerSubnetRepository = new DhcpServerSubnetRepository();
                var dhcpServerClassRepository = new DhcpServerClassRepository();
                var dhcpServerPoolRepository = new DhcpServerPoolRepository();
                var dhcpServerReservationRepository = new DhcpServerReservationRepository();
                var dhcpdIsActive = dhcpServerOptionsRepository.Get() != null && dhcpServerSubnetRepository.Get() != null;
                viewModel.DhcpdIsActive = dhcpdIsActive;
                viewModel.DhcpdOptions = dhcpServerOptionsRepository.Get();
                viewModel.DhcpdSubnet = dhcpServerSubnetRepository.Get();
                viewModel.DhcpdClass = dhcpServerClassRepository.GetAll();
                viewModel.DhcpdPools = dhcpServerPoolRepository.GetAll();
                viewModel.DhcpdReservation = dhcpServerReservationRepository.GetAll();
                return View["antd/part/page-antd-dhcp", viewModel];
            };

            Get["/part/net"] = x => {
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
                return View["antd/part/page-antd-network", viewModel];
            };

            Get["/part/fw"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.NftTables = _nfTables.Tables();
                viewModel.MacAddressList = new MacAddressRepository().GetAll();
                return View["antd/part/page-antd-firewall", viewModel];
            };

            Get["/part/cron"] = x => {
                dynamic viewModel = new ExpandoObject();
                var scheduledJobs = _timers.GetAll();
                viewModel.Jobs = scheduledJobs?.ToList().OrderBy(_ => _.Alias);
                return View["antd/part/page-antd-scheduler", viewModel];
            };

            Get["/part/storage"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Mounts = new MountRepository().GetAll();
                viewModel.Overlay = OverlayWatcher.ChangedDirectories;
                viewModel.DisksList = Disks.List();
                viewModel.ZpoolList = _zpool.List();
                viewModel.ZfsList = _zfs.List();
                viewModel.ZfsSnap = _zfsSnap.List();
                viewModel.ZpoolHistory = _zpool.History();
                return View["antd/part/page-antd-storage", viewModel];
            };

            Get["/part/sync"] = x => {
                dynamic viewModel = new ExpandoObject();
                var glusterConfig = _glusterConfiguration.Get();
                viewModel.GlusterName = glusterConfig.Name;
                viewModel.GlusterPath = glusterConfig.Path;
                viewModel.GlusterNodes = glusterConfig.Nodes;
                viewModel.GlusterVolumes = glusterConfig.Volumes;
                return View["antd/part/page-antd-gluster", viewModel];
            };

            Get["/part/vm"] = x => {
                dynamic viewModel = new ExpandoObject();
                var vmList = _virsh.GetVmList();
                viewModel.VMListAny = vmList.Any();
                viewModel.VMList = vmList;
                return View["antd/part/page-antd-vm", viewModel];
            };

            Get["/part/users"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Users = _userRepository.GetAll().OrderBy(_ => _.Alias);
                return View["antd/part/page-antd-users", viewModel];
            };

            Get["/part/ssh"] = x => {
                dynamic viewModel = new ExpandoObject();
                viewModel.Keys = null;
                return View["antd/part/page-antd-ssh", viewModel];
            };
        }
    }
}