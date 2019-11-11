using anthilla.core;
using System;
using System.IO;
using System.Linq;
using SSO = System.StringSplitOptions;

namespace Antd2.cmds {
    public class Mod {

        private const string lsmodCommand = "lsmod";
        private const string modprobeCommand = "modprobe";
        private const string rmmodCommand = "rmmod";
        private const string blacklistFileLocation = "/etc/modprobe.d/blacklist.conf";
        private const string modulesDirectory = "/lib/modules";
        private const string moduleExtension = ".ko.xz";

        public static (string Module, string[] UsedBy)[] Get() {
            var result = Bash.Execute(lsmodCommand)
                .Skip(1)
                .Select(_ => ParseLsmodLine(_))
                .ToArray();
            return result;
        }

        private static (string Module, string[] UsedBy) ParseLsmodLine(string line) {
            var arr = line.Split(new[] { ' ' }, SSO.RemoveEmptyEntries);
            if (arr.Length != 2) {
                return (string.Empty, Array.Empty<string>());
            }
            return (arr.FirstOrDefault().Trim(), arr.LastOrDefault().Trim().Split(new[] { ',' }, SSO.RemoveEmptyEntries).ToArray());
        }

        public static bool Add(string module) {
            Bash.Do($"{modprobeCommand} {module}");
            Console.WriteLine($"[mod] add module '{module}'");
            return true;
        }

        public static bool Remove(string module) {
            Bash.Do($"{rmmodCommand} {module}");
            Console.WriteLine($"[mod] remove module '{module}'");
            return true;
        }

        public static bool Remove((string Module, string[] UsedBy) module) {
            foreach(var usingModule in module.UsedBy) {
                Remove(module);
            }
            Bash.Do($"{rmmodCommand} {module.Module}");
            return true;
        }

        public static bool Blacklist(string module) {
            if (!File.Exists(blacklistFileLocation)) {
                return false;
            }
            var blacklistLine = CommonString.Append("blacklist ", module);
            var runningBlacklist = File.ReadAllLines(blacklistFileLocation);
            if (runningBlacklist.Contains(blacklistLine)) {
                return true;
            }
            File.AppendAllLines(blacklistFileLocation, new[] { blacklistLine });
            Bash.Do($"{rmmodCommand} {module}");
            Console.WriteLine($"[mod] blacklist module '{module}'");
            return true;
        }

        public static string[] GetList() {
            var kernel = Uname.GetKernel();
            if (string.IsNullOrEmpty(kernel)) {
                return new string[0];
            }
            var modulesLocation = CommonString.Append(modulesDirectory, "/", kernel);
            if (!Directory.Exists(modulesLocation)) {
                return new string[0];
            }
            var files = Directory.EnumerateFiles(modulesLocation, "*", SearchOption.AllDirectories).Where(_ => _.Contains(moduleExtension)).ToArray();
            var list = new string[files.Length];
            for (var i = 0; i < files.Length; i++) {
                list[i] = Path.GetFileName(files[i]).Replace(moduleExtension, "");
            }
            return list;
        }
    }
}
