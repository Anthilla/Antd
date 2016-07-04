using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class CommandModel : EntityModel {
        public CommandModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public CommandModel(CommandSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Command = sourceModel.Command;
            Layout = sourceModel.Layout;
            Notes = sourceModel.Notes;
            LaunchAtBoot = sourceModel.LaunchAtBoot;
            IsEnabled = sourceModel.IsEnabled;
        }
        public string Command { get; set; }
        public string Layout { get; set; }
        public string Notes { get; set; }
        public bool? LaunchAtBoot { get; set; } = false;
        public bool? IsEnabled { get; set; } = true;
    }

    #region [    View    ]
    public class CommandSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Command { get; set; }
        public string Layout { get; set; }
        public string Notes { get; set; }
        public bool? LaunchAtBoot { get; set; }
        public bool? IsEnabled { get; set; }
    }

    [RegisterView]
    public class CommandView : View<CommandModel> {
        public CommandView() {
            Name = "Command";
            Description = "Primary view for CommandModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(CommandSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<CommandModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaCommands = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Command,
                    doc.Layout,
                    doc.Notes,
                    doc.LaunchAtBoot,
                    doc.IsEnabled,
                };
                api.Emit(docid, schemaCommands);
            };
        }
    }
    #endregion
}