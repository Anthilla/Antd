using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class BindServerZoneModel : EntityModel {
        public BindServerZoneModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public BindServerZoneModel(BindServerZoneSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Name = sourceModel.Name;
            Type = sourceModel.Type;
            File = sourceModel.File;
            SerialUpdateMethod = sourceModel.SerialUpdateMethod;
            AllowUpdate = sourceModel.AllowUpdate.SplitToList();
            AllowQuery = sourceModel.AllowQuery.SplitToList();
            AllowTransfer = sourceModel.AllowTransfer.SplitToList();
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string File { get; set; }
        public string SerialUpdateMethod { get; set; } = "unixtime";
        public List<string> AllowUpdate { get; set; } = new List<string> { "loif", "iif", "lonet", "inet", "onet", "key updbindkey" };
        public List<string> AllowQuery { get; set; } = new List<string> { "any" };
        public List<string> AllowTransfer { get; set; } = new List<string> { "loif", "iif", "lonet", "inet", "onet" };
    }

    #region [    View    ]
    public class BindServerZoneSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Name { get; set; }
        public string Type { get; set; }
        public string File { get; set; }
        public string SerialUpdateMethod { get; set; } 
        public string AllowUpdate { get; set; } 
        public string AllowQuery { get; set; } 
        public string AllowTransfer { get; set; }
    }

    [RegisterView]
    public class BindServerZoneView : View<BindServerZoneModel> {
        public BindServerZoneView() {
            Name = "BindServerZone";
            Description = "Primary view for BindServerZoneModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(BindServerZoneSchema);
            Mapper = (api, docid, doc) => {
                if(doc.Status != EntityStatus.New)
                    return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<BindServerZoneModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaBindServerZones = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Name,
                    doc.Type,
                    doc.File,
                    doc.SerialUpdateMethod,
                    doc.AllowUpdate.JoinToString(),
                    doc.AllowQuery.JoinToString(),
                    doc.AllowTransfer.JoinToString()
                };
                api.Emit(docid, schemaBindServerZones);
            };
        }
    }
    #endregion
}