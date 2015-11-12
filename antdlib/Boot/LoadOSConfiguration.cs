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

using antdlib.MountPoint;
using antdlib.Network;
using System.IO;
using antdlib.Common;

namespace antdlib.Boot {
    public class LoadOsConfiguration {
        public static void LoadCollectd() {
            File.Copy($"{Folder.Resources}/FILE_etc_collectd.conf", $"{Folder.Dirs}/FILE_etc_collectd.conf", true);
            var realFileName = Mount.GetFilesPath("FILE_etc_collectd.conf");
            if (Mount.IsAlreadyMounted($"{Folder.Dirs}/FILE_etc_collectd.conf", realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Terminal.Execute("systemctl restart collectd.service");
        }

        public static void LoadSystemdJournald() {
            File.Copy($"{Folder.Resources}/FILE_etc_systemd_journald.conf", $"{Folder.Dirs}/{"FILE_etc_systemd_journald.conf"}", true);
            var realFileName = Mount.GetFilesPath("FILE_etc_systemd_journald.conf");
            if (Mount.IsAlreadyMounted($"{Folder.Dirs}/{"FILE_etc_systemd_journald.conf"}", realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Terminal.Execute("systemctl restart systemd-journald.service");
        }

        public static void LoadWpaSupplicant() {
            File.Copy($"{Folder.Resources}/{"FILE_etc_wpa_supplicant_wpa_suplicant.conf"}", $"{Folder.Dirs}/{"FILE_etc_wpa_supplicant_wpa_suplicant.conf"}", true);
            var realFileName = Mount.GetFilesPath("FILE_etc_wpa_supplicant_wpa_suplicant.conf");
            if (Mount.IsAlreadyMounted($"{Folder.Dirs}/{"FILE_etc_wpa_supplicant_wpa_suplicant.conf"}", realFileName) == false) {
                Mount.File(realFileName);
            }
            Terminal.Terminal.Execute("systemctl restart wpa_supplicant.service");
        }

        public static void LoadFirewall() {
            File.Copy($"{Folder.Resources}/antd.boot.firewall.conf", $"{Folder.Dirs}/antd.boot.firewall.conf", true);
        }

        public static void LoadWebsocketd() {
            var filePath = $"{Folder.Root}/websocketd";
            if (File.Exists(filePath)) return;
            ConsoleLogger.Info("Downloading websocketd");
            File.Copy($"{Folder.Resources}/websocketd", filePath, true);
            Terminal.Terminal.Execute($"chmod 777 {filePath}");
        }
    }
}
