using System;
using System.Collections.Generic;
using System.Linq;
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
                var type = cmd.GetType();
                var methodInfo = type.GetMethods().First(m => m.Name == "Launch");
                var result = methodInfo.Invoke(cmd, null);
                return result;
            }
            catch(Exception) {
                ConsoleLogger.Log($"Failed to Launch {name} command");
                return null;
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
                var type = cmd.GetType();
                var methodInfo = type.GetMethods().First(m => m.Name == "LaunchD");
                var result = methodInfo.Invoke(cmd, new object[] { dict });
                return result;
            }
            catch(Exception) {
                ConsoleLogger.Log($"Failed to Launch {name} command");
                return null;
            }
        }
    }
}
