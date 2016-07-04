using System;
using System.Reflection.Emit;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class LogModel : EntityModel {
        public LogModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public LogModel(LogSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Message = sourceModel.Message;
        }
        public string Message { get; set; }
    }

    #region [    View    ]
    public class LogSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Message { get; set; }
    }

    [RegisterView]
    public class LogView : View<LogModel> {
        public LogView() {
            Name = "Log";
            Description = "Primary view for LogModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(LogSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<LogModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaLogs = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Message
                };
                api.Emit(docid, schemaLogs);
            };
        }
    }
    #endregion
}