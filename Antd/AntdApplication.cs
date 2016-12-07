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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using antdlib;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.common.Tool;
using antdlib.Systemd;
using antdlib.views;
using Antd.Apps;
using Antd.Bind;
using Antd.Certificates;
using Antd.Configuration;
using Antd.Database;
using Antd.Dhcpd;
using Antd.Discovery;
using Antd.Firewall;
using Antd.Gluster;
using Antd.MountPoint;
using Antd.Network;
using Antd.Overlay;
using Antd.Rsync;
using Antd.Samba;
using Antd.Ssh;
using Antd.Storage;
using Antd.SystemdTimer;
using Antd.Timer;
using Antd.Users;
using Nancy;
using Nancy.Hosting.Self;
using RaptorDB;
using HostConfiguration = Antd.Host.HostConfiguration;

namespace Antd {
    internal static class AntdApplication {
        public static RaptorDB.RaptorDB Database;

        #region [    private classes init    ]
        private static readonly ApplicationRepository ApplicationRepository = new ApplicationRepository();
        private static readonly ApplicationSetting ApplicationSetting = new ApplicationSetting();
        private static readonly AppTarget AppTarget = new AppTarget();
        private static readonly Bash Bash = new Bash();
        private static readonly BindConfiguration BindConfiguration = new BindConfiguration();
        private static readonly CaConfiguration CaConfiguration = new CaConfiguration();
        private static readonly DhcpdConfiguration DhcpdConfiguration = new DhcpdConfiguration();
        private static readonly FirewallConfiguration FirewallConfiguration = new FirewallConfiguration();
        private static readonly GlusterConfiguration GlusterConfiguration = new GlusterConfiguration();
        private static readonly HostConfiguration HostConfiguration = new HostConfiguration();
        private static readonly LanConfiguration LanConfiguration = new LanConfiguration();
        private static readonly Mount Mount = new Mount();
        private static readonly NetworkConfiguration NetworkConfiguration = new NetworkConfiguration();
        private static readonly RsyncConfiguration RsyncConfiguration = new RsyncConfiguration();
        private static readonly SambaConfiguration SambaConfiguration = new SambaConfiguration();
        private static readonly SetupConfiguration SetupConfiguration = new SetupConfiguration();
        private static readonly SyslogConfiguration SyslogConfiguration = new SyslogConfiguration();
        private static readonly Timers Timers = new Timers();
        private static readonly UserConfiguration UserConfiguration = new UserConfiguration();
        private static readonly Zpool Zpool = new Zpool();

        #endregion

        private static void Main() {
            ConsoleLogger.Log("");
            ConsoleLogger.Log("starting antd");
            var startTime = DateTime.Now;
            Procedure();
            var port = ApplicationSetting.HttpPort();
            var uri = $"http://localhost:{port}/";
            var host = new NancyHost(new Uri(uri));
            host.Start();
            ConsoleLogger.Log("host ready");
            StaticConfiguration.DisableErrorTraces = false;
            ConsoleLogger.Log($"http port: {port}");
            ConsoleLogger.Log("antd is running");
            ConsoleLogger.Log($"loaded in: {DateTime.Now - startTime}");
            if(Environment.OSVersion.Platform == PlatformID.Unix) {
                KeepAlive();
                ConsoleLogger.Log("antd is closing");
                host.Stop();
                Console.WriteLine("host shutdown");
                Database.Shutdown();
                Console.WriteLine("database shutdown");
            }
            else {
                HandlerRoutine hr = ConsoleCtrlCheck;
                GC.KeepAlive(hr);
                SetConsoleCtrlHandler(hr, true);
                while(!_isclosing) { }
                Console.WriteLine("antd is stopping");
                host.Stop();
                Console.WriteLine("host shutdown");
                Database.Shutdown();
                Console.WriteLine("database shutdown");
            }
        }

        private static void Procedure() {
            #region [    Remove Limits    ]

            if(Parameter.IsUnix) {
                const string limitsFile = "/etc/security/limits.conf";
                if(File.Exists(limitsFile)) {
                    if(!File.ReadAllText(limitsFile).Contains("root - nofile 1024000")) {
                        File.AppendAllLines(limitsFile, new[] { "root - nofile 1024000" });
                    }
                }
                Bash.Execute("ulimit -n 1024000", false);
            }

            #endregion

            #region [    Overlay Watcher    ]

            new OverlayWatcher().StartWatching();
            ConsoleLogger.Log("overlay watcher ready");

            #endregion

            #region [    Check OS    ]

            Bash.Execute($"{Parameter.Aossvc} reporemountrw", false);
            ConsoleLogger.Log("os checked");

            #endregion

            #region [    Working Directories    ]

            Directory.CreateDirectory("/cfg/antd");
            Directory.CreateDirectory("/cfg/antd/database");
            Directory.CreateDirectory("/cfg/antd/services");
            Directory.CreateDirectory("/mnt/cdrom/DIRS");
            if(Parameter.IsUnix) {
                Mount.WorkingDirectories();
            }
            ConsoleLogger.Log("working directories ready");

            #endregion

            #region [    Core Parameters    ]

            ApplicationSetting.WriteDefaults();
            ConsoleLogger.Log("antd core parameters ready");

            #endregion

            #region [    Database    ]

            Database = RaptorDB.RaptorDB.Open(ApplicationSetting.DatabasePath());
            Global.RequirePrimaryView = false;
            Global.BackupCronSchedule = null;
            Global.SaveIndexToDiskTimerSeconds = 30;
            Database.RegisterView(new ApplicationView());
            Database.RegisterView(new AuthorizedKeysView());
            Database.RegisterView(new TimerView());
            Database.RegisterView(new MacAddressView());
            Database.RegisterView(new SyslogView());
            ConsoleLogger.Log("database ready");

            #endregion

            #region [    Mounts    ]

            if(Parameter.IsUnix) {
                if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware") == false) {
                    Bash.Execute("mount /mnt/cdrom/Kernel/active-firmware /lib64/firmware", false);
                }
                var kernelRelease = Bash.Execute("uname -r").Trim();
                var linkedRelease = Bash.Execute("file /mnt/cdrom/Kernel/active-modules").Trim();
                if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-modules") == false &&
                    linkedRelease.Contains(kernelRelease)) {
                    var moduleDir = $"/lib64/modules/{kernelRelease}/";
                    Directory.CreateDirectory(moduleDir);
                    Bash.Execute($"mount /mnt/cdrom/Kernel/active-modules {moduleDir}", false);
                }
                Bash.Execute("systemctl restart systemd-modules-load.service", false);
                Mount.AllDirectories();
                ConsoleLogger.Log("mounts ready");
            }

            #endregion

            if(Parameter.IsUnix) {
                #region [    Host Configuration    ]
                HostConfiguration.Setup();
                ConsoleLogger.Log("host configuration prepared");
                HostConfiguration.ApplyHostInfo();
                ConsoleLogger.Log("host configured");
                #endregion

                #region [    Name Service    ]
                HostConfiguration.ApplyNsHosts();
                HostConfiguration.ApplyNsNetworks();
                HostConfiguration.ApplyNsResolv();
                HostConfiguration.ApplyNsSwitch();
                ConsoleLogger.Log("name service ready");
                #endregion

                #region [    OS Parameters    ]
                HostConfiguration.ApplyHostOsParameters();
                ConsoleLogger.Log("os parameters ready");
                #endregion

                #region [    Modules    ]
                HostConfiguration.ApplyHostBlacklistModules();
                HostConfiguration.ApplyHostModprobes();
                HostConfiguration.ApplyHostRemoveModules();
                ConsoleLogger.Log("modules ready");
                #endregion

                #region [    Import Commands    ]
                File.Copy($"{Parameter.RootFrameworkAntdShellScript}/var/kerbynet.conf", "/etc/kerbynet.conf", true);
                ConsoleLogger.Log("commands and scripts configuration imported");
                #endregion
            }

            #region [    Users    ]
            var manageMaster = new ManageMaster();
            manageMaster.Setup();
            if(Parameter.IsUnix) {
                UserConfiguration.Import();
                UserConfiguration.Set();
            }
            ConsoleLogger.Log("users config ready");
            #endregion

            #region [    Time & Date    ]
            if(Parameter.IsUnix) {
                HostConfiguration.ApplyNtpdate();
                HostConfiguration.ApplyTimezone();
                HostConfiguration.ApplyNtpd();
                ConsoleLogger.Log("time and date configured");
            }
            #endregion

            #region [    Network    ]
            if(Parameter.IsUnix) {
                if(LanConfiguration.NothingIsConfigured()) {
                    ConsoleLogger.Log("lan set configuration");
                    var netIf = LanConfiguration.ConfigureInterface();
                    if(!string.IsNullOrEmpty(netIf)) {
                        ConsoleLogger.Log($"lan configured on {netIf}");
                    }
                }
                NetworkConfiguration.Start();
                ConsoleLogger.Log("network configured");
            }
            #endregion

            #region [    Apply Setup Configuration    ]
            SetupConfiguration.Set();
            ConsoleLogger.Log("machine configured");
            #endregion

            #region [    Services    ]
            HostConfiguration.ApplyHostServices();
            ConsoleLogger.Log("services ready");
            #endregion

            if(Parameter.IsUnix) {
                #region [    Ssh    ]
                if(!Directory.Exists(Parameter.RootSsh)) {
                    Directory.CreateDirectory(Parameter.RootSsh);
                }
                if(!Directory.Exists(Parameter.RootSshMntCdrom)) {
                    Directory.CreateDirectory(Parameter.RootSshMntCdrom);
                }
                if(!MountHelper.IsAlreadyMounted(Parameter.RootSsh)) {
                    var mnt = new Mount();
                    mnt.Dir(Parameter.RootSsh);
                }
                var rk = new RootKeys();
                if(rk.Exists == false) {
                    rk.Create();
                }
                var storedKeyRepo = new AuthorizedKeysRepository();
                var storedKeys = storedKeyRepo.GetAll();
                foreach(var storedKey in storedKeys) {
                    var home = storedKey.User == "root" ? "/root/.ssh" : $"/home/{storedKey.User}/.ssh";
                    var authorizedKeysPath = $"{home}/authorized_keys";
                    if(!File.Exists(authorizedKeysPath)) {
                        File.Create(authorizedKeysPath);
                    }
                    File.AppendAllLines(authorizedKeysPath, new List<string> { $"{storedKey.KeyValue} {storedKey.RemoteUser}" });
                    Bash.Execute($"chmod 600 {authorizedKeysPath}");
                    Bash.Execute($"chown {storedKey.User}:{storedKey.User} {authorizedKeysPath}");
                }
                ConsoleLogger.Log("ssh ready");
                #endregion

                #region [    Firewall    ]
                if(FirewallConfiguration.IsActive()) {
                    FirewallConfiguration.Set();
                }
                #endregion

                #region [    Dhcpd    ]
                if(DhcpdConfiguration.IsActive()) {
                    DhcpdConfiguration.Set();
                }
                #endregion

                #region [    Bind    ]
                if(BindConfiguration.IsActive()) {
                    BindConfiguration.Set();
                }
                #endregion

                #region [    Samba    ]
                if(SambaConfiguration.IsActive()) {
                    SambaConfiguration.Set();
                }
                #endregion

                #region [    Syslog    ]
                if(SyslogConfiguration.Set()) {
                    ConsoleLogger.Log("syslog ready");
                }
                #endregion

                #region [    Avahi    ]
                const string avahiServicePath = "/etc/avahi/services/antd.service";
                if(File.Exists(avahiServicePath)) {
                    File.Delete(avahiServicePath);
                }
                File.WriteAllLines(avahiServicePath, AvahiCustomXml.Generate(ApplicationSetting.HttpPort()));
                Bash.Execute("chmod 755 /etc/avahi/services", false);
                Bash.Execute($"chmod 644 {avahiServicePath}", false);
                Systemctl.Restart("avahi-daemon.service");
                Systemctl.Restart("avahi-daemon.socket");
                ConsoleLogger.Log("avahi ready");
                #endregion

                #region [    Storage    ]
                foreach(var pool in Zpool.ImportList().ToList()) {
                    if(string.IsNullOrEmpty(pool))
                        continue;
                    ConsoleLogger.Log($"pool {pool} imported");
                    Zpool.Import(pool);
                }
                ConsoleLogger.Log("storage ready");
                #endregion

                #region [    Scheduler    ]
                Timers.CleanUp();
                Timers.Setup();
                Timers.Import();
                Timers.Export();
                foreach(var zp in Zpool.List()) {
                    Timers.Create(zp.Name.ToLower() + "snap", "hourly", $"/sbin/zfs snap -r {zp.Name}@${{TTDATE}}");
                }
                Timers.StartAll();
                new SnapshotCleanup().Start(new TimeSpan(2, 00, 00));
                new SyncTime().Start(new TimeSpan(0, 42, 00));
                ConsoleLogger.Log("scheduler and timers ready");
                #endregion

                #region [    Sync    ]
                if(GlusterConfiguration.IsActive()) {
                    GlusterConfiguration.Set();
                }
                if(RsyncConfiguration.IsActive()) {
                    RsyncConfiguration.Set();
                }
                #endregion

                #region [    C A    ]
                if(CaConfiguration.IsActive()) {
                    CaConfiguration.Set();
                }
                #endregion

                #region [    Apps    ]
                AppTarget.Setup();
                var apps = ApplicationRepository.GetAll().Select(_ => new ApplicationModel(_)).ToList();
                foreach(var app in apps) {
                    var units = app.UnitLauncher;
                    foreach(var unit in units) {
                        if(Systemctl.IsActive(unit) == false) {
                            Systemctl.Restart(unit);
                        }
                    }
                }
                //AppTarget.StartAll();
                ConsoleLogger.Log("apps ready");
                #endregion
            }
        }

        #region [    unused methods    ]
        private static void SetWebsocketd() {
            var filePath = $"{Parameter.AntdCfg}/websocketd";
            if(File.Exists(filePath))
                return;
            File.Copy($"{Parameter.Resources}/websocketd", filePath);
            Bash.Execute($"chmod 777 {filePath}", false);
            ConsoleLogger.Log("websocketd ready");
        }

        private static void SetSystemdJournald() {
            var file = $"{Parameter.RepoDirs}/FILE_etc_systemd_journald.conf";
            if(File.Exists(file)) {
                return;
            }
            File.Copy($"{Parameter.Resources}/FILE_etc_systemd_journald.conf", file);
            var realFileName = MountHelper.GetFilesPath("FILE_etc_systemd_journald.conf");
            if(MountHelper.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Bash.Execute("systemctl restart systemd-journald.service", false);
            ConsoleLogger.Log("journald config ready");
        }

        private static void LoadCollectd() {
            var file = $"{Parameter.RepoDirs}/FILE_etc_collectd.conf";
            File.Copy($"{Parameter.Resources}/FILE_etc_collectd.conf", file);
            var realFileName = MountHelper.GetFilesPath("FILE_etc_collectd.conf");
            if(MountHelper.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Bash.Execute("systemctl restart collectd.service", false);
        }

        private static void LoadWpaSupplicant() {
            var file = $"{Parameter.RepoDirs}/FILE_etc_wpa_supplicant_wpa_suplicant.conf";
            File.Copy($"{Parameter.Resources}/FILE_etc_wpa_supplicant_wpa_suplicant.conf", file);
            var realFileName = MountHelper.GetFilesPath("FILE_etc_wpa_supplicant_wpa__suplicant.conf");
            if(MountHelper.IsAlreadyMounted(file, realFileName) == false) {
                Mount.File(realFileName);
            }
            Bash.Execute("systemctl restart wpa_supplicant.service", false);
        }
        #endregion

        #region [    Shutdown Management    ]
        private static void KeepAlive() {
            var r = Console.ReadLine();
            while(r != "quit") {
                r = Console.ReadLine();
            }
        }

        private static bool _isclosing;

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType) {
            Database.Shutdown();
            switch(ctrlType) {
                case CtrlTypes.CtrlCEvent:
                    _isclosing = true;
                    Console.WriteLine("CTRL+C received!");
                    break;
                case CtrlTypes.CtrlBreakEvent:
                    _isclosing = true;
                    Console.WriteLine("CTRL+BREAK received!");
                    break;
                case CtrlTypes.CtrlCloseEvent:
                    _isclosing = true;
                    Console.WriteLine("Program being closed!");
                    break;
                case CtrlTypes.CtrlLogoffEvent:
                case CtrlTypes.CtrlShutdownEvent:
                    _isclosing = true;
                    Console.WriteLine("User is logging off!");
                    break;
            }
            return true;
        }

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        public delegate bool HandlerRoutine(CtrlTypes ctrlType);

        public enum CtrlTypes {
            CtrlCEvent = 0,
            CtrlBreakEvent,
            CtrlCloseEvent,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent
        }
        #endregion
    }
}