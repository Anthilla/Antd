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

        public static void RemounwRwOs() {
            Terminal.Terminal.Execute($"{Parameter.Aossvc} reporemountrw");
        }

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
                Terminal.Terminal.Execute($"kill -9 {pid.Trim()}");
                _retryCountTryStopProcess = _retryCountTryStopProcess + 1;
                Thread.Sleep(500);
            }
        }

        public static void StopServices() {
            TryStopProcess("mono /framework/antd/Antd.exe");
            Thread.Sleep(2000);
        }

        public static void CheckRunningExists() {
            var running = Terminal.Terminal.Execute("ls -la " + Parameter.AntdVersionsDir + " | grep " + AntdFile.AntdRunning);
            if (running.Contains(AntdFile.AntdRunning))
                return;
            WriteLine("There's no running version of antd.");
        }

        public static KeyValuePair<string, string> GetNewestVersion() {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Parameter.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(AntdFile.ZipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(AntdFile.SquashEndsWith)).ToArray();
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
            Terminal.Terminal.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        public static void RemoveLink() {
            var running = Parameter.AntdVersionsDir + "/" + AntdFile.AntdRunning;
            WriteLine("Removing running {0}", running);
            Terminal.Terminal.Execute("rm " + running);
        }

        public static string GetRunningVersion() {
            var running = Terminal.Terminal.Execute("ls -la " + Parameter.AntdVersionsDir + " | grep " + AntdFile.AntdRunning);
            if (!running.Contains(AntdFile.AntdRunning)) {
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
                .Replace(Parameter.AntdVersionsDir, "")
                .Replace(AntdFile.ZipStartsWith, "")
                .Replace(AntdFile.ZipEndsWith, "")
                .Replace(AntdFile.SquashStartsWith, "")
                .Replace(AntdFile.SquashEndsWith, "")
                .Replace("DIR_framework_", "")
                .Replace("/", "")
                .Trim()
                );
        }

        public static string RunningPath => Path.Combine(Parameter.AntdVersionsDir, AntdFile.AntdRunning);

        public static void ExtractZip(string file) {
            Terminal.Terminal.Execute("7z x " + file);
        }

        public static void ExtractZipTmp(string file) {
            ZipFile.ExtractToDirectory(file.Replace(Parameter.AntdVersionsDir, Parameter.AntdTmpDir), file.Replace(Parameter.AntdVersionsDir, Parameter.AntdTmpDir).Replace(AntdFile.ZipEndsWith, ""));
        }

        public static void MountTmpRam() {
            Directory.CreateDirectory(Parameter.AntdTmpDir);
            Terminal.Terminal.Execute($"mount -t tmpfs tmpfs {Parameter.AntdTmpDir}");
        }

        public static void UmountTmpRam() {
            while (true) {
                var r = Terminal.Terminal.Execute($"cat /proc/mounts | grep {Parameter.AntdTmpDir}");
                if (r.Length > 0 && !r.StartsWith("----")) {
                    Terminal.Terminal.Execute($"umount -t tmpfs {Parameter.AntdTmpDir}");
                    UmountTmpRam();
                }
                var f = Terminal.Terminal.Execute($"df | grep {Parameter.AntdTmpDir}");
                if (f.Length <= 0 || f.StartsWith("----"))
                    return;
                Terminal.Terminal.Execute($"umount -t tmpfs {Parameter.AntdTmpDir}");
            }
        }

        public static void CopyToTmp(string file) {
            Terminal.Terminal.Execute("cp " + file + " " + Parameter.AntdTmpDir);
        }

        public static void MoveToTmp(string file) {
            Terminal.Terminal.Execute("mv " + file + " " + Parameter.AntdTmpDir);
        }

        public static void RemoveTmpZips() {
            var files = Directory.EnumerateFiles(Parameter.AntdTmpDir, "*.*").Where(f => f.EndsWith(".7z") || f.EndsWith(".zip"));
            foreach (var file in files) {
                WriteLine("Deleting {0}", file);
                File.Delete(file);
            }
        }

        public static void RemoveTmpAll() {
            Terminal.Terminal.Execute($"rm -fR {Parameter.AntdTmpDir}");
        }

        public static void CreateSquash(string squashName) {
            var src = Directory.EnumerateDirectories(Parameter.AntdTmpDir).FirstOrDefault(d => d.Contains("antd"));
            if (src == null) {
                WriteLine("Unexpected error while creating the squashfs");
                return;
            }
            WriteLine($"squashfs creation of: {squashName}");
            Terminal.Terminal.Execute("mksquashfs " + src + " " + squashName + " -comp xz -Xbcj x86 -Xdict-size 75%");
        }

        public static void CleanTmp() {
            UmountTmpRam();
            RemoveTmpAll();
        }

        public static void PrintVersions() {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Parameter.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(AntdFile.ZipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(AntdFile.SquashEndsWith)).ToArray();
            if (squashes.Length > 0) {
                foreach (var squash in squashes) {
                    versions.Add(SetVersionKeyValuePair(squash));
                }
            }
            if (versions.ToArray().Length <= 0)
                return;
            foreach (var version in versions) {
                WriteLine($"{0}    -    {1}", version.Key, version.Value);
            }
        }

        public static KeyValuePair<string, string> GetVersionByNumber(string number) {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Parameter.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(AntdFile.ZipEndsWith)).ToArray();
            if (zips.Length > 0) {
                foreach (var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(AntdFile.SquashEndsWith)).ToArray();
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
            var to = Parameter.AntdTmpDir + "/" + AntdFile.DownloadName;
            WriteLine("Download file to: {0}", to);
            Terminal.Terminal.Execute("wget " + url + " -o " + to);
            WriteLine("Download complete");
        }

        public static void DownloadFromUrl(string url, string destination) {
            WriteLine("Download file from: {0}", url);
            WriteLine("Download file to: {0}", destination);
            Terminal.Terminal.Execute("wget " + url + " -O " + destination);
            WriteLine("Download complete!");
        }

        public static void ExtractDownloadedFile() {
            var downloadedFile = Parameter.AntdTmpDir + "/" + AntdFile.DownloadName;
            if (!File.Exists(downloadedFile)) {
                WriteLine("The file you're looking for does not exist!");
                return;
            }
            var destination = Parameter.AntdTmpDir + "/" + AntdFile.DownloadFirstDir;
            WriteLine("Extract from {0} to {1}", downloadedFile, destination);
            ZipFile.ExtractToDirectory(downloadedFile, destination);
        }

        public static void RemoveDownloadedFile() {
            var dir = Parameter.AntdTmpDir + "/" + AntdFile.DownloadFirstDir;
            Directory.Delete(dir, true);
        }

        public static void PickAndMoveZipFileInDownloadedDirectory() {
            var mainDownloadedDir = Parameter.AntdTmpDir + "/" + AntdFile.DownloadFirstDir;
            if (!Directory.Exists(mainDownloadedDir)) {
                WriteLine("This {0} directory does not exist.", mainDownloadedDir);
                return;
            }
            var fileToPick = Directory.EnumerateFiles(mainDownloadedDir, "*.*", SearchOption.AllDirectories).FirstOrDefault(f => f.Contains("antd") && f.EndsWith("zip"));
            WriteLine("Trying to pick: {0}", fileToPick);
            var destination = Parameter.AntdTmpDir + "/" + Path.GetFileName(fileToPick);
            WriteLine("and moving it here: {0}", destination);
            if (fileToPick != null)
                File.Move(fileToPick, destination);
        }

        public static void ExtractPickedZip() {
            var downloadedZip = Directory.GetFiles(Parameter.AntdTmpDir, "*.*").FirstOrDefault(f => f.Contains("antd"));
            if (!File.Exists(downloadedZip)) {
                WriteLine("A file does not exist!");
                return;
            }
            ZipFile.ExtractToDirectory(downloadedZip, Parameter.AntdTmpDir);
        }

        public static void RestartSystemctlAntdServices() {
            Terminal.Terminal.Execute("systemctl daemon-reload");
            Terminal.Terminal.Execute("systemctl restart app-antd-01-prepare");
            Terminal.Terminal.Execute("systemctl restart app-antd-02-mount");
            Terminal.Terminal.Execute("systemctl restart app-antd-03-launcher");
        }

        public static void Umount(string dir) {
            while (true) {
                if (Mount.IsAlreadyMounted(dir) != true)
                    return;
                Terminal.Terminal.Execute($"umount {dir}");
            }
        }
    }
}
