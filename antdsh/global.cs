using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class global {
        public readonly static string root = config.downloadDirectory.Get();  
        public readonly static string configDir = AppDomain.CurrentDomain.BaseDirectory;
        public const string configFile = "antdsh.config";
        public const string antdRunning = "antdRunning";

        public const string zipStartsWith = "antd";
        public const string zipEndsWith = ".7z";
        public const string squashStartsWith = "DIR_framework_antd";
        public const string squashEndsWith = ".squashfs.xz";

        public const string dateFormat = "yyyyMMdd";
    }
}
