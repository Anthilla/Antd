using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class DhcpServerOptionsModel : EntityModel {
        public DhcpServerOptionsModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public DhcpServerOptionsModel(DhcpServerOptionsSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Allow = sourceModel.Allow.SplitToList();
            UpdateStaticLeases = sourceModel.UpdateStaticLeases;
            UpdateConflictDetection = sourceModel.UpdateConflictDetection;
            UseHostDeclNames = sourceModel.UseHostDeclNames;
            DoForwardUpdates = sourceModel.DoForwardUpdates;
            DoReverseUpdates = sourceModel.DoReverseUpdates;
            LogFacility = sourceModel.LogFacility;
            Option = sourceModel.Option.SplitToList();
            ZoneName = sourceModel.ZoneName;
            ZonePrimaryAddress = sourceModel.ZonePrimaryAddress;
            DdnsUpdateStyle = sourceModel.DdnsUpdateStyle;
            DdnsUpdates = sourceModel.DdnsUpdates;
            DdnsDomainName = sourceModel.DdnsDomainName;
            DdnsRevDomainName = sourceModel.DdnsRevDomainName;
            DefaultLeaseTime = sourceModel.DefaultLeaseTime;
            MaxLeaseTime = sourceModel.MaxLeaseTime;
            KeyName = sourceModel.KeyName;
            KeySecret = sourceModel.KeySecret;
        }
        public List<string> Allow { get; set; }
        public string UpdateStaticLeases { get; set; } //on off
        public string UpdateConflictDetection { get; set; }
        public string UseHostDeclNames { get; set; } //on off
        public string DoForwardUpdates { get; set; } //on off
        public string DoReverseUpdates { get; set; } //on off
        public string LogFacility { get; set; }
        public List<string> Option { get; set; }
        public string ZoneName { get; set; }
        public string ZonePrimaryAddress { get; set; }
        public string DdnsUpdateStyle { get; set; }
        public string DdnsUpdates { get; set; }
        public string DdnsDomainName { get; set; }
        public string DdnsRevDomainName { get; set; }
        public string DefaultLeaseTime { get; set; }
        public string MaxLeaseTime { get; set; }
        public string KeyName { get; set; }
        public string KeySecret { get; set; }
    }

    #region [    View    ]
    public class DhcpServerOptionsSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Allow { get; set; }
        public string UpdateStaticLeases { get; set; } //on off
        public string UpdateConflictDetection { get; set; }
        public string UseHostDeclNames { get; set; } //on off
        public string DoForwardUpdates { get; set; } //on off
        public string DoReverseUpdates { get; set; } //on off
        public string LogFacility { get; set; }
        public string Option { get; set; }
        public string ZoneName { get; set; }
        public string ZonePrimaryAddress { get; set; }
        public string DdnsUpdateStyle { get; set; }
        public string DdnsUpdates { get; set; }
        public string DdnsDomainName { get; set; }
        public string DdnsRevDomainName { get; set; }
        public string DefaultLeaseTime { get; set; }
        public string MaxLeaseTime { get; set; }
        public string KeyName { get; set; }
        public string KeySecret { get; set; }
    }

    [RegisterView]
    public class DhcpServerOptionsView : View<DhcpServerOptionsModel> {
        public DhcpServerOptionsView() {
            Name = "DhcpServerOptions";
            Description = "Primary view for DhcpServerOptionsModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(DhcpServerOptionsSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<DhcpServerOptionsModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaDhcpServerOptionss = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Allow.JoinToString(),
                    doc.UpdateStaticLeases,
                    doc.UpdateConflictDetection,
                    doc.UseHostDeclNames,
                    doc.DoForwardUpdates,
                    doc.DoReverseUpdates,
                    doc.LogFacility,
                    doc.Option.JoinToString(),
                    doc.ZoneName,
                    doc.ZonePrimaryAddress,
                    doc.DdnsUpdateStyle,
                    doc.DdnsUpdates,
                    doc.DdnsDomainName,
                    doc.DdnsRevDomainName,
                    doc.DefaultLeaseTime,
                    doc.MaxLeaseTime,
                    doc.KeyName,
                    doc.KeySecret
                };
                api.Emit(docid, schemaDhcpServerOptionss);
            };
        }
    }
    #endregion
}