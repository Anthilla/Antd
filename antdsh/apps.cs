using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class apps {

        public class global {
            public static string versionsDir { get { return "/mnt/cdrom/DIRS/DIR_framework_anthillasp_versions"; } }
            public static string tmpDir { get { return "/mnt/cdrom/DIRS/DIR_framework_anthillasp_tmp"; } }
            public static string appsDir { get { return "/mnt/cdrom/Apps"; } }
            public const string anthillaspRunning = "running";
            public const string downloadName = "anthillaspDownload.zip";
            public const string downloadFirstDir = "anthillaspDownloadFirst";
        }

        public static void Start() { }

        public static void Stop() { }

        public static void Restart() { }

        public static void Status() { }

        public static void IsRunning() { }

        public static void UpdateCheck() { }
       
        public static void UpdateLaunch() { }
        
        public static void UpdateFromUrl() { }
        
        public static void UpdateSelect() { }
        
        public static void UmountAll() { }
        
        public static void CleanTmp() { }
    }
}
