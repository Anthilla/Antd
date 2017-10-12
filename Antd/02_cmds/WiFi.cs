using anthilla.core;
using System.IO;

namespace Antd.cmds {

    public class WiFi {

        private const string wpasupplicantFileLocation = "/usr/sbin/wpa_supplicant";
        private const string wpasupplicantConfFile = "/cfg/antd/conf/wpa_supplicant.conf";
        private const string nftArgs = "-f ";

        /// <summary>
        /// wpa_supplicant -i wlan0 -c /mnt/cdrom/Config/network/wpa_supplicant-wlan0.conf -B
        /// </summary>
        public static void Apply() {
            var current = Application.CurrentConfiguration.Network.WpaSupplicant;
            if(current == null) {
                return;
            }
            if(current.Active == false) {
                return;
            }
            ConsoleLogger.Log($"[wifi] connecting '{current.Interface}' to '{current.Ssid}' ");
            WriteFile(current);
            if(!File.Exists(wpasupplicantConfFile)) {
                return;
            }
            var arg = CommonString.Append("-i ", current.Interface, " -c ", wpasupplicantConfFile, " -B");
            CommonProcess.Do(wpasupplicantFileLocation);
        }

        public static void WriteFile(WpaSupplicant conf) {
            var lines = new string[] {
                "###### Global Configuration ######",
                "ctrl_interface=/var/run/wpa_supplicant",
                "ctrl_interface_group=wheel",
                "update_config=1",
                "fast_reauth=1",
                "ap_scan=1",
                "country=IT",
                "",
                "network={",
                "    priority=1",
                $"    ssid=\"{conf.Ssid}\"",
                "    mode=0",
                "    key_mgmt=WPA-PSK",
                $"    psk=\"{conf.Password}\"",
                "}"
            };
            File.WriteAllLines(wpasupplicantConfFile, lines);
            ConsoleLogger.Log("[wifi] configured");
        }
    }
}


