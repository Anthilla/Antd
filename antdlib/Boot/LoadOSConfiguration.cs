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

using antdlib.MountPoint;
using System.IO;
using System.Net;

namespace antdlib.Boot {
    public class LoadOSConfiguration {
        public static void LoadCollectd() {
            var fileName = "FILE_etc_collectd.conf";
            var FILE = $"{Folder.Dirs}/{fileName}";
            if (!File.Exists(FILE)) {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, FILE);
                }
            }
            else {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, $"{FILE}+");
                }
            }
            if (File.Exists($"{FILE}+")) {
                if (FileSystem.FilesAreEqual(new FileInfo(FILE), new FileInfo($"{FILE}+")) == false) {
                    File.Copy($"{FILE}+", FILE, true);
                }
                File.Delete($"{FILE}+");
            }
            var realFileName = Mount.GetFILESPath(fileName);
            if (Mount.IsAlreadyMounted(FILE, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart collectd.service");
        }

        public static void LoadSystemdJournald() {
            var fileName = "FILE_etc_systemd_journald.conf";
            var FILE = $"{Folder.Dirs}/{fileName}";
            if (!File.Exists(FILE)) {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, FILE);
                }
            }
            else {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, $"{FILE}+");
                }
            }
            if (File.Exists($"{FILE}+")) {
                if (FileSystem.FilesAreEqual(new FileInfo(FILE), new FileInfo($"{FILE}+")) == false) {
                    File.Copy($"{FILE}+", FILE, true);
                }
                File.Delete($"{FILE}+");
            }
            var realFileName = Mount.GetFILESPath(fileName);
            if (Mount.IsAlreadyMounted(FILE, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart systemd-journald.service");
        }

        public static void LoadEtcSSH() {
            var fileName = "sshd_config";
            var dir = "/etc/ssh";
            var DIR = Mount.GetDIRSPath(dir);
            var FILE = $"{DIR}/{fileName}";
            if (!File.Exists(FILE)) {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, FILE);
                }
            }
            else {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, $"{FILE}+");
                }
            }
            if (File.Exists($"{FILE}+")) {
                if (FileSystem.FilesAreEqual(new FileInfo(FILE), new FileInfo($"{FILE}+")) == false) {
                    File.Copy($"{FILE}+", FILE, true);
                }
                File.Delete($"{FILE}+");
            }
            if (Mount.IsAlreadyMounted(DIR, dir) == false) {
                Mount.Dir(dir);
            }
            Terminal.Execute("systemctl reload sshd.service");
        }

        public static void LoadWPASupplicant() {
            var fileName = "FILE_etc_wpa_supplicant_wpa_suplicant.conf";
            var FILE = $"{Folder.Dirs}/{fileName}";
            if (!File.Exists(FILE)) {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, FILE);
                }
            }
            else {
                using (var client = new WebClient()) {
                    client.DownloadFile("http://localhost:7777/repo/" + fileName, $"{FILE}+");
                }
            }
            if (File.Exists($"{FILE}+")) {
                if (FileSystem.FilesAreEqual(new FileInfo(FILE), new FileInfo($"{FILE}+")) == false) {
                    File.Copy($"{FILE}+", FILE, true);
                }
                File.Delete($"{FILE}+");
            }
            var realFileName = Mount.GetFILESPath(fileName);
            if (Mount.IsAlreadyMounted(FILE, realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Execute("systemctl restart wpa_supplicant.service");
        }
    }
}
