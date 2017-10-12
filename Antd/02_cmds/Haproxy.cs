using anthilla.core;
using System.Collections.Generic;
using System.IO;

namespace Antd.cmds {
    public class Haproxy {

        private const string haproxyFileLocation = "/usr/bin/haproxy";
        private const string killallFileLocation = "/usr/bin/killall";
        private const string processName = "haproxy";

        public static bool Start(string haproxyConfigurationFile) {
            if(!File.Exists(haproxyConfigurationFile)) {
                return false;
            }
            ConsoleLogger.Log($"[haproxy] start with conf {haproxyConfigurationFile}");
            var args = "-f " + haproxyConfigurationFile;
            CommonProcess.Do(haproxyFileLocation, args);
            return true;
        }

        public static bool Stop() {
            ConsoleLogger.Log("[haproxy] stop");
            CommonProcess.Do(killallFileLocation, processName);
            return true;
        }

        public static IEnumerable<string> TestConfiguration(string haproxyConfigurationFile) {
            var args = "-f " + haproxyConfigurationFile + " -c";
            return CommonProcess.Execute(haproxyFileLocation, args);
        }
    }
}
