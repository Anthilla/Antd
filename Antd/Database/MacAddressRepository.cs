using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class MacAddressRepository {
        private const string ViewName = "MacAddress";

        public IEnumerable<MacAddressSchema> GetAll() {
            var result = DatabaseRepository.Query<MacAddressSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public MacAddressSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<MacAddressSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public MacAddressSchema GetEnabled(string guid) {
            var result = DatabaseRepository.Query<MacAddressSchema>(AntdApplication.Database, ViewName, schema => schema.IsEnabled.ToBoolean() == true);
            return result.FirstOrDefault();
        }

        public MacAddressSchema GetDisabled(string guid) {
            var result = DatabaseRepository.Query<MacAddressSchema>(AntdApplication.Database, ViewName, schema => schema.IsEnabled.ToBoolean() == false);
            return result.FirstOrDefault();
        }

        public MacAddressSchema GetNew(string guid) {
            var result = DatabaseRepository.Query<MacAddressSchema>(AntdApplication.Database, ViewName, schema => schema.IsNew.ToBoolean() == true);
            return result.FirstOrDefault();
        }

        public MacAddressSchema GetByValue(string value) {
            var result = DatabaseRepository.Query<MacAddressSchema>(AntdApplication.Database, ViewName, schema => schema.Value == value);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var guid = dict["Guid"];
            var value = dict["Value"];
            var description = dict["Description"];
            var type = dict["Type"];
            var isEnabled = dict["IsEnabled"];
            var isNew = dict["IsNew"];
            var obj = new MacAddressModel {
                Guid = guid,
                Value = value,
                Description = description,
                Type = type,
                IsEnabled = isEnabled.ToBoolean(),
                IsNew = isNew.ToBoolean()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var guid = dict["Guid"];
            var value = dict["Value"];
            var description = dict["Description"];
            var type = dict["Type"];
            var isEnabled = dict["IsEnabled"];
            var isNew = dict["IsNew"];
            var objUpdate = new MacAddressModel {
                Id = id.ToGuid(),
                Guid = guid.IsNullOrEmpty() ? null : guid,
                Value = value.IsNullOrEmpty() ? null : value,
                Description = description.IsNullOrEmpty() ? null : description,
                Type = type.IsNullOrEmpty() ? null : type,
                IsEnabled = isEnabled.IsNullOrEmpty() ? null : isEnabled.ToBoolean(),
                IsNew = isNew.IsNullOrEmpty() ? null : isNew.ToBoolean()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<MacAddressModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        //public static void Discover() {
        //    //todo trova comando
        //    var lines = Terminal.Terminal.Execute("comando per recuperare dei mac address...").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (var line in lines) {
        //        //todo fai qualcosa
        //        AddMacAddress("", "");
        //    }
        //}
    }
}
