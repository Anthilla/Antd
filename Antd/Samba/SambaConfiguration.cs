using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.Systemd;
using antdlib.views;
using Antd.Database;
using IoDir = System.IO.Directory;

namespace Antd.Samba {
    public class SambaConfiguration {

        private const string Directory = "/etc/samba";
        private const string ServiceName1 = "smbd.service";
        private const string ServiceName2 = "nmbd.service";
        private const string ServiceName3 = "winbindd.service";
        private const string MainFilePath = "/etc/samba/smb.conf";
        private const string MainFilePathBackup = "/etc/samba/.smb.conf";
        private readonly SambaGlobalRepository _sambaGlobalRepository = new SambaGlobalRepository();
        private readonly SambaResourceRepository _sambaResourceRepository = new SambaResourceRepository();

        public void Set() {
            if(!IoDir.Exists(Directory)) {
                IoDir.CreateDirectory(Directory);
            }
            Enable();
            Stop();

            #region [    smb.conf generation    ]
            var g = _sambaGlobalRepository.Get();
            if(g == null) {
                return;
            }
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string> {
                "[global]"
            };
            var global = new SambaGlobalModel(g);
            lines.Add($"dos charset = {global.DosCharset}");
            lines.Add($"workgroup = {global.Workgroup}");
            lines.Add($"server string = {global.ServerString}");
            lines.Add($"map to guest = {global.MapToGuest}");
            lines.Add($"obey pam restrictions = {global.ObeyPamRestrictions}");
            lines.Add($"guest account = {global.GuestAccount}");
            lines.Add($"pam password change = {global.PamPasswordChange}");
            lines.Add($"passwd program = {global.PasswdProgram}");
            lines.Add($"unix password sync = {global.UnixPasswordSync}");
            lines.Add($"reset on zero vc = {global.ResetOnZeroVc}");
            lines.Add($"hostname lookups = {global.HostnameLookups}");
            lines.Add($"load printers = {global.LoadPrinters}");
            lines.Add($"printcap name = {global.PrintcapName}");
            lines.Add($"disable spoolss = {global.DisableSpoolss}");
            lines.Add($"template shell = {global.TemplateShell}");
            lines.Add($"winbind enum users = {global.WinbindEnumUsers}");
            lines.Add($"winbind enum groups = {global.WinbindEnumGroups}");
            lines.Add($"winbind use default domain = {global.WinbindUseDefaultDomain}");
            lines.Add($"winbind nss info = {global.WinbindNssInfo}");
            lines.Add($"winbind refresh tickets = {global.WinbindRefreshTickets}");
            lines.Add($"winbind normalize names = {global.WinbindNormalizeNames}");
            lines.Add($"recycle:touch = {global.RecycleTouch}");
            lines.Add($"recycle:keeptree = {global.RecycleKeeptree}");
            lines.Add($"recycle:repository = {global.RecycleRepository}");
            lines.Add($"nfs4:chown = {global.Nfs4Chown}");
            lines.Add($"nfs4:acedup = {global.Nfs4Acedup}");
            lines.Add($"nfs4:mode = {global.Nfs4Mode}");
            lines.Add($"shadow:format = {global.ShadowFormat}");
            lines.Add($"shadow:localtime = {global.ShadowLocaltime}");
            lines.Add($"shadow:sort = {global.ShadowSort}");
            lines.Add($"shadow:snapdir = {global.ShadowSnapdir}");
            lines.Add($"rpc_server:default = {global.RpcServerDefault}");
            lines.Add($"rpc_server:svcctl = {global.RpcServerSvcctl}");
            lines.Add($"rpc_server:srvsvc = {global.RpcServerSrvsvc}");
            lines.Add($"rpc_server:eventlog = {global.RpcServerEventlog}");
            lines.Add($"rpc_server:ntsvcs = {global.RpcServerNtsvcs}");
            lines.Add($"rpc_server:winreg = {global.RpcServerWinreg}");
            lines.Add($"rpc_server:spoolss = {global.RpcServerSpoolss}");
            lines.Add($"rpc_daemon:spoolssd = {global.RpcDaemonSpoolssd}");
            lines.Add($"rpc_server:tcpip = {global.RpcServerTcpip}");
            lines.Add($"idmap config * : backend = {global.IdmapConfigBackend}");
            lines.Add($"read only = {global.ReadOnly}");
            lines.Add($"guest ok = {global.GuestOk}");
            lines.Add($"aio read size = {global.AioReadSize}");
            lines.Add($"aio write size = {global.AioWriteSize}");
            lines.Add($"ea support = {global.EaSupport}");
            lines.Add($"directory name cache size = {global.DirectoryNameCacheSize}");
            lines.Add($"case sensitive = {global.CaseSensitive}");
            lines.Add($"map readonly = {global.MapReadonly}");
            lines.Add($"store dos attributes = {global.StoreDosAttributes}");
            lines.Add($"wide links = {global.WideLinks}");
            lines.Add($"dos filetime resolution = {global.DosFiletimeResolution}");
            lines.Add($"vfs objects = {global.VfsObjects}");
            lines.Add("");

            var resources = _sambaResourceRepository.GetAll().Select(_ => new SambaResourceModel(_)).ToList();
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

            Restart();
        }

        public void Enable() {
            if(Systemctl.IsEnabled(ServiceName1) == false) {
                Systemctl.Enable(ServiceName1);
            }
            if(Systemctl.IsEnabled(ServiceName2) == false) {
                Systemctl.Enable(ServiceName2);
            }
            if(Systemctl.IsEnabled(ServiceName3) == false) {
                Systemctl.Enable(ServiceName3);
            }
        }

        public void Disable() {
            Systemctl.Disable(ServiceName1);
            Systemctl.Disable(ServiceName2);
            Systemctl.Disable(ServiceName3);
        }

        public void Stop() {
            Systemctl.Stop(ServiceName1);
            Systemctl.Stop(ServiceName2);
            Systemctl.Stop(ServiceName3);
        }

        public void Restart() {
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
        }
    }
}
