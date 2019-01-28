using anthilla.core;
using System.IO;

namespace Antd.cmds {
    public class Ntp {

        public const string Service = "ntpd.service";
        private const string ntpConfFile = "/etc/ntp.conf";
        private const string ntpConfFileTmp = "/etc/ntp.conf.tmp";

        public static void Prepare() {
            Systemctl.Enable(Service);
        }

        public static void Start() {
            Systemctl.Start(Service);
        }

        public static void Stop() {
            Systemctl.Stop(Service);
        }

        public static bool Set() {
            var currentConfig = Application.CurrentConfiguration.Network.InternalNetwork;
            var parsedNetworkParameter = LukeSkywalker.IPNetwork.IPNetwork.Parse(currentConfig.IpAddress, currentConfig.NetworkRange);
            var network = parsedNetworkParameter.Network.ToString();
            var mask = parsedNetworkParameter.Netmask.ToString();
            var lines = new string[] {
                "interface ignore wildcard",
                $"interface listen {currentConfig.IpAddress}",
                $"restrict {network} mask {mask} nomodify",
                "",
                "server 0.it.pool.ntp.org",
                "server 1.it.pool.ntp.org",
                "server 2.it.pool.ntp.org",
                "server 3.it.pool.ntp.org",
                "server 193.204.114.232",
                "server 193.204.114.233",
                "server ntp1.ien.it",
                "server ntp2.ien.it",
                "",
                "statistics loopstats",
                "driftfile /var/lib/ntp/ntp.drift",
                "logfile /var/log/ntp/ntpd.log",
                "statsdir /var/log/ntp/",
                "filegen peerstats file peers type day link enable",
                "filegen loopstats file loops type day link enable"
            };
            File.WriteAllLines(ntpConfFileTmp, lines);
            if(File.Exists(ntpConfFile)) {
                var existingFileHash = CommonFile.GetHash(ntpConfFile);
                var newFileHash = CommonFile.GetHash(ntpConfFileTmp);
                if(CommonString.AreEquals(existingFileHash, newFileHash) == true) {
                    return true;
                }
                else {
                    File.Copy(ntpConfFileTmp, ntpConfFile, true);
                }
            }
            else {
                File.WriteAllLines(ntpConfFile, lines);
            }
            if(File.Exists(ntpConfFileTmp)) {
                File.Delete(ntpConfFileTmp);
            }
            Systemctl.Start(Service);
            return true;
        }
    }
}

