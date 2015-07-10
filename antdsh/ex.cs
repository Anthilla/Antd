using antdlib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class ex {
        /// <summary>
        /// ok
        /// </summary>
        public static void CheckRunningExists() {
            var running = Terminal.Execute("ls -la " + global.versionsDir + " | grep " + global.antdRunning);
            if (!running.Contains(global.antdRunning)) {
                Console.WriteLine("> There's no running version of antd.");
                return;
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetNewestVersion() {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(global.versionsDir, "*.*");
            var zips = files.Where(s => s.EndsWith(global.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = files.Where(s => s.EndsWith(global.squashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(SetVersionKeyValuePair(squash));
                }
            }
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if (versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versionsOrdered.Length > 0) {
                newestVersionFound = versionsOrdered.FirstOrDefault();
            }
            return newestVersionFound;
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="fileToLink"></param>
        public static void LinkVersionToRunning(string fileToLink) {
            Console.WriteLine("> Linking {0} to {1}", fileToLink, RunningPath);
            Terminal.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveLink() {
            Terminal.Execute("rm " + global.versionsDir + "/" + global.antdRunning);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static string GetRunningVersion() {
            var running = Terminal.Execute("ls -la " + global.versionsDir + " | grep " + global.antdRunning);
            if (!running.Contains(global.antdRunning)) {
                Console.WriteLine("> There's no running version of antd.");
                return null;
            }
            Console.WriteLine(Terminal.Execute("file " + RunningPath));
            var version = Terminal.Execute("file " + RunningPath).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            Console.WriteLine("> Running version detected: {0}", version);
            return version;
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="linkedVersionName"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> SetVersionKeyValuePair(string versionName) {
            return new KeyValuePair<string, string>(
                versionName.Trim(),
                versionName
                .Replace(global.versionsDir, "")
                .Replace(global.zipStartsWith, "")
                .Replace(global.zipEndsWith, "")
                .Replace(global.squashStartsWith, "")
                .Replace(global.squashEndsWith, "")
                .Replace("DIR_framework_", "")
                .Replace("/", "")
                .Trim()
                );
        }

        /// <summary>
        /// ok
        /// </summary>
        public static string RunningPath { get { return Path.Combine(global.versionsDir, global.antdRunning); } }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="file"></param>
        public static void ExtractZip(string file) {
            Terminal.Execute("7z x " + file);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="file"></param>
        public static void ExtractZipTmp(string file) {
            Terminal.Execute("7z x " + file.Replace(global.versionsDir, global.tmpDir));
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void MountTmpRam() {
            Terminal.Execute("mount -t tmpfs tmpfs " + global.tmpDir);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UmountTmpRam() {
            Terminal.Execute("umount " + global.tmpDir);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="file"></param>
        public static void CopyToTmp(string file) {
            Terminal.Execute("cp " + file + " " + global.tmpDir);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="file"></param>
        public static void MoveToTmp(string file) {
            Terminal.Execute("mv " + file + " " + global.tmpDir);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveTmpZips() {
            var files = Directory.EnumerateFiles(global.tmpDir, global.zipEndsWith);
            foreach (var file in files) {
                File.Delete(file);
            }
        }

        public static void CreateSquashFromZip(string squashName) {
            var src = Directory.EnumerateFiles(global.tmpDir).Where(d => d.Contains("antd")).FirstOrDefault();
            if (src == null) {
                Console.WriteLine("Unexpected error while creating the squashfs");
                return;
            }
            Terminal.Execute("mksquashfs " + src + " " + squashName + " -comp xz -Xbcj x86 -Xdict-size 75%");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void CleanTmp() {
            Terminal.Execute("rm -fR " + global.tmpDir + "/*");
        }

        public static void ChangeRunningVersion(KeyValuePair<string, string> newestVersionFound, string linkedVersionValue) {
            Console.WriteLine("> Updating!");
            string fileToLink;
            if (newestVersionFound.Key.Contains(global.squashEndsWith)) {
                fileToLink = newestVersionFound.Key;
            }
            else if (newestVersionFound.Key.Contains(global.zipEndsWith)) {
                fileToLink = Path.GetFullPath(newestVersionFound.Key.Replace(global.zipStartsWith, global.squashStartsWith).Replace(global.zipEndsWith, global.squashEndsWith));
                Terminal.Execute("7z x " + Path.GetFullPath(newestVersionFound.Key));
                Terminal.Execute("mksquashfs " +
                    Path.GetFullPath(newestVersionFound.Key.Replace(global.zipEndsWith, "")) + " " +
                    fileToLink +
                    " -comp xz -Xbcj x86 -Xdict-size 75%");
            }
            else {
                Console.WriteLine("> Update failed unexpectedly");
                return;
            }
            Terminal.Execute("ln -s " + Path.GetFullPath(fileToLink) + " " + Path.GetFullPath(global.antdRunning));
            Terminal.Execute("systemctl restart antd-prepare.service");
            Terminal.Execute("systemctl restart framework-antd.mount");
            Terminal.Execute("systemctl antd-launcher.service");
            return;
        }
    }
}
