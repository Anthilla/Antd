using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class SambaResourceModel : EntityModel {
        public SambaResourceModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public SambaResourceModel(SambaResourceSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Name = sourceModel.Name;
            Comment = sourceModel.Comment;
            Path = sourceModel.Path;
        }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Path { get; set; }
    }

    #region [    View    ]

    public class SambaResourceSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Path { get; set; }
    }

    [RegisterView]
    public class SambaResourceView : View<SambaResourceModel> {
        public SambaResourceView() {
            Name = "SambaResource";
            Description = "Primary view for SambaResourceModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(SambaResourceSchema);
            Mapper = (api, docid, doc) => {
                if(doc.Status != EntityStatus.New)
                    return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<SambaResourceModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaSambaResources = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Name,
                    doc.Comment,
                    doc.Path
                };
                api.Emit(docid, schemaSambaResources);
            };
        }
    }
    #endregion
}