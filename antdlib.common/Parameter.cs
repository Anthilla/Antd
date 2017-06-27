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

namespace antdlib.common {
    public class Parameter {
        public static string Cloud => "http://api.anthilla.com:80/";

        public static string LabelAntdRoot => "antd_root";
        public static string LabelAntdPort => "antd_port";
        public static string LabelAntdDatabase => "antd_database";
        public static string LabelAntdFiles => "antd_files";
        public static string LabelSmtpUrl => "smtp_url";
        public static string LabelSmtpPort => "smtp_port";
        public static string LabelSmtpAccount => "smtp_account";
        public static string LabelSmtpPassword => "smtp_password";
        public static string LabelImapUrl => "imap_url";
        public static string LabelImapPort => "imap_port";
        public static string LabelImapAccount => "imap_account";
        public static string LabelImapPassword => "imap_password";
        public static string LabelAuthIsEnabled => "isenabled";

        public static string RootData => "/Data";
        public static string RootFramework => "/framework";
        public static string RootFrameworkAntd => $"{RootFramework}/antd";
        public static string RootFrameworkAntdResources => $"{RootFrameworkAntd}/Resources";
        public static string RootFrameworkAntdShellScript => $"{RootFrameworkAntd}/ShellScript";
        public static string RootFrameworkAntdsh => $"{RootFramework}/antdsh";
        public static string RootSystem => "/System";
        public static string RootPorts => "/ports";
        public static string RootCfg => "/cfg";
        public static string RootSsh => "/root";
        public static string RootSshMntCdrom => $"{RepoDirs}/DIR_root";

        public static string AntdCfg => $"{RootCfg}/antd";
        public static string AntdCfgServices => $"{AntdCfg}/services";
        public static string AntdCfgDatabase => $"{AntdCfg}/database";
        public static string AntdCfgNetworkd => $"{AntdCfg}/networkd";
        public static string AntdCfgNetwork => $"{AntdCfg}/network";
        public static string AntdCfgParameters => $"{AntdCfg}/parameters";
        public static string AntdCfgCluster => $"{AntdCfg}/cluster";
        public static string AntdCfgKeys => $"{AntdCfg}/keys";
        public static string AntdCfgReport => $"{AntdCfg}/report";
        public static string AntdCfgCommands => $"{AntdCfg}/commands";
        public static string AntdCfgSecret => $"{AntdCfg}/secret";
        public static string AntdCfgVfs => $"{AntdCfg}/vfs";

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
        public static string RepoTemp => $"{Repo}/temp";
        public static string RepoUnits => $"{Repo}/Units";
        public static string RepoBoot => $"{Repo}/boot";
        public static string RepoEfi => $"{Repo}/efi";
        public static string RepoGrub => $"{Repo}/grub";
        public static string RepoLivecdFile => $"{Repo}/livecd";

        public static string Overlay => "/mnt/overlay";

        public static string Livecd => "/mnt/livecd";
        public static string AnthillaUnits => $"{RepoUnits}/anthillaUnits";
        public static string AntdUnits => $"{RepoUnits}/antd.target.wants";
        public static string AppsUnits => $"{RepoUnits}/app.target.wants";
        public static string ApplicativeUnits => $"{RepoUnits}/applicative.target.wants";
        public static string WebsocketUnits => $"{RepoUnits}/websocket.target.wants";
        public static string KernelUnits => $"{RepoUnits}/kernelpkgload.target.wants";
        public static string TimerUnits => $"{RepoUnits}/ttUnits";
        public static string TimerUnitsLinks => $"{RepoUnits}/tt.target.wants";
        public static string AntdVersionsDir => $"{RepoApps}/Anthilla_Antd";
        public static string AntdshVersionsDir => $"{RepoApps}/Anthilla_antdsh";
        public static string AntdTmpDir => $"{RepoApps}/tmp";
        public static string CertificateAuthority => $"{RepoConfig}/ca";
        public static string Resources => $"{RootFramework}/antd/Resources";

        public static string CertificateAuthorityX509 => "antd";

        public static string Aossvc => "/usr/sbin/aossvc";

        public static string AuthKeys => "/root/.ssh/authorized_keys";

        public static string Home => "/home";

        public static string ExeAntd => "Antd.exe";
        public static string ExeAntdsh => "antdsh.exe";

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

        public static PlatformID Platform => Environment.OSVersion.Platform;
        public static bool IsUnix => Platform == PlatformID.Unix;

        public static string UnitAntdPrepare => "app-antd-01-prepare.service";
        public static string UnitAntdMount => "app-antd-02-mount.service";
        public static string UnitAntdLauncher => "app-antd-03-launcher.service";

        public static string EtcSsh => "/etc/ssh";
    }
}
