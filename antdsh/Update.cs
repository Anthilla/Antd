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
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using antdlib.common;

namespace antdsh {
    public class Update {

        public class FileInfoModel {
            public string FileName { get; set; }
            public string FileContext { get; set; }
            public string FileHash { get; set; }
            public string FileDate { get; set; }
        }

        #region Private Parameters
        private const string UpdateVerbForAntd = "update.antd";
        private const string UpdateVerbForAntdsh = "update.antdsh";
        private const string UpdateVerbForSystem = "update.system";
        private const string UpdateVerbForKernel = "update.kernel";
        private const string UpdateVerbForUnits = "update.units";
        private const string UnitsTargetApp = "/mnt/cdrom/Units/antd.target.wants";
        private const string UnitsTargetKpl = "/mnt/cdrom/Units/kernelpkgload.target.wants";
        private static string _publicRepositoryUrl = "http://srv.anthilla.com/";
        private const string RepositoryFileNameZip = "repo.txt.xz";
        private static string AppsDirectory => "/mnt/cdrom/Apps";
        private static string TmpDirectory => $"{Parameter.RepoTemp}/update";
        private static string AntdDirectory => $"{AppsDirectory}/Anthilla_Antd";
        private static string AntdActive => $"{AntdDirectory}/active-version";
        private static string AntdshDirectory => "/mnt/cdrom/Apps/Anthilla_antdsh";
        private static string AntdshActive => $"{AntdshDirectory}/active-version";
        private static string SystemDirectory => "/mnt/cdrom/System";
        private static string SystemActive => $"{SystemDirectory}/active-system";
        private static string KernelDirectory => "/mnt/cdrom/Kernel";
        private static string SystemMapActive => $"{KernelDirectory}/active-System.map";
        private static string FirmwareActive => $"{KernelDirectory}/active-firmware";
        private static string InitrdActive => $"{KernelDirectory}/active-initrd";
        private static string KernelActive => $"{KernelDirectory}/active-kernel";
        private static string ModulesActive => $"{KernelDirectory}/active-modules";
        private static string XenActive => $"{KernelDirectory}/active-xen";
        #endregion Parameters

        #region Public Medhod

        public static void Check() {
            var info = GetRepositoryInfo().OrderBy(_ => _.FileContext);
            Console.WriteLine("");
            foreach (var i in info) {
                AntdshLogger.WriteLine($"{i.FileContext}\t{i.FileDate}\t{i.FileName}");
            }
            Console.WriteLine("");
        }

        public static void LaunchUpdateFor(string context) {
            _publicRepositoryUrl = GetRandomServer("http");
            AntdshLogger.WriteLine($"repo = {_publicRepositoryUrl}");
            Terminal.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");
            switch (context) {
                case "check":
                    var info = GetRepositoryInfo().OrderBy(_ => _.FileContext);
                    Console.WriteLine("");
                    foreach (var i in info) {
                        AntdshLogger.WriteLine($"{i.FileContext}\t{i.FileDate}\t{i.FileName}");
                    }
                    Console.WriteLine("");
                    break;
                case "all":
                    UpdateContext(UpdateVerbForAntd, AntdActive, AntdDirectory);
                    UpdateUnits("antd", UnitsTargetApp, "AppAntd.");
                    UpdateContext(UpdateVerbForAntdsh, AntdshActive, AntdshDirectory);
                    UpdateUnits("antdsh", UnitsTargetApp, "AppAntdsh.");
                    UpdateContext(UpdateVerbForSystem, SystemActive, SystemDirectory);
                    UpdateKernel(UpdateVerbForKernel, ModulesActive, KernelDirectory);
                    UpdateUnits("kernel", UnitsTargetKpl, "kpl.");
                    RestartAntd();
                    RestartAntdsh();
                    break;
                case "antd":
                    UpdateContext(UpdateVerbForAntd, AntdActive, AntdDirectory);
                    UpdateUnits("antd", UnitsTargetApp, "AppAntd.");
                    RestartAntd();
                    break;
                case "antdsh":
                    UpdateContext(UpdateVerbForAntdsh, AntdshActive, AntdshDirectory);
                    UpdateUnits("antdsh", UnitsTargetApp, "AppAntdsh.");
                    RestartAntdsh();
                    break;
                case "system":
                    UpdateContext(UpdateVerbForSystem, SystemActive, SystemDirectory);
                    break;
                case "kernel":
                    UpdateKernel(UpdateVerbForKernel, ModulesActive, KernelDirectory);
                    UpdateUnits("kernel", UnitsTargetKpl, "kpl.");
                    break;
                default:
                    Console.WriteLine("Nothing to update...");
                    break;
            }
            Terminal.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");
            AntdshLogger.WriteLine("");
        }
        #endregion

        #region Private Methods
        private static int _updateCounter;
        private static void UpdateContext(string currentContext, string activeVersionPath, string contextDestinationDirectory) {
            while (true) {
                _updateCounter++;
                if (_updateCounter > 5) {
                    AntdshLogger.WriteLine($"{currentContext} update failed, too many retries");
                    _updateCounter = 0;
                    return;
                }
                Directory.CreateDirectory(Parameter.RepoTemp);
                Directory.CreateDirectory(TmpDirectory);
                var linkedFile = Terminal.Execute($"file {activeVersionPath}");
                var currentVersionDate = GetVersionDateFromFile(linkedFile);
                var repositoryInfo = GetRepositoryInfo().ToList();
                var currentContextRepositoryInfo = repositoryInfo.Where(_ => _.FileContext == currentContext).OrderByDescending(_ => _.FileDate);
                AntdshLogger.WriteLine($"found for: {currentContext}");
                foreach (var cri in currentContextRepositoryInfo) {
                    AntdshLogger.WriteLine($"   -> {cri.FileName} > {cri.FileDate}");
                }
                AntdshLogger.WriteLine("");
                var latestFileInfo = currentContextRepositoryInfo.FirstOrDefault();
                if (latestFileInfo == null) {
                    AntdshLogger.WriteLine($"cannot retrieve a more recent version of {currentContext}.");
                    _updateCounter = 0;
                    return;
                }
                var isUpdateNeeded = IsUpdateNeeded(currentVersionDate, latestFileInfo.FileDate);
                if (!isUpdateNeeded) {
                    AntdshLogger.WriteLine($"current version of {currentContext} is already up to date.");
                    _updateCounter = 0;
                    return;
                }
                AntdshLogger.WriteLine($"updating {currentContext}");

                var isDownloadValid = DownloadLatestFile(latestFileInfo);
                if (isDownloadValid) {
                    AntdshLogger.WriteLine($"{latestFileInfo.FileName} download complete");
                    var latestTmpFilePath = $"{TmpDirectory}/{latestFileInfo.FileName}";
                    var newVersionPath = $"{contextDestinationDirectory}/{latestFileInfo.FileName}";
                    InstallDownloadedFile(latestTmpFilePath, newVersionPath, activeVersionPath);
                    _updateCounter = 0;
                    return;
                }
                File.Delete($"{TmpDirectory}/{latestFileInfo.FileName}");
                AntdshLogger.WriteLine($"{latestFileInfo.FileName}: downloaded file is not valid");
            }
        }

        private static bool _updateRetry;
        private static void UpdateKernel(string currentContext, string activeVersionPath, string contextDestinationDirectory) {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _updateCounter++;
            if (_updateCounter > 5) {
                AntdshLogger.WriteLine($"{currentContext} update failed, too many retries");
                _updateCounter = 0;
                _updateRetry = false;
                return;
            }
            var currentVersionDate = GetVersionDateFromFile(activeVersionPath);
            var repositoryInfo = GetRepositoryInfo();
            var currentContextRepositoryInfo = repositoryInfo.Where(_ => _.FileContext == currentContext).OrderByDescending(_ => _.FileDate);
            var latestFileInfo = currentContextRepositoryInfo.LastOrDefault();
            var isUpdateNeeded = IsUpdateNeeded(currentVersionDate, latestFileInfo?.FileDate);
            if (!isUpdateNeeded) {
                AntdshLogger.WriteLine($"current version of {currentContext} is already up to date.");
                _updateCounter = 0;
                _updateRetry = false;
                return;
            }
            AntdshLogger.WriteLine($"updating {currentContext}");

            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "system.map", SystemMapActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            _updateRetry = false;
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "lib64_firmware", FirmwareActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            _updateRetry = false;
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "initramfs", InitrdActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            _updateRetry = false;
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "kernel", KernelActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            _updateRetry = false;
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "lib64_modules", ModulesActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            _updateRetry = false;
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "xen", XenActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            _updateRetry = false;
            _updateCounter = 0;
        }

        private static void UpdateUnits(string currentContext, string unitsTargetDir, string filter) {
            AntdshLogger.WriteLine("");
            AntdshLogger.WriteLine($"Updating units for {currentContext}");

            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            var tmpMountDirectory = $"{TmpDirectory}/{currentContext}";
            Directory.CreateDirectory(tmpMountDirectory);
            Terminal.Execute($"umount {tmpMountDirectory}");
            var repositoryInfo = GetRepositoryInfo().ToList();
            var latestFileInfo = repositoryInfo.FirstOrDefault(_ => _.FileContext == UpdateVerbForUnits && _.FileName.Contains(filter));
            DownloadLatestFile(latestFileInfo);
            if (latestFileInfo == null) return;

            AntdshLogger.WriteLine($"{latestFileInfo.FileName} download complete");
            var latestTmpFilePath = $"{TmpDirectory}/{latestFileInfo.FileName}";
            Terminal.Execute($"mount {latestTmpFilePath} {tmpMountDirectory}");
            var downloadedUnits = Directory.EnumerateFiles(tmpMountDirectory).ToList();
            foreach (var downloadedUnit in downloadedUnits) {
                var fullPath = Path.GetFullPath(downloadedUnit);
                AntdshLogger.WriteLine($"copy {fullPath} to {unitsTargetDir}/{Path.GetFileName(fullPath)}");
                File.Copy(fullPath, $"{unitsTargetDir}/{Path.GetFileName(fullPath)}", true);
            }
            Terminal.Execute("systemctl daemon-reload");
            AntdshLogger.WriteLine($"{currentContext} units installation complete");
            Terminal.Execute($"umount {tmpMountDirectory}");
        }

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        private static IEnumerable<FileInfoModel> GetRepositoryInfo() {
            new ApiConsumer().GetFile($"{_publicRepositoryUrl}/{RepositoryFileNameZip}", $"{TmpDirectory}/{RepositoryFileNameZip}");
            if (!File.Exists($"{TmpDirectory}/{RepositoryFileNameZip}")) {
                return new List<FileInfoModel>();
            }
            var tmpRepoListText = Terminal.Execute($"xzcat {TmpDirectory}/{RepositoryFileNameZip}");
            var list = tmpRepoListText.SplitToList(Environment.NewLine);
            var files = new List<FileInfoModel>();
            foreach (var f in list) {
                var fileInfo = f.Split(new[] { ' ' }, 4);
                if (fileInfo.Length <= 3) continue;
                var fi = new FileInfoModel {
                    FileHash = fileInfo[0],
                    FileContext = fileInfo[1],
                    FileDate = fileInfo[2],
                    FileName = fileInfo[3]
                };
                var date = GetVersionDateFromFile(fi.FileName);
                if (date != "00000000") {
                    fi.FileDate = date;
                }
                files.Add(fi);
            }
            return files;
        }

        private static bool IsUpdateNeeded(string currentVersion, string lastVersionFound) {
            var current = Convert.ToInt32(currentVersion);
            var latest = Convert.ToInt32(lastVersionFound);
            return latest > current;
        }

        private static bool DownloadLatestFile(FileInfoModel latestFileInfo) {
            var latestFileDownloadUrl = $"{_publicRepositoryUrl}/{latestFileInfo.FileName}";
            AntdshLogger.WriteLine($"downloading file from {latestFileDownloadUrl}");
            var latestFile = $"{TmpDirectory}/{latestFileInfo.FileName}";
            if (File.Exists(latestFile)) {
                File.Delete(latestFile);
            }
            new ApiConsumer().GetFile(latestFileDownloadUrl, latestFile);
            AntdshLogger.WriteLine($"repository file hash: {latestFileInfo.FileHash}");
            AntdshLogger.WriteLine($"downloaded file hash: {GetFileHash(latestFile)}");
            return latestFileInfo.FileHash == GetFileHash(latestFile);
        }

        private static string GetFileHash(string filePath) {
            using (var fileStreamToRead = File.OpenRead(filePath)) {
                return BitConverter.ToString(new SHA1Managed().ComputeHash(fileStreamToRead)).Replace("-", string.Empty);
            }
        }

        private static void InstallDownloadedFile(string latestTmpFilePath, string newVersionPath, string activeVersionPath) {
            File.Copy(latestTmpFilePath, newVersionPath, true);
            File.Delete(activeVersionPath);
            Terminal.Execute($"ln -s {Path.GetFileName(newVersionPath)} {activeVersionPath}");
            Terminal.Execute($"chown root:wheel {newVersionPath}");
            Terminal.Execute($"chmod 775 {newVersionPath}");
        }

        private static bool DownloadAndInstallSingleFile(IEnumerable<FileInfoModel> repositoryInfo, string query, string activePath, string contextDestinationDirectory) {
            var latestFileInfo = repositoryInfo.FirstOrDefault(_ => _.FileName.ToLower().Contains(query));
            var isDownloadValid = DownloadLatestFile(latestFileInfo);
            if (isDownloadValid == false) {
                AntdshLogger.WriteLine($"{latestFileInfo?.FileName}: downloaded file is not valid");
                _updateRetry = true;
                return false;
            }
            AntdshLogger.WriteLine($"{latestFileInfo?.FileName} download complete");
            var latestFileTmpFilePath = $"{TmpDirectory}/{latestFileInfo?.FileName}";
            var newFileVersionPath = $"{contextDestinationDirectory}/{latestFileInfo?.FileName}";
            InstallDownloadedFile(latestFileTmpFilePath, newFileVersionPath, activePath);
            return true;
        }

        private static IEnumerable<string> GetServerList(string filter = "") {
            var text = new ApiConsumer().GetString("http://srv.anthilla.com/server.txt");
            var list = text.SplitToList("\n");
            if (!string.IsNullOrEmpty(filter)) {
                list = list.Where(_ => _.StartsWith(filter)).ToList();
            }
            return list;
        }

        private static string GetRandomServer(string filter = "") {
            return _publicRepositoryUrl;
            var arr = GetServerList(filter).ToArray();
            var rnd = new Random().Next(0, arr.Length);
            return arr[rnd];
        }
        #endregion

        private static void RestartAntd() {
            Terminal.Execute("systemctl daemon-reload");
            Terminal.Execute("systemctl stop app-antd-03-launcher.service");
            Terminal.Execute("systemctl stop framework-antd.mount");
            Terminal.Execute("systemctl restart app-antd-02-mount.service");
            Terminal.Execute("systemctl restart app-antd-03-launcher.service");
        }

        private static void RestartAntdsh() {
            Terminal.Execute("systemctl daemon-reload");
            Terminal.Execute("systemd-run --on-active=4 /usr/bin/systemctl stop framework-antdsh.mount");
            Terminal.Execute("systemd-run --on-active=5 /usr/bin/systemctl restart app-antdsh-02-mount.service");
            Environment.Exit(1);
        }
    }
}
