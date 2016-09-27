using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;
using Newtonsoft.Json;

namespace Antd.Database {
    public class CommandRepository {

        public class RootObject {
            public string Key { get; set; }
            public List<string> Value { get; set; }
            public string Description { get; set; }
        }

        private const string ViewName = "Command";

        public IEnumerable<CommandSchema> GetAll() {
            var result = DatabaseRepository.Query<CommandSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public CommandSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<CommandSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public CommandSchema GetByName(string alias) {
            var result = DatabaseRepository.Query<CommandSchema>(AntdApplication.Database, ViewName, schema => schema.Name == alias);
            return result.FirstOrDefault();
        }

        public void Import() {
            Directory.CreateDirectory(Parameter.AntdCfgCommands);
            var path = $"{Parameter.AntdCfgCommands}/commands.json";
            var resourceFile = $"Resources/commands.json";
            if (!File.Exists(path)) {
                if (!File.Exists(resourceFile)) {
                    return;
                }
                File.Copy(resourceFile, path, true);
            }
            else {
                var existingFileHash = FileSystem.GetFileHash(path);
                var storedFileHash = FileSystem.GetFileHash(resourceFile);
                if (existingFileHash != storedFileHash) {
                    File.Copy(resourceFile, path, true);
                }
            }
            var text = File.ReadAllText(path);
            if (string.IsNullOrEmpty(text)) {
                return;
            }
            try {
                var objs = JsonConvert.DeserializeObject<List<RootObject>>(text);
                DeleteAll();
                foreach (var o in objs) {
                    var t = GetByName(o.Key);
                    if (t == null && !o.Key.StartsWith("_")) {
                        var obj = new CommandModel {
                            Name = o.Key,
                            Command = o.Value.JoinToString(Environment.NewLine),
                            Description = o.Description
                        };
                        DatabaseRepository.Save(AntdApplication.Database, obj, true);
                    }
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex);
            }
        }

        public bool Create(IDictionary<string, string> dict) {
            var alias = dict["Name"];
            var command = dict["Command"];
            var description = dict["Description"];
            var obj = new CommandModel {
                Name = alias,
                Command = command,
                Description = description,
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var alias = dict["Name"];
            var command = dict["Command"];
            var description = dict["Description"];
            var objUpdate = new CommandModel {
                Id = id.ToGuid(),
                Name = alias.IsNullOrEmpty() ? null : alias,
                Command = command.IsNullOrEmpty() ? null : command,
                Description = description.IsNullOrEmpty() ? null : description,
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<CommandModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public void DeleteAll() {
            var all = GetAll();
            foreach (var el in all) {
                Delete(el.Id);
            }
        }

        private readonly CommandValuesRepository _commandValuesRepository = new CommandValuesRepository();

        public string Launch(string alias) {
            var cmd = GetByName(alias);
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
