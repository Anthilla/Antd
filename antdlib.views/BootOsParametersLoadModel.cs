using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class BootOsParametersLoadModel : EntityModel {
        public BootOsParametersLoadModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public BootOsParametersLoadModel(BootOsParametersLoadSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Values = sourceModel.Values.ToObject<Dictionary<string, string>>();
        }
        public IDictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
    }

    #region [    View    ]
    public class BootOsParametersLoadSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Values { get; set; }
    }

    [RegisterView]
    public class BootOsParametersLoadView : View<BootOsParametersLoadModel> {
        public BootOsParametersLoadView() {
            Name = "BootOsParametersLoad";
            Description = "Primary view for BootOsParametersLoadModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(BootOsParametersLoadSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<BootOsParametersLoadModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaBootOsParametersLoads = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Values.ToJson()
                };
                api.Emit(docid, schemaBootOsParametersLoads);
            };
        }
    }
    #endregion
}