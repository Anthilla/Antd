using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class AuthorizedKeysModel : EntityModel {
        public AuthorizedKeysModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public AuthorizedKeysModel(AuthorizedKeysSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            User = sourceModel.User;
            KeyValue = sourceModel.KeyValue;
        }
        public string RemoteUser { get; set; }
        public string User { get; set; }
        public string KeyValue { get; set; }
    }

    #region [    View    ]
    public class AuthorizedKeysSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        //---
        public string RemoteUser { get; set; }
        public string User { get; set; }
        public string KeyValue { get; set; }
    }

    [RegisterView]
    public class AuthorizedKeysView : View<AuthorizedKeysModel> {
        public AuthorizedKeysView() {
            Name = "AuthorizedKeys";
            Description = "Primary view for AuthorizedKeysModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 10;
            Schema = typeof(AuthorizedKeysSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<AuthorizedKeysModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaAuthorizedKeyss = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.RemoteUser,
                    doc.User,
                    doc.KeyValue
                };
                api.Emit(docid, schemaAuthorizedKeyss);
            };
        }
    }
    #endregion
}