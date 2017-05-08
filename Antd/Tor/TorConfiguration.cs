using System.Linq;
using anthilla.commands;
using anthilla.commands.Utils;

namespace Antd.Tor {
    public class TorConfiguration {
        private const string BinFilePath = "/usr/bin/tor";
        private const string ConfigDirectoryPath = "/etc/tor";
        private static readonly string ConfigFilePath = $"{ConfigDirectoryPath}/torrc";
        private const string ShareDirectoryPath = "/usr/share/tor";
        private const string HiddenServiceDirectoryPath = "/var/lib/tor/hidden_service";
        private static readonly string HostnameFilePath = $"{HiddenServiceDirectoryPath}/hostname";

        public static bool IsAvailable => _isAvailable();

        private static bool _isAvailable() {
            return System.IO.File.Exists(BinFilePath);
        }

        private static readonly CommandLauncher Launcher = new CommandLauncher();
        private static readonly Bash Bash = new Bash();

        public static bool IsActive => _isActive();

        private static bool _isActive() {
            var ps = Launcher.Launch("ps-aef");
            return ps.Contains("tor");
        }

        public static void Start() {
            System.IO.Directory.CreateDirectory(HiddenServiceDirectoryPath);
            Bash.Execute($"chown -R tor:root {HiddenServiceDirectoryPath}");
            Bash.Execute($"chmod -R 700 {HiddenServiceDirectoryPath}");
            if(IsActive == false) {
                var ths = new System.Threading.ThreadStart(() => {
                    Launcher.Launch("tor");
                });
                var th = new System.Threading.Thread(ths);
                th.Start();
            }
        }

        public static void Stop() {
            if(IsActive) {
                Launcher.Launch("tor-kill");
            }
        }

        public static void Restart() {
            Launcher.Launch("tor-kill");
            System.IO.Directory.CreateDirectory(HiddenServiceDirectoryPath);
            Bash.Execute($"chown -R tor:root {HiddenServiceDirectoryPath}");
            Bash.Execute($"chmod -R 700 {HiddenServiceDirectoryPath}");
            var ths = new System.Threading.ThreadStart(() => {
                Launcher.Launch("tor");
            });
            var th = new System.Threading.Thread(ths);
            th.Start();
        }

        public static bool AddVirtualPort(string virtualPort, string localService) {
            if(!System.IO.File.Exists(ConfigFilePath)) {
                return false;
            }
            var fileContent = System.IO.File.ReadAllLines(ConfigFilePath).ToList();
            var str1 = $"HiddenServiceDir {HiddenServiceDirectoryPath}";
            if(!fileContent.Contains(str1)) {
                fileContent.Add(str1);
            }
            var str2 = $"HiddenServicePort {virtualPort} {localService}";
            if(!fileContent.Contains(str2)) {
                fileContent.Add(str2);
            }
            if(System.IO.File.Exists(ConfigFilePath)) {
                System.IO.File.Copy(ConfigFilePath, $"{ConfigFilePath}.bck", true);
            }
            System.IO.File.WriteAllLines(ConfigFilePath, fileContent);
            Restart();
            return true;
        }

        public static string Hostname => _hostname();

        private static string _hostname() {
            if(!System.IO.File.Exists(HostnameFilePath)) {
                return null;
            }
            return System.IO.File.ReadAllText(HostnameFilePath);
        }
    }
}
