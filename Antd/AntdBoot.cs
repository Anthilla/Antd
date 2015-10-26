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

using antdlib.Scheduler;
using antdlib;
using antdlib.Status;
using antdlib.Boot;
using Microsoft.AspNet.SignalR;
using Nancy;
using Owin;
using System.IO;
using System.Text.RegularExpressions;
using antdlib.MountPoint;
using antdlib.Apps;

namespace Antd {

    public class AntdBoot {

        public static void CheckIfGlobalRepositoryIsWriteable() {
            if (AssemblyInfo.IsUnix == true) {
                var bootExtData = Terminal.Execute("blkid | grep BootExt");
                if (bootExtData.Length > 0) {
                    var bootExtDevice = new Regex(".*:").Matches(bootExtData)[0].Value.Replace(":", "").Trim();
                    var bootExtUid = new Regex("[\\s]UUID=\"[\\d\\w\\-]+\"").Matches(bootExtData)[0].Value.Replace("UUID=", "").Replace("\"", "").Trim();
                    ConsoleLogger.Log("    global repository -> checking");
                    var mountResult = Terminal.Execute($"cat /proc/mounts | grep '{bootExtDevice} /mnt/cdrom '");
                    if (mountResult.Length > 0) {
                        if (mountResult.Contains("ro") && !mountResult.Contains("rw")) {
                            ConsoleLogger.Log("                      is RO -> remounting");
                            Terminal.Execute("mount -o remount,rw,discard,noatime /mnt/cdrom");
                        }
                        else if (mountResult.Contains("rw") && !mountResult.Contains("ro")) {
                            ConsoleLogger.Log("                      is RW -> ok!");
                        }
                    }
                    else {
                        ConsoleLogger.Log("                      is not mounted -> IMPOSSIBLE");
                    }
                    ConsoleLogger.Log($"    global repository -> {bootExtDevice} - {bootExtUid}");
                    ConsoleLogger.Log("    global repository -> checked");
                }
            }
        }

        public static void SetWorkingDirectories() {
            if (AssemblyInfo.IsUnix == true) {
                Mount.WorkingDirectories();
                ConsoleLogger.Log("    working directories -> checked");
            }
        }

        public static void SetMounts() {
            if (AssemblyInfo.IsUnix == true) {
                Mount.AllDirectories();
                ConsoleLogger.Log("    mounts -> checked");
            }
        }

        public static void SetUsersMount(bool isActive) {
            if (isActive == true && AssemblyInfo.IsUnix == true) {
                antdlib.Users.SystemUser.SetReady();
                antdlib.Users.SystemGroup.SetReady();
                ConsoleLogger.Log("    users mount -> checked");
            }
        }

        public static void SetOSMount() {
            if (AssemblyInfo.IsUnix == true) {
                var firmware = "/mnt/cdrom/Kernel/active-firmware";
                var firmwareDir = "/lib64/firmware";
                if (Mount.IsAlreadyMounted(firmware, firmwareDir) == false) {
                    Terminal.Execute($"mount {firmware} {firmwareDir}");
                }
                var module = "/mnt/cdrom/Kernel/active-modules";
                var kernelRelease = Terminal.Execute("uname -r").Trim();
                var linkedRelease = Terminal.Execute($"file {module}").Trim();
                if (Mount.IsAlreadyMounted(module) == false && linkedRelease.Contains(kernelRelease)) {
                    var moduleDir = $"/lib64/modules/{kernelRelease}/";
                    //todo rimuovere poi la riga qui sotto
                    Directory.CreateDirectory($"/mnt/cdrom/DIRS/prova-{kernelRelease}");
                    ConsoleLogger.Log($"Creating {moduleDir} to mount OS-modules");
                    Directory.CreateDirectory(moduleDir);
                    Terminal.Execute($"mount {module} {moduleDir}");
                }
                ConsoleLogger.Log("    os mount -> checked");
                Terminal.Execute($"systemctl restart systemd-modules-load.service");
            }
        }

        public static void SetOsConfiguration() {
            if (AssemblyInfo.IsUnix == true) {
                ConsoleLogger.Log("    os -> loading configuration");
                //ConsoleLogger.Log("          load /etc/ssh");
                //LoadOSConfiguration.LoadEtcSSH();
                //ConsoleLogger.Log("          load collectd");
                //LoadOSConfiguration.LoadCollectd();
                ConsoleLogger.Log("          load journald");
                LoadOSConfiguration.LoadSystemdJournald();
                //ConsoleLogger.Log("          load wpa-supplicant");
                //LoadOSConfiguration.LoadWPASupplicant();
                ConsoleLogger.Log("          load network");
                LoadOSConfiguration.LoadNetwork();
                ConsoleLogger.Log("          load firewall");
                LoadOSConfiguration.LoadFirewall();
                ConsoleLogger.Log("          installing websocketd");
                LoadOSConfiguration.LoadWebsocketd();
                ConsoleLogger.Log("    os -> checked");
            }
        }

        public static void SetCoreParameters() {
            CoreParametersConfig.WriteDefaults();
            ConsoleLogger.Log("    antd core parameters -> loaded");
        }

        public static void CheckSysctl(bool isActive) {
            if (AssemblyInfo.IsUnix == true) {
                if (isActive) {
                    Sysctl.WriteConfig();
                    Sysctl.LoadConfig();
                    ConsoleLogger.Log("    sysctl -> loaded");
                }
                else {
                    ConsoleLogger.Log("    sysctl -> skipped");
                }
            }
        }

        public static void StartNetworkd() {
            if (AssemblyInfo.IsUnix == true) {
                Networkd.SetConfiguration();
            }
        }

        public static void StartScheduler(bool loadFromDatabase) {
            JobScheduler.Start(loadFromDatabase);
            ConsoleLogger.Log("    scheduler -> loaded");
        }

        private readonly static string[] WatchDirectories = new string[] {
            Folder.Root
        };

        public static void StartDirectoryWatcher(bool isActive) {
            if (isActive) {
                ConsoleLogger.Log("    directory watcher -> enabled");
                foreach (string folder in WatchDirectories) {
                    if (Directory.Exists(folder)) {
                        new DirectoryWatcher(folder).Watch();
                        ConsoleLogger.Log("    directory watcher -> enabled for {0}", folder);
                    }
                    else {
                        ConsoleLogger.Log("    directory watcher -> {0} does not exist", folder);
                    }
                }
            }
            else {
                ConsoleLogger.Log("    directory watcher -> skipped");
            }
        }

        public static void StartDatabase() {
            var applicationDatabasePath = CoreParametersConfig.GetDb();
            ConsoleLogger.Log("root info -> application database path: {0}", applicationDatabasePath);
            if (Directory.Exists(applicationDatabasePath)) {
                var databases = new[] { applicationDatabasePath };
                DatabaseBoot.Start(databases, true);
                ConsoleLogger.Log("    database -> loaded");
            }
            else {
                ConsoleLogger.Warn("    database -> failed to load");
                ConsoleLogger.Warn("                directory does not exist");
            }
        }

        public static void StartSignalR(IAppBuilder app, bool detailedErrors, bool isActive) {
            if (isActive) {
                var hubConfiguration = new HubConfiguration { EnableDetailedErrors = detailedErrors };
                app.MapSignalR(hubConfiguration);
                ConsoleLogger.Log("    signalR -> loaded");
            }
            else {
                ConsoleLogger.Log("    signalR -> skipped");
            }
        }

        public static void StartNancy(IAppBuilder app) {
            StaticConfiguration.DisableErrorTraces = false;
            app.UseNancy();
            ConsoleLogger.Log("    nancy -> loaded");
        }

        public static void TestWebDav(string uri, string path) {
            //NameValueCollection properties = new NameValueCollection();
            //properties["showDateTime"] = "true";
            //LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter(properties);
            //WebDavServer server = new WebDavServer(new WebDavDiskStore(path));
            //server.Listener.Prefixes.Add(uri);
            //server.Start();
        }

        public static void InitAuthentication() {
            ConsoleLogger.Log("    authentication -> initialize");
            if (antdlib.Auth.T2FA.Config.ValueExists == false) {
                antdlib.Auth.T2FA.Config.Disable();
            }
        }

        public static void LaunchApps() {
            if (AssemblyInfo.IsUnix == true) {
                var apps = Management.DetectApps();
                if (apps.Length > 0) {
                    foreach (var app in apps) {
                        var dirs = Management.GetWantedDirectories(app);
                        if (dirs.Length > 0) {
                            foreach (var dir in dirs) {
                                Mount.Dir(dir);
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(10);
                AnthillaSP.SetApp();
            }
        }

        public static void ReloadSSH() {
            Terminal.Execute("systemctl restart sshd.service");
        }
    }
}