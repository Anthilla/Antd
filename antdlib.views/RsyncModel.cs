using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class RsyncModel : EntityModel {
        public RsyncModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public RsyncModel(RsyncSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Source = sourceModel.Source;
            Destination = sourceModel.Destination;
            Options = sourceModel.Options;
            IsEnabled = sourceModel.IsEnabled;
        }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Options { get; set; }
        public bool? IsEnabled { get; set; } = true;
    }

    #region [    View    ]
    public class RsyncSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Options { get; set; }
        public bool? IsEnabled { get; set; }
    }

    [RegisterView]
    public class RsyncView : View<RsyncModel> {
        public RsyncView() {
            Name = "Rsync";
            Description = "Primary view for RsyncModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(RsyncSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<RsyncModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaRsyncs = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Source,
                    doc.Destination,
                    doc.Options,
                    doc.IsEnabled,
                };
                api.Emit(docid, schemaRsyncs);
            };
        }
    }
    #endregion
}