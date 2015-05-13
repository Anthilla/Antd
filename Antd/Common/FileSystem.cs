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
        public static string ReadFile(string path) {
            if (File.Exists(path)) {
                return File.ReadAllText(path);
            }
            else {
                ConsoleLogger.Warn("Path '{0}' doesn't exist", path);
                return String.Empty;
            }
        }

        public static string ReadFile(string directory, string filename) {
            string path = Path.Combine(directory, filename);
            if (File.Exists(path)) {
                return File.ReadAllText(path);
            }
            else {
                ConsoleLogger.Warn("File {0} doesn't exist in {1}", filename, directory);
                return String.Empty;
            }
        }

        public static void WriteFile(string path, string content) {
            if (File.Exists(path)) {
                File.Delete(path);
            }
            using (StreamWriter sw = File.CreateText(path)) {
                sw.Write(content);
            }
        }

        public static void WriteFile(string directory, string filename, string content) {
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, filename);
            if (File.Exists(path)) {
                File.Delete(path);
            }
            using (StreamWriter sw = File.CreateText(path)) {
                sw.Write(content);
            }
        }
    }
}