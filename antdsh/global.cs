using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class global {
        public static string configDir { get { return "/mnt/cdrom/DIRS/DIR_framework_antdsh_config"; } }
        public static string versionsDir { get { return "/mnt/cdrom/DIRS/DIR_framework_antdsh_versions"; } }
        public static string tmpDir { get { return "/mnt/cdrom/DIRS/DIR_framework_antdsh_tmp"; } }
        public static string appsDir { get { return "/mnt/cdrom/Apps"; } }
        public const string configFile = "antdsh.config";
        public const string antdRunning = "running";

        public const string zipStartsWith = "antd";
        public const string zipEndsWith = ".7z";
        public const string squashStartsWith = "DIR_framework_antd";
        public const string squashEndsWith = ".squashfs.xz";

        public const string dateFormat = "yyyyMMdd";
    }
}
