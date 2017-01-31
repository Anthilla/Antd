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
using System.Text.RegularExpressions;
using antd.commands;
using antdlib.common;
using antdlib.common.Tool;
using antdlib.models;
using Antd.Acl;
using Antd.Bind;
using Antd.Dhcpd;
using Antd.Firewall;
using Antd.Gluster;
using Antd.Host;
using Antd.Info;
using Antd.MountPoint;
using Antd.Network;
using Antd.Overlay;
using Antd.Rsync;
using Antd.Samba;
using Antd.Ssh;
using Antd.Storage;
using Antd.SystemdTimer;
using Antd.Users;
using Antd.Vpn;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {
    public class AntdModule : NancyModule {

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        public AntdModule() {
            this.RequiresAuthentication();

            #region [    Home    ]
            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                return View["antd/page-antd", vmod];
            };
            #endregion

            #region [    Partials    ]
            Get["/part/info"] = x => {
                try {
                    var bash = new Bash();
                    var launcher = new CommandLauncher();
                    var machineInfo = new MachineInfo();
                    dynamic viewModel = new ExpandoObject();
                    viewModel.VersionOS = bash.Execute("uname -a");
                    viewModel.AosInfo = machineInfo.GetAosrelease();
                    viewModel.Uptime = machineInfo.GetUptime();
                    viewModel.GentooRelease = launcher.Launch("cat-etc-gentoorel").JoinToString("<br />");
                    viewModel.LsbRelease = launcher.Launch("cat-etc-lsbrel").JoinToString("<br />");
                    viewModel.OsRelease = launcher.Launch("cat-etc-osrel").JoinToString("<br />");
                    return View["antd/part/page-antd-info", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/info/memory"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var machineInfo = new MachineInfo();
                    viewModel.Meminfo = machineInfo.GetMeminfo();
                    viewModel.Free = machineInfo.GetFree();
                    return View["antd/part/page-antd-info-memory", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/info/cpu"] = x => {
                try {
                    var machineInfo = new MachineInfo();
                    dynamic viewModel = new ExpandoObject();
                    viewModel.Cpuinfo = machineInfo.GetCpuinfo();
                    return View["antd/part/page-antd-info-cpu", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/info/services"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var machineInfo = new MachineInfo();
                    viewModel.Services = machineInfo.GetUnits("service");
                    viewModel.Mounts = machineInfo.GetUnits("mount");
                    viewModel.Targets = machineInfo.GetUnits("target");
                    viewModel.Timers = machineInfo.GetUnits("timer");
                    return View["antd/part/page-antd-info-services", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/info/modules"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var machineInfo = new MachineInfo();
                    viewModel.Modules = machineInfo.GetModules();
                    return View["antd/part/page-antd-info-modules", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/system"] = x => {
                try {
                    var machineInfo = new MachineInfo();
                    dynamic viewModel = new ExpandoObject();
                    viewModel.SystemComponents = machineInfo.GetSystemComponentModels();
                    return View["antd/part/page-antd-system", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/system/losetup"] = x => {
                try {
                    var machineInfo = new MachineInfo();
                    dynamic viewModel = new ExpandoObject();
                    viewModel.LosetupInfo = machineInfo.GetLosetup();
                    return View["antd/part/page-antd-system-losetup", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/system/update"] = x => {
                try {
                    var launcher = new CommandLauncher();
                    dynamic viewModel = new ExpandoObject();
                    var bash = new Bash();

                    var updatecheck = launcher.Launch("mono-antdsh-update-check").ToList();
                    ConsoleLogger.Log(updatecheck.JoinToString("; "));
                    var latestAntd = updatecheck.LastOrDefault(_ => _.Contains("update.antd"));
                    var latestAntdsh = updatecheck.LastOrDefault(_ => _.Contains("update.antdsh"));
                    var latestSystem = updatecheck.LastOrDefault(_ => _.Contains("update.system"));
                    var latestKernel = updatecheck.LastOrDefault(_ => _.Contains("update.kernel"));
                    viewModel.AntdLatestVersion = latestAntd;
                    viewModel.AntdshLatestVersion = latestAntdsh;
                    viewModel.SystemLatestVersion = latestSystem;
                    viewModel.KernelLatestVersion = latestKernel;

                    const string antdActive = "/mnt/cdrom/Apps/Anthilla_Antd/active-version";
                    const string antdshActive = "/mnt/cdrom/Apps/Anthilla_antdsh/active-version";
                    const string systemActive = "/mnt/cdrom/System/active-system";
                    const string kernelActive = "/mnt/cdrom/Kernel/active-kernel";
                    viewModel.AntdVersion = GetVersionDateFromFile(bash.Execute($"file {antdActive}"));
                    viewModel.AntdshVersion = GetVersionDateFromFile(bash.Execute($"file {antdshActive}"));
                    viewModel.SystemVersion = GetVersionDateFromFile(bash.Execute($"file {systemActive}"));
                    viewModel.KernelVersion = GetVersionDateFromFile(bash.Execute($"file {kernelActive}"));
                    return View["antd/part/page-antd-system-update", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/host"] = x => {
                try {
                    var launcher = new CommandLauncher();
                    dynamic viewModel = new ExpandoObject();
                    var hostnamectl = launcher.Launch("hostnamectl").ToList();
                    var ssoree = StringSplitOptions.RemoveEmptyEntries;
                    viewModel.StaticHostname = hostnamectl.First(_ => _.Contains("Static hostname:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.IconName = hostnamectl.First(_ => _.Contains("Icon name:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Chassis = hostnamectl.First(_ => _.Contains("Chassis:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Deployment = hostnamectl.First(_ => _.Contains("Deployment:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Location = hostnamectl.First(_ => _.Contains("Location:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.MachineID = hostnamectl.First(_ => _.Contains("Machine ID:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.BootID = hostnamectl.First(_ => _.Contains("Boot ID:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Virtualization = hostnamectl.First(_ => _.Contains("Virtualization:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.OS = hostnamectl.First(_ => _.Contains("Operating System:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Kernel = hostnamectl.First(_ => _.Contains("Kernel:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Architecture = hostnamectl.First(_ => _.Contains("Architecture:")).Split(new[] { ":" }, 2, ssoree)[1];
                    return View["antd/part/page-antd-host", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/time"] = x => {
                try {
                    var bash = new Bash();
                    var launcher = new CommandLauncher();
                    dynamic viewModel = new ExpandoObject();
                    var timezones = bash.Execute("timedatectl list-timezones --no-pager").SplitBash();
                    viewModel.Timezones = timezones;
                    var timedatectl = launcher.Launch("timedatectl").ToList();
                    var ssoree = StringSplitOptions.RemoveEmptyEntries;
                    viewModel.LocalTime = timedatectl.First(_ => _.Contains("Local time:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.UnivTime = timedatectl.First(_ => _.Contains("Universal time:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.RTCTime = timedatectl.First(_ => _.Contains("RTC time:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Timezone = timedatectl.First(_ => _.Contains("Time zone:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Nettimeon = timedatectl.First(_ => _.Contains("Network time on:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Ntpsync = timedatectl.First(_ => _.Contains("NTP synchronized:")).Split(new[] { ":" }, 2, ssoree)[1];
                    viewModel.Rtcintz = timedatectl.First(_ => _.Contains("RTC in local TZ:")).Split(new[] { ":" }, 2, ssoree)[1];
                    var hostConfiguration = new HostConfiguration();
                    viewModel.NtpServer = hostConfiguration.Host.NtpdateServer.StoredValues["$server"];
                    var ntpd = launcher.Launch("cat-etc-ntp").ToArray();
                    viewModel.Ntpd = ntpd.JoinToString("<br />");
                    viewModel.NtpdEdit = ntpd.JoinToString(Environment.NewLine);
                    return View["antd/part/page-antd-time", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/ns"] = x => {
                try {
                    var launcher = new CommandLauncher();
                    dynamic viewModel = new ExpandoObject();
                    viewModel.Hostname = launcher.Launch("cat-etc-hostname").JoinToString("<br />");
                    viewModel.Hosts = launcher.Launch("cat-etc-hosts").JoinToString("<br />");
                    var hostConfiguration = new HostConfiguration();
                    viewModel.DomainInt = hostConfiguration.Host.InternalDomain;
                    viewModel.DomainExt = hostConfiguration.Host.ExternalDomain;
                    var hosts = launcher.Launch("cat-etc-hosts").ToArray();
                    viewModel.Hosts = hosts.JoinToString("<br />");
                    viewModel.HostsEdit = hosts.JoinToString(Environment.NewLine);
                    var networks = launcher.Launch("cat-etc-networks").ToArray();
                    viewModel.Networks = networks.JoinToString("<br />");
                    viewModel.NetworksEdit = networks.JoinToString(Environment.NewLine);
                    var resolv = launcher.Launch("cat-etc-resolv").ToArray();
                    viewModel.Resolv = resolv.JoinToString("<br />");
                    viewModel.ResolvEdit = resolv.JoinToString(Environment.NewLine);
                    var nsswitch = launcher.Launch("cat-etc-nsswitch").ToArray();
                    viewModel.Nsswitch = nsswitch.JoinToString("<br />");
                    viewModel.NsswitchEdit = nsswitch.JoinToString(Environment.NewLine);
                    return View["antd/part/page-antd-ns", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/named"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var bindConfiguration = new BindConfiguration();
                    var bindIsActive = bindConfiguration.IsActive();
                    viewModel.BindIsActive = bindIsActive;
                    viewModel.BindOptions = bindConfiguration.Get() ?? new BindConfigurationModel();
                    viewModel.BindZones = bindConfiguration.Get()?.Zones;
                    return View["antd/part/page-antd-bind", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/dhcp"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var dhcpdConfiguration = new DhcpdConfiguration();
                    var dhcpdIsActive = dhcpdConfiguration.IsActive();
                    viewModel.DhcpdIsActive = dhcpdIsActive;
                    viewModel.DhcpdOptions = dhcpdConfiguration.Get() ?? new DhcpdConfigurationModel();
                    viewModel.DhcpdClass = dhcpdConfiguration.Get()?.Classes;
                    viewModel.DhcpdPools = dhcpdConfiguration.Get()?.Pools;
                    viewModel.DhcpdReservation = dhcpdConfiguration.Get()?.Reservations;
                    return View["antd/part/page-antd-dhcp", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/dhcp/leases"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var dhcpdLeases = new DhcpdLeases();
                    var list = dhcpdLeases.List();
                    viewModel.DhcpdLeases = list;
                    viewModel.EmptyList = !list.Any();
                    return View["antd/part/page-antd-dhcp-leases", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/samba"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var sambaConfiguration = new SambaConfiguration();
                    var sambaIsActive = sambaConfiguration.IsActive();
                    viewModel.SambaIsActive = sambaIsActive;
                    viewModel.SambaOptions = sambaConfiguration.Get() ?? new SambaConfigurationModel();
                    viewModel.SambaResources = sambaConfiguration.Get()?.Resources;
                    return View["antd/part/page-antd-samba", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/sshd"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var sshdConfiguration = new SshdConfiguration();
                    var sshdIsActive = sshdConfiguration.IsActive();
                    viewModel.SshdIsActive = sshdIsActive;
                    viewModel.SshdOptions = sshdConfiguration.Get() ?? new SshdConfigurationModel();
                    return View["antd/part/page-antd-sshd", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/net"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var networkConfiguration = new NetworkConfiguration();
                    var physicalInterfaces = networkConfiguration.InterfacePhysicalModel.ToList();
                    viewModel.NetworkPhysicalIf = physicalInterfaces;
                    var bridgeInterfaces = networkConfiguration.InterfaceBridgeModel.ToList();
                    viewModel.NetworkBridgeIf = bridgeInterfaces;
                    var bondInterfaces = networkConfiguration.InterfaceBondModel.ToList();
                    viewModel.NetworkBondIf = bondInterfaces;
                    var virtualInterfaces = networkConfiguration.InterfaceVirtualModel.ToList();
                    foreach(var vif in virtualInterfaces) {
                        if(physicalInterfaces.Any(_ => _.Interface == vif.Interface) ||
                        bridgeInterfaces.Any(_ => _.Interface == vif.Interface) ||
                        bondInterfaces.Any(_ => _.Interface == vif.Interface)) {
                            virtualInterfaces.Remove(vif);
                        }
                    }
                    viewModel.NetworkVirtualIf = virtualInterfaces;
                    return View["antd/part/page-antd-network", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/net/vpn"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var vpnConfiguration = new VpnConfiguration();
                    var vpnIsActive = vpnConfiguration.IsActive();
                    viewModel.VpnIsActive = vpnIsActive;
                    var conf = vpnConfiguration.Get();
                    viewModel.VpnLocalPoint = conf.LocalPoint;
                    viewModel.VpnRemoteHost = conf.RemoteHost;
                    viewModel.VpnRemotePoint = conf.RemotePoint;
                    return View["antd/part/page-antd-net-vpn", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/fw"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var firewallConfiguration = new FirewallConfiguration();
                    var firewallIsActive = firewallConfiguration.IsActive();
                    viewModel.FirewallIsActive = firewallIsActive;
                    var firewall = firewallConfiguration.Get() ?? new FirewallConfigurationModel();
                    viewModel.FwIp4Filter = firewall.Ipv4FilterTable;
                    viewModel.FwIp4Nat = firewall.Ipv4NatTable;
                    viewModel.FwIp6Filter = firewall.Ipv6FilterTable;
                    viewModel.FwIp6Nat = firewall.Ipv6NatTable;
                    return View["antd/part/page-antd-firewall", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error(
                        $"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            //Get["/part/fw/mac"] = x => {
            //    try {
            //        dynamic viewModel = new ExpandoObject();
            //        viewModel.MacAddressList = new MacAddressRepository().GetAll();
            //        return View["antd/part/page-antd-firewall-mac", viewModel];
            //    }
            //    catch(Exception ex) {
            //        ConsoleLogger.Error(
            //            $"{Request.Url} request failed: {ex.Message}");
            //        ConsoleLogger.Error(ex);
            //        return View["antd/part/page-error"];
            //    }
            //};

            Get["/part/cron"] = x => {
                try {
                    var timers = new Timers();
                    dynamic viewModel = new ExpandoObject();
                    var scheduledJobs = timers.GetAll();
                    viewModel.Jobs = scheduledJobs?.ToList().OrderBy(_ => _.Alias);
                    return View["antd/part/page-antd-scheduler", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/storage"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var disks = new Disks();
                    viewModel.DisksList = disks.GetList();
                    return View["antd/part/page-antd-storage", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/storage/zfs"] = x => {
                try {
                    var zpool = new Zpool();
                    var zfsSnap = new ZfsSnap();
                    var zfs = new Zfs();
                    dynamic viewModel = new ExpandoObject();
                    viewModel.ZpoolList = zpool.List();
                    viewModel.ZfsList = zfs.List();
                    viewModel.ZfsSnap = zfsSnap.List();
                    viewModel.ZpoolHistory = zpool.History();
                    var disks = new Disks();
                    viewModel.DisksList = disks.GetList().Where(_=>_.Type == "disk" && string.IsNullOrEmpty(_.Mountpoint));

                    return View["antd/part/page-antd-storage-zfs", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/storage/usage"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    var diskUsage = new DiskUsage();
                    viewModel.DisksUsage = diskUsage.GetInfo();
                    return View["antd/part/page-antd-storage-usage", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/storage/mounts"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.Mounts = new Mount().GetAll();
                    return View["antd/part/page-antd-storage-mounts", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/storage/overlay"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.Overlay = OverlayWatcher.ChangedDirectories;
                    return View["antd/part/page-antd-storage-overlay", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/acl"] = x => {
                try {
                    var aclConfiguration = new AclConfiguration();
                    dynamic viewModel = new ExpandoObject();
                    var aclConfig = aclConfiguration.Get() ?? new AclConfigurationModel();
                    var aclIsActive = aclConfig.IsActive;
                    viewModel.AclIsActive = aclIsActive;
                    var list = new List<AclPersistentSettingModel>();
                    foreach(var aaa in aclConfig.Settings) {
                        aaa.AclText = System.IO.File.ReadAllLines(aaa.Acl).JoinToString(Environment.NewLine);
                        list.Add(aaa);
                    }
                    viewModel.Acl = list.OrderBy(_ => _.Path);
                    return View["antd/part/page-antd-acl", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/sync"] = x => {
                try {
                    var glusterConfiguration = new GlusterConfiguration();
                    dynamic viewModel = new ExpandoObject();
                    var glusterConfig = glusterConfiguration.Get() ?? new GlusterConfigurationModel();
                    var glusterIsActive = glusterConfig.IsActive;
                    viewModel.GlusterIsActive = glusterIsActive;
                    viewModel.GlusterNodes = glusterConfig.Nodes;
                    viewModel.GlusterVolumes = glusterConfig.Volumes;
                    return View["antd/part/page-antd-gluster", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/rsync"] = x => {
                try {
                    var rsyncConfiguration = new RsyncConfiguration();
                    dynamic viewModel = new ExpandoObject();
                    var rsyncConfig = rsyncConfiguration.Get() ?? new RsyncConfigurationModel();
                    var rsyncIsActive = rsyncConfig.IsActive;
                    viewModel.RsyncIsActive = rsyncIsActive;
                    viewModel.RsyncDirectories = rsyncConfig.Directories.OrderBy(_ => _.Type);
                    return View["antd/part/page-antd-rsync", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/vm"] = x => {
                try {
                    var virsh = new Virsh.Virsh();
                    dynamic viewModel = new ExpandoObject();
                    var vmList = virsh.GetVmList();
                    viewModel.VMListAny = vmList.Any();
                    viewModel.VMList = vmList;
                    return View["antd/part/page-antd-vm", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/users"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.Master = new ManageMaster().Name;
                    var userConfiguration = new UserConfiguration();
                    viewModel.Users = userConfiguration.Get().Where(_ => _.Name.ToLower() != "root").OrderBy(_ => _.Name);
                    return View["antd/part/page-antd-users", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };

            Get["/part/ssh"] = x => {
                try {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.Keys = null;
                    return View["antd/part/page-antd-ssh", viewModel];
                }
                catch(Exception ex) {
                    ConsoleLogger.Error($"{Request.Url} request failed: {ex.Message}");
                    ConsoleLogger.Error(ex);
                    return View["antd/part/page-error"];
                }
            };
            #endregion

            #region [    Hooks    ]
            After += ctx => {
                if(ctx.Response.ContentType == "text/html") {
                    ctx.Response.ContentType = "text/html; charset=utf-8";
                }
            };
            #endregion
        }
    }
}