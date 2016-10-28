using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class SyslogModel : EntityModel {
        public SyslogModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public SyslogModel(SyslogSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            RootPath = sourceModel.RootPath;
            PortNet1 = sourceModel.PortNet1;
            PortNet2 = sourceModel.PortNet2;
            PortNet3 = sourceModel.PortNet3;
            Services = sourceModel.Services.ToObject<Dictionary<string, string>>();
            IsEnabled = sourceModel.IsEnabled.ToNnBoolean();
        }
        public string RootPath { get; set; }
        public string PortNet1 { get; set; }
        public string PortNet2 { get; set; }
        public string PortNet3 { get; set; }
        public Dictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
        public bool IsEnabled { get; set; } = true;
    }

    #region [    View    ]
    public class SyslogSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string RootPath { get; set; }
        public string PortNet1 { get; set; }
        public string PortNet2 { get; set; }
        public string PortNet3 { get; set; }
        public string Services { get; set; }
        public string IsEnabled { get; set; }
    }

    [RegisterView]
    public class SyslogView : View<SyslogModel> {
        public SyslogView() {
            Name = "Syslog";
            Description = "Primary view for SyslogModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 8;
            Schema = typeof(SyslogSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<SyslogModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaSyslogs = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.RootPath,
                    doc.PortNet1,
                    doc.PortNet2,
                    doc.PortNet3,
                    doc.Services.ToJson(),
                    doc.IsEnabled.ToString()
                };
                api.Emit(docid, schemaSyslogs);
            };
        }
    }
    #endregion
}