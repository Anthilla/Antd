namespace Antd2.cmds {

    /// <summary>
    /// https://linux.die.net/man/1/dd
    /// </summary>
    public class Dd {

        private const string ddCommand = "dd";

        public static void CopyBlocks(string sourceDevice, string destinationDevice, string blockStep = "8192K") {
            Bash.Do($"{ddCommand} if={sourceDevice} of={destinationDevice} bs={blockStep}");
        }
    }
}
