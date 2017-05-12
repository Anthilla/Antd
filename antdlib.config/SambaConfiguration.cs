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
