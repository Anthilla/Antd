using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class RsyncRepository {
        private const string ViewName = "Rsync";

        public IEnumerable<RsyncSchema> GetAll() {
            var result = DatabaseRepository.Query<RsyncSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public RsyncSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<RsyncSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var source = dict["Source"];
            var destination = dict["Destination"];
            var options = dict["Options"];
            var obj = new RsyncModel {
                Source = source,
                Destination = destination,
                Options = options,
                IsEnabled = false
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var source = dict["Source"];
            var destination = dict["Destination"];
            var options = dict["Options"];
            var isEnabled = dict["IsEnabled"];
            var objUpdate = new RsyncModel {
                Id = id.ToGuid(),
                Source = source.IsNullOrEmpty() ? null : source,
                Destination = destination.IsNullOrEmpty() ? null : destination,
                Options = options.IsNullOrEmpty() ? null : options,
                IsEnabled = isEnabled.IsNullOrEmpty() ? null : isEnabled.ToBoolean()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<RsyncModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public IEnumerable<string> GetDirectoriesToWatch() {
            var result = DatabaseRepository.Query<RsyncSchema>(AntdApplication.Database, ViewName).Select(_=>_.Source);
            return result;
        }

        public void SyncDirectories(string source) {
            var info = GetAll().FirstOrDefault(_ => _.Source == source);
            if (info != null) {
                new Terminal().Execute($"rsync {info.Options} {info.Source} {info.Destination}");
            }
        }
    }
}
