using anthilla.core;

namespace Antd2.cmds {
    public class Dhclient {

        private const string dhclientFileLocation = "/sbin/dhclient";
        private const string killallFileLocation = "/usr/bin/killall";
        private const string processName = "dhclient";

        public static bool Start(string networkAdapter) {
            CommonProcess.Do(dhclientFileLocation, networkAdapter);
            return true;
        }

        public static bool StartV6(string networkAdapter) {
            CommonProcess.Do(dhclientFileLocation, "-6 " + networkAdapter);
            return true;
        }

        public static bool Stop() {
            CommonProcess.Do(killallFileLocation, processName);
            return true;
        }
    }
}
