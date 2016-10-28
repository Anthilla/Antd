using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class DhcpServerPoolModel : EntityModel {
        public DhcpServerPoolModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public DhcpServerPoolModel(DhcpServerPoolSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Options = sourceModel.Options.SplitToList();
        }
        public List<string> Options { get; set; }
    }

    #region [    View    ]
    public class DhcpServerPoolSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Options { get; set; }
    }

    [RegisterView]
    public class DhcpServerPoolView : View<DhcpServerPoolModel> {
        public DhcpServerPoolView() {
            Name = "DhcpServerPool";
            Description = "Primary view for DhcpServerPoolModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(DhcpServerPoolSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<DhcpServerPoolModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaDhcpServerPools = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Options.JoinToString()
                };
                api.Emit(docid, schemaDhcpServerPools);
            };
        }
    }
    #endregion
}