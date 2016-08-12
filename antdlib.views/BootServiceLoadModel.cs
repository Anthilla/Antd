using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class BootServiceLoadModel : EntityModel {
        public BootServiceLoadModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public BootServiceLoadModel(BootServiceLoadSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Services = sourceModel.Services.SplitToList();
        }
        public IEnumerable<string> Services { get; set; }
    }

    #region [    View    ]
    public class BootServiceLoadSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Services { get; set; }
    }

    [RegisterView]
    public class BootServiceLoadView : View<BootServiceLoadModel> {
        public BootServiceLoadView() {
            Name = "BootServiceLoad";
            Description = "Primary view for BootServiceLoadModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(BootServiceLoadSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<BootServiceLoadModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaBootServiceLoads = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Services.JoinToString()
                };
                api.Emit(docid, schemaBootServiceLoads);
            };
        }
    }
    #endregion
}