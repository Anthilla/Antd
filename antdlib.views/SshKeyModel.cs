using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class SshKeyModel : EntityModel {
        public SshKeyModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public SshKeyModel(SshKeySchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            User = sourceModel.User;
            PublicUser = sourceModel.PublicUser;
            Type = sourceModel.Type;
            Value = sourceModel.Value;
        }
        public string User { get; set; }
        public string PublicUser { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    #region [    View    ]
    public class SshKeySchema : EntitySchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string User { get; set; }
        public string PublicUser { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    [RegisterView]
    public class SshKeyView : View<SshKeyModel> {
        public SshKeyView() {
            Name = "SshKey";
            Description = "Primary view for SshKeyModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 2;
            Schema = typeof(SshKeySchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<SshKeyModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaSshKeys = {
                    doc.Status.ToString(),
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.User,
                    doc.PublicUser,
                    doc.Type,
                    doc.Value
                };
                api.Emit(docid, schemaSshKeys);
            };
        }
    }
    #endregion
}