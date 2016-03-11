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
using antdlib.Common;
using System.Security.Cryptography;

namespace antdlib.Antdsh {
    public class UpdateObject {

        public static class Constants {
            public const string UPDATE_VERB = "update.";
            public const string UPDATE_VERB_FOR_ANTD = "update.antd";
            public const string UPDATE_VERB_FOR_ANTDSH = "update.antdsh";
            public const string UPDATE_VERB_FOR_SYSTEM = "update.system";
            public const string UPDATE_VERB_FOR_KERNEL = "update.kernel";
            public const string UNITS_TARGET_APP = "/mnt/cdrom/Units/applicative.target.wants";
            public const string UNITS_TARGET_KPL = "/mnt/cdrom/Units/kernelpkgload.target.wants";
            public static string[] UNITS_ANTD = new[] {
                "app-antd-01-prepare.service",
                "app-antd-02-mount.service",
                "app-antd-03-launcher.service",
            };
            public static string[] UNITS_ANTDSH = new[] {
                "app-antdsh-01-prepare.service",
                "app-antdsh-02-mount.service"
            };
            public static string[] UNITS_KERNEL = new[] {
                "kpl-firmware-mount.service",
                "kpl-modules-mount.",
                "kpl-modules-prepare.service",
                "kpl-restart-modules-load.service"
            };
        }

        public class FileInfoModel {
            public string FileName { get; set; }
            public string FileContext { get; set; }
            public string FileHash { get; set; }
            public string FileDate { get; set; }
        }

        #region Parameters
        private const string PublicRepositoryUrl = "http://srv.anthilla.com";
        private const string RepositoryFileName = "repo.txt";
        private const string RepositoryFileNameZip = "repo.txt.bz2";
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

        public static void Update(string context) {
            switch (context) {
                case "antd":
                    UpdateContext(Constants.UPDATE_VERB_FOR_ANTD, AntdActive, AntdDirectory);
                    UpdateUnits(Constants.UNITS_ANTD);
                    break;
                case "antdsh":
                    UpdateContext(Constants.UPDATE_VERB_FOR_ANTDSH, AntdshActive, AntdshDirectory);
                    UpdateUnits(Constants.UNITS_ANTDSH);
                    break;
                case "system":
                    UpdateContext(Constants.UPDATE_VERB_FOR_SYSTEM, SystemActive, SystemDirectory);
                    break;
                case "kernel":
                    UpdateKernel(Constants.UPDATE_VERB_FOR_KERNEL, ModulesActive, KernelDirectory);
                    UpdateUnits(Constants.UNITS_KERNEL);
                    break;
                default:
                    Console.WriteLine("Nothing to update...");
                    break;
            }
        }

        private static int _updateCounter;
        private static void UpdateContext(string currentContext, string activeVersionPath, string contextDestinationDirectory) {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _updateCounter++;
            if (_updateCounter > 5) {
                AntdshLogger.WriteLine($"{currentContext} update failed, too many retries");
                return;
            }
            var currentVersionDate = GetVersionDateFromFile(activeVersionPath);
            var repositoryInfo = GetRepositoryInfo();
            var currentContextRepositoryInfo = repositoryInfo.Where(_ => _.FileContext == currentContext).OrderByDescending(_ => _.FileDate);
            var latestFileInfo = currentContextRepositoryInfo.LastOrDefault();
            var isUpdateNeeded = IsUpdateNeeded(currentVersionDate, latestFileInfo.FileDate);
            if (!isUpdateNeeded) {
                AntdshLogger.WriteLine($"current version of {currentContext} is already up to date.");
                return;
            }
            AntdshLogger.WriteLine($"updating {currentContext}");

            if (DownloadLatestFile(latestFileInfo)) {
                AntdshLogger.WriteLine($"{latestFileInfo.FileName}: downloaded file is not valid");
                UpdateContext(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            AntdshLogger.WriteLine($"{latestFileInfo.FileName} download complete");

            var latestTmpFilePath = $"{TmpDirectory}/{latestFileInfo.FileName}";
            var newVersionPath = $"{contextDestinationDirectory}/{latestFileInfo.FileName}";
            InstallDownloadedFile(latestTmpFilePath, newVersionPath, activeVersionPath);
            Directory.Delete(TmpDirectory, true);
        }

        private static bool _updateRetry = false;
        private static void UpdateKernel(string currentContext, string activeVersionPath, string contextDestinationDirectory) {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _updateCounter++;
            if (_updateCounter > 5) {
                AntdshLogger.WriteLine($"{currentContext} update failed, too many retries");
                return;
            }
            var currentVersionDate = GetVersionDateFromFile(activeVersionPath);
            var repositoryInfo = GetRepositoryInfo();
            var currentContextRepositoryInfo = repositoryInfo.Where(_ => _.FileContext == currentContext).OrderByDescending(_ => _.FileDate);
            var latestFileInfo = currentContextRepositoryInfo.LastOrDefault();
            var isUpdateNeeded = IsUpdateNeeded(currentVersionDate, latestFileInfo.FileDate);
            if (!isUpdateNeeded) {
                AntdshLogger.WriteLine($"current version of {currentContext} is already up to date.");
                return;
            }
            AntdshLogger.WriteLine($"updating {currentContext}");

            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "system.map", SystemMapActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateContext(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "lib64_firmware", FirmwareActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateContext(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "initramfs", InitrdActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateContext(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "kernel", KernelActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateContext(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "lib64_modules", ModulesActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateContext(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            if (DownloadAndInstallSingleFile(currentContextRepositoryInfo, "xen", XenActive, contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateContext(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            Directory.Delete(TmpDirectory, true);
        }

        private static void UpdateUnits(IEnumerable<string> units) {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            foreach (var unit in units) {
                var unitDownloadUrl = $"{PublicRepositoryUrl}/{unit}";
                var unitTempPath = $"{TmpDirectory}/{unit}";
                var unitPath = $"{Constants.UNITS_TARGET_APP}/{unit}";
                FileSystem.Download2(unitDownloadUrl, unitTempPath);
                File.Copy(unitTempPath, unitPath, true);
            }
            Directory.Delete(TmpDirectory, true);
        }

        private static bool DownloadAndInstallSingleFile(IEnumerable<FileInfoModel> repositoryInfo, string query, string activePath, string contextDestinationDirectory) {
            var latestFileInfo = repositoryInfo.FirstOrDefault(_ => _.FileName.ToLower().Contains(query));
            if (DownloadLatestFile(latestFileInfo)) {
                AntdshLogger.WriteLine($"{latestFileInfo.FileName}: downloaded file is not valid");
                _updateRetry = true;
                return false;
            }
            AntdshLogger.WriteLine($"{latestFileInfo.FileName} download complete");
            var latestFileTmpFilePath = $"{TmpDirectory}/{latestFileInfo.FileName}";
            var newFileVersionPath = $"{contextDestinationDirectory}/{latestFileInfo.FileName}";
            InstallDownloadedFile(latestFileTmpFilePath, newFileVersionPath, activePath);
            return true;
        }

        private static void InstallDownloadedFile(string latestTmpFilePath, string newVersionPath, string activeVersionPath) {
            File.Copy(latestTmpFilePath, newVersionPath, true);
            File.Delete(activeVersionPath);
            Terminal.Terminal.Execute($"ln -s {newVersionPath} {activeVersionPath}");
        }

        private static bool DownloadLatestFile(FileInfoModel latestFileInfo) {
            var latestFileDownloadUrl = $"{PublicRepositoryUrl}{latestFileInfo.FileName}";
            AntdshLogger.WriteLine($"downloading file from {latestFileDownloadUrl}");
            var latestFile = $"{TmpDirectory}/{latestFileInfo.FileName}";
            if (File.Exists(latestFile)) {
                File.Delete(latestFile);
            }
            FileSystem.Download2(latestFileDownloadUrl, latestFile);
            return latestFileInfo.FileHash == GetFileHash(latestFile);
        }

        private static IEnumerable<FileInfoModel> GetRepositoryInfo() {
            try {
                FileSystem.Download2($"{PublicRepositoryUrl}/{RepositoryFileNameZip}", $"{TmpDirectory}/{RepositoryFileNameZip}");
                Terminal.Terminal.Execute($"bunzip2 -k {TmpDirectory}/{RepositoryFileNameZip}");
                var list = File.ReadAllLines($"{TmpDirectory}/{RepositoryFileName}");
                var files = new List<FileInfoModel>();
                foreach (var f in list) {
                    var s = f.Split(new[] { ' ' }, 3);
                    var fi = new FileInfoModel {
                        FileHash = s[0],
                        FileContext = s[1],
                        FileName = s[2],
                        FileDate = GetVersionDateFromFile(s[2])
                    };
                    files.Add(fi);
                }
                return files;
            }
            catch (Exception ex) {
                AntdshLogger.WriteLine($"error: {ex.Message}");
                return new List<FileInfoModel>();
            }
        }

        private static bool IsUpdateNeeded(string currentVersion, string lastVersionFound) {
            var current = Convert.ToInt32(currentVersion);
            var latest = Convert.ToInt32(lastVersionFound);
            return latest > current;
        }

        private static string GetVersionDateFromFile(string path) {
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentNullException(nameof(path));
            }
            if (!File.Exists(path)) {
                throw new FileNotFoundException($"{Path.GetFileName(path)} not found!");
            }
            try {
                var fileName = Path.GetFileName(path).Trim();
                var from = path.Contains("-aufs-") ?
                    fileName.IndexOf("-aufs-", StringComparison.InvariantCulture) + "-aufs-".Length :
                    fileName.IndexOf("-", StringComparison.InvariantCulture) + "-".Length;
                var to = fileName.LastIndexOf(path.Contains("-x86_64") ?
                    "-x86_6" :
                    ".squashfs.xz", StringComparison.InvariantCulture);
                return fileName.Substring(from, to - from);
            }
            catch (Exception) {
                return "00000000";
            }
        }

        private static string GetFileHash(string filePath) {
            using (var fileStreamToRead = File.OpenRead(filePath)) {
                return BitConverter.ToString(new SHA1Managed().ComputeHash(fileStreamToRead)).Replace("-", string.Empty);
            }
        }
    }
}
