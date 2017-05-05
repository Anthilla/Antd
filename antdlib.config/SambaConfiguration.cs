using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public static class SambaConfiguration {

        private static SambaConfigurationModel _serviceModel => Load();

        private static readonly string _cfgFile = $"{Parameter.AntdCfgServices}/samba.conf";
        private static readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/samba.conf.bck";
        private const string ServiceName1 = "smbd.service";
        private const string ServiceName2 = "nmbd.service";
        private const string ServiceName3 = "winbindd.service";
        private const string MainFilePath = "/etc/samba/smb.conf";
        private const string MainFilePathBackup = "/etc/samba/.smb.conf";

        private static SambaConfigurationModel Load() {
            if(!File.Exists(_cfgFile)) {
                return new SambaConfigurationModel();
            }
            try {
                var text = File.ReadAllText(_cfgFile);
                var obj = JsonConvert.DeserializeObject<SambaConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new SambaConfigurationModel();
            }
        }

        public static void Save(SambaConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
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
            var global = _serviceModel;
            var lines = new List<string> {
                "[global]"
            };
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

            var resources = _serviceModel.Resources;
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

        public static bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public static SambaConfigurationModel Get() {
            return _serviceModel;
        }

        public static void Enable() {
            _serviceModel.IsActive = true;
            Save(_serviceModel);
            ConsoleLogger.Log("[samba] enabled");
        }

        public static void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
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
            var resources = _serviceModel.Resources;
            if(resources.Any(_ => _.Name == model.Name)) {
                return;
            }
            resources.Add(model);
            _serviceModel.Resources = resources;
            Save(_serviceModel);
        }

        public static void RemoveResource(string guid) {
            var resources = _serviceModel.Resources;
            var model = resources.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            resources.Remove(model);
            _serviceModel.Resources = resources;
            Save(_serviceModel);
        }
    }
}
