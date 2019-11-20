using System.IO;

namespace Antd2.cmds {
    public class Echo {

        private const string echoCommand = "echo";


        /// <summary>
        /// -c "echo {value} > {filePath}"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        public static void PipeToFile(string value, string filePath) {
            if (!File.Exists(filePath)) {
                return;
            }
            Bash.Do($"{echoCommand} {value} > {filePath}");
        }
    }
}
