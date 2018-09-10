using anthilla.core;
namespace Antd.cmds {
    public class Kill {

        private const string killFileLocation = "/bin/kill";
        private const string killallFileLocation = "/usr/bin/killall";

        public static void All(string processName) {
            CommonProcess.Execute(killallFileLocation, processName);
        }
    }
}
