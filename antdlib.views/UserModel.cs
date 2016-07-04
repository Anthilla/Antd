using System;
using System.Collections.Generic;
using antdlib.common;
using RaptorDB;

namespace antdlib.views {
    [Serializable]
    public class UserModel : EntityModel {
        public UserModel() {
            Id = System.Guid.NewGuid();
            Guid = System.Guid.NewGuid().ToString();
            Key = System.Guid.NewGuid();
            Vector = System.Guid.NewGuid();
            EntityCode = $"{Status}-{Guid}-{Timestamp}";
            IsEncrypted = false;
            Dump = new byte[] { 0 };
        }
        public UserModel(UserSchema sourceModel) {
            Id = System.Guid.Parse(sourceModel.Id);
            Guid = sourceModel.Guid;
            FirstName = sourceModel.FirstName;
            LastName = sourceModel.LastName;
            Alias = sourceModel.Alias;
            Password = sourceModel.Password;
            Role = sourceModel.Role;
            ProjectGuids = sourceModel.ProjectGuids.SplitToList();
            UsergroupGuids = sourceModel.UsergroupGuids.SplitToList();
            ResourceGuids = sourceModel.ResourceGuids.SplitToList();
            UserGuids = sourceModel.UserGuids.SplitToList();
            TokenGuid = sourceModel.TokenGuid;
            IsInsider = sourceModel.IsInsider;
            IsEnabled = sourceModel.IsEnabled;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string CompanyGuid { get; set; }
        public IEnumerable<string> ProjectGuids { get; set; } = new List<string>();
        public IEnumerable<string> UsergroupGuids { get; set; } = new List<string>();
        public IEnumerable<string> ResourceGuids { get; set; } = new List<string>();
        public IEnumerable<string> UserGuids { get; set; } = new List<string>();
        public string TokenGuid { get; set; }
        public bool? IsInsider { get; set; }
        public bool? IsEnabled { get; set; }
    }

    #region [    View    ]
    public class UserSchema : RDBSchema {
        //---
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public string EntityCode { get; set; }
        public string Tags { get; set; }
        //---
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string CompanyGuid { get; set; }
        public string ProjectGuids { get; set; }
        public string UsergroupGuids { get; set; }
        public string ResourceGuids { get; set; }
        public string UserGuids { get; set; }
        public string TokenGuid { get; set; }
        public bool? IsInsider { get; set; }
        public bool? IsEnabled { get; set; }
    }

    [RegisterView]
    public class UserView : View<UserModel> {
        public UserView() {
            Name = "User";
            Description = "Primary view for UserModel";
            isPrimaryList = true;
            isActive = true;
            BackgroundIndexing = false;
            ConsistentSaveToThisView = true;
            Version = 1;
            Schema = typeof(UserSchema);
            Mapper = (api, docid, doc) => {
                if (doc.Status != EntityStatus.New) return;
                var k = doc.Key.ToKey();
                var v = doc.Vector.ToVector();
                var decryptedDoc = Encryption.DbDecrypt<UserModel>(doc.Dump, k, v);
                doc = decryptedDoc;
                object[] schemaObjects = {
                    doc.Id.ToString(),
                    doc.Guid,
                    doc.Timestamp,
                    doc.EntityCode,
                    doc.Tags.JoinToString(),
                    doc.FirstName,
                    doc.LastName,
                    doc.Alias,
                    doc.Password,
                    doc.Role,
                    doc.Email,
                    doc.CompanyGuid,
                    doc.ProjectGuids.JoinToString(),
                    doc.UsergroupGuids.JoinToString(),
                    doc.ResourceGuids.JoinToString(),
                    doc.UserGuids.JoinToString(),
                    doc.TokenGuid,
                    doc.IsInsider,
                    doc.IsEnabled
                };
                api.Emit(docid, schemaObjects);
            };
        }
    }
    #endregion
}