using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class JobModel : EntityModel {
        public JobModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public JobModel(JobSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            Alias = sourceModel.Alias;
            Data = sourceModel.Data;
            IsEnabled = sourceModel.IsEnabled;
            IntervalSpan = sourceModel.IntervalSpan;
            CronExpression = sourceModel.CronExpression;
        }
        public string Alias { get; set; }
        public string Data { get; set; }
        public bool? IsEnabled { get; set; } = true;
        public string IntervalSpan { get; set; }
        public string CronExpression { get; set; }
        public DateTime StartTime { get; set; }
    }

    #region [    View    ]
    public class JobSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string Alias { get; set; }
        public string Data { get; set; }
        public bool? IsEnabled { get; set; } = true;
        public string IntervalSpan { get; set; }
        public string CronExpression { get; set; }
    }

    [RegisterView]
    public class JobView : View<JobModel> {
        public JobView() {
            Name = "Job";
            Description = "Primary view for JobModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(JobSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<JobModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaJobs = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.Alias,
                    doc.Data,
                    doc.IsEnabled,
                    doc.IntervalSpan,
                    doc.CronExpression
                };
                api.Emit(docid, schemaJobs);
            };
        }
    }
    #endregion
}