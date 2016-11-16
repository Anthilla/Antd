using System.Collections.Generic;

namespace Antd.Samba {
    public class SambaConfigurationModel {
        public bool IsActive { get; set; }

        public string DosCharset { get; set; }
        public string Workgroup { get; set; }
        public string ServerString { get; set; }
        public string MapToGuest { get; set; }
        public string ObeyPamRestrictions { get; set; }
        public string GuestAccount { get; set; }
        public string PamPasswordChange { get; set; }
        public string PasswdProgram { get; set; }
        public string UnixPasswordSync { get; set; }
        public string ResetOnZeroVc { get; set; }
        public string HostnameLookups { get; set; }
        public string LoadPrinters { get; set; }
        public string PrintcapName { get; set; }
        public string DisableSpoolss { get; set; }
        public string TemplateShell { get; set; }
        public string WinbindEnumUsers { get; set; }
        public string WinbindEnumGroups { get; set; }
        public string WinbindUseDefaultDomain { get; set; }
        public string WinbindNssInfo { get; set; }
        public string WinbindRefreshTickets { get; set; }
        public string WinbindNormalizeNames { get; set; }
        public string RecycleTouch { get; set; }
        public string RecycleKeeptree { get; set; }
        public string RecycleRepository { get; set; }
        public string Nfs4Chown { get; set; }
        public string Nfs4Acedup { get; set; }
        public string Nfs4Mode { get; set; }
        public string ShadowFormat { get; set; }
        public string ShadowLocaltime { get; set; }
        public string ShadowSort { get; set; }
        public string ShadowSnapdir { get; set; }
        public string RpcServerDefault { get; set; }
        public string RpcServerSvcctl { get; set; }
        public string RpcServerSrvsvc { get; set; }
        public string RpcServerEventlog { get; set; }
        public string RpcServerNtsvcs { get; set; }
        public string RpcServerWinreg { get; set; }
        public string RpcServerSpoolss { get; set; }
        public string RpcDaemonSpoolssd { get; set; }
        public string RpcServerTcpip { get; set; }
        public string IdmapConfigBackend { get; set; }
        public string ReadOnly { get; set; }
        public string GuestOk { get; set; }
        public string AioReadSize { get; set; }
        public string AioWriteSize { get; set; }
        public string EaSupport { get; set; }
        public string DirectoryNameCacheSize { get; set; }
        public string CaseSensitive { get; set; }
        public string MapReadonly { get; set; }
        public string StoreDosAttributes { get; set; }
        public string WideLinks { get; set; }
        public string DosFiletimeResolution { get; set; }
        public string VfsObjects { get; set; }

        public List<SambaConfigurationResourceModel> Resources { get; set; } = new List<SambaConfigurationResourceModel>();
    }

    public class SambaConfigurationResourceModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Path { get; set; }
    }
}