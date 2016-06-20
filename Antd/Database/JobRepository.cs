using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class JobRepository {
        private const string ViewName = "Job";

        public IEnumerable<JobSchema> GetAll() {
            var result = DatabaseRepository.Query<JobSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public JobSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<JobSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var guid = dict["Guid"];
            var alias = dict["Alias"];
            var data = dict["Data"];
            var intervalSpan = dict["IntervalSpan"];
            var cronExpression = dict["CronExpression"];
            var obj = new JobModel {
                Guid = guid,
                Alias = alias,
                Data = data,
                IntervalSpan = intervalSpan,
                CronExpression = cronExpression,
                IsEnabled = false
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            string alias = null;
            dict.TryGetValue("Alias", out alias);
            string data = null;
            dict.TryGetValue("Data", out data);
            string intervalSpan = null;
            dict.TryGetValue("IntervalSpan", out intervalSpan);
            string cronExpression = null;
            dict.TryGetValue("CronExpression", out cronExpression);
            string isEnabled = null;
            dict.TryGetValue("IsEnabled", out isEnabled);
            var objUpdate = new JobModel {
                Id = id.ToGuid(),
                Alias = alias.IsNullOrEmpty() ? null : alias,
                Data = data.IsNullOrEmpty() ? null : data,
                IntervalSpan = intervalSpan.IsNullOrEmpty() ? null : intervalSpan,
                CronExpression = cronExpression.IsNullOrEmpty() ? null : cronExpression,
                IsEnabled = isEnabled.IsNullOrEmpty() ? null : isEnabled.ToBoolean()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<JobModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
    }
}
