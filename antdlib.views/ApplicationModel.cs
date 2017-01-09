using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class ApplicationModel : EntityModel {
        public ApplicationModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public ApplicationModel(ApplicationSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Name = sourceModel.Name;
            RepositoryName = sourceModel.RepositoryName;
            Exes = sourceModel.Exes.SplitToList();
            WorkingDirectories = sourceModel.WorkingDirectories.SplitToList();
            UnitPrepare = sourceModel.UnitPrepare;
            UnitMount = sourceModel.UnitMount;
            UnitLauncher = sourceModel.UnitLauncher.SplitToList();
        }
        public string Name { get; set; }
        public string RepositoryName { get; set; }
        public IEnumerable<string> Exes { get; set; }
        public IEnumerable<string> WorkingDirectories { get; set; }
        public string UnitPrepare { get; set; }
        public string UnitMount { get; set; }
        public IEnumerable<string> UnitLauncher { get; set; }
    }

    #region [    View    ]
    public class ApplicationSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        //---
        public string Name { get; set; }
        public string RepositoryName { get; set; }
        public string Exes { get; set; }
        public string WorkingDirectories { get; set; }
        public string UnitPrepare { get; set; }
        public string UnitMount { get; set; }
        public string UnitLauncher { get; set; }
    }

    [RegisterView]
    public class ApplicationView : View<ApplicationModel> {
        public ApplicationView() {
            Name = "Application";
            Description = "Primary view for ApplicationModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 10;
            Schema = typeof(ApplicationSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<ApplicationModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaApplications = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Name,
                    doc.RepositoryName,
                    doc.Exes.JoinToString(),
                    doc.WorkingDirectories.JoinToString(),
                    doc.UnitPrepare,
                    doc.UnitMount,
                    doc.UnitLauncher.JoinToString()
                };
                api.Emit(docid, schemaApplications);
            };
        }
    }
    #endregion
}