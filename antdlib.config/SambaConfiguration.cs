using antdlib.models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;

namespace antdlib.config {
    public static class SambaConfiguration {

        private static SambaConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/samba.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/samba.conf.bck";
        private const string ServiceName1 = "smbd.service";
        private const string ServiceName2 = "nmbd.service";
        private const string ServiceName3 = "winbindd.service";
        private const string MainFilePath = "/etc/samba/smb.conf";
        private const string MainFilePathBackup = "/etc/samba/.smb.conf";

        private static SambaConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new SambaConfigurationModel();
            }
            var text = File.ReadAllText(CfgFile);
            var obj = JsonConvert.DeserializeObject<SambaConfigurationModel>(text);
            return obj;
        }

        public static void Save(SambaConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(CfgFile)) {
                File.Copy(CfgFile, CfgFileBackup, true);
            }
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[samba] configuration saved");
        }

        public static void Set() {
            Enable();
            Stop();
            #region [    smb.conf generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var global = ServiceModel;
            var lines = new List<string> {
                "[global]",
                $"dos charset = {global.DosCharset}",
                $"workgroup = {global.Workgroup}",
                $"server string = {global.ServerString}",
                $"map to guest = {global.MapToGuest}",
                $"obey pam restrictions = {global.ObeyPamRestrictions}",
                $"guest account = {global.GuestAccount}",
                $"pam password change = {global.PamPasswordChange}",
                $"passwd program = {global.PasswdProgram}",
                $"unix password sync = {global.UnixPasswordSync}",
                $"reset on zero vc = {global.ResetOnZeroVc}",
                $"hostname lookups = {global.HostnameLookups}",
                $"load printers = {global.LoadPrinters}",
                $"printcap name = {global.PrintcapName}",
                $"disable spoolss = {global.DisableSpoolss}",
                $"template shell = {global.TemplateShell}",
                $"winbind enum users = {global.WinbindEnumUsers}",
                $"winbind enum groups = {global.WinbindEnumGroups}",
                $"winbind use default domain = {global.WinbindUseDefaultDomain}",
                $"winbind nss info = {global.WinbindNssInfo}",
                $"winbind refresh tickets = {global.WinbindRefreshTickets}",
                $"winbind normalize names = {global.WinbindNormalizeNames}",
                $"recycle:touch = {global.RecycleTouch}",
                $"recycle:keeptree = {global.RecycleKeeptree}",
                $"recycle:repository = {global.RecycleRepository}",
                $"nfs4:chown = {global.Nfs4Chown}",
                $"nfs4:acedup = {global.Nfs4Acedup}",
                $"nfs4:mode = {global.Nfs4Mode}",
                $"shadow:format = {global.ShadowFormat}",
                $"shadow:localtime = {global.ShadowLocaltime}",
                $"shadow:sort = {global.ShadowSort}",
                $"shadow:snapdir = {global.ShadowSnapdir}",
                $"rpc_server:default = {global.RpcServerDefault}",
                $"rpc_server:svcctl = {global.RpcServerSvcctl}",
                $"rpc_server:srvsvc = {global.RpcServerSrvsvc}",
                $"rpc_server:eventlog = {global.RpcServerEventlog}",
                $"rpc_server:ntsvcs = {global.RpcServerNtsvcs}",
                $"rpc_server:winreg = {global.RpcServerWinreg}",
                $"rpc_server:spoolss = {global.RpcServerSpoolss}",
                $"rpc_daemon:spoolssd = {global.RpcDaemonSpoolssd}",
                $"rpc_server:tcpip = {global.RpcServerTcpip}",
                $"idmap config * : backend = {global.IdmapConfigBackend}",
                $"read only = {global.ReadOnly}",
                $"guest ok = {global.GuestOk}",
                $"aio read size = {global.AioReadSize}",
                $"aio write size = {global.AioWriteSize}",
                $"ea support = {global.EaSupport}",
                $"directory name cache size = {global.DirectoryNameCacheSize}",
                $"case sensitive = {global.CaseSensitive}",
                $"map readonly = {global.MapReadonly}",
                $"store dos attributes = {global.StoreDosAttributes}",
                $"wide links = {global.WideLinks}",
                $"dos filetime resolution = {global.DosFiletimeResolution}",
                $"vfs objects = {global.VfsObjects}",
                ""
            };

            var resources = ServiceModel.Resources;
            foreach(var resource in resources) {
                lines.Add($"[{resource.Name}]");
                if(!string.IsNullOrEmpty(resource.Comment)) {
                    lines.Add($"comment = {resource.Comment}");
                }
                lines.Add($"path = {resource.Path}");
                lines.Add("");
            }
            FileWithAcl.WriteAllLines(MainFilePath, lines, "644", "root", "wheel");
            #endregion
            Start();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static SambaConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[samba] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[samba] disabled");
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

        public static void AddResource(SambaConfigurationResourceModel model) {
            var resources = ServiceModel.Resources;
            if(resources.Any(_ => _.Name == model.Name)) {
                return;
            }
            resources.Add(model);
            ServiceModel.Resources = resources;
            Save(ServiceModel);
        }

        public static void RemoveResource(string guid) {
            var resources = ServiceModel.Resources;
            var model = resources.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            resources.Remove(model);
            ServiceModel.Resources = resources;
            Save(ServiceModel);
        }
    }
}
