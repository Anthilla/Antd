using anthilla.core;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Antd {
    public class MountHelper {

        public static string ConvertDirectoryTargetPathToDirs(string source) {
            return $"{Parameter.RepoDirs}/DIR{source.Replace("_", "__").Replace("/", "_").Replace("\\", "/")}";
        }

        public static string ConvertDirectoryDirsPathToTarget(string source) {
            var result0 = source.Replace(Parameter.RepoDirs, "").Replace("DIR", "").Replace("_", "/").Replace("__", "_");
            //todo fix this -> 1) sostituisco gli _ singoli con / poi __ con _
            var result1 = new Regex("[^_](_)[^_]").Replace(result0, "/");
            var result2 = new Regex("_{2,}").Replace(result1, "_");
            return result2.Replace("\\", "/").Replace("//", "/");
        }

        public static string ConvertFileTargetPathToDirs(string source) {
            return $"{Parameter.RepoDirs}/FILE{source.Replace("/", "_").Replace("\\", "/").Replace("__", "_")}";
        }

        public static string ConvertFileDirsPathToTarget(string source) {
            return source.Replace(Parameter.RepoDirs, "").Replace("FILE", "").Replace("_", "/").Replace("\\", "/").Replace("//", "/");
        }

        public static bool IsAlreadyMounted(string path) {
            if(!File.Exists("/proc/mounts")) {
                return false;
            }
            var procMounts = File.ReadAllLines("/proc/mounts");
            return procMounts.Any(_ => _.Contains(path));
        }

        public static bool IsAlreadyMounted(string source, string destination) {
            return IsAlreadyMounted(source) || IsAlreadyMounted(destination);
        }

        private static int _umount1Retry;
        public static void Umount(string directory) {
            if(IsAlreadyMounted(directory) && _umount1Retry < 5) {
                ConsoleLogger.Log($"umount, retry #{_umount1Retry}");
                Bash.Execute($"umount {directory}", false);
                _umount1Retry = _umount1Retry + 1;
                Umount(directory);
            }
            _umount1Retry = 0;
        }

        private static int _umount2Retry;
        public static void Umount(string source, string destination) {
            if(IsAlreadyMounted(source, destination) && _umount1Retry < 5) {
                ConsoleLogger.Log($"umount, retry #{_umount2Retry}");
                Bash.Execute($"umount {source}", false);
                Bash.Execute($"umount {destination}", false);
                _umount2Retry = _umount2Retry + 1;
                Umount(source, destination);
            }
            _umount1Retry = 0;
        }
    }
}
