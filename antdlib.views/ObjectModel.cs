using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class ObjectModel : EntityModel {
        public ObjectModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public string Label { get; set; }
        public string Value { get; set; }
    }

    #region [    View    ]
    public class ObjectSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Label { get; set; }
        public string Value { get; set; }
    }

    [RegisterView]
    public class ObjectView : View<ObjectModel> {
        public ObjectView() {
            Name = "Object";
            Description = "Primary view for ObjectModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(ObjectSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<ObjectModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaObjects = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Label,
                    doc.Value
                };
                api.Emit(docid, schemaObjects);
            };
        }
    }
    #endregion
}