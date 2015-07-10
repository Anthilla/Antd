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

        /// <summary>
        /// ok
        /// </summary>
        public static void Info() {
            Console.WriteLine("> This is a shell for antd :)");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void Start() {
            Console.WriteLine("> Looking for antds in {0}", global.versionsDir);
            var newestVersionFound = ex.GetNewestVersion();
            if (newestVersionFound.Key != null) {
                ex.LinkVersionToRunning(newestVersionFound.Key);
                Console.WriteLine("> New antd '{0}' linked to running version", newestVersionFound.Key);
            }
            else {
                Console.WriteLine("> There's no antd to link.");
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateCheck() {
            var linkedVersionName = ex.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = ex.SetVersionKeyValuePair(linkedVersionName);
                var newestVersionFound = ex.GetNewestVersion();
                if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                    Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
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
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UpdateLaunch() {
            var linkedVersionName = ex.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = ex.SetVersionKeyValuePair(linkedVersionName);
                var newestVersionFound = ex.GetNewestVersion();
                if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                    Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        Console.WriteLine("> Antd is already up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        Console.WriteLine("> New version of antd found!! -> {0}", newestDate);
                        Console.WriteLine("> Updating!");
                        if (newestVersionFound.Key.Contains(global.squashEndsWith)) {
                            ex.RemoveLink();
                            ex.LinkVersionToRunning(newestVersionFound.Key);
                        }
                        else if (newestVersionFound.Key.Contains(global.zipEndsWith)) {
                            var squashName = global.versionsDir + "/" + global.squashStartsWith + newestVersionFound.Value + global.squashEndsWith;
                            ex.MountTmpRam();
                            ex.CopyToTmp(newestVersionFound.Key);
                            ex.ExtractZipTmp(newestVersionFound.Key);
                            ex.RemoveTmpZips();
                            ex.CreateSquashFromZip(squashName);
                            ex.CleanTmp();
                            ex.UmountTmpRam();
                            ex.RemoveLink();
                            ex.LinkVersionToRunning(squashName);
                        }
                        else {
                            Console.WriteLine("> Update failed unexpectedly");
                            return;
                        }
                        Terminal.Execute("systemctl restart antd-prepare.service");
                        Terminal.Execute("systemctl restart framework-antd.mount");
                        Terminal.Execute("systemctl antd-launcher.service");
                        return;
                    }
                    else {
                        Console.WriteLine("> There's nothing to update.");
                        return;
                    }
                }
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UpdateGit() {
            var linkedVersionName = ex.GetRunningVersion();
            if (linkedVersionName != null) {
                var linkedVersion = ex.SetVersionKeyValuePair(linkedVersionName);
                var newestVersionFound = ex.GetNewestVersion();
                if (linkedVersion.Key != null && newestVersionFound.Key != null) {
                    Console.WriteLine("> You are running {0} and the latest version is {1}.", linkedVersion.Value, newestVersionFound.Value);
                    var linkedDate = Convert.ToInt32(linkedVersion.Value);
                    var newestDate = Convert.ToInt32(newestVersionFound.Value);
                    if (linkedVersion.Value == newestVersionFound.Value) {
                        Console.WriteLine("> Antd is already up to date!");
                        return;
                    }
                    else if (newestDate > linkedDate) {
                        Console.WriteLine("> New version of antd found!! -> {0}", newestDate);
                        Console.WriteLine("> Updating!");
                        if (newestVersionFound.Key.Contains(global.squashEndsWith)) {
                            ex.RemoveLink();
                            ex.LinkVersionToRunning(newestVersionFound.Key);
                        }
                        else if (newestVersionFound.Key.Contains(global.zipEndsWith)) {
                            var squashName = global.versionsDir + "/" + global.squashStartsWith + newestVersionFound.Value + global.squashEndsWith;
                            ex.MountTmpRam();
                            ex.CopyToTmp(newestVersionFound.Key);
                            ex.ExtractZipTmp(newestVersionFound.Key);
                            ex.RemoveTmpZips();
                            ex.CreateSquashFromZip(squashName);
                            ex.CleanTmp();
                            ex.UmountTmpRam();
                            ex.RemoveLink();
                            ex.LinkVersionToRunning(squashName);
                        }
                        else {
                            Console.WriteLine("> Update failed unexpectedly");
                            return;
                        }
                        Terminal.Execute("systemctl restart antd-prepare.service");
                        Terminal.Execute("systemctl restart framework-antd.mount");
                        Terminal.Execute("systemctl antd-launcher.service");
                        return;
                    }
                    else {
                        Console.WriteLine("> There's nothing to update.");
                        return;
                    }
                }
                return;
            }
        }

        public static void UpdateSelectVersion() {
            ex.CheckRunningExists();
            var versions = new HashSet<KeyValuePair<string, string>>();
            var linkedVersion = new KeyValuePair<string, string>(null, null);
            var linkedVersionName = ex.GetRunningVersion();
            linkedVersion = new KeyValuePair<string, string>(linkedVersionName, linkedVersionName.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "").Replace(global.squashStartsWith, "").Replace(global.squashEndsWith, ""));
            var zips = Directory.EnumerateFiles(global.configDir, "*.*").Where(s => s.StartsWith(global.zipStartsWith) && s.EndsWith(global.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(new KeyValuePair<string, string>(zip, zip.Replace(global.zipStartsWith, "").Replace(global.zipEndsWith, "")));
                }
            }
            var squashes = Directory.EnumerateFiles(global.configDir, "*.*").Where(s => s.StartsWith(global.squashStartsWith) && s.EndsWith(global.squashEndsWith)).ToArray();
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
                Terminal.Execute("ln -s " + newestVersionFound.Key + " " + Path.GetFullPath(global.antdRunning));
            }
            else {
                Console.Write("> Error -> the number you wrote does not exist!");
                return;
            }
            Console.WriteLine("> Changing version from {0} to {1}!", linkedVersion.Value, newestVersionFound.Value);
            Console.Write("Confirm? y/n: ");
            var confirm = Console.ReadLine();
            if (confirm == "y") {
                ex.ChangeRunningVersion(newestVersionFound, linkedVersion.Value);
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
