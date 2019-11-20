namespace Antd2.cmds {
    public class Alias {

        private const string aliasCommand = "alias";

        public static void Set(string alias, string command) {
            Bash.Do($"{aliasCommand} {alias}=\\\"{command}\\\"");
        }
    }
}
