using Antd;
using anthilla.core;
using System.IO;

namespace Antd2.cmds {

    /// <summary>
    /// From: https://wiki.archlinux.org/index.php/WPA_supplicant#At_boot_.28systemd.29
    /// 
    /// At boot (systemd)
    ///
    ///The wpa_supplicant package provides multiple systemd service files:
    ///
    ///wpa_supplicant.service - uses D-Bus, recommended for NetworkManager users.
    ///wpa_supplicant@.service - accepts the interface name as an argument and starts the wpa_supplicant daemon for this interface. It reads a /etc/wpa_supplicant/wpa_supplicant-interface.conf configuration file.
    ///wpa_supplicant-nl80211@.service - also interface specific, but explicitly forces the nl80211 driver(see below). The configuration file path is /etc/wpa_supplicant/wpa_supplicant-nl80211-interface.conf.
    ///wpa_supplicant-wired@.service - also interface specific, uses the wired driver.The configuration file path is /etc/wpa_supplicant/wpa_supplicant-wired-interface.conf.
    ///
    ///To enable wireless at boot, enable an instance of one of the above services on a particular wireless interface. For example, enable the wpa_supplicant@interface systemd unit.
    /// </summary>
    public class WiFi {

        private const string wpasupplicantEtcFolder = "/etc/wpa_supplicant";

        //public static void Apply() {
        //    var current = Application.CurrentConfiguration.Network.WpaSupplicant;
        //    if(current == null) {
        //        return;
        //    }
        //    if(current.Active == false) {
        //        return;
        //    }
        //    Console.WriteLine($"[wifi] connecting '{current.Interface}' to '{current.Ssid}' ");
        //    WriteFile(current);
        //    Start();
        //}

        public static void WriteFile(WpaSupplicant conf) {
            if(!Directory.Exists(wpasupplicantEtcFolder)) {
                return;
            }
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
            var confFilePath = CommonString.Append(wpasupplicantEtcFolder, "/wpa_supplicant-", conf.Interface, ".conf");
            File.WriteAllLines(confFilePath, lines);
        }

        //public static void Start() {
        //    var conf = Application.CurrentConfiguration.Network.WpaSupplicant;
        //    var serviceName = CommonString.Append("wpa_supplicant@", conf.Interface, ".service");
        //    Systemctl.Start(serviceName);
        //}

        //public static void Stop() {
        //    var conf = Application.CurrentConfiguration.Network.WpaSupplicant;
        //    var serviceName = CommonString.Append("wpa_supplicant@", conf.Interface, ".service");
        //    Systemctl.Stop(serviceName);
        //}
    }
}


