using antdlib.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public static class SetupConfiguration {

        private static readonly string FilePath = $"{Parameter.RepoConfig}/setup.conf";

        public static void Set() {
            if(!File.Exists(FilePath)) {
                var backupConfigFile = $"{Parameter.RepoConfig}/network/000network.cfg";
                if(File.Exists(backupConfigFile)) {
                    var lines = File.ReadAllLines(backupConfigFile).ToArray();
                    var importControls = new List<Control>();
                    for(var i = 0; i < lines.Length; i++) {
                        Bash.Execute(lines[i], false);
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
                    if(File.Exists(importFlow.Path))
                        return;
                    File.WriteAllText(importFlow.Path, JsonConvert.SerializeObject(importFlow, Formatting.Indented));
                    ConsoleLogger.Log("a setup configuration file template has been created");
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
                if(!File.Exists(tempFlow.Path)) {
                    File.WriteAllText(tempFlow.Path, JsonConvert.SerializeObject(tempFlow, Formatting.Indented));
                    ConsoleLogger.Log("a setup configuration file template has been created");
                }
                return;
            }
            var text = File.ReadAllText(FilePath);
            var flow = JsonConvert.DeserializeObject<ConfigurationFlow>(text);
            var controls = flow?.Controls?.OrderBy(_ => _.Index);
            if(controls == null)
                return;
            foreach(var control in controls) {
                Launch(control);
            }
        }

        private static int _counter;
        private static void Launch(Control control) {
            if(control?.FirstCommand == null) {
                return;
            }
            try {
                if(_counter > 5) {
                    ConsoleLogger.Log($"{control.FirstCommand} - failed, too many retry...");
                    _counter = 0;
                    return;
                }

                var firstCommand = control.FirstCommand;
                if(string.IsNullOrEmpty(control.ControlCommand)) {
                    ConsoleLogger.Log($"[setup.conf] {control.FirstCommand}");
                    Bash.Execute(firstCommand, false);
                    _counter = 0;
                    return;
                }

                var controlCommand = control.ControlCommand?.SplitToList(Environment.NewLine);
                var controlResult = Bash.Execute(controlCommand);
                var firstCheck = controlResult.Contains(control.Check);
                if(firstCheck) {
                    _counter = 0;
                    return;
                }
                Bash.Execute(firstCommand, false);
                controlResult = Bash.Execute(controlCommand);
                var secondCheck = controlResult.Contains(control.Check);
                if(secondCheck) {
                    _counter = 0;
                    return;
                }
                ConsoleLogger.Log($"{control.FirstCommand} - failed, retry");
                _counter = _counter + 1;
                Launch(control);
            }
            catch(NullReferenceException nrex) {
                ConsoleLogger.Warn(nrex.Message + " " + nrex.Source + " c: " + control.FirstCommand);
            }
        }

        public static List<Control> Get() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if(!File.Exists(FilePath)) {
                return new List<Control>();
            }
            var text = File.ReadAllText(FilePath);
            var flow = JsonConvert.DeserializeObject<ConfigurationFlow>(text);
            var controls = flow?.Controls?.OrderBy(_ => _.Index).ToList();
            return controls;
        }

        public static void Export(List<Control> controls) {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if(File.Exists(FilePath)) {
                File.Delete(FilePath);
            }
            var flow = new ConfigurationFlow {
                Name = "setup.conf",
                Path = FilePath,
                Controls = controls
            };
            if(!File.Exists(FilePath)) {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(flow, Formatting.Indented));
            }
        }
    }
}
