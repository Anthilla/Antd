using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class DhcpServerReservationModel : EntityModel {
        public DhcpServerReservationModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public DhcpServerReservationModel(DhcpServerReservationSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            HostName = sourceModel.HostName;
            MacAddress = sourceModel.MacAddress;
            IpAddress = sourceModel.IpAddress;
        }
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }

    #region [    View    ]
    public class DhcpServerReservationSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }

    [RegisterView]
    public class DhcpServerReservationView : View<DhcpServerReservationModel> {
        public DhcpServerReservationView() {
            Name = "DhcpServerReservation";
            Description = "Primary view for DhcpServerReservationModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(DhcpServerReservationSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<DhcpServerReservationModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaDhcpServerReservations = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.HostName,
                    doc.MacAddress,
                    doc.IpAddress
                };
                api.Emit(docid, schemaDhcpServerReservations);
            };
        }
    }
    #endregion
}