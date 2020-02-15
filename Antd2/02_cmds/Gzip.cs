using System.IO;

namespace Antd2.cmds {
    public class Gzip {

        private const string gzipCommand = "gzip";

        public static void CompressFile(string file) {
            if (File.Exists(file))
                Bash.Execute($"{gzipCommand} {file}");
        }

        public static void CompressRecursive(string directory) {
            if (Directory.Exists(directory))
                Bash.Execute($"{gzipCommand} -r {directory}");
        }
    }
}
