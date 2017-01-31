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

namespace antdsh {
    public class Execute {

        private readonly Bash _bash = new Bash();

        public void RemounwRwOs() {
            _bash.Execute($"{Parameter.Aossvc} reporemountrw", false);
        }

        private int _retryCountTryStopProcess;
        private void TryStopProcess(string query) {
            while(true) {
                var psResult = _bash.Execute("ps -aef").SplitBash().Grep(query).ToList();
                if(!psResult.Any())
                    return;
                var split = psResult.First()?.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(split?.Length <= 0)
                    return;
                var pid = split?[1];
                _bash.Execute($"kill -9 {pid?.Trim()}");
                _retryCountTryStopProcess = _retryCountTryStopProcess + 1;
                Thread.Sleep(500);
            }
        }

        public void StopServices() {
            TryStopProcess("mono /framework/antd/Antd.exe");
            Thread.Sleep(2000);
        }

        public void CheckRunningExists() {
            var running = _bash.Execute("ls -la " + Parameter.AntdVersionsDir).SplitBash().Grep(Parameter.AntdRunning);
            if(running.First().Contains(Parameter.AntdRunning))
                return;
            Console.WriteLine("There's no running version of antd.");
        }

        public KeyValuePair<string, string> GetNewestVersion() {
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

        public void LinkVersionToRunning(string fileToLink) {
            Console.WriteLine("Linking {0} to {1}", fileToLink, RunningPath);
            _bash.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        public void RemoveLink() {
            var running = Parameter.AntdVersionsDir + "/" + Parameter.AntdRunning;
            Console.WriteLine("Removing running {0}", running);
            _bash.Execute("rm " + running);
        }

        public string GetRunningVersion() {
            var running = _bash.Execute("ls -la " + Parameter.AntdVersionsDir).SplitBash().Grep(Parameter.AntdRunning);
            if(!running.First().Contains(Parameter.AntdRunning)) {
                Console.WriteLine("There's no running version of antd.");
                return null;
            }
            var version = _bash.Execute("file " + RunningPath).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            Console.WriteLine("Running version detected: {0}", version);
            return version;
        }

        public KeyValuePair<string, string> SetVersionKeyValuePair(string versionName) {
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

        public string RunningPath => Path.Combine(Parameter.AntdVersionsDir, Parameter.AntdRunning);

        public void ExtractZip(string file) {
            _bash.Execute("7z x " + file);
        }

        public void ExtractZipTmp(string file) {
            ZipFile.ExtractToDirectory(file.Replace(Parameter.AntdVersionsDir, Parameter.AntdTmpDir), file.Replace(Parameter.AntdVersionsDir, Parameter.AntdTmpDir).Replace(Parameter.ZipEndsWith, ""));
        }

        public void MountTmpRam() {
            Directory.CreateDirectory(Parameter.AntdTmpDir);
            _bash.Execute($"mount -t tmpfs tmpfs {Parameter.AntdTmpDir}");
        }

        public void UmountTmpRam() {
            while(true) {
                var procMounts = File.ReadAllLines("/proc/mounts");
                if(procMounts.Any(_ => _.Contains(Parameter.AntdTmpDir) && !_.StartsWith("----"))) {
                    _bash.Execute($"umount -t tmpfs {Parameter.AntdTmpDir}");
                    UmountTmpRam();
                }

                var f = File.ReadAllLines("/proc/mounts");
                if(f.Any(_ => _.Contains(Parameter.AntdTmpDir) && !_.StartsWith("----")))
                    return;
                _bash.Execute($"umount -t tmpfs {Parameter.AntdTmpDir}");
            }
        }

        public void CopyToTmp(string file) {
            _bash.Execute("cp " + file + " " + Parameter.AntdTmpDir);
        }

        public void MoveToTmp(string file) {
            _bash.Execute("mv " + file + " " + Parameter.AntdTmpDir);
        }

        public void RemoveTmpZips() {
            var files = Directory.EnumerateFiles(Parameter.AntdTmpDir, "*.*").Where(f => f.EndsWith(".7z") || f.EndsWith(".zip"));
            foreach(var file in files) {
                Console.WriteLine("Deleting {0}", file);
                File.Delete(file);
            }
        }

        public void RemoveTmpAll() {
            _bash.Execute($"rm -fR {Parameter.AntdTmpDir}");
        }

        public void CreateSquash(string squashName) {
            var src = Directory.EnumerateDirectories(Parameter.AntdTmpDir).FirstOrDefault(d => d.Contains("antd"));
            if(src == null) {
                Console.WriteLine("Unexpected error while creating the squashfs");
                return;
            }
            Console.WriteLine($"squashfs creation of: {squashName}");
            _bash.Execute("mksquashfs " + src + " " + squashName + " -comp xz -Xbcj x86 -Xdict-size 75%");
        }

        public void CleanTmp() {
            UmountTmpRam();
            RemoveTmpAll();
        }

        public void PrintVersions() {
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

        public KeyValuePair<string, string> GetVersionByNumber(string number) {
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

        public void DownloadFromUrl(string url) {
            Console.WriteLine("Download file from: {0}", url);
            var to = Parameter.AntdTmpDir + "/" + Parameter.DownloadName;
            Console.WriteLine("Download file to: {0}", to);
            _bash.Execute("wget " + url + " -o " + to);
            Console.WriteLine("Download complete");
        }

        public void DownloadFromUrl(string url, string destination) {
            Console.WriteLine("Download file from: {0}", url);
            Console.WriteLine("Download file to: {0}", destination);
            _bash.Execute("wget " + url + " -O " + destination);
            Console.WriteLine("Download complete!");
        }

        public void ExtractDownloadedFile() {
            var downloadedFile = Parameter.AntdTmpDir + "/" + Parameter.DownloadName;
            if(!File.Exists(downloadedFile)) {
                Console.WriteLine("The file you're looking for does not exist!");
                return;
            }
            var destination = Parameter.AntdTmpDir + "/" + Parameter.DownloadFirstDir;
            Console.WriteLine("Extract from {0} to {1}", downloadedFile, destination);
            ZipFile.ExtractToDirectory(downloadedFile, destination);
        }

        public void RemoveDownloadedFile() {
            var dir = Parameter.AntdTmpDir + "/" + Parameter.DownloadFirstDir;
            Directory.Delete(dir, true);
        }

        public void PickAndMoveZipFileInDownloadedDirectory() {
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

        public void ExtractPickedZip() {
            var downloadedZip = Directory.GetFiles(Parameter.AntdTmpDir, "*.*").FirstOrDefault(f => f.Contains("antd"));
            if(!File.Exists(downloadedZip)) {
                Console.WriteLine("A file does not exist!");
                return;
            }
            ZipFile.ExtractToDirectory(downloadedZip, Parameter.AntdTmpDir);
        }

        public void RestartSystemctlAntdServices() {
            _bash.Execute("systemctl daemon-reload");
            _bash.Execute("systemctl restart app-antd-01-prepare");
            _bash.Execute("systemctl restart app-antd-02-mount");
            _bash.Execute("systemctl restart app-antd-03-launcher");
        }

        public void Umount(string dir) {
            while(true) {
                if(MountHelper.IsAlreadyMounted(dir) != true)
                    return;
                _bash.Execute($"umount {dir}");
            }
        }
    }
}
