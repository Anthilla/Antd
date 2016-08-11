using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using Newtonsoft.Json;

namespace Antd.Configuration {
    public class MachineConfiguration {

        private static readonly string FilePath = $"{Parameter.RepoConfig}/setup.conf";

        public static void Set() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if (!File.Exists(FilePath)) {
                var backupConfigFile = $"{Parameter.RepoConfig}/network/000network.cfg";
                if (File.Exists(backupConfigFile)) {
                    var lines = File.ReadAllLines(backupConfigFile).ToArray();
                    var importControls = new List<Control>();

                    for (var i = 0; i < lines.Length; i++) {
                        Terminal.Execute(lines[i]);
                        importControls.Add(new Control {
                            Index = i,
                            FirstCommand = lines[i],
                            ControlCommand = "",
                            Check = ""
                        });
                    }
                    ConsoleLogger.Log("setup configuration file does not exist");
                    var importFlow = new ConfigurationFlow {
                        Name = "setup.conf",
                        Path = $"{Parameter.RepoConfig}/setup.conf",
                        Controls = importControls
                    };
                    if (!File.Exists(importFlow.Path)) {
                        File.WriteAllText(importFlow.Path, JsonConvert.SerializeObject(importFlow, Formatting.Indented));
                        ConsoleLogger.Log("a setup configuration file template has been created");
                    }
                    return;
                }
                ConsoleLogger.Log("setup configuration file does not exist");
                var tempFlow = new ConfigurationFlow {
                    Name = ".setup.conf",
                    Path = $"{Parameter.RepoConfig}/.setup.conf",
                    Controls = new List<Control> {
                        new Control {
                            Index = 0,
                            FirstCommand = "command that applies a configuration",
                            ControlCommand = "command that checks if the correct configuration has been applied",
                            Check = "this value must be found in the ControlCommand result, leave blank if not needed"
                        },
                        new Control {
                            Index = 1,
                            FirstCommand = "command that applies a configuration",
                            ControlCommand = "command that checks if the correct configuration has been applied",
                            Check = "this value must be found in the ControlCommand result, leave blank if not needed"
                        }
                    }
                };
                if (!File.Exists(tempFlow.Path)) {
                    File.WriteAllText(tempFlow.Path, JsonConvert.SerializeObject(tempFlow, Formatting.Indented));
                    ConsoleLogger.Log("a setup configuration file template has been created");
                }
                return;
            }
            var text = File.ReadAllText(FilePath);
            var flow = JsonConvert.DeserializeObject<ConfigurationFlow>(text);
            var controls = flow?.Controls?.OrderBy(_ => _.Index);
            if (controls == null) return;
            foreach (var control in controls) {
                Launch(control);
            }
        }

        private static int _counter;
        private static void Launch(Control control) {
            var firstCommand = control.FirstCommand.SplitToList(Environment.NewLine);
            var controlCommand = control.ControlCommand.SplitToList(Environment.NewLine);
            if (_counter > 5) {
                ConsoleLogger.Log($"{control.FirstCommand} - failed, too many retry...");
                _counter = 0;
                return;
            }
            if (string.IsNullOrEmpty(control.ControlCommand)) {
                Terminal.Execute(firstCommand);
                _counter = 0;
                return;
            }
            var controlResult = Terminal.Execute(controlCommand);
            var firstCheck = controlResult.Contains(control.Check);
            if (firstCheck) {
                _counter = 0;
                return;
            }
            Terminal.Execute(firstCommand);
            controlResult = Terminal.Execute(controlCommand);
            var secondCheck = controlResult.Contains(control.Check);
            if (secondCheck) {
                _counter = 0;
                return;
            }
            ConsoleLogger.Log($"{control.FirstCommand} - failed, retry");
            _counter = _counter + 1;
            Launch(control);
        }

        public static List<Control> Get() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if (!File.Exists(FilePath)) {
                return new List<Control>();
            }
            var text = File.ReadAllText(FilePath);
            var flow = JsonConvert.DeserializeObject<ConfigurationFlow>(text);
            var controls = flow?.Controls?.OrderBy(_ => _.Index).ToList();
            return controls;
        }

        public static void Export(List<Control> controls) {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if (File.Exists(FilePath)) {
                File.Delete(FilePath);
            }
            var flow = new ConfigurationFlow {
                Name = "setup.conf",
                Path = FilePath,
                Controls = controls
            };
            if (!File.Exists(FilePath)) {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(flow, Formatting.Indented));
            }
        }
    }
}
