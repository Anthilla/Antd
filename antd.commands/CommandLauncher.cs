using System;
using System.Collections.Generic;
using antdlib.common;

namespace antd.commands {
    public class CommandLauncher {

        public dynamic Launch(string name) {
            try {
                if(string.IsNullOrEmpty(name)) {
                    return string.Empty;
                }
                if(!Commands.List.ContainsKey(name)) {
                    return string.Empty;
                }
                var cmd = Commands.List[name];
                if(cmd == null) {
                    return string.Empty;
                }
                var result = cmd.Launch();
                return result;
            }
            catch(Exception ex) {
                ConsoleLogger.Log($"Failed to Launch {name} command");
                ConsoleLogger.Error(ex);
                return string.Empty;
            }
        }

        public dynamic Launch(string name, Dictionary<string, string> dict) {
            try {
                if(string.IsNullOrEmpty(name)) {
                    return string.Empty;
                }
                if(!Commands.List.ContainsKey(name)) {
                    return string.Empty;
                }
                var cmd = Commands.List[name];
                if(cmd == null) {
                    return string.Empty;
                }
                var result = cmd.Launch(dict);
                return result;
            }
            catch(Exception ex) {
                ConsoleLogger.Log($"Failed to Launch {name} command");
                ConsoleLogger.Error(ex);
                return string.Empty;
            }
        }
    }
}
