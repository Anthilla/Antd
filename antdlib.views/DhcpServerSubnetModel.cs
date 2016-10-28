using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class DhcpServerSubnetModel : EntityModel {
        public DhcpServerSubnetModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public DhcpServerSubnetModel(DhcpServerSubnetSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            OptionRouters = sourceModel.OptionRouters;
            NtpServers = sourceModel.NtpServers;
            TimeServers = sourceModel.TimeServers;
            DomainNameServers = sourceModel.DomainNameServers;
            BroadcastAddress = sourceModel.BroadcastAddress;
            SubnetMask = sourceModel.SubnetMask;
            ZoneName = sourceModel.ZoneName;
            ZonePrimaryAddress = sourceModel.ZonePrimaryAddress;
        }
        public string IpFamily { get; set; }
        public string IpMask { get; set; }
        public string OptionRouters { get; set; }
        public string NtpServers { get; set; }
        public string TimeServers { get; set; }
        public string DomainNameServers { get; set; }
        public string BroadcastAddress { get; set; }
        public string SubnetMask { get; set; }
        public string ZoneName { get; set; }
        public string ZonePrimaryAddress { get; set; }
    }

    #region [    View    ]
    public class DhcpServerSubnetSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string IpFamily { get; set; }
        public string IpMask { get; set; }
        public string OptionRouters { get; set; }
        public string NtpServers { get; set; }
        public string TimeServers { get; set; }
        public string DomainNameServers { get; set; }
        public string BroadcastAddress { get; set; }
        public string SubnetMask { get; set; }
        public string ZoneName { get; set; }
        public string ZonePrimaryAddress { get; set; }
    }

    [RegisterView]
    public class DhcpServerSubnetView : View<DhcpServerSubnetModel> {
        public DhcpServerSubnetView() {
            Name = "DhcpServerSubnet";
            Description = "Primary view for DhcpServerSubnetModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(DhcpServerSubnetSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<DhcpServerSubnetModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaDhcpServerSubnets = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.IpFamily,
                    doc.IpMask,
                    doc.OptionRouters,
                    doc.NtpServers,
                    doc.TimeServers,
                    doc.DomainNameServers,
                    doc.BroadcastAddress,
                    doc.SubnetMask,
                    doc.ZoneName,
                    doc.ZonePrimaryAddress
                };
                api.Emit(docid, schemaDhcpServerSubnets);
            };
        }
    }
    #endregion
}