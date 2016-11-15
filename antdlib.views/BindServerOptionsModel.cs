using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class BindServerOptionsModel : EntityModel {
        public BindServerOptionsModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public BindServerOptionsModel(BindServerOptionsSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Notify = sourceModel.Notify;
            MaxCacheSize = sourceModel.MaxCacheSize;
            MaxCacheTtl = sourceModel.MaxCacheTtl;
            MaxNcacheTtl = sourceModel.MaxNcacheTtl;
            Forwarders = sourceModel.Forwarders.SplitToList();
            AllowNotify = sourceModel.AllowNotify.SplitToList();
            AllowTransfer = sourceModel.AllowTransfer.SplitToList();
            Recursion = sourceModel.Recursion;
            TransferFormat = sourceModel.TransferFormat;
            QuerySourceAddress = sourceModel.QuerySourceAddress;
            QuerySourcePort = sourceModel.QuerySourcePort;
            Version = sourceModel.Version;
            AllowQuery = sourceModel.AllowQuery.SplitToList();
            AllowRecursion = sourceModel.AllowRecursion.SplitToList();
            IxfrFromDifferences = sourceModel.IxfrFromDifferences;
            ListenOnV6 = sourceModel.ListenOnV6.SplitToList();
            ListenOnPort53 = sourceModel.ListenOnPort53.SplitToList();
            DnssecEnabled = sourceModel.DnssecEnabled;
            DnssecValidation = sourceModel.DnssecValidation;
            DnssecLookaside = sourceModel.DnssecLookaside;
            AuthNxdomain = sourceModel.AuthNxdomain;
            KeyName = sourceModel.KeyName;
            KeySecret = sourceModel.KeySecret;
            ControlAcl = sourceModel.ControlAcl;
            ControlIp = sourceModel.ControlIp;
            ControlPort = sourceModel.ControlPort;
            ControlAllow = sourceModel.ControlAllow.SplitToList();
            LoggingChannel = sourceModel.LoggingChannel;
            LoggingDaemon = sourceModel.LoggingDaemon;
            LoggingSeverity = sourceModel.LoggingSeverity;
            LoggingPrintCategory = sourceModel.LoggingPrintCategory;
            LoggingPrintSeverity = sourceModel.LoggingPrintSeverity;
            LoggingPrintTime = sourceModel.LoggingPrintTime;
            TrustedKeys = sourceModel.TrustedKeys;
            AclLocalInterfaces = sourceModel.AclLocalInterfaces.SplitToList();
            AclInternalInterfaces = sourceModel.AclInternalInterfaces.SplitToList();
            AclExternalInterfaces = sourceModel.AclExternalInterfaces.SplitToList();
            AclLocalNetworks = sourceModel.AclLocalNetworks.SplitToList();
            AclInternalNetworks = sourceModel.AclInternalNetworks.SplitToList();
            AclExternalNetworks = sourceModel.AclExternalNetworks.SplitToList();
        }
        public string Notify { get; set; } = "no";
        public string MaxCacheSize { get; set; } = "128M";
        public string MaxCacheTtl { get; set; } = "108000";
        public string MaxNcacheTtl { get; set; } = "3";
        public List<string> Forwarders { get; set; } = new List<string> { "8.8.8.8", "4.4.4.4" };
        public List<string> AllowNotify { get; set; } = new List<string> { "iif", "inet" };
        public List<string> AllowTransfer { get; set; } = new List<string> { "iif", "inet" };
        public string Recursion { get; set; } = "yes";
        public string TransferFormat { get; set; } = "many-answers";
        public string QuerySourceAddress { get; set; } = "*";
        public string QuerySourcePort { get; set; } = "*";
        public string Version { get; set; } = "none";
        public List<string> AllowQuery { get; set; } = new List<string> { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public List<string> AllowRecursion { get; set; } = new List<string> { "loif", "iif", "oif", "lonet", "inet", "onet" };
        public string IxfrFromDifferences { get; set; } = "yes";
        public List<string> ListenOnV6 { get; set; } = new List<string> { "none" };
        public List<string> ListenOnPort53 { get; set; } = new List<string> { "loif", "iif", "oif" };
        public string DnssecEnabled { get; set; } = "yes";
        public string DnssecValidation { get; set; } = "yes";
        public string DnssecLookaside { get; set; } = "auto";
        public string AuthNxdomain { get; set; } = "yes";
        public string KeyName { get; set; }
        public string KeySecret { get; set; }
        public string ControlAcl { get; set; } = "inet";
        public string ControlIp { get; set; } = "10.1.19.1";
        public string ControlPort { get; set; } = "953";
        public List<string> ControlAllow { get; set; } = new List<string> { "loif", "iif", "oif" };
        public string LoggingChannel { get; set; } = "syslog";
        public string LoggingDaemon { get; set; } = "syslogsyslog daemon";
        public string LoggingSeverity { get; set; } = "info";
        public string LoggingPrintCategory { get; set; } = "yes";
        public string LoggingPrintSeverity { get; set; } = "yes";
        public string LoggingPrintTime { get; set; } = "yes";
        public string TrustedKeys { get; set; }
        public List<string> AclLocalInterfaces { get; set; } = new List<string> { "127.0.0.1" };
        public List<string> AclInternalInterfaces { get; set; } = new List<string> { "10.1.19.1", "10.99.19.1" };
        public List<string> AclExternalInterfaces { get; set; } = new List<string> { "192.168.111.2", "192.168.222.2" };
        public List<string> AclLocalNetworks { get; set; } = new List<string> { "127.0.0.0/8" };
        public List<string> AclInternalNetworks { get; set; } = new List<string> { "10.1.0.0/16", "10.99.0.0/16" };
        public List<string> AclExternalNetworks { get; set; } = new List<string> { "192.168.111.2/32", "192.168.222.2/32" };
    }

    #region [    View    ]
    public class BindServerOptionsSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Notify { get; set; }
        public string MaxCacheSize { get; set; }
        public string MaxCacheTtl { get; set; }
        public string MaxNcacheTtl { get; set; }
        public string Forwarders { get; set; }
        public string AllowNotify { get; set; }
        public string AllowTransfer { get; set; }
        public string Recursion { get; set; }
        public string TransferFormat { get; set; }
        public string QuerySourceAddress { get; set; }
        public string QuerySourcePort { get; set; }
        public string Version { get; set; }
        public string AllowQuery { get; set; }
        public string AllowRecursion { get; set; }
        public string IxfrFromDifferences { get; set; }
        public string ListenOnV6 { get; set; }
        public string ListenOnPort53 { get; set; }
        public string DnssecEnabled { get; set; }
        public string DnssecValidation { get; set; }
        public string DnssecLookaside { get; set; }
        public string AuthNxdomain { get; set; }
        public string KeyName { get; set; }
        public string KeySecret { get; set; }
        public string ControlAcl { get; set; }
        public string ControlIp { get; set; }
        public string ControlPort { get; set; }
        public string ControlAllow { get; set; }
        public string LoggingChannel { get; set; }
        public string LoggingDaemon { get; set; }
        public string LoggingSeverity { get; set; }
        public string LoggingPrintCategory { get; set; }
        public string LoggingPrintSeverity { get; set; }
        public string LoggingPrintTime { get; set; }
        public string TrustedKeys { get; set; }
        public string AclLocalInterfaces { get; set; }
        public string AclInternalInterfaces { get; set; }
        public string AclExternalInterfaces { get; set; }
        public string AclLocalNetworks { get; set; }
        public string AclInternalNetworks { get; set; }
        public string AclExternalNetworks { get; set; }
    }

    [RegisterView]
    public class BindServerOptionsView : View<BindServerOptionsModel> {
        public BindServerOptionsView() {
            Name = "BindServerOptions";
            Description = "Primary view for BindServerOptionsModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(BindServerOptionsSchema);
            Mapper = (api, docid, doc) => {
                if(doc.Status != EntityStatus.New)
                    return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<BindServerOptionsModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaBindServerOptionss = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Notify,
                    doc.MaxCacheSize,
                    doc.MaxCacheTtl,
                    doc.MaxNcacheTtl,
                    doc.Forwarders.JoinToString(),
                    doc.AllowNotify.JoinToString(),
                    doc.AllowTransfer.JoinToString(),
                    doc.Recursion,
                    doc.TransferFormat,
                    doc.QuerySourceAddress,
                    doc.QuerySourcePort,
                    doc.Version,
                    doc.AllowQuery.JoinToString(),
                    doc.AllowRecursion.JoinToString(),
                    doc.IxfrFromDifferences,
                    doc.ListenOnV6.JoinToString(),
                    doc.ListenOnPort53.JoinToString(),
                    doc.DnssecEnabled,
                    doc.DnssecValidation,
                    doc.DnssecLookaside,
                    doc.AuthNxdomain,
                    doc.KeyName,
                    doc.KeySecret,
                    doc.ControlAcl,
                    doc.ControlIp,
                    doc.ControlPort,
                    doc.ControlAllow.JoinToString(),
                    doc.LoggingChannel,
                    doc.LoggingDaemon,
                    doc.LoggingSeverity,
                    doc.LoggingPrintCategory,
                    doc.LoggingPrintSeverity,
                    doc.LoggingPrintTime,
                    doc.TrustedKeys,
                    doc.AclLocalInterfaces.JoinToString(),
                    doc.AclInternalInterfaces.JoinToString(),
                    doc.AclExternalInterfaces.JoinToString(),
                    doc.AclLocalNetworks.JoinToString(),
                    doc.AclInternalNetworks.JoinToString(),
                    doc.AclExternalNetworks.JoinToString()
                };
                api.Emit(docid, schemaBindServerOptionss);
            };
        }
    }
    #endregion
}