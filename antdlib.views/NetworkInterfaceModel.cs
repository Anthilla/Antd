using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    public enum NetworkInterfaceType {
        Physical = 1,
        Virtual = 2,
        Bond = 3,
        Bridge = 4,
        Other = 99
    }

    [Serializable]
    public class NetworkInterfaceModel : EntityModel {
        public NetworkInterfaceModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public NetworkInterfaceModel(NetworkInterfaceSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Name = sourceModel.Name;
            Type = (NetworkInterfaceType)Enum.Parse(typeof(NetworkInterfaceType), sourceModel.Type);
        }
        public string Name { get; set; }
        public NetworkInterfaceType Type { get; set; }
    }

    #region [    View    ]
    public class NetworkInterfaceSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Name { get; set; }
        public string Type { get; set; }
    }

    [RegisterView]
    public class NetworkInterfaceView : View<NetworkInterfaceModel> {
        public NetworkInterfaceView() {
            Name = "NetworkInterface";
            Description = "Primary view for NetworkInterfaceModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(NetworkInterfaceSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<NetworkInterfaceModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaNetworkInterfaces = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Name,
                    doc.Type.ToString()
                };
                api.Emit(docid, schemaNetworkInterfaces);
            };
        }
    }
    #endregion
}