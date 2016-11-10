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
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.common.Tool;

namespace antdsh {
    public class Execute {

        private static readonly Bash Bash = new Bash();

        public static void RemounwRwOs() {
            Bash.Execute($"{Parameter.Aossvc} reporemountrw", false);
        }

        private static int _retryCountTryStopProcess;
        private static void TryStopProcess(string query) {
            while(true) {
                var psResult = Bash.Execute("ps -aef").SplitBash().Grep(query).ToList();
                if(!psResult.Any())
                    return;
                var split = psResult.First()?.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(split?.Length <= 0)
                    return;
                var pid = split?[1];
                Bash.Execute($"kill -9 {pid?.Trim()}");
                _retryCountTryStopProcess = _retryCountTryStopProcess + 1;
                Thread.Sleep(500);
            }
        }

        public static void StopServices() {
            TryStopProcess("mono /framework/antd/Antd.exe");
            Thread.Sleep(2000);
        }

        public static void CheckRunningExists() {
            var running = Bash.Execute("ls -la " + Parameter.AntdVersionsDir).SplitBash().Grep(Parameter.AntdRunning);
            if(running.First().Contains(Parameter.AntdRunning))
                return;
            Console.WriteLine("There's no running version of antd.");
        }

        public static KeyValuePair<string, string> GetNewestVersion() {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Parameter.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(Parameter.ZipEndsWith)).ToArray();
            if(zips.Length > 0) {
                foreach(var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(Parameter.SquashEndsWith)).ToArray();
            if(squashes.Length > 0) {
                foreach(var squash in squashes) {
                    versions.Add(SetVersionKeyValuePair(squash));
                }
            }
            var versionsOrdered = new KeyValuePair<string, string>[] { };
            if(versions.ToArray().Length > 0) {
                versionsOrdered = versions.OrderByDescending(i => i.Value).ToArray();
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if(versionsOrdered.Length > 0) {
                newestVersionFound = versionsOrdered.FirstOrDefault();
            }
            return newestVersionFound;
        }

        public static void LinkVersionToRunning(string fileToLink) {
            Console.WriteLine("Linking {0} to {1}", fileToLink, RunningPath);
            Bash.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        public static void RemoveLink() {
            var running = Parameter.AntdVersionsDir + "/" + Parameter.AntdRunning;
            Console.WriteLine("Removing running {0}", running);
            Bash.Execute("rm " + running);
        }

        public static string GetRunningVersion() {
            var running = Bash.Execute("ls -la " + Parameter.AntdVersionsDir).SplitBash().Grep(Parameter.AntdRunning);
            if(!running.First().Contains(Parameter.AntdRunning)) {
                Console.WriteLine("There's no running version of antd.");
                return null;
            }
            var version = Bash.Execute("file " + RunningPath).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            Console.WriteLine("Running version detected: {0}", version);
            return version;
        }

        public static KeyValuePair<string, string> SetVersionKeyValuePair(string versionName) {
            return new KeyValuePair<string, string>(
                versionName.Trim(),
                versionName
                .Replace(Parameter.AntdVersionsDir, "")
                .Replace(Parameter.ZipStartsWith, "")
                .Replace(Parameter.ZipEndsWith, "")
                .Replace(Parameter.SquashStartsWith, "")
                .Replace(Parameter.SquashEndsWith, "")
                .Replace("DIR_framework_", "")
                .Replace("/", "")
                .Trim()
                );
        }

        public static string RunningPath => Path.Combine(Parameter.AntdVersionsDir, Parameter.AntdRunning);

        public static void ExtractZip(string file) {
            Bash.Execute("7z x " + file);
        }

        public static void ExtractZipTmp(string file) {
            ZipFile.ExtractToDirectory(file.Replace(Parameter.AntdVersionsDir, Parameter.AntdTmpDir), file.Replace(Parameter.AntdVersionsDir, Parameter.AntdTmpDir).Replace(Parameter.ZipEndsWith, ""));
        }

        public static void MountTmpRam() {
            Directory.CreateDirectory(Parameter.AntdTmpDir);
            Bash.Execute($"mount -t tmpfs tmpfs {Parameter.AntdTmpDir}");
        }

        public static void UmountTmpRam() {
            while(true) {
                var procMounts = File.ReadAllLines("/proc/mounts");
                if(procMounts.Any(_ => _.Contains(Parameter.AntdTmpDir) && !_.StartsWith("----"))) {
                    Bash.Execute($"umount -t tmpfs {Parameter.AntdTmpDir}");
                    UmountTmpRam();
                }

                var f = File.ReadAllLines("/proc/mounts");
                if(f.Any(_ => _.Contains(Parameter.AntdTmpDir) && !_.StartsWith("----")))
                    return;
                Bash.Execute($"umount -t tmpfs {Parameter.AntdTmpDir}");
            }
        }

        public static void CopyToTmp(string file) {
            Bash.Execute("cp " + file + " " + Parameter.AntdTmpDir);
        }

        public static void MoveToTmp(string file) {
            Bash.Execute("mv " + file + " " + Parameter.AntdTmpDir);
        }

        public static void RemoveTmpZips() {
            var files = Directory.EnumerateFiles(Parameter.AntdTmpDir, "*.*").Where(f => f.EndsWith(".7z") || f.EndsWith(".zip"));
            foreach(var file in files) {
                Console.WriteLine("Deleting {0}", file);
                File.Delete(file);
            }
        }

        public static void RemoveTmpAll() {
            Bash.Execute($"rm -fR {Parameter.AntdTmpDir}");
        }

        public static void CreateSquash(string squashName) {
            var src = Directory.EnumerateDirectories(Parameter.AntdTmpDir).FirstOrDefault(d => d.Contains("antd"));
            if(src == null) {
                Console.WriteLine("Unexpected error while creating the squashfs");
                return;
            }
            Console.WriteLine($"squashfs creation of: {squashName}");
            Bash.Execute("mksquashfs " + src + " " + squashName + " -comp xz -Xbcj x86 -Xdict-size 75%");
        }

        public static void CleanTmp() {
            UmountTmpRam();
            RemoveTmpAll();
        }

        public static void PrintVersions() {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Parameter.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(Parameter.ZipEndsWith)).ToArray();
            if(zips.Length > 0) {
                foreach(var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(Parameter.SquashEndsWith)).ToArray();
            if(squashes.Length > 0) {
                foreach(var squash in squashes) {
                    versions.Add(SetVersionKeyValuePair(squash));
                }
            }
            if(versions.ToArray().Length <= 0)
                return;
            foreach(var version in versions) {
                Console.WriteLine($"{0}    -    {1}", version.Key, version.Value);
            }
        }

        public static KeyValuePair<string, string> GetVersionByNumber(string number) {
            var versions = new HashSet<KeyValuePair<string, string>>();
            var files = Directory.EnumerateFiles(Parameter.AntdVersionsDir, "*.*");
            var enumerable = files as string[] ?? files.ToArray();
            var zips = enumerable.Where(s => s.EndsWith(Parameter.ZipEndsWith)).ToArray();
            if(zips.Length > 0) {
                foreach(var zip in zips) {
                    versions.Add(SetVersionKeyValuePair(zip));
                }
            }
            var squashes = enumerable.Where(s => s.EndsWith(Parameter.SquashEndsWith)).ToArray();
            if(squashes.Length > 0) {
                foreach(var squash in squashes) {
                    versions.Add(SetVersionKeyValuePair(squash));
                }
            }
            var newestVersionFound = new KeyValuePair<string, string>(null, null);
            if(versions.ToArray().Length > 0) {
                newestVersionFound = versions.FirstOrDefault(v => v.Value == number);
            }
            return newestVersionFound;
        }

        public static void DownloadFromUrl(string url) {
            Console.WriteLine("Download file from: {0}", url);
            var to = Parameter.AntdTmpDir + "/" + Parameter.DownloadName;
            Console.WriteLine("Download file to: {0}", to);
            Bash.Execute("wget " + url + " -o " + to);
            Console.WriteLine("Download complete");
        }

        public static void DownloadFromUrl(string url, string destination) {
            Console.WriteLine("Download file from: {0}", url);
            Console.WriteLine("Download file to: {0}", destination);
            Bash.Execute("wget " + url + " -O " + destination);
            Console.WriteLine("Download complete!");
        }

        public static void ExtractDownloadedFile() {
            var downloadedFile = Parameter.AntdTmpDir + "/" + Parameter.DownloadName;
            if(!File.Exists(downloadedFile)) {
                Console.WriteLine("The file you're looking for does not exist!");
                return;
            }
            var destination = Parameter.AntdTmpDir + "/" + Parameter.DownloadFirstDir;
            Console.WriteLine("Extract from {0} to {1}", downloadedFile, destination);
            ZipFile.ExtractToDirectory(downloadedFile, destination);
        }

        public static void RemoveDownloadedFile() {
            var dir = Parameter.AntdTmpDir + "/" + Parameter.DownloadFirstDir;
            Directory.Delete(dir, true);
        }

        public static void PickAndMoveZipFileInDownloadedDirectory() {
            var mainDownloadedDir = Parameter.AntdTmpDir + "/" + Parameter.DownloadFirstDir;
            if(!Directory.Exists(mainDownloadedDir)) {
                Console.WriteLine("This {0} directory does not exist.", mainDownloadedDir);
                return;
            }
            var fileToPick = Directory.EnumerateFiles(mainDownloadedDir, "*.*", SearchOption.AllDirectories).FirstOrDefault(f => f.Contains("antd") && f.EndsWith("zip"));
            Console.WriteLine("Trying to pick: {0}", fileToPick);
            var destination = Parameter.AntdTmpDir + "/" + Path.GetFileName(fileToPick);
            Console.WriteLine("and moving it here: {0}", destination);
            if(fileToPick != null)
                File.Move(fileToPick, destination);
        }

        public static void ExtractPickedZip() {
            var downloadedZip = Directory.GetFiles(Parameter.AntdTmpDir, "*.*").FirstOrDefault(f => f.Contains("antd"));
            if(!File.Exists(downloadedZip)) {
                Console.WriteLine("A file does not exist!");
                return;
            }
            ZipFile.ExtractToDirectory(downloadedZip, Parameter.AntdTmpDir);
        }

        public static void RestartSystemctlAntdServices() {
            Bash.Execute("systemctl daemon-reload");
            Bash.Execute("systemctl restart app-antd-01-prepare");
            Bash.Execute("systemctl restart app-antd-02-mount");
            Bash.Execute("systemctl restart app-antd-03-launcher");
        }

        public static void Umount(string dir) {
            while(true) {
                if(Mounts.IsAlreadyMounted(dir) != true)
                    return;
                Bash.Execute($"umount {dir}");
            }
        }
    }
}
