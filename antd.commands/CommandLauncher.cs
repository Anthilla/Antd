using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;

namespace antd.commands {
    public class CommandLauncher {

        public IEnumerable<string> Launch(string name) {
            try {
                if(string.IsNullOrEmpty(name)) {
                    return new List<string>();
                }
                if(!Commands.List.ContainsKey(name)) {
                    return new List<string>();
                }
                var cmd = Commands.List[name];
                if(cmd == null) {
                    return new List<string>();
                }
                var type = cmd.GetType();
                var methodInfo = type.GetMethods().First(m => m.Name == "Launch");
                var result = methodInfo.Invoke(cmd, null);
                return result as IEnumerable<string>;
            }
            catch(Exception) {
                ConsoleLogger.Log($"Failed to Launch {name} command");
                return null;
            }
        }

        public IEnumerable<string> Launch(string name, Dictionary<string, string> dict) {
            try {
                if(string.IsNullOrEmpty(name)) {
                    return new List<string>();
                }
                if(!Commands.List.ContainsKey(name)) {
                    return new List<string>();
                }
                var cmd = Commands.List[name];
                if(cmd == null) {
                    return new List<string>();
                }
                var type = cmd.GetType();
                var methodInfo = type.GetMethods().First(m => m.Name == "LaunchD");
                var result = methodInfo.Invoke(cmd, new object[] { dict });
                return result as IEnumerable<string>;
            }
            catch(Exception) {
                ConsoleLogger.Log($"Failed to Launch {name} command");
                return null;
            }
        }
    }
}
