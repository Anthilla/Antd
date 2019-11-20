namespace Antd2.cmds {
    public class Rndc {

        private const string rndcCommand = "rndc";
        private const string reconfigArg = "reconfig";
        private const string reloadArg = "reload";

        public static void Reconfig() {
            Bash.Do($"{rndcCommand} {reconfigArg}");
        }

        public static void Reload() {
            Bash.Do($"{rndcCommand} {reloadArg}");
        }
    }
}
