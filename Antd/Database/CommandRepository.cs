using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Create(IDictionary<string, string> dict) {
            var command = dict["Command"];
            var layout = dict["Layout"];
            var notes = dict["Notes"];
            var obj = new CommandModel {
                Command = command,
                Layout = layout,
                Notes = notes,
                LaunchAtBoot = false,
                IsEnabled = false
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var command = dict["Command"];
            var layout = dict["Layout"];
            var notes = dict["Notes"];
            var launchAtBoot = dict["LaunchAtBoot"];
            var isEnabled = dict["IsEnabled"];
            var objUpdate = new CommandModel {
                Id = id.ToGuid(),
                Command = command.IsNullOrEmpty() ? null : command,
                Layout = layout.IsNullOrEmpty() ? null : layout,
                Notes = notes.IsNullOrEmpty() ? null : notes,
                LaunchAtBoot = launchAtBoot.IsNullOrEmpty() ? null : launchAtBoot.ToBoolean(),
                IsEnabled = isEnabled.IsNullOrEmpty() ? null : isEnabled.ToBoolean()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<CommandModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public string Launch(string guid) {
            var command = GetByGuid(guid);
            if (command == null) {
                return string.Empty;
            }
            var strings = command.Command.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return Terminal.Execute(strings);
        }

        public string LaunchAndGetOutputUsingNewValue(string guid) {
            var command = GetByGuid(guid);
            if (command == null) {
                return string.Empty;
            }
            var strings = command.Layout.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return Terminal.Execute(strings);
        }

        public string LaunchAndGetOutputUsingNewValue(string guid, string value) {
            var command = GetByGuid(guid);
            if (command == null) {
                return string.Empty;
            }
            var layout = command.Layout;
            var findReplace = "{" + guid + "}";
            var newCommand = layout.Replace(findReplace, value);
            var strings = newCommand.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return Terminal.Execute(strings);
        }
    }
}
