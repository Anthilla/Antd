using System.Collections.Generic;
using System.IO;
using anthilla.core;

namespace Antd.cmds {

    public class Samba {

        private const string ServiceName1 = "smbd.service";
        private const string ServiceName2 = "nmbd.service";
        private const string ServiceName3 = "winbindd.service";
        private const string MainFilePath = "/etc/samba/smb.conf";
        private const string MainFilePathBackup = "/etc/samba/smb.conf.bck";

        public static void Parse() {
            return;
        }

        public static void Apply() {
            var options = Application.CurrentConfiguration.Services.Samba;
            if(options == null) {
                return;
            }
            Stop();
            #region [    smb.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "[global]",
                $"dos charset = {options.DosCharset}",
                $"workgroup = {options.Workgroup}",
                $"server string = {options.ServerString}",
                $"map to guest = {options.MapToGuest}",
                $"obey pam restrictions = {options.ObeyPamRestrictions}",
                $"guest account = {options.GuestAccount}",
                $"pam password change = {options.PamPasswordChange}",
                $"passwd program = {options.PasswdProgram}",
                $"unix password sync = {options.UnixPasswordSync}",
                $"reset on zero vc = {options.ResetOnZeroVc}",
                $"hostname lookups = {options.HostnameLookups}",
                $"load printers = {options.LoadPrinters}",
                $"printcap name = {options.PrintcapName}",
                $"disable spoolss = {options.DisableSpoolss}",
                $"template shell = {options.TemplateShell}",
                $"winbind enum users = {options.WinbindEnumUsers}",
                $"winbind enum groups = {options.WinbindEnumGroups}",
                $"winbind use default domain = {options.WinbindUseDefaultDomain}",
                $"winbind nss info = {options.WinbindNssInfo}",
                $"winbind refresh tickets = {options.WinbindRefreshTickets}",
                $"winbind normalize names = {options.WinbindNormalizeNames}",
                $"recycle:touch = {options.RecycleTouch}",
                $"recycle:keeptree = {options.RecycleKeeptree}",
                $"recycle:repository = {options.RecycleRepository}",
                $"nfs4:chown = {options.Nfs4Chown}",
                $"nfs4:acedup = {options.Nfs4Acedup}",
                $"nfs4:mode = {options.Nfs4Mode}",
                $"shadow:format = {options.ShadowFormat}",
                $"shadow:localtime = {options.ShadowLocaltime}",
                $"shadow:sort = {options.ShadowSort}",
                $"shadow:snapdir = {options.ShadowSnapdir}",
                $"rpc_server:default = {options.RpcServerDefault}",
                $"rpc_server:svcctl = {options.RpcServerSvcctl}",
                $"rpc_server:srvsvc = {options.RpcServerSrvsvc}",
                $"rpc_server:eventlog = {options.RpcServerEventlog}",
                $"rpc_server:ntsvcs = {options.RpcServerNtsvcs}",
                $"rpc_server:winreg = {options.RpcServerWinreg}",
                $"rpc_server:spoolss = {options.RpcServerSpoolss}",
                $"rpc_daemon:spoolssd = {options.RpcDaemonSpoolssd}",
                $"rpc_server:tcpip = {options.RpcServerTcpip}",
                $"idmap config * : backend = {options.IdmapConfigBackend}",
                $"read only = {options.ReadOnly}",
                $"guest ok = {options.GuestOk}",
                $"aio read size = {options.AioReadSize}",
                $"aio write size = {options.AioWriteSize}",
                $"ea support = {options.EaSupport}",
                $"directory name cache size = {options.DirectoryNameCacheSize}",
                $"case sensitive = {options.CaseSensitive}",
                $"map readonly = {options.MapReadonly}",
                $"store dos attributes = {options.StoreDosAttributes}",
                $"wide links = {options.WideLinks}",
                $"dos filetime resolution = {options.DosFiletimeResolution}",
                $"vfs objects = {options.VfsObjects}",
                ""
            };

            var resources = options.Resources;
            foreach(var resource in resources) {
                lines.Add($"[{resource.Name}]");
                if(!string.IsNullOrEmpty(resource.Comment)) {
                    lines.Add($"comment = {resource.Comment}");
                }
                lines.Add($"path = {resource.Path}");
                lines.Add("");
            }
            File.WriteAllLines(MainFilePath, lines);
            #endregion
            Start();
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName1);
            Systemctl.Stop(ServiceName2);
            Systemctl.Stop(ServiceName3);
            ConsoleLogger.Log("[samba] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName1) == false) {
                Systemctl.Enable(ServiceName1);
            }
            if(Systemctl.IsActive(ServiceName1) == false) {
                Systemctl.Restart(ServiceName1);
            }
            if(Systemctl.IsEnabled(ServiceName2) == false) {
                Systemctl.Enable(ServiceName2);
            }
            if(Systemctl.IsActive(ServiceName2) == false) {
                Systemctl.Restart(ServiceName2);
            }
            if(Systemctl.IsEnabled(ServiceName3) == false) {
                Systemctl.Enable(ServiceName3);
            }
            if(Systemctl.IsActive(ServiceName3) == false) {
                Systemctl.Restart(ServiceName3);
            }
            ConsoleLogger.Log("[samba] start");
        }
    }
}