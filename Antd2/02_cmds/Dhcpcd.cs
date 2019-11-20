namespace Antd2.cmds {
    public class Dhcpcd {

        private const string dhcpcdCommand = "dhcpcd";

        public static void Start(string networkAdapter) {
            Bash.Do($"{dhcpcdCommand} {networkAdapter}");
        }
    }
}
