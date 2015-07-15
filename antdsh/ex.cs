using antdlib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class ex {
        /// <summary>
        /// ok
        /// </summary>
        public static void StopServices() {
            Terminal.Execute("systemctl stop antd-prepare.service");
            Terminal.Execute("systemctl stop framework-antd.mount");
            Terminal.Execute("systemctl stop antd-launcher.service");
            //Terminal.Execute("killall mono");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void CheckRunningExists() {
            var running = Terminal.Execute("ls -la " + global.versionsDir + " | grep " + global.antdRunning);
            if (!running.Contains(global.antdRunning)) {
                Console.WriteLine("There's no running version of antd.");
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
            Console.WriteLine("Linking {0} to {1}", fileToLink, RunningPath);
            Terminal.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveLink() {
            var running = global.versionsDir + "/" + global.antdRunning;
            Console.WriteLine("Removing running {0}", running);
            Terminal.Execute("rm " + running);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static string GetRunningVersion() {
            var running = Terminal.Execute("ls -la " + global.versionsDir + " | grep " + global.antdRunning);
            if (!running.Contains(global.antdRunning)) {
                Console.WriteLine("There's no running version of antd.");
                return null;
            }
            var version = Terminal.Execute("file " + RunningPath).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            Console.WriteLine("Running version detected: {0}", version);
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
            ZipFile.ExtractToDirectory(file.Replace(global.versionsDir, global.tmpDir), file.Replace(global.versionsDir, global.tmpDir).Replace(global.zipEndsWith, ""));
            //Terminal.Execute("7z x " + file.Replace(global.versionsDir, global.tmpDir));
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
            Terminal.Execute("umount -t tmpfs " + global.tmpDir);
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
            var files = Directory.EnumerateFiles(global.tmpDir, "*.*").Where(f => f.EndsWith(".7z") || f.EndsWith(".zip"));
            foreach (var file in files) {
                Console.WriteLine("Deleting {0}", file);
                File.Delete(file);
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveTmpAll() {
            var files = Directory.EnumerateFiles(global.tmpDir);
            foreach (var file in files) {
                Console.WriteLine("Deleting file {0}", file);
                File.Delete(file);
            }
            var dirs = Directory.EnumerateDirectories(global.tmpDir);
            foreach (var dir in dirs) {
                Console.WriteLine("Deleting directory {0}", dir);
                Directory.Delete(dir, true);
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="squashName"></param>
        public static void CreateSquash(string squashName) {
            var src = Directory.EnumerateDirectories(global.tmpDir).Where(d => d.Contains("antd")).FirstOrDefault();
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
            RemoveTmpAll();
            UmountTmpRam();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void PrintVersions() {
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
                foreach (var version in versions) {
                    Console.WriteLine("   {0}    -    {1}", version.Key, version.Value);
                }
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetVersionByNumber(string number) {
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
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versions.ToArray().Length > 0) {
                newestVersionFound = versions.Where(v => v.Value == number).FirstOrDefault();
            }
            return newestVersionFound;
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="url"></param>
        public static void DownloadFromUrl(string url) {
            Console.WriteLine("Download file from: {0}", url);
            var to = global.tmpDir + "/" + global.downloadName;
            Console.WriteLine("Download file to: {0}", to);
            Terminal.Execute("wget " + url + " -O " + to);
            Console.WriteLine("Download complete");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void ExtractDownloadedFile() {
            var downloadedFile = global.tmpDir + "/" + global.downloadName;
            if (!File.Exists(downloadedFile)) {
                Console.WriteLine("A file does not exist!");
                return;
            }
            var destination = global.tmpDir + "/" + global.downloadFirstDir;
            Console.WriteLine("Extract from {0} to {1}", downloadedFile, destination);
            ZipFile.ExtractToDirectory(downloadedFile, destination);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveDownloadedFile() {
            var dir = global.tmpDir + "/" + global.downloadFirstDir;
            Directory.Delete(dir, true);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void PickAndMoveZipFileInDownloadedDirectory() {
            var mainDownloadedDir = global.tmpDir + "/" + global.downloadFirstDir;
            if (!Directory.Exists(mainDownloadedDir)) {
                Console.WriteLine("This {0} directory does not exist.", mainDownloadedDir);
                return;
            }
            var fileToPick = Directory.EnumerateFiles(mainDownloadedDir, "*.*", SearchOption.AllDirectories).FirstOrDefault(f => f.Contains("antd") && f.EndsWith("zip"));
            Console.WriteLine("Trying to pick: {0}", fileToPick);
            var destination = global.tmpDir + "/" + Path.GetFileName(fileToPick);
            Console.WriteLine("and moving it here: {0}", destination);
            File.Move(fileToPick, destination);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void ExtractPickedZip() {
            var downloadedZip = Directory.GetFiles(global.tmpDir, "*.*").FirstOrDefault(f => f.Contains("antd"));
            if (!File.Exists(downloadedZip)) {
                Console.WriteLine("A file does not exist!");
                return;
            }
            //var destination = global.tmpDir;
            ZipFile.ExtractToDirectory(downloadedZip, global.tmpDir);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RestartSystemctlAntdServices() {
            Terminal.Execute("systemctl restart antd-prepare.service");
            Terminal.Execute("systemctl restart framework-antd.mount");
            Terminal.Execute("systemctl restart antd-launcher.service");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UmountAntd() {
            Terminal.Execute("umount " + antdconst.Folder.Networkd);
            Terminal.Execute("umount " + antdconst.Folder.FileRepository);
            Terminal.Execute("umount " + antdconst.Folder.Database);
            Terminal.Execute("umount " + antdconst.Folder.Config);
            Terminal.Execute("umount " + antdconst.Folder.Root);
            Terminal.Execute("umount /framework/antd");
        }
    }
}
