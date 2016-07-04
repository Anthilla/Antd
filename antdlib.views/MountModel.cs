using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {

    public enum MountStatus : byte {
        Mounted = 1,
        Unmounted = 2,
        MountedTmp = 3,
        DifferentMount = 4,
        MountedReadOnly = 5,
        MountedReadWrite = 6,
        Error = 99
    }

    public enum MountContext : byte {
        Core = 1,
        External = 2,
        Other = 99
    }

    public enum MountEntity : byte {
        Directory = 1,
        File = 2,
        Other = 99
    }

    [Serializable]
    public class MountModel : EntityModel {
        public MountModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public MountModel(MountSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            DfpTimestamp = sourceModel.DfpTimestamp;
            Device = sourceModel.Device;
            Path = sourceModel.Path;
            DirsPath = sourceModel.DirsPath;
            HtmlStatusIcon = sourceModel.HtmlStatusIcon;
            MountedPath = sourceModel.MountedPath;
            MountStatus = sourceModel.MountStatus.ToEnum<MountStatus>();
            MountContext = sourceModel.MountContext.ToEnum<MountContext>();
            Type = sourceModel.Type;
            Options = sourceModel.Options;
            AssociatedUnits = sourceModel.AssociatedUnits.SplitToList();
            MountEntity = sourceModel.MountEntity.ToEnum<MountEntity>();
        }
        public string DfpTimestamp { get; set; }
        public string Device { get; set; } = "";
        public string Path { get; set; } = "";
        public string DirsPath { get; set; } = "";
        public string HtmlStatusIcon { get; set; } = "";
        public string MountedPath { get; set; } = "";
        public MountStatus MountStatus { get; set; } = MountStatus.Unmounted;
        public MountContext MountContext { get; set; } = MountContext.Core;
        public string Type { get; set; } = "";
        public string Options { get; set; } = "";
        public IEnumerable<string> AssociatedUnits { get; set; } = new HashSet<string>();
        public MountEntity MountEntity { get; set; }
    }

    #region [    View    ]
    public class MountSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string DfpTimestamp { get; set; }
        public string Device { get; set; } 
        public string Path { get; set; }
        public string DirsPath { get; set; }
        public string HtmlStatusIcon { get; set; }
        public string MountedPath { get; set; } 
        public string MountStatus { get; set; } 
        public string MountContext { get; set; } 
        public string Type { get; set; } 
        public string Options { get; set; }
        public string AssociatedUnits { get; set; } 
        public string MountEntity { get; set; }
    }

    [RegisterView]
    public class MountView : View<MountModel> {
        public MountView() {
            Name = "Mount";
            Description = "Primary view for MountModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(MountSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<MountModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaMounts = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.DfpTimestamp,
                    doc.Device,
                    doc.Path,
                    doc.DirsPath,
                    doc.HtmlStatusIcon,
                    doc.MountedPath,
                    doc.MountStatus.ToString(),
                    doc.MountContext.ToString(),
                    doc.Type,
                    doc.Options,
                    doc.AssociatedUnits.JoinToString(),
                    doc.MountEntity,
                };
                api.Emit(docid, schemaMounts);
            };
        }
    }
    #endregion
}