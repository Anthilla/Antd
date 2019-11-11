using anthilla.core;
using System;
using System.IO;
using System.Linq;

namespace Antd2.cmds {
    public class Mod {

        public class Status {
            public string Module { get; set; }
            public string[] UsedBy { get; set; }
        }

        private const string lsmodCommand = "lsmod";
        private const string modprobeCommand = "modprobe";
        private const string rmmodCommand = "rmmod";
        private const string blacklistFileLocation = "/etc/modprobe.d/blacklist.conf";
        private const string modulesDirectory = "/lib/modules";
        private const string moduleExtension = ".ko.xz";

        //public static Status[] Get() {
        //    var result = CommonProcess.Execute(lsmodFileLocation).Skip(1).ToArray();
        //    var status = new Status[result.Length];
        //    for(var i = 0; i < result.Length; i++) {
        //        var currentLine = result[i];
        //        status[i] = new Status() {
        //            Module = Help.CaptureGroup(currentLine, "([a-zA-Z_0-9]+)[\\s]*[0-9]+[0-9][\\s]+[0-9]+\\s"),
        //            UsedBy = Help.CaptureGroup(currentLine, "[a-zA-Z_0-9]+[\\s]*[0-9]+[0-9][\\s]+[0-9]+\\s([a-zA-Z_0-9, ]*)").Split(',')
        //        };
        //    }
        //    return status;
        //}

        //public static bool Set() {
        //    var current = Application.CurrentConfiguration.Boot.Modules;
        //    for(var i = 0; i < current.Length; i++) {
        //        var currentModule = current[i];
        //        if(currentModule.Blacklist) {
        //            Blacklist(currentModule.Module);
        //            continue;
        //        }
        //        if(currentModule.Remove) {
        //            Remove(currentModule.Module);
        //            continue;
        //        }
        //        if(currentModule.Active) {
        //            var moduleToLoad = CommonString.Append(currentModule.Module, " ", currentModule.Arguments);
        //            Add(moduleToLoad.Trim());
        //        }
        //    }
        //    return true;
        //}

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
