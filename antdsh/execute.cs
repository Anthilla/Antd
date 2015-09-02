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

using antdlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static System.Console;

namespace antdsh {
    public class execute {
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
            var running = Terminal.Execute("ls -la " + global.versionsDir + " | grep " + global.antdRunning);
            if (!running.Contains(global.antdRunning)) {
                WriteLine("There's no running version of antd.");
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
            WriteLine("Linking {0} to {1}", fileToLink, RunningPath);
            Terminal.Execute("ln -s " + fileToLink + " " + RunningPath);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveLink() {
            var running = global.versionsDir + "/" + global.antdRunning;
            WriteLine("Removing running {0}", running);
            Terminal.Execute("rm " + running);
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static string GetRunningVersion() {
            var running = Terminal.Execute("ls -la " + global.versionsDir + " | grep " + global.antdRunning);
            if (!running.Contains(global.antdRunning)) {
                WriteLine("There's no running version of antd.");
                return null;
            }
            var version = Terminal.Execute("file " + RunningPath).Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
            WriteLine("Running version detected: {0}", version);
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
                WriteLine("Deleting {0}", file);
                File.Delete(file);
            }
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void RemoveTmpAll() {
            var files = Directory.EnumerateFiles(global.tmpDir);
            foreach (var file in files) {
                WriteLine("Deleting file {0}", file);
                File.Delete(file);
            }
            var dirs = Directory.EnumerateDirectories(global.tmpDir);
            foreach (var dir in dirs) {
                WriteLine("Deleting directory {0}", dir);
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
                WriteLine("Unexpected error while creating the squashfs");
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
                    WriteLine("   {0}    -    {1}", version.Key, version.Value);
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
            WriteLine("Download file from: {0}", url);
            var to = global.tmpDir + "/" + global.downloadName;
            WriteLine("Download file to: {0}", to);
            Terminal.Execute("wget " + url + " -O " + to);
            WriteLine("Download complete");
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="url"></param>
        public static void DownloadFromUrl(string url, string destination) {
            WriteLine("Download file from: {0}", url);
            var to = destination;
            WriteLine("Download file to: {0}", to);
            Terminal.Execute("wget " + url + " -O " + to);
            WriteLine("Download complete");
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void ExtractDownloadedFile() {
            var downloadedFile = global.tmpDir + "/" + global.downloadName;
            if (!File.Exists(downloadedFile)) {
                WriteLine("A file does not exist!");
                return;
            }
            var destination = global.tmpDir + "/" + global.downloadFirstDir;
            WriteLine("Extract from {0} to {1}", downloadedFile, destination);
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
                WriteLine("This {0} directory does not exist.", mainDownloadedDir);
                return;
            }
            var fileToPick = Directory.EnumerateFiles(mainDownloadedDir, "*.*", SearchOption.AllDirectories).FirstOrDefault(f => f.Contains("antd") && f.EndsWith("zip"));
            WriteLine("Trying to pick: {0}", fileToPick);
            var destination = global.tmpDir + "/" + Path.GetFileName(fileToPick);
            WriteLine("and moving it here: {0}", destination);
            File.Move(fileToPick, destination);
        }

        /// <summary>
        /// ok
        /// </summary>
        public static void ExtractPickedZip() {
            var downloadedZip = Directory.GetFiles(global.tmpDir, "*.*").FirstOrDefault(f => f.Contains("antd"));
            if (!File.Exists(downloadedZip)) {
                WriteLine("A file does not exist!");
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
            var r = Terminal.Execute("cat /proc/mounts | grep /antd");
            var f = Terminal.Execute("df | grep /cfg/antd");
            if (r.Length > 0 || f.Length > 0) {
                Terminal.Execute("umount " + Folder.Networkd);
                Terminal.Execute("umount " + Folder.FileRepository);
                Terminal.Execute("umount " + Folder.Database);
                Terminal.Execute("umount " + Folder.Config);
                Terminal.Execute("umount " + Folder.Root);
                Terminal.Execute("umount /framework/antd");
                UmountAntd();
            }
            else
                return;
        }

        /// <summary>
        /// ok
        /// </summary>
        /// <returns></returns>
        public static bool IsAntdRunning() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            return (res.Length > 0) ? true : false;
        }
    }
}
