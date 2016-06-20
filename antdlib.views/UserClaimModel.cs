using System;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class UserClaimModel : EntityModel {

        public static ClaimType ConvertClaimType(string claimString) {
            switch (claimString) {
                case "identity":
                    return ClaimType.UserIdentity;
                case "password":
                    return ClaimType.UserPassword;
                case "token":
                    return ClaimType.UserToken;
                case "vnc":
                    return ClaimType.Vnc;
                case "pin":
                    return ClaimType.UserPin;
                default:
                    return ClaimType.Other;
            }
        }

        public enum ClaimType : byte {
            UserIdentity = 1,
            UserPassword = 2,
            UserToken = 3,
            UserPin = 4,
            Vnc = 50,
            Other = 99
        }

        public static ClaimMode ConvertClaimMode(string claimString) {
            switch (claimString) {
                case "antd":
                    return ClaimMode.Antd;
                case "system":
                    return ClaimMode.System;
                case "activedirectory":
                    return ClaimMode.ActiveDirectory;
                case "anthillasp":
                    return ClaimMode.AnthillaSp;
                default:
                    return ClaimMode.Other;
            }
        }

        public enum ClaimMode : byte {
            Antd = 1,
            System = 2,
            ActiveDirectory = 3,
            AnthillaSp = 4,
            Null = 98,
            Other = 99
        }

        public UserClaimModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }

        public string UserGuid { get; set; }
        public ClaimType Type { get; set; }
        public ClaimMode Mode { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool? IsEnabled { get; set; }
    }

    #region [    View    ]
    public class UserClaimSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string UserGuid { get; set; }
        public string Type { get; set; }
        public string Mode { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool? IsEnabled { get; set; }
    }

    [RegisterView]
    public class UserClaimView : View<UserClaimModel> {
        public UserClaimView() {
            Name = "UserClaim";
            Description = "Primary view for UserClaimModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(UserClaimSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<UserClaimModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaObjects = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.UserGuid,
                    doc.Type.ToString(),
                    doc.Mode.ToString(),
                    doc.Label,
                    doc.Value,
                    doc.IsEnabled
                };
                api.Emit(docid, schemaObjects);
            };
        }
    }
    #endregion
}