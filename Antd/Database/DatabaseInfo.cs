using System;
using System.IO;

namespace Antd.Database {
    public class DatabaseInfo {
        public static string GetJnlFilePath() {
            var applicationRoot = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(applicationRoot, "*.jnl", SearchOption.AllDirectories);
            return string.Join(", ", files).ToString();
        }

        public static string GetVersion() {
            return DeNSo.Configuration.Version.ToString();
        }
    }
}
