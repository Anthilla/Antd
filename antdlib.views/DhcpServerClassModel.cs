using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class DhcpServerClassModel : EntityModel {
        public DhcpServerClassModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public DhcpServerClassModel(DhcpServerClassSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Name = sourceModel.Name;
            MacVendor = sourceModel.MacVendor;
        }
        public string Name { get; set; }
        public string MacVendor { get; set; }
    }

    #region [    View    ]
    public class DhcpServerClassSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Name { get; set; }
        public string MacVendor { get; set; }
    }

    [RegisterView]
    public class DhcpServerClassView : View<DhcpServerClassModel> {
        public DhcpServerClassView() {
            Name = "DhcpServerClass";
            Description = "Primary view for DhcpServerClassModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(DhcpServerClassSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<DhcpServerClassModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaDhcpServerClasss = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Name,
                    doc.MacVendor
                };
                api.Emit(docid, schemaDhcpServerClasss);
            };
        }
    }
    #endregion
}