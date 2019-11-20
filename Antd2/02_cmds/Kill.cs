namespace Antd2.cmds {
    public class Kill {

        private const string killCommand = "/bin/kill";
        private const string killallCommand = "/usr/bin/killall";

        public static void Process(string pid) {
            Bash.Do($"{killCommand} -9 {pid}");
        }

        public static void All(string processName) {
            Bash.Do($"{killallCommand} {killCommand}");
        }
    }
}
