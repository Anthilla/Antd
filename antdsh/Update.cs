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
        private const string UnitsTargetApp = "/mnt/cdrom/Units/applicative.target.wants";
        private const string UnitsTargetKpl = "/mnt/cdrom/Units/kernelpkgload.target.wants";
        private static readonly string[] UnitsAntd = {
                "app-antd-01-prepare.service",
                "app-antd-02-mount.service",
                "app-antd-03-launcher.service",
            };
        private static readonly string[] UnitsAntdsh = {
                "app-antdsh-01-prepare.service",
                "app-antdsh-02-mount.service"
            };
        private static readonly string[] UnitsKernel = {
                "kpl-firmware-mount.service",
                "kpl-modules-mount.",
                "kpl-modules-prepare.service",
                "kpl-restart-modules-load.service"
            };
        private const string PublicRepositoryUrl = "http://srv.anthilla.com";
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
        public static void LaunchUpdateFor(string context) {
            switch (context) {
                case "antd":
                    UpdateContext(UpdateVerbForAntd, AntdActive, AntdDirectory);
                    //UpdateUnits2(UpdateVerbForAntd, AntdActive, AntdDirectory);
                    break;
                case "antdsh":
                    UpdateContext(UpdateVerbForAntdsh, AntdshActive, AntdshDirectory);
                    //UpdateUnits(UnitsAntdsh, UnitsTargetApp);
                    break;
                case "system":
                    UpdateContext(UpdateVerbForSystem, SystemActive, SystemDirectory);
                    break;
                case "kernel":
                    //UpdateKernel(UpdateVerbForKernel, ModulesActive, KernelDirectory);
                    //UpdateUnits(UnitsKernel, UnitsTargetKpl);
                    break;
                default:
                    Console.WriteLine("Nothing to update...");
                    break;
            }
        }
        #endregion

        #region Private Methods
        private static int _updateCounter;
        private static void UpdateContext(string currentContext, string activeVersionPath, string contextDestinationDirectory) {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _updateCounter++;
            if (_updateCounter > 5) {
                AntdshLogger.WriteLine($"{currentContext} update failed, too many retries");
                return;
            }
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
                return;
            }
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
            _updateCounter = 0;
        }

        private static void UpdateUnits2(string currentContext, string activeVersionPath, string contextDestinationDirectory) {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _updateCounter++;
            if (_updateCounter > 5) {
                AntdshLogger.WriteLine($"{currentContext} update failed, too many retries");
                return;
            }
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
                return;
            }
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
            _updateCounter = 0;
        }
        #endregion

        #region More Private Methods
        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        private static IEnumerable<FileInfoModel> GetRepositoryInfo() {
            try {
                AntdshLogger.WriteLine($"Downloading: {PublicRepositoryUrl}/{RepositoryFileNameZip}");
                FileSystem.Download2($"{PublicRepositoryUrl}/{RepositoryFileNameZip}", $"{TmpDirectory}/{RepositoryFileNameZip}");
                var tmpRepoListText = Terminal.Execute($"xzcat {TmpDirectory}/{RepositoryFileNameZip}");
                var list = tmpRepoListText.SplitToList(Environment.NewLine);
                var files = new List<FileInfoModel>();
                foreach (var f in list) {
                    var fileInfo = f.Split(new[] { ' ' }, 3);
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
            catch (Exception ex) {
                AntdshLogger.WriteLine($"error while downloading repository info: {Environment.NewLine}\t{ex.Message}");
                return new List<FileInfoModel>();
            }
        }

        private static bool IsUpdateNeeded(string currentVersion, string lastVersionFound) {
            var current = Convert.ToInt32(currentVersion);
            var latest = Convert.ToInt32(lastVersionFound);
            return latest > current;
        }

        private static bool DownloadLatestFile(FileInfoModel latestFileInfo) {
            var latestFileDownloadUrl = $"{PublicRepositoryUrl}/{latestFileInfo.FileName}";
            AntdshLogger.WriteLine($"downloading file from {latestFileDownloadUrl}");
            var latestFile = $"{TmpDirectory}/{latestFileInfo.FileName}";
            if (File.Exists(latestFile)) {
                File.Delete(latestFile);
            }
            FileSystem.Download2(latestFileDownloadUrl, latestFile);
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
            Terminal.Execute($"ln -s {newVersionPath} {activeVersionPath}");
        }
        #endregion
    }
}
