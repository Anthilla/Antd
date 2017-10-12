using anthilla.core;

namespace Antd.cmds {
    public class Rndc {

        private const string rndcFileLocation = "/usr/sbin/rndc";
        private const string reconfigArg = "reconfig";
        private const string reloadArg = "reload";

        public static bool Reconfig() {
            CommonProcess.Do(rndcFileLocation, reconfigArg);
            return true;
        }

        public static bool Reload() {
            CommonProcess.Do(rndcFileLocation, reloadArg);
            return true;
        }
    }
}
