using anthilla.core;
using System.IO;

namespace Antd.cmds {
    public class Echo {

        private const string bashFileLocation = "/bin/bash";

        public static bool PipeToFile(string value, string filePath) {
            if(!File.Exists(filePath)) {
                return false;
            }
            //-c "echo {value} > {filePath}"
            CommonProcess.Do(bashFileLocation, $"-c \"echo {value} > {filePath}\"");
            return true;
        }
    }
}
