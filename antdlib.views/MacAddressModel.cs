using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class MacAddressModel : EntityModel {
        public MacAddressModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public MacAddressModel(MacAddressSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Value = sourceModel.Value;
            Description = sourceModel.Description;
            Type = sourceModel.Type;
            IsEnabled = sourceModel.IsEnabled.ToBoolean();
            IsNew = sourceModel.IsNew.ToBoolean();
        }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsNew { get; set; }
    }

    #region [    View    ]
    public class MacAddressSchema : EntitySchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string IsEnabled { get; set; }
        public string IsNew { get; set; }
    }

    [RegisterView]
    public class MacAddressView : View<MacAddressModel> {
        public MacAddressView() {
            Name = "MacAddress";
            Description = "Primary view for MacAddressModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 5;
            Schema = typeof(MacAddressSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<MacAddressModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaMacAddresss = {
                    doc.Status.ToString(),
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Value,
                    doc.Description,
                    doc.Type,
                    doc.IsEnabled.ToString(),
                    doc.IsNew.ToString()
                };
                api.Emit(docid, schemaMacAddresss);
            };
        }
    }
    #endregion
}