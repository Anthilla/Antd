using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class DhcpConfigModel : EntityModel {
        public DhcpConfigModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public DhcpConfigModel(DhcpConfigSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Config = sourceModel.Config;
        }
        public string Config { get; set; }
    }

    #region [    View    ]
    public class DhcpConfigSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Config { get; set; }
    }

    [RegisterView]
    public class DhcpConfigView : View<DhcpConfigModel> {
        public DhcpConfigView() {
            Name = "DhcpConfig";
            Description = "Primary view for DhcpConfigModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(DhcpConfigSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<DhcpConfigModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaDhcpConfigs = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Config
                };
                api.Emit(docid, schemaDhcpConfigs);
            };
        }
    }
    #endregion
}