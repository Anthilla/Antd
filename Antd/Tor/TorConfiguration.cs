using System.IO;
using System.Linq;
using antdlib.common;
using anthilla.commands;

namespace Antd.Tor {
    public class TorConfiguration {
        private const string BinFilePath = "/usr/bin/tor";
        private const string ConfigDirectoryPath = "/etc/tor";
        private static readonly string ConfigFilePath = $"{ConfigDirectoryPath}/torrc";
        //private const string ShareDirectoryPath = "/usr/share/tor";
        private const string HiddenServiceDirectoryPath = "/var/lib/tor/hidden_service";
        private static readonly string HostnameFilePath = $"{HiddenServiceDirectoryPath}/hostname";

        public static bool IsAvailable => _isAvailable();

        private static bool _isAvailable() {
            return File.Exists(BinFilePath);
        }

        public static bool IsActive => _isActive();

        private static bool _isActive() {
            var ps = CommandLauncher.Launch("ps-aef");
            return ps.Contains("tor");
        }

        public static void Start() {
            Directory.CreateDirectory(HiddenServiceDirectoryPath);
            Bash.Execute($"chown -R tor:root {HiddenServiceDirectoryPath}");
            Bash.Execute($"chmod -R 700 {HiddenServiceDirectoryPath}");
            if(IsActive == false) {
                var ths = new System.Threading.ThreadStart(() => {
                    CommandLauncher.Launch("tor");
                });
                var th = new System.Threading.Thread(ths);
                th.Start();
            }
        }

        public static void Stop() {
            if(IsActive) {
                CommandLauncher.Launch("tor-kill");
            }
        }

        public static void Restart() {
            CommandLauncher.Launch("tor-kill");
            Directory.CreateDirectory(HiddenServiceDirectoryPath);
            Bash.Execute($"chown -R tor:root {HiddenServiceDirectoryPath}");
            Bash.Execute($"chmod -R 700 {HiddenServiceDirectoryPath}");
            var ths = new System.Threading.ThreadStart(() => {
                CommandLauncher.Launch("tor");
            });
            var th = new System.Threading.Thread(ths);
            th.Start();
        }

        public static bool AddVirtualPort(string virtualPort, string localService) {
            if(!File.Exists(ConfigFilePath)) {
                return false;
            }
            var fileContent = File.ReadAllLines(ConfigFilePath).ToList();
            var str1 = $"HiddenServiceDir {HiddenServiceDirectoryPath}";
            if(!fileContent.Contains(str1)) {
                fileContent.Add(str1);
            }
            var str2 = $"HiddenServicePort {virtualPort} {localService}";
            if(!fileContent.Contains(str2)) {
                fileContent.Add(str2);
            }
            FileWithAcl.WriteAllLines(ConfigFilePath, fileContent, "644", "root", "wheel");
            Restart();
            return true;
        }

        public static string Hostname => _hostname();

        private static string _hostname() {
            if(!File.Exists(HostnameFilePath)) {
                return null;
            }
            return File.ReadAllText(HostnameFilePath);
        }
    }
}
