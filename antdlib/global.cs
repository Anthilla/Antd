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
using System.Reflection;
using System.Runtime.InteropServices;

namespace antdlib {

    public class Label {
        public static string Root => "antd_root";
        public static string Port => "antd_port";
        public static string Database => "antd_database";
        public static string Files => "antd_files";

        public class Smtp {
            public static string Url => "smtp_url";
            public static string Port => "smtp_port";
            public static string Account => "smtp_account";
            public static string Password => "smtp_password";
        }

        public class Imap {
            public static string Url => "imap_url";
            public static string Port => "imap_port";
            public static string Account => "imap_account";
            public static string Password => "imap_password";
        }

        public class Auth {
            public static string IsEnabled => "isenabled";
        }
    }

    public class Folder {
        public static string RootData => "/Data";
        public static string RootFramework => "/framework";
        public static string RootSystem => "/System";
        public static string RootPorts => "/ports";
        public static string RootCfg => "/cfg";

        public static string AntdCfg => $"{RootCfg}/antd";
        public static string AntdCfgDatabase => $"{AntdCfg}/database";
        public static string AntdCfgDatabaseName => "antd-database";
        public static string AntdCfgDatabaseNamePath => $"{AntdCfgDatabase}/{AntdCfgDatabaseName}";
        public static string AntdCfgDatabaseJournalPath => $"{AntdCfgDatabaseNamePath}/denso.jnl";
        public static string AntdCfgNetworkd => $"{AntdCfg}/networkd";
        public static string AntdCfgKeys => $"{AntdCfg}/keys";
        public static string AntdCfgReport => $"{AntdCfg}/report";

        public static string Repo => "/mnt/cdrom";
        public static string RepoApps => $"{Repo}/Apps";
        public static string RepoBackup => $"{Repo}/Backup";
        public static string RepoConfig => $"{Repo}/Config";
        public static string RepoDirs => $"{Repo}/DIRS";
        public static string RepoKernel => $"{Repo}/Kernel";
        public static string RepoOverlay => $"{Repo}/Overlay";
        public static string RepoScripts => $"{Repo}/Scripts";
        public static string RepoSecureBoot => $"{Repo}/SecureBoot";
        public static string RepoSystem => $"{Repo}/System";
        public static string RepoUnits => $"{Repo}/Units";
        public static string RepoBoot => $"{Repo}/boot";
        public static string RepoEfi => $"{Repo}/efi";
        public static string RepoGrub => $"{Repo}/grub";
        public static string RepoLivecdFile => $"{Repo}/livecd";

        public static string Livecd => "/mnt/livecd";
        public static string AppsUnits => $"{RepoUnits}/applicative.target.wants";
        public static string WebsocketUnits => $"{RepoUnits}/websocket.target.wants";
        public static string KernelUnits => $"{RepoUnits}/kernelpkgload.target.wants";
        public static string AntdVersionsDir => $"{RepoApps}/Anthilla_Antd";
        public static string AntdshVersionsDir => $"{RepoApps}/Anthilla_antdsh";
        public static string AntdTmpDir => $"{RepoApps}/tmp";
        public static string CertificateAuthority => $"{RepoConfig}/ca";
        public static string Resources => $"{RootFramework}/antd/Resources";
    }

    public class AntdFile {
        public const string NetworkConfig = "antd.boot.network.conf";
        public const string FirewallConfig = "antd.boot.firewall.conf";
        public const string ZipStartsWith = "antd";
        public const string ZipEndsWith = ".7z";
        public const string SquashStartsWith = "DIR_framework_antd-";
        public const string SquashEndsWith = ".squashfs.xz";
        public const string ConfigFile = "antdsh.config";
        public const string AntdRunning = "active-version";
        public const string DownloadName = "antdDownload.zip";
        public const string DownloadFirstDir = "antdDownloadFirst";
    }

    public class Uid {
        public static string Guid => System.Guid.NewGuid().ToString();
        public static string ShortGuid => System.Guid.NewGuid().ToString().Substring(0, 8);
    }

    public class Port {
        public static string Websocket => "8888";
    }

    public class AssemblyInfo {
        public const string DateFormat = "yyyyMMdd";

        private static string GetGuid() {
            var assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return attribute.Value;
        }

        public static string Guid => GetGuid();

        public static OperatingSystem OS => Environment.OSVersion;

        public static PlatformID Platform => Environment.OSVersion.Platform;

        public static bool IsUnix => (Platform == PlatformID.Unix);
    }

    public class Units {

        public class Name {
            public static string NamePrepare => "app-antd-01-prepare.service";
            public static string NameMount => "app-antd-02-mount.service";
            public static string NameLauncher => "app-antd-03-launcher.service";
        }

        public static string Prepare = $"{Folder.AppsUnits}/{Name.NamePrepare}";
        public static string Mount = $"{Folder.AppsUnits}/{Name.NameMount}";
        public static string Launcher = $"{Folder.AppsUnits}/{Name.NameLauncher}";
    }

    public class Update {
        public const string RemoteRepo = "http://srv.anthilla.com:8081";
        public const string RemoteAntdDir = "antd-update";
        public const string RemoteAntdshDir = "antdsh-update";
        public const string RemoteUpdateInfo = "update.txt";
    }
}
