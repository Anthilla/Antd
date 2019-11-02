using anthilla.core;

namespace Antd.cmds {
    public class Dhcpcd {

        private const string dhcpcdFileLocation = "/sbin/dhcpcd";
        private const string processName = "dhcpcd";

        public static bool Start(string networkAdapter) {
            CommonProcess.Do(dhcpcdFileLocation, networkAdapter);
            return true;
        }

        public static bool Stop() {
            CommonProcess.Do(dhcpcdFileLocation, processName);
            return true;
        }
    }
}
