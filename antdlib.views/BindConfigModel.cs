using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class BindConfigModel : EntityModel {
        public BindConfigModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public BindConfigModel(BindConfigSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Config = sourceModel.Config;
        }
        public string Config { get; set; }
    }

    #region [    View    ]
    public class BindConfigSchema : RDBSchema {
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
    public class BindConfigView : View<BindConfigModel> {
        public BindConfigView() {
            Name = "BindConfig";
            Description = "Primary view for BindConfigModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 2;
            Schema = typeof(BindConfigSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<BindConfigModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaBindConfigs = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Config
                };
                api.Emit(docid, schemaBindConfigs);
            };
        }
    }
    #endregion
}