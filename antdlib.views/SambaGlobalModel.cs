using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class SambaGlobalModel : EntityModel {
        public SambaGlobalModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public SambaGlobalModel(SambaGlobalSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            DosCharset = sourceModel.DosCharset;
            Workgroup = sourceModel.Workgroup;
            ServerString = sourceModel.ServerString;
            MapToGuest = sourceModel.MapToGuest;
            ObeyPamRestrictions = sourceModel.ObeyPamRestrictions;
            GuestAccount = sourceModel.GuestAccount;
            PamPasswordChange = sourceModel.PamPasswordChange;
            PasswdProgram = sourceModel.PasswdProgram;
            UnixPasswordSync = sourceModel.UnixPasswordSync;
            ResetOnZeroVc = sourceModel.ResetOnZeroVc;
            HostnameLookups = sourceModel.HostnameLookups;
            LoadPrinters = sourceModel.LoadPrinters;
            PrintcapName = sourceModel.PrintcapName;
            DisableSpoolss = sourceModel.DisableSpoolss;
            TemplateShell = sourceModel.TemplateShell;
            WinbindEnumUsers = sourceModel.WinbindEnumUsers;
            WinbindEnumGroups = sourceModel.WinbindEnumGroups;
            WinbindUseDefaultDomain = sourceModel.WinbindUseDefaultDomain;
            WinbindNssInfo = sourceModel.WinbindNssInfo;
            WinbindRefreshTickets = sourceModel.WinbindRefreshTickets;
            WinbindNormalizeNames = sourceModel.WinbindNormalizeNames;
            RecycleTouch = sourceModel.RecycleTouch;
            RecycleKeeptree = sourceModel.RecycleKeeptree;
            RecycleRepository = sourceModel.RecycleRepository;
            Nfs4Chown = sourceModel.Nfs4Chown;
            Nfs4Acedup = sourceModel.Nfs4Acedup;
            Nfs4Mode = sourceModel.Nfs4Mode;
            ShadowFormat = sourceModel.ShadowFormat;
            ShadowLocaltime = sourceModel.ShadowLocaltime;
            ShadowSort = sourceModel.ShadowSort;
            ShadowSnapdir = sourceModel.ShadowSnapdir;
            RpcServerDefault = sourceModel.RpcServerDefault;
            RpcServerSvcctl = sourceModel.RpcServerSvcctl;
            RpcServerSrvsvc = sourceModel.RpcServerSrvsvc;
            RpcServerEventlog = sourceModel.RpcServerEventlog;
            RpcServerNtsvcs = sourceModel.RpcServerNtsvcs;
            RpcServerWinreg = sourceModel.RpcServerWinreg;
            RpcServerSpoolss = sourceModel.RpcServerSpoolss;
            RpcDaemonSpoolssd = sourceModel.RpcDaemonSpoolssd;
            RpcServerTcpip = sourceModel.RpcServerTcpip;
            IdmapConfigBackend = sourceModel.IdmapConfigBackend;
            ReadOnly = sourceModel.ReadOnly;
            GuestOk = sourceModel.GuestOk;
            AioReadSize = sourceModel.AioReadSize;
            AioWriteSize = sourceModel.AioWriteSize;
            EaSupport = sourceModel.EaSupport;
            DirectoryNameCacheSize = sourceModel.DirectoryNameCacheSize;
            CaseSensitive = sourceModel.CaseSensitive;
            MapReadonly = sourceModel.MapReadonly;
            StoreDosAttributes = sourceModel.StoreDosAttributes;
            WideLinks = sourceModel.WideLinks;
            DosFiletimeResolution = sourceModel.DosFiletimeResolution;
            VfsObjects = sourceModel.VfsObjects;
        }
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
    }

    #region [    View    ]
    public class SambaGlobalSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
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
    }

    [RegisterView]
    public class SambaGlobalView : View<SambaGlobalModel> {
        public SambaGlobalView() {
            Name = "SambaGlobal";
            Description = "Primary view for SambaGlobalModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(SambaGlobalSchema);
            Mapper = (api, docid, doc) => {
                if(doc.Status != EntityStatus.New)
                    return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<SambaGlobalModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaSambaGlobals = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.DosCharset,
                    doc.Workgroup,
                    doc.ServerString,
                    doc.MapToGuest,
                    doc.ObeyPamRestrictions,
                    doc.GuestAccount,
                    doc.PamPasswordChange,
                    doc.PasswdProgram,
                    doc.UnixPasswordSync,
                    doc.ResetOnZeroVc,
                    doc.HostnameLookups,
                    doc.LoadPrinters,
                    doc.PrintcapName,
                    doc.DisableSpoolss,
                    doc.TemplateShell,
                    doc.WinbindEnumUsers,
                    doc.WinbindEnumGroups,
                    doc.WinbindUseDefaultDomain,
                    doc.WinbindNssInfo,
                    doc.WinbindRefreshTickets,
                    doc.WinbindNormalizeNames,
                    doc.RecycleTouch,
                    doc.RecycleKeeptree,
                    doc.RecycleRepository,
                    doc.Nfs4Chown,
                    doc.Nfs4Acedup,
                    doc.Nfs4Mode,
                    doc.ShadowFormat,
                    doc.ShadowLocaltime,
                    doc.ShadowSort,
                    doc.ShadowSnapdir,
                    doc.RpcServerDefault,
                    doc.RpcServerSvcctl,
                    doc.RpcServerSrvsvc,
                    doc.RpcServerEventlog,
                    doc.RpcServerNtsvcs,
                    doc.RpcServerWinreg,
                    doc.RpcServerSpoolss,
                    doc.RpcDaemonSpoolssd,
                    doc.RpcServerTcpip,
                    doc.IdmapConfigBackend,
                    doc.ReadOnly,
                    doc.GuestOk,
                    doc.AioReadSize,
                    doc.AioWriteSize,
                    doc.EaSupport,
                    doc.DirectoryNameCacheSize,
                    doc.CaseSensitive,
                    doc.MapReadonly,
                    doc.StoreDosAttributes,
                    doc.WideLinks,
                    doc.DosFiletimeResolution,
                    doc.VfsObjects
                };
                api.Emit(docid, schemaSambaGlobals);
            };
        }
    }
    #endregion
}