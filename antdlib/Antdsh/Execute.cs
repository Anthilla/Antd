using antdlib.MountPoint;
using antdlib.Systemd;
///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static System.Console;

namespace antdlib.Antdsh {
    public class Execute {
        /// <summary>
        /// ok
        /// </summary>
        public static void StopServices() {
            Terminal.Execute("systemctl stop antd-prepare.service");
            Terminal.Execute("systemctl stop framework-antd.mount");
            Terminal.Execute("systemctl stop antd-launcher.service");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void CheckRunningExists() {
            var running = Terminal.Execute("ls -la " + Folder.AntdVersionsDir + " | grep " + AntdFile.antdRunning);
            if (running.Contains(AntdFile.antdRunning))
                return;
            WriteLine("There's no running version of antd.");
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetNewestVersion() {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Folder.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(AntdFile.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(AntdFile.squashEndsWith)).ToArray();
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
            WriteLine("Linking {0} to {1}", fileToLink, RunningPath);
            Terminal.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveLink() {
            var running = Folder.AntdVersionsDir + "/" + AntdFile.antdRunning;
            WriteLine("Removing running {0}", running);
            Terminal.Execute("rm " + running);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static string GetRunningVersion() {
            var running = Terminal.Execute("ls -la " + Folder.AntdVersionsDir + " | grep " + AntdFile.antdRunning);
            if (!running.Contains(AntdFile.antdRunning)) {
                WriteLine("There's no running version of antd.");
                return null;
            }
            var version = Terminal.Execute("file " + RunningPath).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            WriteLine("Running version detected: {0}", version);
            return version;
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> SetVersionKeyValuePair(string versionName) {
            return new KeyValuePair<string, string>(
                versionName.Trim(),
                versionName
                .Replace(Folder.AntdVersionsDir, "")
                .Replace(AntdFile.zipStartsWith, "")
                .Replace(AntdFile.zipEndsWith, "")
                .Replace(AntdFile.squashStartsWith, "")
                .Replace(AntdFile.squashEndsWith, "")
                .Replace("DIR_framework_", "")
                .Replace("/", "")
                .Trim()
                );
        }

        /// <summary>
        /// ok
        /// </summary>
        public static string RunningPath => Path.Combine(Folder.AntdVersionsDir, AntdFile.antdRunning);

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
            ZipFile.ExtractToDirectory(file.Replace(Folder.AntdVersionsDir, Folder.AntdTmpDir), file.Replace(Folder.AntdVersionsDir, Folder.AntdTmpDir).Replace(AntdFile.zipEndsWith, ""));
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void MountTmpRam() {
            Directory.CreateDirectory(Folder.AntdTmpDir);
            Terminal.Execute($"mount -t tmpfs tmpfs {Folder.AntdTmpDir}");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void UmountTmpRam() {
            while (true) {
                var r = Terminal.Execute($"cat /proc/mounts | grep {Folder.AntdTmpDir}");
                if (r.Length > 0 && !r.StartsWith("----")) {
                    Terminal.Execute($"umount -t tmpfs {Folder.AntdTmpDir}");
                    UmountTmpRam();
                }
                var f = Terminal.Execute($"df | grep {Folder.AntdTmpDir}");
                if (f.Length <= 0 || f.StartsWith("----"))
                    return;
                Terminal.Execute($"umount -t tmpfs {Folder.AntdTmpDir}");
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="file"></param>
        public static void CopyToTmp(string file) {
            Terminal.Execute("cp " + file + " " + Folder.AntdTmpDir);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="file"></param>
        public static void MoveToTmp(string file) {
            Terminal.Execute("mv " + file + " " + Folder.AntdTmpDir);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveTmpZips() {
            var files = Directory.EnumerateFiles(Folder.AntdTmpDir, "*.*").Where(f => f.EndsWith(".7z") || f.EndsWith(".zip"));
            foreach (var file in files) {
                WriteLine("Deleting {0}", file);
                File.Delete(file);
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveTmpAll() {
            //var files = Directory.EnumerateFiles(Folder.AntdTmpDir);
            //foreach (var file in files) {
            //    WriteLine("Deleting file {0}", file);
            //    File.Delete(file);
            //}
            //var dirs = Directory.EnumerateDirectories(Folder.AntdTmpDir);
            //foreach (var dir in dirs) {
            //    WriteLine("Deleting directory {0}", dir);
            //    Directory.Delete(dir, true);
            //}
            Terminal.Execute($"rm -fR {Folder.AntdTmpDir}");
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="squashName"></param>
        public static void CreateSquash(string squashName) {
            var src = Directory.EnumerateDirectories(Folder.AntdTmpDir).FirstOrDefault(d => d.Contains("antd"));
            if (src == null) {
                WriteLine("Unexpected error while creating the squashfs");
                return;
            }
            WriteLine($"squashfs creation of: {squashName}");
            Terminal.Execute("mksquashfs " + src + " " + squashName + " -comp xz -Xbcj x86 -Xdict-size 75%");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void CleanTmp() {
            UmountTmpRam();
            RemoveTmpAll();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void PrintVersions() {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Folder.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(AntdFile.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(AntdFile.squashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(SetVersionKeyValuePair(squash));
                }
            }
            if (versions.ToArray().Length <= 0)
                return;
            foreach (var version in versions) {
                WriteLine("   {0}    -    {1}", version.Key, version.Value);
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetVersionByNumber(string number) {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Folder.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(AntdFile.zipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(AntdFile.squashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(SetVersionKeyValuePair(squash));
                }
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if (versions.ToArray().Length > 0) {
                newestVersionFound = versions.FirstOrDefault(v => v.Value == number);
            }
            return newestVersionFound;
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="url"></param>
        public static void DownloadFromUrl(string url) {
            WriteLine("Download file from: {0}", url);
            var to = Folder.AntdTmpDir + "/" + AntdFile.downloadName;
            WriteLine("Download file to: {0}", to);
            Terminal.Execute("wget " + url + " -o " + to);
            WriteLine("Download complete");
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="url"></param>
        /// <param name="destination"></param>
        public static void DownloadFromUrl(string url, string destination) {
            WriteLine("Download file from: {0}", url);
            WriteLine("Download file to: {0}", destination);
            Terminal.Execute("wget " + url + " -O " + destination);
            WriteLine("Download complete!");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void ExtractDownloadedFile() {
            var downloadedFile = Folder.AntdTmpDir + "/" + AntdFile.downloadName;
            if (!File.Exists(downloadedFile)) {
                WriteLine("The file you're looking for does not exist!");
                return;
            }
            var destination = Folder.AntdTmpDir + "/" + AntdFile.downloadFirstDir;
            WriteLine("Extract from {0} to {1}", downloadedFile, destination);
            ZipFile.ExtractToDirectory(downloadedFile, destination);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveDownloadedFile() {
            var dir = Folder.AntdTmpDir + "/" + AntdFile.downloadFirstDir;
            Directory.Delete(dir, true);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void PickAndMoveZipFileInDownloadedDirectory() {
            var mainDownloadedDir = Folder.AntdTmpDir + "/" + AntdFile.downloadFirstDir;
            if (!Directory.Exists(mainDownloadedDir)) {
                WriteLine("This {0} directory does not exist.", mainDownloadedDir);
                return;
            }
            var fileToPick = Directory.EnumerateFiles(mainDownloadedDir, "*.*", SearchOption.AllDirectories).FirstOrDefault(f => f.Contains("antd") && f.EndsWith("zip"));
            WriteLine("Trying to pick: {0}", fileToPick);
            var destination = Folder.AntdTmpDir + "/" + Path.GetFileName(fileToPick);
            WriteLine("and moving it here: {0}", destination);
            if (fileToPick != null)
                File.Move(fileToPick, destination);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void ExtractPickedZip() {
            var downloadedZip = Directory.GetFiles(Folder.AntdTmpDir, "*.*").FirstOrDefault(f => f.Contains("antd"));
            if (!File.Exists(downloadedZip)) {
                WriteLine("A file does not exist!");
                return;
            }
            ZipFile.ExtractToDirectory(downloadedZip, Folder.AntdTmpDir);
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
            while (true) {
                var r = Terminal.Execute("cat /proc/mounts | grep /antd");
                var f = Terminal.Execute("df | grep /cfg/antd");
                if (r.Length <= 0 && f.Length <= 0)
                    return;
                Terminal.Execute($"umount {Folder.Root}");
                Terminal.Execute($"umount {Folder.Database}");
                Terminal.Execute("umount /framework/antd");
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="dir"></param>
        public static void Umount(string dir) {
            while (true) {
                if (Mount.IsAlreadyMounted(dir) != true)
                    return;
                Terminal.Execute($"umount {dir}");
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static bool IsAntdRunning() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            return (res.Length > 0);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void CheckAntdUnits() {
            var unitsToCheck = new[] {
                $"{Units.prepare}",
                $"{Units.mount}",
                $"{Units.launch}"
            };
            foreach (var unit in unitsToCheck) {
                if (File.Exists(unit)) {
                    File.Delete(unit);
                }
                if (unit == Units.prepare) {
                    WriteAntdPrepareUnit();
                }
                else if (unit == Units.mount) {
                    WriteAntdMountUnit();
                }
                else if (unit == Units.launch) {
                    WriteAntdLaunchUnit();
                }
            }
            Systemctl.DaemonReload();
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void WriteAntdPrepareUnit() {
            var path = Units.prepare;
            if (!File.Exists(path)) {
                using (StreamWriter sw = File.CreateText(path)) {
                    sw.WriteLine("[Unit]");
                    sw.WriteLine("Description=External Volume Unit, Application: Antd Prepare Service");
                    sw.WriteLine("Requires=local-fs.target sysinit.target");
                    sw.WriteLine("Before=framework-antd.mount");
                    sw.WriteLine("");
                    sw.WriteLine("[Service]");
                    sw.WriteLine("ExecStart=/bin/mkdir -p /framework/antd");
                    sw.WriteLine("");
                    sw.WriteLine("[Install]");
                    sw.WriteLine("WantedBy=applicative.target");
                }
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void WriteAntdMountUnit() {
            var path = Units.mount;
            if (!File.Exists(path)) {
                using (StreamWriter sw = File.CreateText(path)) {
                    sw.WriteLine("[Unit]");
                    sw.WriteLine("Description=External Volume Unit, Application: DIR_framework_antd Mount ");
                    sw.WriteLine("ConditionPathExists=/framework/antd");
                    sw.WriteLine("");
                    sw.WriteLine("[Mount]");
                    sw.WriteLine("What=/mnt/cdrom/Apps/Anthilla_Antd/active-version");
                    sw.WriteLine("Where=/framework/antd/");
                    sw.WriteLine("");
                    sw.WriteLine("[Install]");
                    sw.WriteLine("WantedBy=applicative.target");
                }
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void WriteAntdLaunchUnit() {
            var path = Units.launch;
            if (!File.Exists(path)) {
                using (StreamWriter sw = File.CreateText(path)) {
                    sw.WriteLine("[Unit]");
                    sw.WriteLine("Description=External Volume Unit, Application: Antd Launcher Service");
                    sw.WriteLine("Requires=local-fs.target sysinit.target");
                    sw.WriteLine("After=framework-antd.mount");
                    sw.WriteLine("");
                    sw.WriteLine("[Service]");
                    sw.WriteLine("ExecStart=/usr/bin/mono /framework/antd/Antd.exe");
                    sw.WriteLine("");
                    sw.WriteLine("[Install]");
                    sw.WriteLine("WantedBy=applicative.target");
                }
            }
        }
    }
}
