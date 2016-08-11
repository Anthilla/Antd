using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class FirewallListModel : EntityModel {
        public FirewallListModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public FirewallListModel(FirewallListSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            IsEnabled = sourceModel.IsEnabled;
            TableId = sourceModel.TableId;
            TypeId = sourceModel.TypeId;
            HookId = sourceModel.HookId;
            Rule = sourceModel.Rule;
            Label = sourceModel.Label;
            Values = sourceModel.Values.SplitToList();
        }
        public bool? IsEnabled { get; set; } = true;
        public string TableId { get; set; }
        public string TypeId { get; set; }
        public string HookId { get; set; }
        public string Rule { get; set; }
        public string Label { get; set; }
        public IEnumerable<string> Values { get; set; } = new List<string>();

        public string ReplaceValues => string.Join(", ", Values);
        public string ReplaceTemplateValues => $"{{ {string.Join(", ", Values)} }}";
        public string TemplateWord => $"${TableId}_{TypeId}_{HookId}_{Label}";
    }

    #region [    View    ]
    public class FirewallListSchema : EntitySchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public bool? IsEnabled { get; set; } = true;
        public string TableId { get; set; }
        public string TypeId { get; set; }
        public string HookId { get; set; }
        public string Rule { get; set; }
        public string Label { get; set; }
        public string Values { get; set; }

        public string ReplaceValues { get; set; }
        public string ReplaceTemplateValues { get; set; }
        public string TemplateWord { get; set; }
    }

    [RegisterView]
    public class FirewallListView : View<FirewallListModel> {
        public FirewallListView() {
            Name = "FirewallList";
            Description = "Primary view for FirewallListModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 5;
            Schema = typeof(FirewallListSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<FirewallListModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaFirewallLists = {
                    doc.Status.ToString(),
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.IsEnabled,
                    doc.TableId,
                    doc.TypeId,
                    doc.HookId,
                    doc.Rule,
                    doc.Label,
                    doc.Values.JoinToString(),
                    string.Join(", ", doc.Values),
                    $"{{ {string.Join(", ", doc.Values)} }}",
                    $"${doc.TableId}_{doc.TypeId}_{doc.HookId}_{doc.Label}"
                };
                api.Emit(docid, schemaFirewallLists);
            };
        }
    }
    #endregion
}