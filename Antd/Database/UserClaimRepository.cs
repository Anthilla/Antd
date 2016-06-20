using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class UserClaimRepository {
        private const string ViewName = "UserClaim";

        public IEnumerable<UserClaimSchema> GetAll() {
            var result = DatabaseRepository.Query<UserClaimSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public UserClaimSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<UserClaimSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var guid = dict["Guid"];
            var userGuid = dict["UserGuid"];
            var type = dict["Type"];
            var mode = dict["Mode"];
            var label = dict["Label"];
            var value = dict["Value"];
            var obj = new UserClaimModel {
                Guid = guid,
                UserGuid = userGuid,
                Type = type.ToClaimType(),
                Mode = mode.ToClaimMode(),
                Label = label,
                Value = value,
                IsEnabled = true
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var guid = dict["Guid"];
            var userGuid = dict["UserGuid"];
            var type = dict["Type"];
            var mode = dict["Mode"];
            var label = dict["Label"];
            var value = dict["Value"];
            var isEnabled = dict["IsEnabled"].ToBoolean();
            var objUpdate = new UserClaimModel {
                Id = id.ToGuid(),
                Guid = guid.IsNullOrEmpty() ? null : guid,
                UserGuid = userGuid.IsNullOrEmpty() ? null : userGuid,
                Type = type.IsNullOrEmpty() ? UserClaimModel.ClaimType.Other : type.ToClaimType(),
                Mode = mode.IsNullOrEmpty() ? UserClaimModel.ClaimMode.Null : mode.ToClaimMode(),
                Label = label.IsNullOrEmpty() ? null : label,
                Value = value.IsNullOrEmpty() ? null : value,
                IsEnabled = isEnabled.ToString().IsNullOrEmpty() ? null : isEnabled
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<UserClaimModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public UserClaimSchema GetByUserGuid(string guid) {
            var result = DatabaseRepository.Query<UserClaimSchema>(AntdApplication.Database, ViewName, schema => schema.UserGuid == guid);
            return result.FirstOrDefault();
        }
    }
}
