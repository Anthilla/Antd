using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class CommandRepository {
        private const string ViewName = "Command";

        public IEnumerable<CommandSchema> GetAll() {
            var result = DatabaseRepository.Query<CommandSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public CommandSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<CommandSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public CommandSchema GetByAlias(string alias) {
            var result = DatabaseRepository.Query<CommandSchema>(AntdApplication.Database, ViewName, schema => schema.Alias == alias);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var alias = dict["Alias"];
            var command = dict["Command"];
            var obj = new CommandModel {
                Alias = alias,
                Command = command,
                LaunchAtBoot = false,
                IsEnabled = false
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var alias = dict["Alias"];
            var command = dict["Command"];
            var objUpdate = new CommandModel {
                Id = id.ToGuid(),
                Alias = alias.IsNullOrEmpty() ? null : alias,
                Command = command.IsNullOrEmpty() ? null : command,
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<CommandModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        private readonly CommandValuesRepository _commandValuesRepository = new CommandValuesRepository();

        public string Launch(string alias) {
            var cmd = GetByAlias(alias);
            if (cmd == null) {
                return string.Empty;
            }
            var command = cmd.Command;

            var matches = Regex.Matches(command, "\\$[a-zA-Z0-9_]+");
            foreach (var match in matches) {
                var val = _commandValuesRepository.GetByName(match.ToString());
                if (string.IsNullOrEmpty(val.Value)) {
                    continue;
                }
                command = command.Replace(match.ToString(), val.Value);
            }

            return Terminal.Execute(command);
        }
    }
}
