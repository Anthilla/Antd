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
        private static string _publicRepositoryUrlHttps = "http://srv.anthilla.com/";
        //private static string _publicRepositoryUrlHttp = "http://srv.anthilla.com/";
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

        public void Check() {
            _publicRepositoryUrlHttps = GetRandomServer("http");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            var info = GetRepositoryInfo().OrderBy(_ => _.FileContext);
            Console.WriteLine("");
            foreach(var i in info) {
                Console.WriteLine($"{i.FileContext}\t{i.FileDate}\t{i.FileName}");
            }
            Console.WriteLine("");
        }

        public void All(bool forced) {
            _publicRepositoryUrlHttps = GetRandomServer("http");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");

            UpdateContext(UpdateVerbForAntd, AntdActive, AntdDirectory, forced);
            UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "AppAntd.");
            UpdateContext(UpdateVerbForAntdsh, AntdshActive, AntdshDirectory, forced);
            UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "AppAntdsh.");
            UpdateContext(UpdateVerbForSystem, SystemActive, SystemDirectory, forced);
            UpdateKernel(UpdateVerbForKernel, ModulesActive, KernelDirectory, forced);
            UpdateUnits(UpdateVerbForUnits, UnitsTargetKpl, "kpl.");
            RestartAntd();
            RestartAntdsh();

            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");
        }

        public void Antd(bool forced) {
            _publicRepositoryUrlHttps = GetRandomServer("http");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");

            UpdateContext(UpdateVerbForAntd, AntdActive, AntdDirectory, forced);
            UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "AppAntd.");
            RestartAntd();

            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");
        }

        public void Antdsh(bool forced) {
            _publicRepositoryUrlHttps = GetRandomServer("http");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");

            UpdateContext(UpdateVerbForAntdsh, AntdshActive, AntdshDirectory, forced);
            UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "AppAntdsh.");
            RestartAntdsh();

            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");
        }

        public void System(bool forced) {
            _publicRepositoryUrlHttps = GetRandomServer("http");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");

            UpdateContext(UpdateVerbForSystem, SystemActive, SystemDirectory, forced);

            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");
        }

        private static readonly Bash Bash = new Bash();

        public void Kernel(bool forced) {
            _publicRepositoryUrlHttps = GetRandomServer("http");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");

            UpdateKernel(UpdateVerbForKernel, ModulesActive, KernelDirectory, forced);
            UpdateUnits(UpdateVerbForUnits, UnitsTargetKpl, "kpl.");

            Bash.Execute($"rm -fR {TmpDirectory}; mkdir -p {TmpDirectory}");
        }

        #endregion

        #region Private Methods

        private static int _updateCounter;

        public void UpdateContext(string currentContext, string activeVersionPath, string contextDestinationDirectory,
            bool force = false) {
            while(true) {
                _updateCounter++;
                if(_updateCounter > 5) {
                    Console.WriteLine($"{currentContext} update failed, too many retries");
                    _updateCounter = 0;
                    return;
                }
                Directory.CreateDirectory(Parameter.RepoTemp);
                Directory.CreateDirectory(TmpDirectory);
                var linkedFile = Bash.Execute($"file {activeVersionPath}");
                var currentVersionDate = GetVersionDateFromFile(linkedFile);
                var repositoryInfo = GetRepositoryInfo().ToList();
                var currentContextRepositoryInfo =
                    repositoryInfo.Where(_ => _.FileContext == currentContext).OrderByDescending(_ => _.FileDate);
                Console.WriteLine($"found for: {currentContext}");
                foreach(var cri in currentContextRepositoryInfo) {
                    Console.WriteLine($"   -> {cri.FileName} > {cri.FileDate}");
                }
                Console.WriteLine("");
                var latestFileInfo = currentContextRepositoryInfo.FirstOrDefault();
                if(latestFileInfo == null) {
                    Console.WriteLine($"cannot retrieve a more recent version of {currentContext}.");
                    _updateCounter = 0;
                    return;
                }
                if(force == false) {
                    var isUpdateNeeded = IsUpdateNeeded(currentVersionDate, latestFileInfo.FileDate);
                    if(!isUpdateNeeded) {
                        Console.WriteLine($"current version of {currentContext} is already up to date.");
                        _updateCounter = 0;
                        return;
                    }
                }
                Console.WriteLine($"updating {currentContext}");

                if(force == false) {
                    var isDownloadValid = DownloadLatestFile(latestFileInfo);
                    if(isDownloadValid) {
                        Console.WriteLine($"{latestFileInfo.FileName} download complete");
                        var latestTmpFilePath = $"{TmpDirectory}/{latestFileInfo.FileName}";
                        var newVersionPath = $"{contextDestinationDirectory}/{latestFileInfo.FileName}";
                        InstallDownloadedFile(latestTmpFilePath, newVersionPath, activeVersionPath);
                        _updateCounter = 0;
                        return;
                    }
                }
                else {
                    DownloadLatestFile(latestFileInfo);
                    Console.WriteLine($"{latestFileInfo.FileName} download complete");
                    var latestTmpFilePath = $"{TmpDirectory}/{latestFileInfo.FileName}";
                    var newVersionPath = $"{contextDestinationDirectory}/{latestFileInfo.FileName}";
                    InstallDownloadedFile(latestTmpFilePath, newVersionPath, activeVersionPath);
                    _updateCounter = 0;
                    return;
                }
                File.Delete($"{TmpDirectory}/{latestFileInfo.FileName}");
                Console.WriteLine($"{latestFileInfo.FileName}: downloaded file is not valid");
            }
        }

        private static bool _updateRetry;

        public void UpdateKernel(string currentContext, string activeVersionPath, string contextDestinationDirectory,
            bool force = false) {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _updateCounter++;
            if(_updateCounter > 5) {
                Console.WriteLine($"{currentContext} update failed, too many retries");
                _updateCounter = 0;
                _updateRetry = false;
                return;
            }
            var currentVersionDate = GetVersionDateFromFile(activeVersionPath);
            var repositoryInfo = GetRepositoryInfo();
            var currentContextRepositoryInfo =
                repositoryInfo.Where(_ => _.FileContext == currentContext).OrderByDescending(_ => _.FileDate);
            var latestFileInfo = currentContextRepositoryInfo.LastOrDefault();
            var isUpdateNeeded = IsUpdateNeeded(currentVersionDate, latestFileInfo?.FileDate);
            if(force == false) {
                if(!isUpdateNeeded) {
                    Console.WriteLine($"current version of {currentContext} is already up to date.");
                    _updateCounter = 0;
                    _updateRetry = false;
                    return;
                }
            }
            Console.WriteLine($"updating {currentContext}");

            if(force) {
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            else if(
                DownloadAndInstallSingleFile(currentContextRepositoryInfo, "system.map", SystemMapActive,
                    contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }

            _updateRetry = false;

            if(force) {
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            else if(
                DownloadAndInstallSingleFile(currentContextRepositoryInfo, "lib64_firmware", FirmwareActive,
                    contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }

            _updateRetry = false;

            if(force) {
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            else if(
                DownloadAndInstallSingleFile(currentContextRepositoryInfo, "initramfs", InitrdActive,
                    contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }

            _updateRetry = false;

            if(force) {
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            else if(
                DownloadAndInstallSingleFile(currentContextRepositoryInfo, "kernel", KernelActive,
                    contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }

            _updateRetry = false;

            if(force) {
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            else if(
                DownloadAndInstallSingleFile(currentContextRepositoryInfo, "lib64_modules", ModulesActive,
                    contextDestinationDirectory) == false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }

            _updateRetry = false;

            if(force) {
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }
            else if(
                DownloadAndInstallSingleFile(currentContextRepositoryInfo, "xen", XenActive, contextDestinationDirectory) ==
                false && _updateRetry == false) {
                _updateRetry = true;
                UpdateKernel(currentContext, activeVersionPath, contextDestinationDirectory);
            }

            _updateRetry = false;
            _updateCounter = 0;
        }

        public void UpdateUnits(string currentContext, string unitsTargetDir, string filter) {
            Console.WriteLine("");
            Console.WriteLine($"Updating units for {currentContext}");

            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            var tmpMountDirectory = $"{TmpDirectory}/{currentContext}";
            Directory.CreateDirectory(tmpMountDirectory);
            Bash.Execute($"umount {tmpMountDirectory}");
            var repositoryInfo = GetRepositoryInfo().ToList();
            var latestFileInfo =
                repositoryInfo.FirstOrDefault(_ => _.FileContext == UpdateVerbForUnits && _.FileName.Contains(filter));
            if(latestFileInfo == null)
                return;
            DownloadLatestFile(latestFileInfo);

            Console.WriteLine($"{latestFileInfo.FileName} download complete");
            var latestTmpFilePath = $"{TmpDirectory}/{latestFileInfo.FileName}";
            Bash.Execute($"mount {latestTmpFilePath} {tmpMountDirectory}");
            var downloadedUnits = Directory.EnumerateFiles(tmpMountDirectory).ToList();
            foreach(var downloadedUnit in downloadedUnits) {
                var fullPath = Path.GetFullPath(downloadedUnit);
                Console.WriteLine($"copy {fullPath} to {unitsTargetDir}/{Path.GetFileName(fullPath)}");
                File.Copy(fullPath, $"{unitsTargetDir}/{Path.GetFileName(fullPath)}", true);
            }
            Bash.Execute("systemctl daemon-reload");
            Console.WriteLine($"{currentContext} units installation complete");
            Bash.Execute($"umount {tmpMountDirectory}");
        }

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        private static IEnumerable<FileInfoModel> GetRepositoryInfo() {
            new ApiConsumer().GetFile($"{_publicRepositoryUrlHttps}/{RepositoryFileNameZip}",
                $"{TmpDirectory}/{RepositoryFileNameZip}");
            if(!File.Exists($"{TmpDirectory}/{RepositoryFileNameZip}")) {
                return new List<FileInfoModel>();
            }
            var tmpRepoListText = Bash.Execute($"xzcat {TmpDirectory}/{RepositoryFileNameZip}");
            var list = tmpRepoListText.SplitToList(Environment.NewLine);
            var files = new List<FileInfoModel>();
            foreach(var f in list) {
                var fileInfo = f.Split(new[] { ' ' }, 4);
                if(fileInfo.Length <= 3)
                    continue;
                var fi = new FileInfoModel {
                    FileHash = fileInfo[0],
                    FileContext = fileInfo[1],
                    FileDate = fileInfo[2],
                    FileName = fileInfo[3]
                };
                var date = GetVersionDateFromFile(fi.FileName);
                if(date != "00000000") {
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
            var latestFileDownloadUrl = $"{_publicRepositoryUrlHttps}/{latestFileInfo.FileName}";
            Console.WriteLine($"downloading file from {latestFileDownloadUrl}");
            var latestFile = $"{TmpDirectory}/{latestFileInfo.FileName}";
            if(File.Exists(latestFile)) {
                File.Delete(latestFile);
            }
            new ApiConsumer().GetFile(latestFileDownloadUrl, latestFile);
            Console.WriteLine($"repository file hash: {latestFileInfo.FileHash}");
            Console.WriteLine($"downloaded file hash: {GetFileHash(latestFile)}");
            return latestFileInfo.FileHash == GetFileHash(latestFile);
        }

        private static string GetFileHash(string filePath) {
            using(var fileStreamToRead = File.OpenRead(filePath)) {
                return BitConverter.ToString(new SHA1Managed().ComputeHash(fileStreamToRead)).Replace("-", string.Empty);
            }
        }

        private static void InstallDownloadedFile(string latestTmpFilePath, string newVersionPath,
            string activeVersionPath) {
            File.Copy(latestTmpFilePath, newVersionPath, true);
            File.Delete(activeVersionPath);
            Bash.Execute($"ln -s {Path.GetFileName(newVersionPath)} {activeVersionPath}");
            Bash.Execute($"chown root:wheel {newVersionPath}");
            Bash.Execute($"chmod 775 {newVersionPath}");
        }

        private static bool DownloadAndInstallSingleFile(IEnumerable<FileInfoModel> repositoryInfo, string query,
            string activePath, string contextDestinationDirectory) {
            var latestFileInfo = repositoryInfo.FirstOrDefault(_ => _.FileName.ToLower().Contains(query));
            var isDownloadValid = DownloadLatestFile(latestFileInfo);
            if(isDownloadValid == false) {
                Console.WriteLine($"{latestFileInfo?.FileName}: downloaded file is not valid");
                _updateRetry = true;
                return false;
            }
            Console.WriteLine($"{latestFileInfo?.FileName} download complete");
            var latestFileTmpFilePath = $"{TmpDirectory}/{latestFileInfo?.FileName}";
            var newFileVersionPath = $"{contextDestinationDirectory}/{latestFileInfo?.FileName}";
            InstallDownloadedFile(latestFileTmpFilePath, newFileVersionPath, activePath);
            return true;
        }

        private static IEnumerable<string> GetServerList(string filter = "") {
            var text = new ApiConsumer().GetString("http://srv.anthilla.com/server.txt");
            var list = text.SplitToList("\n");
            if(!string.IsNullOrEmpty(filter)) {
                list = list.Where(_ => _.StartsWith(filter)).ToList();
            }
            return list;
        }

        private static int _getServerRetry;

        private static string GetRandomServer(string filter = "") {
            try {
                var server = _publicRepositoryUrlHttps;
                while(_getServerRetry < 5) {
                    var arr = GetServerList(filter).ToArray();
                    var rnd = new Random().Next(0, arr.Length);
                    server = arr[rnd];
                    _getServerRetry++;
                }
                return server;
            }
            catch(Exception) {
                return _publicRepositoryUrlHttps;
            }
        }
        #endregion

        private static void RestartAntd() {
            Bash.Execute("systemctl daemon-reload");
            Bash.Execute("systemctl stop app-antd-03-launcher.service");
            Bash.Execute("systemctl stop framework-antd.mount");
            Bash.Execute("systemctl restart app-antd-02-mount.service");
            Bash.Execute("systemctl restart app-antd-03-launcher.service");
        }

        private static readonly Target Target = new Target();
        private static readonly Units Units = new Units();

        private static void RestartAntdsh() {
            Target.Setup();
            Units.CreateRemountUnits();
            Bash.Execute("systemctl restart tt-antdsh-01-remount.timer");
            Environment.Exit(0);
        }
    }
}
