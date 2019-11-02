namespace Antd2.cmds {
    public class AptGet {

        private const string aptgetCommand = "apt-get";

        public static void Install(string package) {
            Bash.Execute($"{aptgetCommand} install {package}");
        }
    }
}
