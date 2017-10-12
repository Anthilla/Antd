using System.Collections.Generic;
using System.IO;
using anthilla.core;

namespace Antd.cmds {

    public class Tor {

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/tor.conf";
        private const string ServiceName = "tor.service";
        private const string MainFilePath = "/etc/tor/torrc";
        private const string MainFilePathBackup = "/etc/tor/.torrc";

        private const string LibDir = "/var/lib/tor";
        private static readonly string LibDirMnt = $"{Parameter.RepoDirs}/DIR_lib_tor";

        public static void Parse() {
            return;
        }

        public static void Apply() {
            var options = Application.CurrentConfiguration.Services.Tor;
            if(options == null) {
                return;
            }
            Stop();
            DirectoryWithAcl.CreateDirectory(LibDirMnt, "755", "root", "root");
            Mount.MountWithBind(LibDirMnt, LibDir);
            #region [    torrc generation    ]
            if(File.Exists(MainFilePath)) {
                if(File.Exists(MainFilePathBackup)) {
                    File.Delete(MainFilePathBackup);
                }
                File.Copy(MainFilePath, MainFilePathBackup);
            }
            var lines = new List<string>();
            foreach(var svc in options.Services) {
                if(string.IsNullOrEmpty(svc.Name)
                    || string.IsNullOrEmpty(svc.IpAddress)
                    || string.IsNullOrEmpty(svc.TorPort)) {
                    continue;
                }
                //HiddenServiceDir /var/lib/tor/hidden_service/
                //HiddenServicePort 80 127.0.0.1:8080
                var dire = $"{LibDirMnt}/{svc.Name}";
                DirectoryWithAcl.CreateDirectory(dire, "755", "root", "root");
                lines.Add($"HiddenServiceDir {dire}");
                lines.Add($"HiddenServicePort {svc.TorPort} {svc.IpAddress}");
            }
            FileWithAcl.WriteAllLines(MainFilePath, lines, "700", "tor", "root");
            #endregion
            Start();
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[tor] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[tor] start");
        }
    }
}