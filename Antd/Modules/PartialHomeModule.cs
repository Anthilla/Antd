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
using System.Dynamic;
using System.Linq;
using antd.commands;
using antdlib.common.Tool;
using Antd.Bind;
using Antd.Database;
using Antd.Dhcpd;
using Antd.Firewall;
using Antd.Gluster;
using Antd.Info;
using Antd.MountPoint;
using Antd.Network;
using Antd.Overlay;
using Antd.Samba;
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
                var hostnamectl = _launcher.Launch("hostnamectl").ToList();
                var ssoree = StringSplitOptions.RemoveEmptyEntries;
                viewModel.StaticHostname = hostnamectl.First(_=>_.Contains("Transient hostname:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.IconName = hostnamectl.First(_=>_.Contains("Icon name:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Chassis = hostnamectl.First(_=>_.Contains("Chassis:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Deployment = hostnamectl.First(_=>_.Contains("Deployment:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Location = hostnamectl.First(_=>_.Contains("Location:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.MachineID = hostnamectl.First(_=>_.Contains("Machine ID:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.BootID = hostnamectl.First(_=>_.Contains("Boot ID:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Virtualization = hostnamectl.First(_=>_.Contains("Virtualization:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.OS = hostnamectl.First(_=>_.Contains("Operating System:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Kernel = hostnamectl.First(_=>_.Contains("Kernel:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Architecture = hostnamectl.First(_=>_.Contains("Architecture:")).Split(new []{":"}, 2, ssoree)[1];
                return View["antd/part/page-antd-host", viewModel];
            };

            Get["/part/time"] = x => {
                dynamic viewModel = new ExpandoObject();
                var timezones = _bash.Execute("timedatectl list-timezones --no-pager").SplitBash();
                viewModel.Timezones = timezones;
                var timedatectl = _launcher.Launch("timedatectl").ToList();
                var ssoree = StringSplitOptions.RemoveEmptyEntries;
                viewModel.LocalTime = timedatectl.First(_ => _.Contains("Local time:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.UnivTime = timedatectl.First(_ => _.Contains("Universal time:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.RTCTime = timedatectl.First(_ => _.Contains("RTC time:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Timezone = timedatectl.First(_ => _.Contains("Time zone:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Nettimeon = timedatectl.First(_ => _.Contains("Network time on:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Ntpsync = timedatectl.First(_ => _.Contains("NTP synchronized:")).Split(new []{":"}, 2, ssoree)[1];
                viewModel.Rtcintz = timedatectl.First(_ => _.Contains("RTC in local TZ:")).Split(new []{":"}, 2, ssoree)[1];
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
                var bindConfiguration = new BindConfiguration();
                var bindIsActive = bindConfiguration.IsActive();
                viewModel.BindIsActive = bindIsActive;
                viewModel.BindOptions = bindConfiguration.Get();
                viewModel.BindZones = bindConfiguration.Get()?.Zones;
                return View["antd/part/page-antd-bind", viewModel];
            };

            Get["/part/dhcp"] = x => {
                dynamic viewModel = new ExpandoObject();
                var dhcpdConfiguration = new DhcpdConfiguration();
                var dhcpdIsActive = dhcpdConfiguration.IsActive();
                viewModel.DhcpdIsActive = dhcpdIsActive;
                viewModel.DhcpdOptions = dhcpdConfiguration.Get();
                viewModel.DhcpdSubnet = dhcpdConfiguration.Get();
                viewModel.DhcpdClass = dhcpdConfiguration.Get()?.Classes;
                viewModel.DhcpdPools = dhcpdConfiguration.Get()?.Pools;
                viewModel.DhcpdReservation = dhcpdConfiguration.Get()?.Reservations;
                return View["antd/part/page-antd-dhcp", viewModel];
            };

            Get["/part/dhcp/leases"] = x => {
                dynamic viewModel = new ExpandoObject();
                var dhcpdLeases = new DhcpdLeases();
                var list = dhcpdLeases.List();
                viewModel.DhcpdLeases = list;
                viewModel.EmptyList = !list.Any();
                return View["antd/part/page-antd-dhcp-leases", viewModel];
            };

            Get["/part/samba"] = x => {
                dynamic viewModel = new ExpandoObject();
                var sambaConfiguration = new SambaConfiguration();
                var sambaIsActive = sambaConfiguration.IsActive();
                viewModel.SambaIsActive = sambaIsActive;
                viewModel.SambaOptions = sambaConfiguration.Get();
                viewModel.SambaResources = sambaConfiguration.Get()?.Resources;
                return View["antd/part/page-antd-samba", viewModel];
            };

            Get["/part/net"] = x => {
                dynamic viewModel = new ExpandoObject();
                var nif = new NetworkInterfaceManagement();
                var networkInterfaces = nif.GetAll().ToList();
                var phyIf = networkInterfaces.Where(_ => _.Value == NetworkInterfaceManagement.NetworkInterfaceType.Physical).OrderBy(_ => _.Key);
                viewModel.NetworkPhysicalIf = phyIf;
                var brgIf = networkInterfaces.Where(_ => _.Value == NetworkInterfaceManagement.NetworkInterfaceType.Bridge).OrderBy(_ => _.Key);
                viewModel.NetworkBridgeIf = brgIf;
                var bndIf = networkInterfaces.Where(_ => _.Value == NetworkInterfaceManagement.NetworkInterfaceType.Bond).OrderBy(_ => _.Key);
                viewModel.NetworkBondIf = bndIf;
                var vrtIf = networkInterfaces.Where(_ => _.Value == NetworkInterfaceManagement.NetworkInterfaceType.Virtual).OrderBy(_ => _.Key).ToList();
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
                viewModel.Mounts = new Mount().GetAll();
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