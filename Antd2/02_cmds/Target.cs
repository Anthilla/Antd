using System;
using System.IO;
using System.Linq;

namespace Antd2.cmds {
    public class Target {

        public static void CreateAntdTarget() {
            var targetDirectory = GetSystemdTargetDirectory();
            if (string.IsNullOrEmpty(targetDirectory)) {
                Console.WriteLine("  unable to find target location");
                return;
            }

            if (!File.Exists($"{targetDirectory}/antd.target"))
                File.Copy("Targets/antd.target", $"{targetDirectory}/antd.target");

            Directory.CreateDirectory("/etc/systemd/system/antd.target.wants");
            Bash.Do("systemctl enable antd.target");
        }

        public static void CreateApplicativeTarget() {
            var targetDirectory = GetSystemdTargetDirectory();
            if (string.IsNullOrEmpty(targetDirectory)) {
                Console.WriteLine("  unable to find target location");
                return;
            }

            if (!File.Exists($"{targetDirectory}/applicative.target"))
                File.Copy("Targets/applicative.target", $"{targetDirectory}/applicative.target");

            Directory.CreateDirectory("/etc/systemd/system/applicative.target.wants");
            Bash.Do("systemctl enable applicative.target");
        }

        private static string GetSystemdTargetDirectory() {
            var whereisSystemd = Bash.Execute("whereis systemd").FirstOrDefault();
            var result = whereisSystemd.Split(new[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (result.Length < 2) {
                Console.WriteLine("  systemd not found");
                return string.Empty;
            }

            var location = result[1].Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(_ => _.StartsWith("/lib") || _.StartsWith("/usr/lib"));
            if (string.IsNullOrEmpty(location)) {
                Console.WriteLine("  systemd not found");
                return string.Empty;
            }

            var targetDirectory = $"{location}/system";
            if (!Directory.Exists(targetDirectory)) {
                Console.WriteLine($"  {targetDirectory} not found");
                return string.Empty;
            }
            return targetDirectory;
        }
    }
}
