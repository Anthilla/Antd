using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class TimerRepository {
        private const string ViewName = "Timer";

        public IEnumerable<TimerSchema> GetAll() {
            var result = DatabaseRepository.Query<TimerSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public TimerSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<TimerSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public TimerSchema GetByName(string name) {
            var result = DatabaseRepository.Query<TimerSchema>(AntdApplication.Database, ViewName, schema => schema.Alias == name);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var guid = dict["Guid"];
            var alias = dict["Alias"];
            var time = dict["Time"];
            var command = dict["Command"];
            var obj = new TimerModel {
                Guid = guid,
                Alias = alias,
                Time = time,
                Command = command,
                TimerStatus = "active"
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            string alias;
            dict.TryGetValue("Alias", out alias);
            string time;
            dict.TryGetValue("Time", out time);
            string command;
            dict.TryGetValue("Command", out command);
            string timerStatus;
            dict.TryGetValue("TimerStatus", out timerStatus);
            var objUpdate = new TimerModel {
                Id = id.ToGuid(),
                Alias = alias.IsNullOrEmpty() ? null : alias,
                Time = time.IsNullOrEmpty() ? null : time,
                Command = command.IsNullOrEmpty() ? null : command,
                TimerStatus = timerStatus.IsNullOrEmpty() ? null : timerStatus
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<TimerModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
    }
}
