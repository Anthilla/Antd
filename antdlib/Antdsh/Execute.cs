using antdlib.MountPoint;
//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using static System.Console;

namespace antdlib.Antdsh {
    public class Execute {

        private static int _retryCountTryStopProcess;
        private static void TryStopProcess(string query) {
            while (true) {
                var psResult = Terminal.Terminal.Execute($"ps -aef|grep '{query}'|grep -v grep");
                //var netResult = psResult.Length > 0 ? Terminal.Terminal.Execute($"netstat -anp |grep {psResult}") : "";
                if (psResult.Length <= 0)
                    return;
                var split = psResult.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length <= 0)
                    return;
                var pid = split[1];
                Terminal.Terminal.Background.Execute($"kill -9 {pid.Trim()}");
                _retryCountTryStopProcess = _retryCountTryStopProcess + 1;
                Thread.Sleep(500);
            }
        }

        public static void StopServices() {
            TryStopProcess("mono /framework/antd/Antd.exe");
            Thread.Sleep(2000);
        }

        public static void CheckRunningExists() {
            var running = Terminal.Terminal.Execute("ls -la " + Folder.AntdVersionsDir + " | grep " + AntdFile.antdRunning);
            if (running.Contains(AntdFile.antdRunning))
                return;
            WriteLine("There's no running version of antd.");
        }

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

        public static void LinkVersionToRunning(string fileToLink) {
            WriteLine("Linking {0} to {1}", fileToLink, RunningPath);
            Terminal.Terminal.Background.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        public static void RemoveLink() {
            var running = Folder.AntdVersionsDir + "/" + AntdFile.antdRunning;
            WriteLine("Removing running {0}", running);
            Terminal.Terminal.Background.Execute("rm " + running);
        }

        public static string GetRunningVersion() {
            var running = Terminal.Terminal.Execute("ls -la " + Folder.AntdVersionsDir + " | grep " + AntdFile.antdRunning);
            if (!running.Contains(AntdFile.antdRunning)) {
                WriteLine("There's no running version of antd.");
                return null;
            }
            var version = Terminal.Terminal.Execute("file " + RunningPath).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            WriteLine("Running version detected: {0}", version);
            return version;
        }

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

        public static string RunningPath => Path.Combine(Folder.AntdVersionsDir, AntdFile.antdRunning);

        public static void ExtractZip(string file) {
            Terminal.Terminal.Execute("7z x " + file);
        }

        public static void ExtractZipTmp(string file) {
            ZipFile.ExtractToDirectory(file.Replace(Folder.AntdVersionsDir, Folder.AntdTmpDir), file.Replace(Folder.AntdVersionsDir, Folder.AntdTmpDir).Replace(AntdFile.zipEndsWith, ""));
        }

        public static void MountTmpRam() {
            Directory.CreateDirectory(Folder.AntdTmpDir);
            Terminal.Terminal.Background.Execute($"mount -t tmpfs tmpfs {Folder.AntdTmpDir}");
        }

        public static void UmountTmpRam() {
            while (true) {
                var r = Terminal.Terminal.Execute($"cat /proc/mounts | grep {Folder.AntdTmpDir}");
                if (r.Length > 0 && !r.StartsWith("----")) {
                    Terminal.Terminal.Background.Execute($"umount -t tmpfs {Folder.AntdTmpDir}");
                    UmountTmpRam();
                }
                var f = Terminal.Terminal.Execute($"df | grep {Folder.AntdTmpDir}");
                if (f.Length <= 0 || f.StartsWith("----"))
                    return;
                Terminal.Terminal.Background.Execute($"umount -t tmpfs {Folder.AntdTmpDir}");
            }
        }

        public static void CopyToTmp(string file) {
            Terminal.Terminal.Background.Execute("cp " + file + " " + Folder.AntdTmpDir);
        }

        public static void MoveToTmp(string file) {
            Terminal.Terminal.Background.Execute("mv " + file + " " + Folder.AntdTmpDir);
        }

        public static void RemoveTmpZips() {
            var files = Directory.EnumerateFiles(Folder.AntdTmpDir, "*.*").Where(f => f.EndsWith(".7z") || f.EndsWith(".zip"));
            foreach (var file in files) {
                WriteLine("Deleting {0}", file);
                File.Delete(file);
            }
        }

        public static void RemoveTmpAll() {
            Terminal.Terminal.Background.Execute($"rm -fR {Folder.AntdTmpDir}");
        }

        public static void CreateSquash(string squashName) {
            var src = Directory.EnumerateDirectories(Folder.AntdTmpDir).FirstOrDefault(d => d.Contains("antd"));
            if (src == null) {
                WriteLine("Unexpected error while creating the squashfs");
                return;
            }
            WriteLine($"squashfs creation of: {squashName}");
            Terminal.Terminal.Background.Execute("mksquashfs " + src + " " + squashName + " -comp xz -Xbcj x86 -Xdict-size 75%");
        }

        public static void CleanTmp() {
            UmountTmpRam();
            RemoveTmpAll();
        }

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

        public static void DownloadFromUrl(string url) {
            WriteLine("Download file from: {0}", url);
            var to = Folder.AntdTmpDir + "/" + AntdFile.downloadName;
            WriteLine("Download file to: {0}", to);
            Terminal.Terminal.Background.Execute("wget " + url + " -o " + to);
            WriteLine("Download complete");
        }

        public static void DownloadFromUrl(string url, string destination) {
            WriteLine("Download file from: {0}", url);
            WriteLine("Download file to: {0}", destination);
            Terminal.Terminal.Background.Execute("wget " + url + " -O " + destination);
            WriteLine("Download complete!");
        }

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

        public static void RemoveDownloadedFile() {
            var dir = Folder.AntdTmpDir + "/" + AntdFile.downloadFirstDir;
            Directory.Delete(dir, true);
        }

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

        public static void ExtractPickedZip() {
            var downloadedZip = Directory.GetFiles(Folder.AntdTmpDir, "*.*").FirstOrDefault(f => f.Contains("antd"));
            if (!File.Exists(downloadedZip)) {
                WriteLine("A file does not exist!");
                return;
            }
            ZipFile.ExtractToDirectory(downloadedZip, Folder.AntdTmpDir);
        }

        public static void RestartSystemctlAntdServices() {
            Terminal.Terminal.Background.Execute("systemctl daemon-reload");
            Terminal.Terminal.Background.Execute("systemctl restart app-antd-01-prepare");
            Terminal.Terminal.Background.Execute("systemctl restart app-antd-02-mount");
            Terminal.Terminal.Background.Execute("systemctl restart app-antd-03-launcher");
        }

        public static void UmountAntd() {
            while (true) {
                var r = Terminal.Terminal.Execute("cat /proc/mounts | grep /antd");
                var f = Terminal.Terminal.Execute("df | grep /cfg/antd");
                if (r.Length <= 0 && f.Length <= 0)
                    return;
                Terminal.Terminal.Background.Execute($"umount {Folder.Root}");
                Terminal.Terminal.Background.Execute($"umount {Folder.Database}");
                Terminal.Terminal.Background.Execute("umount /framework/antd");
            }
        }

        public static void Umount(string dir) {
            while (true) {
                if (Mount.IsAlreadyMounted(dir) != true)
                    return;
                Terminal.Terminal.Background.Execute($"umount {dir}");
            }
        }

        public static bool IsAntdRunning() {
            return Terminal.Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep").Length > 0;
        }
    }
}
