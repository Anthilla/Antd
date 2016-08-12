using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class BootModuleLoadModel : EntityModel {
        public BootModuleLoadModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public BootModuleLoadModel(BootModuleLoadSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Modules = sourceModel.Modules.SplitToList();
        }
        public IEnumerable<string> Modules { get; set; }
    }

    #region [    View    ]
    public class BootModuleLoadSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Modules { get; set; }
    }

    [RegisterView]
    public class BootModuleLoadView : View<BootModuleLoadModel> {
        public BootModuleLoadView() {
            Name = "BootModuleLoad";
            Description = "Primary view for BootModuleLoadModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(BootModuleLoadSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<BootModuleLoadModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaBootModuleLoads = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Modules.JoinToString()
                };
                api.Emit(docid, schemaBootModuleLoads);
            };
        }
    }
    #endregion
}