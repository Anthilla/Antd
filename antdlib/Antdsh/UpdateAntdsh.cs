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
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using antdlib.Common;
using antdlib.Log;

namespace antdlib.Antdsh {
    public class UpdateObject {
        public static void Update(string context) {
            if (context == "antd") {
                UpdateAntd();
            }
            if (context == "antdsh") {
                UpdateAntdsh();
            }
            if (context == "system") {
                UpdateSystem();
            }
            if (context == "kernel") {
                UpdateKernel();
            }
        }

        private static string GetVersionDate(string path) {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return "00000000";
            try {
                var fName = Path.GetFileName(path).Trim();
                var from = path.Contains("-aufs-")
                    ? fName.IndexOf("-aufs-", StringComparison.InvariantCulture) + "-aufs-".Length
                    : fName.IndexOf("-", StringComparison.InvariantCulture) + "-".Length;
                var to = fName.LastIndexOf(path.Contains("-x86_64") ? "-x86_6" : ".squashfs.xz", StringComparison.InvariantCulture);
                return fName.Substring(from, to - from);
            }
            catch (Exception) {
                return "00000000";
            }
        }

        private static string GetShaSum(string path) => !File.Exists(path) ? null : Terminal.Terminal.Execute($"sha1sum {path}").Split(' ').FirstOrDefault();

        private static async Task<T> GetResponseFromUrl<T>(string requestUrl) {
            try {
                var awaitResponse = new HttpClient().GetStringAsync(requestUrl);
                return JsonConvert.DeserializeObject<T>(await awaitResponse);
            }
            catch (Exception ex) {
                AntdshLogger.WriteLine($"unable to get response from {requestUrl}: {ex.Message}");
                return default(T);
            }
        }

        #region Parameters
        private const string PublicRepositoryUrl = "http://srv.anthilla.com";
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

        private static int _countAntd;
        private static void UpdateAntd() {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _countAntd++;
            if (_countAntd > 5) {
                AntdshLogger.WriteLine("antd update failed");
                return;
            }
            var currentVersion = Terminal.Terminal.Execute($"file {AntdActive}").Split(' ').Last();
            var date = GetVersionDate(currentVersion);
            var requestUrl = $"{PublicRepositoryUrl}/update/info/antd/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).FirstOrDefault() == "false") {
                AntdshLogger.WriteLine("antd is already up to date");
            }
            AntdshLogger.WriteLine("updating antd");
            var downloadUrlInfo = info.Where(_ => _.Key == "url").Select(_ => _.Value).FirstOrDefault();
            var downloadUrl = $"{PublicRepositoryUrl}{downloadUrlInfo}";
            var filename = downloadUrl.Split('/').Last();
            AntdshLogger.WriteLine($"downloading file from {downloadUrl}");
            var downloadedFile = $"{TmpDirectory}/{filename}";
            AntdshLogger.WriteLine(downloadedFile);
            if (File.Exists(downloadedFile)) {
                File.Delete(downloadedFile);
            }
            //Terminal.Terminal.Execute($"wget {downloadUrl} -O {downloadedFile}");
            FileSystem.Download2(downloadUrl, downloadedFile);
            AntdshLogger.WriteLine("check downloaded file");
            var shasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).FirstOrDefault();
            var currentSha = GetShaSum(downloadedFile);
            if (shasum != currentSha) {
                AntdshLogger.WriteLine($"{filename}: downloaded file is not valid");
                UpdateAntd();
            }
            AntdshLogger.WriteLine($"{filename} download complete");
            var newVersion = $"{AntdDirectory}/{filename}";
            File.Copy(downloadedFile, newVersion, true);
            AntdshLogger.WriteLine("restart antd");
            File.Delete(AntdActive);
            Terminal.Terminal.Execute($"ln -s {newVersion} {AntdActive}");
            Terminal.Terminal.Execute("systemctl stop app-antd-03-launcher");
            Terminal.Terminal.Execute("systemctl stop framework-antd.mount");
            Terminal.Terminal.Execute("systemctl restart app-antd-02-mount");
            Terminal.Terminal.Execute("systemctl restart app-antd-03-launcher");
            Directory.Delete(TmpDirectory, true);
        }

        private static int _countAntdsh;
        private static void UpdateAntdsh() {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _countAntdsh++;
            if (_countAntdsh > 5) {
                AntdshLogger.WriteLine("antdsh update failed");
                return;
            }
            var currentVersion = Terminal.Terminal.Execute($"file {AntdshActive}").Split(' ').Last();
            var date = GetVersionDate(currentVersion);
            var requestUrl = $"{PublicRepositoryUrl}/update/info/antdsh/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).FirstOrDefault() == "false") {
                AntdshLogger.WriteLine("antdsh is already up to date");
            }
            AntdshLogger.WriteLine("updating antdsh");
            var downloadUrlInfo = info.Where(_ => _.Key == "url").Select(_ => _.Value).FirstOrDefault();
            var downloadUrl = $"{PublicRepositoryUrl}{downloadUrlInfo}";
            var filename = downloadUrl.Split('/').Last();
            AntdshLogger.WriteLine($"downloading file from {downloadUrl}");

            var downloadedFile = $"{TmpDirectory}/{filename}";
            AntdshLogger.WriteLine(downloadedFile);
            if (File.Exists(downloadedFile)) {
                File.Delete(downloadedFile);
            }
            //Terminal.Terminal.Execute($"wget {downloadUrl} -O {downloadedFile}");
            FileSystem.Download2(downloadUrl, downloadedFile);
            AntdshLogger.WriteLine("check downloaded file");
            var shasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).FirstOrDefault();
            var currentSha = GetShaSum(downloadedFile);
            if (shasum != currentSha) {
                AntdshLogger.WriteLine($"{filename} downloaded file is not valid");
                UpdateAntdsh();
            }
            AntdshLogger.WriteLine($"{filename} download complete");
            var newVersion = $"{AntdshDirectory}/{filename}";
            File.Copy(downloadedFile, newVersion, true);
            AntdshLogger.WriteLine("restart antdsh");
            File.Delete(AntdshActive);
            Terminal.Terminal.Execute($"ln -s {newVersion} {AntdshActive}");
            Terminal.Terminal.Execute("systemctl stop app-antdsh-03-launcher");
            Terminal.Terminal.Execute("systemctl stop framework-antdsh.mount");
            Directory.Delete(TmpDirectory, true);
        }

        private static int _countSystem;
        private static void UpdateSystem() {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _countSystem++;
            if (_countSystem > 5) {
                AntdshLogger.WriteLine("system update failed");
                return;
            }
            var currentVersion = Terminal.Terminal.Execute($"file {SystemActive}").Split(' ').Last();
            var date = GetVersionDate(currentVersion);
            var requestUrl = $"{PublicRepositoryUrl}/update/info/system/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).FirstOrDefault() == "false") {
                AntdshLogger.WriteLine("System is already up to date");
            }
            AntdshLogger.WriteLine("updating system");
            var downloadUrlInfo = info.Where(_ => _.Key == "url").Select(_ => _.Value).FirstOrDefault();
            var downloadUrl = $"{PublicRepositoryUrl}{downloadUrlInfo}";
            var filename = downloadUrl.Split('/').Last();
            AntdshLogger.WriteLine($"downloading file from {downloadUrl}");

            var downloadedFile = $"{TmpDirectory}/{filename}";
            AntdshLogger.WriteLine(downloadedFile);
            if (File.Exists(downloadedFile)) {
                File.Delete(downloadedFile);
            }
            //Terminal.Terminal.Execute($"wget {downloadUrl} -O {downloadedFile}");
            FileSystem.Download2(downloadUrl, downloadedFile);
            AntdshLogger.WriteLine("check downloaded file");
            var shasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).FirstOrDefault();
            var currentSha = GetShaSum(downloadedFile);
            if (shasum != currentSha) {
                AntdshLogger.WriteLine($"{filename} downloaded file is not valid");
                UpdateSystem();
            }
            AntdshLogger.WriteLine($"{filename} download complete");
            var newVersion = $"{SystemDirectory}/{filename}";
            File.Copy(downloadedFile, newVersion, true);
            AntdshLogger.WriteLine("restart system");
            File.Delete(SystemActive);
            Terminal.Terminal.Execute($"ln -s {newVersion} {SystemActive}");
            Directory.Delete(TmpDirectory, true);
        }

        private static int _countKernel;
        private static bool firmwareDone = false;
        private static bool modulesDone = false;
        private static bool sysmapDone = false;
        private static bool initramfsDone = false;
        private static bool kernelDone = false;
        private static bool xenDone = false;
        private static void UpdateKernel() {
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);
            _countKernel++;
            if (_countKernel > 5) {
                AntdshLogger.WriteLine("kernel update failed");
                return;
            }
            var currentVersion = Terminal.Terminal.Execute($"file {ModulesActive}").Split(' ').Last();
            var date = GetVersionDate(currentVersion);
            var requestUrl = $"{PublicRepositoryUrl}/update/info/kernel/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).FirstOrDefault() == "false") {
                AntdshLogger.WriteLine("kernel is already up to date");
                return;
            }
            Directory.CreateDirectory(Parameter.RepoTemp);
            Directory.CreateDirectory(TmpDirectory);

            Console.WriteLine("-----");
            Console.WriteLine(JsonConvert.SerializeObject(info));
            Console.WriteLine("-----");
            var retry = false;

            if (firmwareDone == false) {
                AntdshLogger.WriteLine("updating firmware");
                var firmwareDownloadPath = info.Where(_ => _.Key == "firmware-url").Select(_ => _.Value).FirstOrDefault();
                var firmwareDownloadUrl = $"{PublicRepositoryUrl}{firmwareDownloadPath}";
                var firmwareFilename = firmwareDownloadUrl.Split('/').Last();
                var firmwareDownloadedFile = $"{TmpDirectory}/{firmwareFilename}";
                var firmwareShasum = info.Where(_ => _.Key == "firmware-hash").Select(_ => _.Value).FirstOrDefault();
                ConsoleLogger.Point(firmwareShasum);
                FileSystem.Download2(firmwareDownloadUrl, firmwareDownloadedFile);
                AntdshLogger.WriteLine("check downloaded file");
                var firmwareCurrentSha = GetShaSum(firmwareDownloadedFile);
                if (firmwareShasum != firmwareCurrentSha) {
                    AntdshLogger.WriteLine($"{firmwareDownloadedFile} downloaded file is not valid");
                    retry = true;
                }
                AntdshLogger.WriteLine($"{firmwareDownloadedFile} download complete");
                var firmwareNewVersion = $"{KernelDirectory}/{firmwareFilename}";
                File.Copy(firmwareDownloadedFile, firmwareNewVersion, true);
                Terminal.Terminal.Execute($"ln -s {firmwareNewVersion} {FirmwareActive}");
            }
            firmwareDone = true;

            if (modulesDone == false) {
                AntdshLogger.WriteLine("updating modules");
                var modulesDownloadPath = info.Where(_ => _.Key == "modules-url").Select(_ => _.Value).FirstOrDefault();
                var modulesDownloadUrl = $"{PublicRepositoryUrl}{modulesDownloadPath}";
                var modulesFilename = modulesDownloadUrl.Split('/').Last();
                var modulesDownloadedFile = $"{TmpDirectory}/{modulesFilename}";
                var modulesShasum = info.Where(_ => _.Key == "modules-hash").Select(_ => _.Value).FirstOrDefault();
                FileSystem.Download2(modulesDownloadUrl, modulesDownloadedFile);
                AntdshLogger.WriteLine("check downloaded file");
                var moduleCurrentSha = GetShaSum(modulesDownloadedFile);
                if (modulesShasum != moduleCurrentSha) {
                    AntdshLogger.WriteLine($"{modulesDownloadedFile} downloaded file is not valid");
                    retry = true;
                }
                AntdshLogger.WriteLine($"{modulesDownloadedFile} download complete");
                var modulesNewVersion = $"{KernelDirectory}/{modulesFilename}";
                File.Copy(modulesDownloadedFile, modulesNewVersion, true);
                Terminal.Terminal.Execute($"ln -s {modulesNewVersion} {ModulesActive}");
            }
            modulesDone = true;

            if (sysmapDone == false) {
                AntdshLogger.WriteLine("updating sysmapFile");
                var sysmapFileDownloadPath = info.Where(_ => _.Key == "sysmapFile-url").Select(_ => _.Value).FirstOrDefault();
                var sysmapFileDownloadUrl = $"{PublicRepositoryUrl}{sysmapFileDownloadPath}";
                var sysmapFileFilename = sysmapFileDownloadUrl.Split('/').Last();
                var sysmapFileDownloadedFile = $"{TmpDirectory}/{sysmapFileFilename}";
                var sysmapFileShasum = info.Where(_ => _.Key == "sysmapFile-hash").Select(_ => _.Value).FirstOrDefault();
                FileSystem.Download2(sysmapFileDownloadUrl, sysmapFileDownloadedFile);
                AntdshLogger.WriteLine("check downloaded file");
                var sysmapCurrentSha = GetShaSum(sysmapFileDownloadedFile);
                if (sysmapFileShasum != sysmapCurrentSha) {
                    AntdshLogger.WriteLine($"{sysmapFileDownloadedFile} downloaded file is not valid");
                    retry = true;
                }
                AntdshLogger.WriteLine($"{sysmapFileDownloadedFile} download complete");
                var sysmapFileNewVersion = $"{KernelDirectory}/{sysmapFileFilename}";
                File.Copy(sysmapFileDownloadedFile, sysmapFileNewVersion, true);
                Terminal.Terminal.Execute($"ln -s {sysmapFileNewVersion} {SystemMapActive}");
            }
            sysmapDone = true;

            if (initramfsDone == false) {
                AntdshLogger.WriteLine("updating initramfs");
                var initramfsDownloadPath = info.Where(_ => _.Key == "initramfs-url").Select(_ => _.Value).FirstOrDefault();
                var initramfsDownloadUrl = $"{PublicRepositoryUrl}{initramfsDownloadPath}";
                var initramfsFilename = initramfsDownloadUrl.Split('/').Last();
                var initramfsDownloadedFile = $"{TmpDirectory}/{initramfsFilename}";
                var initramfsShasum = info.Where(_ => _.Key == "initramfs-hash").Select(_ => _.Value).FirstOrDefault();
                FileSystem.Download2(initramfsDownloadUrl, initramfsDownloadedFile);
                AntdshLogger.WriteLine("check downloaded file");
                var initramfsCurrentSha = GetShaSum(initramfsDownloadedFile);
                if (initramfsShasum != initramfsCurrentSha) {
                    AntdshLogger.WriteLine($"{initramfsDownloadedFile} downloaded file is not valid");
                    retry = true;
                }
                AntdshLogger.WriteLine($"{initramfsDownloadedFile} download complete");
                var initramfsNewVersion = $"{KernelDirectory}/{initramfsFilename}";
                File.Copy(initramfsDownloadedFile, initramfsNewVersion, true);
                Terminal.Terminal.Execute($"ln -s {initramfsNewVersion} {InitrdActive}");
            }
            initramfsDone = true;

            if (kernelDone == false) {
                AntdshLogger.WriteLine("updating kernel");
                var kernelDownloadPath = info.Where(_ => _.Key == "kernel-url").Select(_ => _.Value).FirstOrDefault();
                var kernelDownloadUrl = $"{PublicRepositoryUrl}{kernelDownloadPath}";
                var kernelFilename = kernelDownloadUrl.Split('/').Last();
                var kernelDownloadedFile = $"{TmpDirectory}/{kernelFilename}";
                var kernelShasum = info.Where(_ => _.Key == "kernel-hash").Select(_ => _.Value).FirstOrDefault();
                FileSystem.Download2(kernelDownloadUrl, kernelDownloadedFile);
                AntdshLogger.WriteLine("check downloaded file");
                var kernelCurrentSha = GetShaSum(kernelDownloadedFile);
                if (kernelShasum != kernelCurrentSha) {
                    AntdshLogger.WriteLine($"{kernelDownloadedFile} downloaded file is not valid");
                    retry = true;
                }
                AntdshLogger.WriteLine($"{kernelDownloadedFile} download complete");
                var kernelNewVersion = $"{KernelDirectory}/{kernelFilename}";
                File.Copy(kernelDownloadedFile, kernelNewVersion, true);
                Terminal.Terminal.Execute($"ln -s {kernelNewVersion} {KernelActive}");
            }
            kernelDone = true;

            if (xenDone == false) {
                AntdshLogger.WriteLine("updating xen");
                var xenDownloadPath = info.Where(_ => _.Key == "xen-url").Select(_ => _.Value).FirstOrDefault();
                var xenDownloadUrl = $"{PublicRepositoryUrl}{xenDownloadPath}";
                var xenFilename = xenDownloadUrl.Split('/').Last();
                var xenDownloadedFile = $"{TmpDirectory}/{xenFilename}";
                var xenShasum = info.Where(_ => _.Key == "xen-hash").Select(_ => _.Value).FirstOrDefault();
                FileSystem.Download2(xenDownloadUrl, xenDownloadedFile);
                AntdshLogger.WriteLine("check downloaded file");
                var xenCurrentSha = GetShaSum(xenDownloadedFile);
                if (xenShasum != xenCurrentSha) {
                    AntdshLogger.WriteLine($"{xenDownloadedFile} downloaded file is not valid");
                    retry = true;
                }
                AntdshLogger.WriteLine($"{xenDownloadedFile} download complete");
                var xenNewVersion = $"{KernelDirectory}/{xenFilename}";
                File.Copy(xenDownloadedFile, xenNewVersion, true);
                Terminal.Terminal.Execute($"ln -s {xenNewVersion} {XenActive}");
            }
            xenDone = true;

            if (retry) {
                AntdshLogger.WriteLine($"one or more downloaded files are not valid");
                UpdateKernel();
            }

            Directory.Delete(TmpDirectory, true);
        }

        private static void BoHelpDownloadFile(string url, string destination, string shasum) {
            //Terminal.Terminal.Execute($"wget {url} -O {destination}");
            FileSystem.Download2(url, destination);
            AntdshLogger.WriteLine("check downloaded file");
            var currentSha = GetShaSum(destination);
            if (shasum != currentSha) {
                AntdshLogger.WriteLine($"{destination} downloaded file is not valid");
                UpdateKernel();
            }
            AntdshLogger.WriteLine($"{destination} download complete");
        }
    }
}
