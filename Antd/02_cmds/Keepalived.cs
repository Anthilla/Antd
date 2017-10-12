using anthilla.core;
using System.IO;

namespace Antd.cmds {
    public class Keepalived {

        private const string keepalivedFileLocation = "/usr/sbin/keepalived";
        private const string killallFileLocation = "/usr/bin/killall";
        private const string processName = "keepalived";

        public static bool Start(string haproxyConfigurationFile) {
            if(!File.Exists(haproxyConfigurationFile)) {
                return false;
            }
            var args = "-f " + haproxyConfigurationFile;
            ConsoleLogger.Log($"[keepalived] start with conf {haproxyConfigurationFile}");
            CommonProcess.Do(keepalivedFileLocation, args);
            return true;
        }

        public static bool Stop() {
            ConsoleLogger.Log("[keepalived] stop");
            CommonProcess.Do(killallFileLocation, processName);
            return true;
        }
    }
}
