using System.Text.RegularExpressions;

namespace antdlib.common.Helpers {
    public class Mounts {
        public static string SetDirsPath(string source) {
            return $"{Parameter.RepoDirs}/DIR{source.Replace("_", "__").Replace("/", "_").Replace("\\", "/")}";
        }

        public static string GetDirsPath(string source) {
            var result0 = source.Replace(Parameter.RepoDirs, "").Replace("DIR", "").Replace("_", "/").Replace("__", "_");
            //todo fix this -> 1) sostituisco gli _ singoli con / poi __ con _
            var result1 = new Regex("[^_](_)[^_]").Replace(result0, "/");
            var result2 = new Regex("_{2,}").Replace(result1, "_");
            return result2.Replace("\\", "/").Replace("//", "/");
        }

        public static string SetFilesPath(string source) {
            return $"{Parameter.RepoDirs}/FILE{source.Replace("/", "_").Replace("\\", "/").Replace("__", "_")}";
        }

        public static string GetFilesPath(string source) {
            return source.Replace(Parameter.RepoDirs, "").Replace("FILE", "").Replace("_", "/").Replace("\\", "/").Replace("//", "/");
        }

        public static bool IsAlreadyMounted(string directory) {
            var df = Terminal.Execute($"df | grep {directory}");
            var pm = Terminal.Execute($"cat /proc/mounts | grep {directory}");
            return df.Length > 0 || pm.Length > 0;
        }

        public static bool IsAlreadyMounted(string source, string destination) {
            return IsAlreadyMounted(source) || IsAlreadyMounted(destination);
        }

        private static int _umount1Retry;
        public static void Umount(string directory) {
            if (IsAlreadyMounted(directory) && _umount1Retry < 5) {
                ConsoleLogger.Log($"umount, retry #{_umount1Retry}");
                Terminal.Execute($"umount {directory}");
                _umount1Retry = _umount1Retry + 1;
                Umount(directory);
            }
            _umount1Retry = 0;
        }

        private static int _umount2Retry;
        public static void Umount(string source, string destination) {
            if (IsAlreadyMounted(source, destination) && _umount1Retry < 5) {
                ConsoleLogger.Log($"umount, retry #{_umount2Retry}");
                Terminal.Execute($"umount {source}");
                Terminal.Execute($"umount {destination}");
                _umount2Retry = _umount2Retry + 1;
                Umount(source, destination);
            }
            _umount1Retry = 0;
        }
    }
}
