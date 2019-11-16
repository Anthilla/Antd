using anthilla.core;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Antd {
    public class MountHelper {

        public static string ConvertDirectoryTargetPathToDirs(string source) {
            return $"/Antd/DIRS/DIR{source.Replace("_", "__").Replace("/", "_").Replace("\\", "/")}";
        }

        public static string ConvertDirectoryDirsPathToTarget(string source) {
            var result0 = source
                .Replace("/Antd/DIRS", "")
                .Replace("DIR", "")
                .Replace("__", "{SP1}")
                .Replace("_", "/")
                .Replace("{SP1}", "_");
            return result0.Replace("\\", "/").Replace("//", "/");
        }

        public static string ConvertFileTargetPathToDirs(string source) {
            return $"/Antd/DIRS/FILE{source.Replace("_", "__").Replace("/", "_").Replace("\\", "/")}";
        }

        public static string ConvertFileDirsPathToTarget(string source) {
            var result0 = source
                .Replace("/Antd/DIRS", "")
                .Replace("FILE", "")
                .Replace("__", "{SP1}")
                .Replace("_", "/")
                .Replace("{SP1}", "_");
            return result0.Replace("\\", "/").Replace("//", "/");
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
                Console.WriteLine($"umount, retry #{_umount1Retry}");
                Bash.Execute($"umount {directory}", false);
                _umount1Retry = _umount1Retry + 1;
                Umount(directory);
            }
            _umount1Retry = 0;
        }

        private static int _umount2Retry;
        public static void Umount(string source, string destination) {
            if(IsAlreadyMounted(source, destination) && _umount1Retry < 5) {
                Console.WriteLine($"umount, retry #{_umount2Retry}");
                Bash.Execute($"umount {source}", false);
                Bash.Execute($"umount {destination}", false);
                _umount2Retry = _umount2Retry + 1;
                Umount(source, destination);
            }
            _umount1Retry = 0;
        }
    }
}
