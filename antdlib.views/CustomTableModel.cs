using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    public enum CustomTableType {
        Settings = 1,
        DataView = 2,
        Conf = 3,
        New = 4,
        None = 99
    }

    //public enum OsiLevel {
    //    Physical = 1,
    //    DataLink = 2,
    //    Network = 3,
    //    Transport = 4,
    //    Session = 5,
    //    Presentation = 6,
    //    Application = 7,
    //    None = 99
    //}

    //public enum CommandFunction {
    //    Stable = 0,
    //    Testing = 1,
    //    None = 99
    //}

    //public enum ConfType : byte {
    //    File = 0,
    //    Directory = 1
    //}

    [Serializable]
    public class CustomTableModel : EntityModel {
        public CustomTableModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public string Alias { get; set; }
        public string Context { get; set; }
        public CustomTableType Type { get; set; }
        public IEnumerable<string> Content { get; set; } = new List<string>();
    }

    #region [    View    ]
    public class CustomTableSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Alias { get; set; }
        public string Context { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }

    [RegisterView]
    public class CustomTableView : View<CustomTableModel> {
        public CustomTableView() {
            Name = "CustomTable";
            Description = "Primary view for CustomTableModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(CustomTableSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<CustomTableModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaCustomTables = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Alias,
                    doc.Context,
                    doc.Type.ToString(),
                    doc.Content.JoinToString()
                };
                api.Emit(docid, schemaCustomTables);
            };
        }
    }
    #endregion
}