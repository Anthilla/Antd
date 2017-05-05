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

using antdlib.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace antdsh {
    public class Update2 {

        public class FileInfoModel {
            public string FileName { get; set; }
            public string FileContext { get; set; }
            public string FileHash { get; set; }
            public string FileDate { get; set; }
        }

        #region [    Private Parameters    ]

        private const string UpdateVerbForAntd = "update.antd";
        private const string UpdateVerbForAntdUi = "update.antdui";
        private const string UpdateVerbForAntdsh = "update.antdsh";
        private const string UpdateVerbForAossvc = "update.aosssvc";
        private const string UpdateVerbForSystem = "update.system";
        private const string UpdateVerbForKernel = "update.kernel";
        private const string UpdateVerbForUnits = "update.units";

        private const string UnitsTargetApp = "/mnt/cdrom/Units/antd.target.wants";
        private const string UnitsTargetKpl = "/mnt/cdrom/Units/kernelpkgload.target.wants";

        private static string _officialRepo = "https://srv.anthilla.com/repo/";
        private static string _publicRepositoryUrlHttps = _officialRepo;
        private static string _publicRepositoryUrlHttp = _officialRepo.Replace("https", "http");
        private const string RepositoryFileNameZip = "repo.txt.anth";
        private static string AppsDirectory => "/mnt/cdrom/Apps";
        private static string AppsDirectoryTmp => "/tmp";
        private static string TmpDirectory => $"{AppsDirectoryTmp}/update";

        private static string AntdDirectory => $"{AppsDirectory}/Anthilla_Antd";
        private static string AntdActive => $"{AntdDirectory}/active-version";

        private static string AntdUiDirectory => $"{AppsDirectory}/Anthilla_AntdUi";
        private static string AntdUiActive => $"{AntdUiDirectory}/active-version";

        private static string AntdshDirectory => "/mnt/cdrom/Apps/Anthilla_antdsh";
        private static string AntdshActive => $"{AntdshDirectory}/active-version";

        private static string AossvcDirectory => "/usr/sbin/aossvc.exe";
        private static string AossvcActive => $"{AossvcDirectory}/aossvc.exe";

        private static string SystemDirectory => "/mnt/cdrom/System";
        private static string SystemActive => $"{SystemDirectory}/active-system";
        private static string KernelDirectory => "/mnt/cdrom/Kernel";
        private static string SystemMapActive => $"{KernelDirectory}/active-System.map";
        private static string FirmwareActive => $"{KernelDirectory}/active-firmware";
        private static string InitrdActive => $"{KernelDirectory}/active-initrd";
        private static string KernelActive => $"{KernelDirectory}/active-kernel";
        private static string ModulesActive => $"{KernelDirectory}/active-modules";
        private static string XenActive => $"{KernelDirectory}/active-xen";

        private static readonly Bash Bash = new Bash();

        #endregion Parameters

        #region [    Private Members   ]

        private static string GetVersionDateFromFile(string path) {
            var r = new Regex("(-\\d{8})", RegexOptions.IgnoreCase);
            var m = r.Match(path);
            var vers = m.Success ? m.Groups[0].Value.Replace("-", "") : "00000000";
            return vers;
        }

        private static bool IsUpdateNeeded(string currentVersion, string lastVersionFound) {
            var current = Convert.ToInt32(currentVersion);
            var latest = Convert.ToInt32(lastVersionFound);
            return latest > current;
        }

        private static IEnumerable<string> GetServerList(string filter = "") {
            var text = new ApiConsumer().GetString($"{_officialRepo}server.txt");
            var list = text.SplitToList("\n");
            if(!string.IsNullOrEmpty(filter)) {
                list = list.Where(_ => _.StartsWith(filter)).ToList();
            }
            return list;
        }

        private static int _getServerRetry;

        private static string GetReferenceServer(string filter = "") {
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

        private static void CleanTmp() {
            try {
                if(Directory.Exists(TmpDirectory)) {
                    Directory.Delete(TmpDirectory, true);
                }
                Directory.CreateDirectory(TmpDirectory);
            }
            catch(Exception) {
                //throw;
            }
        }

        public static string GetFileHash(string filePath) {
            using(var fileStreamToRead = File.OpenRead(filePath)) {
                return BitConverter.ToString(new SHA1Managed().ComputeHash(fileStreamToRead)).Replace("-", string.Empty);
            }
        }

        private static bool DownloadLatestFile(FileInfoModel latestFileInfo) {
            var sourceUrl = $"{_publicRepositoryUrlHttps}{latestFileInfo.FileName}";
            Console.WriteLine($"downloading file from {sourceUrl}");
            var destinationPath = $"{TmpDirectory}/{latestFileInfo.FileName}";
            if(File.Exists(destinationPath)) {
                File.Delete(destinationPath);
            }
            var download = new FileDownloader(sourceUrl, destinationPath).DownloadFile();
            if(download != true) {
                Console.WriteLine($"An error occurred downloading {sourceUrl}, download process error");
                return false;
            }
            var downloadedFileHash = GetFileHash(destinationPath);
            Console.WriteLine($"repository file hash: {latestFileInfo.FileHash}");
            Console.WriteLine($"downloaded file hash: {downloadedFileHash}");
            return latestFileInfo.FileHash == downloadedFileHash;
        }

        private static IEnumerable<FileInfoModel> GetRepositoryInfo() {
            var sourceUrl = $"{_publicRepositoryUrlHttps}/{RepositoryFileNameZip}";
            var destinationPath = $"{TmpDirectory}/{RepositoryFileNameZip}";
            var download = new FileDownloader(sourceUrl, destinationPath).DownloadFile();
            if(download != true) {
                Console.WriteLine($"An error occurred downloading {sourceUrl}, download process error");
                return new List<FileInfoModel>();
            }
            if(!File.Exists(destinationPath)) {
                Console.WriteLine($"An error occurred downloading {sourceUrl}, downloaded file not found");
                return new List<FileInfoModel>();
            }
            var repoLines = File.ReadAllLines(destinationPath).Select(StringCompression.DecompressString);
            var files = new List<FileInfoModel>();
            foreach(var f in repoLines) {
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
            return files.OrderBy(_ => _.FileContext).ThenBy(_ => _.FileName);
        }

        private static int _updateCounter;

        private static void UpdateContext(string currentContext, string activeVersionPath, string contextDestinationDirectory, bool forced, string query = "") {
            while(true) {
                _updateCounter++;
                if(_updateCounter > 5) {
                    Console.WriteLine($"{currentContext} update failed, too many retries");
                    _updateCounter = 0;
                    return;
                }
                var linkedFile = Bash.Execute($"file {activeVersionPath}");
                var currentVersionDate = GetVersionDateFromFile(linkedFile);
                var repositoryInfo = GetRepositoryInfo();
                var currentContextRepositoryInfo = repositoryInfo.Where(_ => _.FileContext == currentContext).OrderByDescending(_ => _.FileDate).ToList();

                if(!string.IsNullOrEmpty(query)) {
                    currentContextRepositoryInfo = currentContextRepositoryInfo.Where(_ => _.FileName.ToLower().Contains(query)).ToList();
                }

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
                if(forced == false) {
                    var isUpdateNeeded = IsUpdateNeeded(currentVersionDate, latestFileInfo.FileDate);
                    if(!isUpdateNeeded) {
                        Console.WriteLine($"current version of {currentContext} is already up to date.");
                        _updateCounter = 0;
                        return;
                    }
                }
                Console.WriteLine($"updating {currentContext}");
                if(forced == false) {
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

        private static void UpdateUnits(string currentContext, string unitsTargetDir, string filter) {
            Console.WriteLine("");
            Console.WriteLine($"Updating units for {currentContext}");

            var tmpMountDirectory = $"{TmpDirectory}/{currentContext}";
            Directory.CreateDirectory(tmpMountDirectory);
            Bash.Execute($"umount {tmpMountDirectory}");
            var repositoryInfo = GetRepositoryInfo().ToList();
            var latestFileInfo = repositoryInfo.FirstOrDefault(_ => _.FileContext == UpdateVerbForUnits && _.FileName.ToLower().Contains(filter.ToLower()));
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

        private static void InstallDownloadedFile(string latestTmpFilePath, string newVersionPath, string activeVersionPath) {
            File.Copy(latestTmpFilePath, newVersionPath, true);
            File.Delete(activeVersionPath);
            Bash.Execute($"ln -s {Path.GetFileName(newVersionPath)} {activeVersionPath}");
            Bash.Execute($"chown root:wheel {newVersionPath}");
            Bash.Execute($"chmod 775 {newVersionPath}");
        }

        private static readonly Target Target = new Target();
        private static readonly Units Units = new Units();

        private static void RestartAntdUi() {
            Bash.Execute("systemctl daemon-reload");
            Bash.Execute("systemctl stop app-antdui-03-launcher.service");
            Bash.Execute("systemctl stop framework-antdui.mount");
            Bash.Execute("systemctl restart app-antdui-02-mount.service");
            Bash.Execute("systemctl restart app-antdui-03-launcher.service");
        }

        private static void RestartAntd() {
            Bash.Execute("systemctl daemon-reload");
            Bash.Execute("systemctl stop app-antd-03-launcher.service");
            Bash.Execute("systemctl stop framework-antd.mount");
            Bash.Execute("systemctl restart app-antd-02-mount.service");
            Bash.Execute("systemctl restart app-antd-03-launcher.service");
        }

        private static void RestartAntdsh() {
            if(Parameter.IsUnix) {
                Target.Setup();
                Units.CreateRemountUnits();
                Bash.Execute("systemctl restart tt-antdsh-01-remount.timer");
                Environment.Exit(0);
            }
        }
        #endregion

        #region [    Public Medhods    ]
        public static void Check() {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            var info = GetRepositoryInfo();
            foreach(var i in info) {
                Console.WriteLine($"{i.FileContext}\t{i.FileDate}\t{i.FileName}");
            }
            CleanTmp();
            Console.WriteLine();
        }

        public static void Antd(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            Directory.CreateDirectory(AntdDirectory);
            if(unitsOnly == false) {
                UpdateContext(UpdateVerbForAntd, AntdActive, AntdDirectory, forced);
            }
            UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "-AppAntd.");
            RestartAntd();
            CleanTmp();
            Console.WriteLine();
        }

        public static void AntdUi(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            Directory.CreateDirectory(AntdUiDirectory);
            if(unitsOnly == false) {
                UpdateContext(UpdateVerbForAntdUi, AntdUiActive, AntdUiDirectory, forced);
            }
            UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "-AppAntdUi.");
            RestartAntdUi();
            CleanTmp();
            Console.WriteLine();
        }

        public static void Antdsh(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            if(unitsOnly) {
                UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "AppAntdsh.");
                return;
            }
            UpdateContext(UpdateVerbForAntdsh, AntdshActive, AntdshDirectory, forced);
            UpdateUnits(UpdateVerbForUnits, UnitsTargetApp, "AppAntdsh.");
            RestartAntdsh();
            CleanTmp();
            Console.WriteLine();
        }

        public static void Aossvc(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            var sourceUrl = $"{_publicRepositoryUrlHttps}/aossvc.exe";
            const string destinationPath = "/usr/sbin/aossvc.exe";
            var download = new FileDownloader(sourceUrl, destinationPath).DownloadFile();
            if(download != true) {
                Console.WriteLine($"An error occurred downloading {sourceUrl}, download process error");
                return;
            }
            var svcUrl = $"{_publicRepositoryUrlHttps}/aossvc.service";
            const string svcBin = "/usr/lib/systemd/system/aossvc.service";
            var download2 = new FileDownloader(svcUrl, svcBin).DownloadFile();
            if(download2 != true) {
                Console.WriteLine($"An error occurred downloading {svcUrl}, download process error");
                return;
            }
            Bash.Execute("systemctl daemon-reload");
            Bash.Execute("systemctl start aossvc.service");
            CleanTmp();
            Console.WriteLine();
        }

        public static void System(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            UpdateContext(UpdateVerbForSystem, SystemActive, SystemDirectory, forced);
            CleanTmp();
            Console.WriteLine();
        }

        public static void KernelSystemMap(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            UpdateContext(UpdateVerbForKernel, SystemMapActive, KernelDirectory, forced, "system.map");
            CleanTmp();
            Console.WriteLine();
        }

        public static void KernelFirmware(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            UpdateContext(UpdateVerbForKernel, FirmwareActive, KernelDirectory, forced, "lib64_firmware");
            CleanTmp();
            Console.WriteLine();
        }

        public static void KernelInitrd(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            UpdateContext(UpdateVerbForKernel, InitrdActive, KernelDirectory, forced, "initramfs");
            CleanTmp();
            Console.WriteLine();
        }

        public static void KernelKernel(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            UpdateContext(UpdateVerbForKernel, KernelActive, KernelDirectory, forced, "kernel");
            CleanTmp();
            Console.WriteLine();
        }

        public static void KernelModules(bool forced, bool unitsOnly) {
            Console.WriteLine();
            _publicRepositoryUrlHttps = GetReferenceServer("https");
            Console.WriteLine($"repo = {_publicRepositoryUrlHttps}");
            CleanTmp();
            UpdateContext(UpdateVerbForKernel, ModulesActive, KernelDirectory, forced, "lib64_modules");
            CleanTmp();
            Console.WriteLine();
        }

        #endregion
    }
}
