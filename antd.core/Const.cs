using System;

namespace antd.core {
    public class Const {
        public static string BasePath => "";
        public static string Cloud => "http://localhost:80/";

        public static string RootData => "/Data";
        public static string RootFramework => "/framework";
        public static string RootFrameworkAnthCloud => $"{RootFramework}/anthillacloud";
        public static string RootFrameworkAnthCloudResources => $"{RootFrameworkAnthCloud}/Resources";
        public static string RootSystem => "/System";
        public static string RootPorts => "/ports";
        public static string RootCfg => "/cfg";
        public static string Root => "/root";
        public static string RootSsh => "/root/.ssh";
        public static string RootSshMntCdrom => $"{RepoDirs}/DIR_root";

        public static string AntdCfg => $"{RootCfg}/antd";
        public static string AntdCfgMachineIdFile => $"{AntdCfg}/machine-id";
        public static string AntdCfgConf => $"{AntdCfg}/conf";
        public static string AntdCfgLog => $"{AntdCfg}/log";
        public static string AntdCfgKeys => $"{AntdCfg}/keys";
        public static string AntdCfgSecret => $"{AntdCfg}/secret";
        public static string AntdCfgSetup => $"{AntdCfg}/setup";
        public static string AntdCfgVfs => $"{AntdCfg}/vfs";
        public static string AntdCfgRestore => $"{AntdCfg}/restore";

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
        public static string ApplicativeUnits => $"{RepoUnits}/applicative.target.wants";
        public static string WebsocketUnits => $"{RepoUnits}/websocket.target.wants";
        public static string KernelUnits => $"{RepoUnits}/kernelpkgload.target.wants";
        public static string TimerUnits => $"{RepoUnits}/tt.target.wants";
        public static string AnthCloudVersionsDir => $"{RepoApps}/Anthilla_AnthCloud";
        public static string AnthCloudTmpDir => $"{RepoApps}/tmp";
        public static string Resources => $"{RootFramework}/anthillacloud/Resources";

        public static string CertificateAuthorityX509 => "anthillacloud";

        public static string Aossvc => "/usr/sbin/aossvc";

        public static string AuthKeys => "/root/.ssh/authorized_keys";

        public static string Home => "/home";

        public static string ExeAnthCloud => "AnthCloud.exe";

        public static PlatformID Platform => Environment.OSVersion.Platform;
        public static bool IsUnix => Platform == PlatformID.Unix;

        public static string UnitAnthCloudPrepare => "app-anthillacloud-01-prepare.service";
        public static string UnitAnthCloudMount => "app-anthillacloud-02-mount.service";
        public static string UnitAnthCloudLauncher => "app-anthillacloud-03-launcher.service";

        public static string EtcSsh => "/etc/ssh";

        public static string AppsUnits => $"{RepoUnits}/app.target.wants";
        public static string TimerUnitsLinks => $"{RepoUnits}/tt.target.wants";
        public static string AntdVersionsDir => $"{RepoApps}/Anthilla_Antd";
        public static string AntdshVersionsDir => $"{RepoApps}/Anthilla_antdsh";
        public static string AntdTmpDir => $"{RepoApps}/tmp";
        public static string CertificateAuthority => $"{RepoConfig}/ca";

        public static string UnitAntdPrepare => "app-antd-01-prepare.service";
        public static string UnitAntdMount => "app-antd-02-mount.service";
        public static string UnitAntdLauncher => "app-antd-03-launcher.service";

        public static string AnthSanistCfg => $"{RootCfg}/anthillasanist";
        public static string AnthSanistCfgDatabase => $"{RootCfg}/anthillasanist/database";
        public static string AnthSanistCfgFiles => $"{RootCfg}/anthillasanist/files";
    }
}
