using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class CommandValuesModel : EntityModel {
        public CommandValuesModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public CommandValuesModel(CommandValuesSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Name = sourceModel.Name;
            Value = sourceModel.Value;
            //Index = sourceModel.Index;
        }
        public string Name { get; set; }
        public string Value { get; set; }
        //public string Index { get; set; }
    }

    #region [    View    ]
    public class CommandValuesSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Name { get; set; }
        public string Value { get; set; }
        //public string Index { get; set; }
    }

    [RegisterView]
    public class CommandValuesView : View<CommandValuesModel> {
        public CommandValuesView() {
            Name = "CommandValues";
            Description = "Primary view for CommandValuesModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(CommandValuesSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<CommandValuesModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaCommandValuess = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Name,
                    //doc.Index,
                    doc.Value
            };
                api.Emit(docid, schemaCommandValuess);
            };
        }
    }
    #endregion
}