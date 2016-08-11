using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class TimerModel : EntityModel {
        public TimerModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public TimerModel(TimerSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Alias = sourceModel.Alias;
            Time = sourceModel.Time;
            Command = sourceModel.Command;
            TimerStatus = sourceModel.TimerStatus;
            IsEnabled = sourceModel.IsEnabled;
        }
        public string Alias { get; set; }
        public string Time { get; set; }
        public string Command { get; set; }
        public string TimerStatus { get; set; }
        public bool IsEnabled { get; set; }
    }

    #region [    View    ]
    public class TimerSchema : EntitySchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Alias { get; set; }
        public string Time { get; set; }
        public string Command { get; set; }
        public string TimerStatus { get; set; }
        public bool IsEnabled { get; set; }
    }

    [RegisterView]
    public class TimerView : View<TimerModel> {
        public TimerView() {
            Name = "Timer";
            Description = "Primary view for TimerModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 5;
            Schema = typeof(TimerSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<TimerModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaTimers = {
                    doc.Status.ToString(),
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Alias,
                    doc.Time,
                    doc.Command,
                    doc.TimerStatus,
                    doc.IsEnabled
                };
                api.Emit(docid, schemaTimers);
            };
        }
    }
    #endregion
}