using anthilla.core;
using System.IO;
using System.Linq;

namespace Antd.cmds {

    public class SetupCommands {

        private const string bashFileLocation = "/bin/bash";
        private const string bashArgs0 = "-c \"";
        private const string bashArgs1 = "\"";

        private const string setupFilename = "setup.conf";

        private static string SetBahArguments(string input) {
            return CommonString.Append(bashArgs0, input, bashArgs1);
        }

        public static bool Set() {
            var current = Application.CurrentConfiguration.SetupCommands;
            if(current == null) {
                return false;
            }
            var setupLines = new string[current.Length];
            for(var i = 0; i < current.Length; i++) {
                var setupCommand = current[i];
                var exe = true;
                if(!string.IsNullOrEmpty(setupCommand.ControlBashCommand) && !string.IsNullOrEmpty(setupCommand.ControlResult)) {
                    var setupCheckResult = CommonProcess.Execute(bashFileLocation, SetBahArguments(setupCommand.ControlBashCommand));
                    if(setupCheckResult.Any(_ => _.Contains(setupCommand.ControlResult))) {
                        exe = false;
                    }
                }
                if(exe) {
                    ConsoleLogger.Log($"[setup] {setupCommand.BashCommand}");
                    CommonProcess.Do(bashFileLocation, SetBahArguments(setupCommand.BashCommand));
                }
                setupLines[i] = setupCommand.BashCommand;
            }
            File.WriteAllLines($"{Parameter.AntdCfgSetup}/{setupFilename}", setupLines);
            return true;
        }

        public static bool Import(string[] lines) {
            var commands = new Command[lines.Length];
            for(var i= 0; i < lines.Length; i++) {
                commands[i] = new Command() {
                    BashCommand = lines[i]
                };
            }
            Application.CurrentConfiguration.SetupCommands = commands;
            ConfigRepo.Save();
            return true;
        }
    }
}
