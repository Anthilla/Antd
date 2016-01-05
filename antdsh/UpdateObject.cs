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
using antdlib;
using antdlib.Common;
using antdlib.Terminal;
using Newtonsoft.Json;

namespace antdsh {
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
            if (string.IsNullOrEmpty(path))
                return "";
            try {
                var fName = Path.GetFileName(path).Trim();
                var from = path.Contains("-aufs-")
                    ? fName.IndexOf("-aufs-", StringComparison.InvariantCulture) + "-aufs-".Length
                    : fName.IndexOf("-", StringComparison.InvariantCulture) + "-".Length;
                var to = fName.LastIndexOf(path.Contains("-x86_64") ? "-x86_6" : ".squashfs.xz", StringComparison.InvariantCulture);
                return fName.Substring(from, to - from);
            }
            catch (Exception) {
                return "";
            }
        }

        private static string GetShaSum(string path) => !File.Exists(path) ? null : Terminal.Execute($"sha1sum {path}").Split(' ').First();

        private static async Task<T> GetResponseFromUrl<T>(string requestUrl) {
            var awaitResponse = new HttpClient().GetStringAsync(requestUrl);
            return JsonConvert.DeserializeObject<T>(await awaitResponse);
        }

        #region Parameters
        private const string PublicRepositoryUrl = "http://srv.anthilla.com/";
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
            Directory.Delete(TmpDirectory, true);
            _countAntd++;
            if (_countAntd > 5) {
                Console.WriteLine("antd update failed");
                return;
            }
            var date = GetVersionDate(AntdActive);
            var requestUrl = $"{PublicRepositoryUrl}update/info/antd/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).First() == "false") {
                Console.WriteLine("antd is already up to date");
            }
            Console.WriteLine("updating antd");
            var downloadUrl = info.Where(_ => _.Key == "url").Select(_ => _.Value).First();
            var filename = downloadUrl.Split('/').Last();
            Directory.CreateDirectory(TmpDirectory);
            var downloadedFile = $"{TmpDirectory}/{filename}";
            FileSystem.Download(downloadUrl, downloadedFile);
            Console.WriteLine("check downloaded file");
            var shasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            var currentSha = GetShaSum(downloadedFile);
            if (shasum != currentSha) {
                Console.WriteLine($"{filename} downloaded file is not valid");
                UpdateAntd();
            }
            Console.WriteLine($"{filename} download complete");
            var newVersion = $"{AntdDirectory}/{filename}";
            File.Copy(downloadedFile, newVersion, true);
            Directory.Delete(TmpDirectory, true);
            Console.WriteLine("restart antd");
            Terminal.Execute($"ln -s {newVersion} {AntdActive}");
            Terminal.Execute("systemctl stop app-antd-03-launcher");
            Terminal.Execute("systemctl stop framework-antd.mount");
            Terminal.Execute("systemctl restart framework-antd.mount");
            Terminal.Execute("systemctl restart app-antd-03-launcher");
        }

        private static int _countAntdsh;
        private static void UpdateAntdsh() {
            Directory.Delete(TmpDirectory, true);
            _countAntdsh++;
            if (_countAntdsh > 5) {
                Console.WriteLine("antdsh update failed");
                return;
            }
            var date = GetVersionDate(AntdshActive);
            var requestUrl = $"{PublicRepositoryUrl}update/info/antdsh/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).First() == "false") {
                Console.WriteLine("antdsh is already up to date");
                return;
            }
            Console.WriteLine("updating antdsh");
            var downloadUrl = info.Where(_ => _.Key == "url").Select(_ => _.Value).First();
            var filename = downloadUrl.Split('/').Last();
            Directory.CreateDirectory(TmpDirectory);
            var downloadedFile = $"{TmpDirectory}/{filename}";
            FileSystem.Download(downloadUrl, downloadedFile);
            Console.WriteLine("check downloaded file");
            var shasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            var currentSha = GetShaSum(downloadedFile);
            if (shasum != currentSha) {
                Console.WriteLine($"{filename} downloaded file is not valid");
                UpdateAntdsh();
            }
            Console.WriteLine($"{filename} download complete");
            var newVersion = $"{AntdshDirectory}/{filename}";
            File.Copy(downloadedFile, newVersion, true);
            Directory.Delete(TmpDirectory, true);
            Console.WriteLine("restart antdsh");
            Terminal.Execute($"ln -s {newVersion} {AntdshActive}");
            Terminal.Execute("systemctl stop app-anth-03-launcher");
            Terminal.Execute("systemctl stop framework-antdsh.mount");
            Terminal.Execute("systemctl restart framework-antdsh.mount");
            Terminal.Execute("systemctl restart app-anth-03-launcher");
        }

        private static int _countSystem;
        private static void UpdateSystem() {
            Directory.Delete(TmpDirectory, true);
            _countSystem++;
            if (_countSystem > 5) {
                Console.WriteLine("system update failed");
                return;
            }
            var date = GetVersionDate(SystemActive);
            var requestUrl = $"{PublicRepositoryUrl}update/info/system/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).First() == "false") {
                Console.WriteLine("system is already up to date");
                return;
            }
            Console.WriteLine("updating system");
            var downloadUrl = info.Where(_ => _.Key == "url").Select(_ => _.Value).First();
            var filename = downloadUrl.Split('/').Last();
            Directory.CreateDirectory(TmpDirectory);
            var downloadedFile = $"{TmpDirectory}/{filename}";
            FileSystem.Download(downloadUrl, downloadedFile);
            Console.WriteLine("check downloaded file");
            var shasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            var currentSha = GetShaSum(downloadedFile);
            if (shasum != currentSha) {
                Console.WriteLine($"{filename} downloaded file is not valid");
                UpdateSystem();
            }
            Console.WriteLine($"{filename} download complete");
            var newVersion = $"{SystemDirectory}/{filename}";
            File.Copy(downloadedFile, newVersion, true);
            Directory.Delete(TmpDirectory, true);
            Console.WriteLine("restart system");
            Terminal.Execute($"ln -s {newVersion} {SystemActive}");
            //todo check this
            //Terminal.Execute("systemctl stop app-anth-03-launcher");
            //Terminal.Execute("systemctl stop framework-System.mount");
            //Terminal.Execute("systemctl restart framework-System.mount");
            //Terminal.Execute("systemctl restart app-anth-03-launcher");
        }

        private static int _countKernel;
        private static void UpdateKernel() {
            Directory.Delete(TmpDirectory, true);
            _countKernel++;
            if (_countKernel > 5) {
                Console.WriteLine("kernel update failed");
                return;
            }
            var date = GetVersionDate(ModulesActive);
            var requestUrl = $"{PublicRepositoryUrl}update/info/kernel/{date}";
            var info = GetResponseFromUrl<List<KeyValuePair<string, string>>>(requestUrl).Result;
            if (info.Where(_ => _.Key == "update").Select(_ => _.Value).First() == "false") {
                Console.WriteLine("kernel is already up to date");
                return;
            }
            Directory.CreateDirectory(TmpDirectory);

            Console.WriteLine("updating firmware");
            var firmwareDownloadUrl = info.Where(_ => _.Key == "firmware-url").Select(_ => _.Value).First();
            var firmwareFilename = firmwareDownloadUrl.Split('/').Last();
            var firmwareDownloadedFile = $"{TmpDirectory}/{firmwareFilename}";
            var firmwareShasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            HelpDownloadFile(firmwareDownloadUrl, firmwareDownloadedFile, firmwareShasum);
            var firmwareNewVersion = $"{KernelDirectory}/{firmwareFilename}";
            File.Copy(firmwareDownloadedFile, firmwareNewVersion, true);
            Terminal.Execute($"ln -s {firmwareNewVersion} {FirmwareActive}");

            Console.WriteLine("updating modules");
            var modulesDownloadUrl = info.Where(_ => _.Key == "modules-url").Select(_ => _.Value).First();
            var modulesFilename = modulesDownloadUrl.Split('/').Last();
            var modulesDownloadedFile = $"{TmpDirectory}/{modulesFilename}";
            var modulesShasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            HelpDownloadFile(modulesDownloadUrl, modulesDownloadedFile, modulesShasum);
            var modulesNewVersion = $"{KernelDirectory}/{modulesFilename}";
            File.Copy(modulesDownloadedFile, modulesNewVersion, true);
            Terminal.Execute($"ln -s {modulesNewVersion} {ModulesActive}");

            Console.WriteLine("updating sysmapFile");
            var sysmapFileDownloadUrl = info.Where(_ => _.Key == "sysmapFile-url").Select(_ => _.Value).First();
            var sysmapFileFilename = sysmapFileDownloadUrl.Split('/').Last();
            var sysmapFileDownloadedFile = $"{TmpDirectory}/{sysmapFileFilename}";
            var sysmapFileShasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            HelpDownloadFile(sysmapFileDownloadUrl, sysmapFileDownloadedFile, sysmapFileShasum);
            var sysmapFileNewVersion = $"{KernelDirectory}/{sysmapFileFilename}";
            File.Copy(sysmapFileDownloadedFile, sysmapFileNewVersion, true);
            Terminal.Execute($"ln -s {sysmapFileNewVersion} {SystemMapActive}");

            Console.WriteLine("updating initramfs");
            var initramfsDownloadUrl = info.Where(_ => _.Key == "initramfs-url").Select(_ => _.Value).First();
            var initramfsFilename = initramfsDownloadUrl.Split('/').Last();
            var initramfsDownloadedFile = $"{TmpDirectory}/{initramfsFilename}";
            var initramfsShasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            HelpDownloadFile(initramfsDownloadUrl, initramfsDownloadedFile, initramfsShasum);
            var initramfsNewVersion = $"{KernelDirectory}/{initramfsFilename}";
            File.Copy(initramfsDownloadedFile, initramfsNewVersion, true);
            Terminal.Execute($"ln -s {initramfsNewVersion} {InitrdActive}");

            Console.WriteLine("updating kernel");
            var kernelDownloadUrl = info.Where(_ => _.Key == "kernel-url").Select(_ => _.Value).First();
            var kernelFilename = kernelDownloadUrl.Split('/').Last();
            var kernelDownloadedFile = $"{TmpDirectory}/{kernelFilename}";
            var kernelShasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            HelpDownloadFile(kernelDownloadUrl, kernelDownloadedFile, kernelShasum);
            var kernelNewVersion = $"{KernelDirectory}/{kernelFilename}";
            File.Copy(kernelDownloadedFile, kernelNewVersion, true);
            Terminal.Execute($"ln -s {kernelNewVersion} {KernelActive}");

            Console.WriteLine("updating xen");
            var xenDownloadUrl = info.Where(_ => _.Key == "xen-url").Select(_ => _.Value).First();
            var xenFilename = xenDownloadUrl.Split('/').Last();
            var xenDownloadedFile = $"{TmpDirectory}/{xenFilename}";
            var xenShasum = info.Where(_ => _.Key == "hash").Select(_ => _.Value).First();
            HelpDownloadFile(xenDownloadUrl, xenDownloadedFile, xenShasum);
            var xenNewVersion = $"{KernelDirectory}/{xenFilename}";
            File.Copy(xenDownloadedFile, xenNewVersion, true);
            Terminal.Execute($"ln -s {xenNewVersion} {XenActive}");

            Directory.Delete(TmpDirectory, true);
            //todo check this
            //Console.WriteLine("restart kernel");
            //Terminal.Execute("Kernelctl stop app-anth-03-launcher");
            //Terminal.Execute("Kernelctl stop framework-kernel.mount");
            //Terminal.Execute("Kernelctl restart framework-kernel.mount");
            //Terminal.Execute("Kernelctl restart app-anth-03-launcher");
        }

        private static void HelpDownloadFile(string url, string destination, string shasum) {
            FileSystem.Download(url, destination);
            Console.WriteLine("check downloaded file");
            var currentSha = GetShaSum(destination);
            if (shasum != currentSha) {
                Console.WriteLine($"{destination} downloaded file is not valid");
                UpdateKernel();
            }
            Console.WriteLine($"{destination} download complete");
        }
    }
}
