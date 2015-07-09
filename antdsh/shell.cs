using antdlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class shell {

        public static void Info() {
            Console.WriteLine("> This is a shell for antd :)");
        }

        public static void UpdateCheck() {
            var running = Directory.EnumerateFiles(global.root, global.antdRunning).ToArray();
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            if (running.Length > 0) {
                var linkedVersionName = Terminal.Execute("file " + global.antdRunning).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "").Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, ""));
            }
            else {
                Console.WriteLine("> There's no running version of antd.");
                return;
            }
            var zips = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.zipStartsWith) && s.EndsWith(global.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(new KeyValuePair<string, string>(zip, zip.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "")));
                }
            }
            var squashes = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.squashStartsWith) && s.EndsWith(global.squashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(new KeyValuePair<string, string>(squash, squash.Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, "")));
                }
            }
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if (versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            else {
                Console.WriteLine("> There's no new version of antd.");
                return;
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versionsOrdered.Length > 0) {
                newestVersionFound = versionsOrdered.Last();
            }
            if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                var linkedDate = DateTime.ParseExact(linkedVersion.Value, global.dateFormat, CultureInfo.InvariantCulture);
                var newestDate = DateTime.ParseExact(newestVersionFound.Value, global.dateFormat, CultureInfo.InvariantCulture);
                if (linkedVersion.Value == newestVersionFound.Value) {
                    Console.WriteLine("> Antd is up to date!");
                    return;
                }
                else if (newestDate > linkedDate) {
                    Console.WriteLine("> New version of antd found!! -> {0}", newestDate);
                    return;
                }
                else {
                    Console.WriteLine("> There's nothing to update.");
                    return;
                }
            }
            return;
        }

        public static void UpdateLaunch() {
            var running = Directory.EnumerateFiles(global.root, global.antdRunning).ToArray();
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            if (running.Length > 0) {
                var linkedVersionName = Terminal.Execute("file " + global.antdRunning).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "").Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, ""));
            }
            else {
                Console.WriteLine("> There's no running version of antd.");
                return;
            }
            var zips = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.zipStartsWith) && s.EndsWith(global.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(new KeyValuePair<string, string>(zip, zip.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "")));
                }
            }
            var squashes = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.squashStartsWith) && s.EndsWith(global.squashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(new KeyValuePair<string, string>(squash, squash.Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, "")));
                }
            }
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if (versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            else {
                Console.WriteLine("> There's no new version of antd.");
                return;
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versionsOrdered.Length > 0) {
                newestVersionFound = versionsOrdered.Last();
            }
            if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                var linkedDate = DateTime.ParseExact(linkedVersion.Value, global.dateFormat, CultureInfo.InvariantCulture);
                var newestDate = DateTime.ParseExact(newestVersionFound.Value, global.dateFormat, CultureInfo.InvariantCulture);
                if (linkedVersion.Value == newestVersionFound.Value) {
                    Console.WriteLine("> Update exited -> antd is already up to date!");
                    return;
                }
                else if (newestDate > linkedDate) {
                    Console.WriteLine("> New version of antd found!! -> {0}", newestDate);
                    common.ChangeRunningVersion(newestVersionFound, linkedVersion.Value);
                }
                else {
                    Console.WriteLine("> Update failed unexpectedly");
                    return;
                }
            }
            return;
        }

        public static void UpdateForce() {
            var running = Directory.EnumerateFiles(global.root, global.antdRunning).ToArray();
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            if (running.Length > 0) {
                var linkedVersionName = Terminal.Execute("file " + global.antdRunning).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "").Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, ""));
            }
            else {
                Console.WriteLine("> There's no running version of antd.");
                return;
            }
            var zips = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.zipStartsWith) && s.EndsWith(global.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(new KeyValuePair<string, string>(zip, zip.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "")));
                }
            }
            var squashes = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.squashStartsWith) && s.EndsWith(global.squashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(new KeyValuePair<string, string>(squash, squash.Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, "")));
                }
            }
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if (versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            else {
                Console.WriteLine("> There's no new version of antd.");
                return;
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versionsOrdered.Length > 0) {
                newestVersionFound = versionsOrdered.Last();
            }
            if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                var linkedDate = DateTime.ParseExact(linkedVersion.Value, global.dateFormat, CultureInfo.InvariantCulture);
                var newestDate = DateTime.ParseExact(newestVersionFound.Value, global.dateFormat, CultureInfo.InvariantCulture);
                Console.WriteLine("> Updating!");
                common.ChangeRunningVersion(newestVersionFound, linkedVersion.Value);
                return;
            }
            return;
        }

        public static void UpdateGit() {
            var running = Directory.EnumerateFiles(global.root, global.antdRunning).ToArray();
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            if (running.Length > 0) {
                var linkedVersionName = Terminal.Execute("file " + global.antdRunning).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "").Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, ""));
            }
            if (!File.Exists(Path.Combine(global.configDir, global.configFile))) {
                Console.WriteLine("> There's no config file!");
                Console.WriteLine(">     try with: set-directory-download");
                return;
            }
            else if (config.downloadDirectory.Get().ToCharArray().Length < 1) {
                Console.WriteLine("> Impossible to read the config file");
                Console.WriteLine(">     try with: set-directory-download");
                return;
            }
            else {
                using (var client = new WebClient()) {
                    Console.Write("> ");
                    client.DownloadFile("https://github.com/Anthilla/Antd/archive/master.zip", config.downloadDirectory.Get());
                    Console.WriteLine("Download from github repository completed");
                    var newestVersionFound = new KeyValuePair<string, string>(null, null);
                    common.ChangeRunningVersion(newestVersionFound, linkedVersion.Value);
                }
            }
            return;
        }

        public static void UpdateSelectVersion() {
            var running = Directory.EnumerateFiles(global.root, global.antdRunning).ToArray();
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            if (running.Length > 0) {
                var linkedVersionName = Terminal.Execute("file " + global.antdRunning).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "").Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, ""));
            }
            else {
                Console.WriteLine("> There's no running version of antd.");
                return;
            }
            var zips = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.zipStartsWith) && s.EndsWith(global.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(new KeyValuePair<string, string>(zip, zip.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "")));
                }
            }
            var squashes = Directory.EnumerateFiles(global.root, "*.*").Where(s => s.StartsWith(global.squashStartsWith) && s.EndsWith(global.squashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(new KeyValuePair<string, string>(squash, squash.Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, "")));
                }
            }
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if (versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            else {
                Console.WriteLine("> There's no new version of antd.");
                return;
            }
            Console.WriteLine("> Which version of antd do you want to run?");
            foreach (var version in versionsOrdered) {
                Console.WriteLine(">     {0} - {1}", version.Key, version.Value);
            }
            Console.Write("> Write the version number here:");
            var versionNumber = Console.ReadLine();
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            var getVersion = versionsOrdered.Where(v => v.Value == versionNumber).FirstOrDefault();
            if (getVersion.Key != null) {
                newestVersionFound = getVersion;
            }
            else {
                Console.Write("> Error -> the number you wrote does not exist!");
                return;
            }
            Console.WriteLine("> Changing version from {0} to {1}!", linkedVersion.Value, newestVersionFound.Value);
            Console.Write("Confirm? y/n: ");
            var confirm = Console.ReadLine();
            if (confirm == "y") {
                common.ChangeRunningVersion(newestVersionFound, linkedVersion.Value);
            }
            else if (confirm == "n") {
                Console.Write("> Change version closed...");
                return;
            }
            else {

            }
        }

        public static void SetDirectoryDownload() {
            Console.WriteLine("> Write a path to download-directory:");
            var path = Console.ReadLine();
            config.downloadDirectory.Set(path);
        }
    }
}
