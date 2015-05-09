using System;
using System.IO;

namespace Antd.Common {
    public static class FileSystem {
        /// <summary>
        /// If file exists return content else return empty string
        /// </summary>
        /// <param name="directory">file location</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string ReadFile(string directory, string filename) {
            string path = Path.Combine(directory, filename);
            if (File.Exists(path)) {
                return File.ReadAllText(path);
            } else {
                ConsoleLogger.Warn("File {0} doesn't exist in {1}", filename, directory);
                return String.Empty;
            }
        }
    }
}
