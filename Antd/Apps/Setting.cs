using Antd.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Apps {
    public class AnthillaSP {

        public class Setting {
            public static bool CheckSquash(string squashName) {
                var result = false;
                var lookInto = "/mnt/cdrom/Apps";
                string[] filePaths = Directory.GetFiles(lookInto);
                for (int i = 0; i < filePaths.Length; i++) {
                    if (filePaths[i].Contains(squashName)) {
                        result = true;
                    }
                }
                return result;
            }

            public static void MountSquash(string where) {
                Directory.CreateDirectory("/framework/anthillasp");
                Command.Launch("mount", "/mnt/cdrom/Apps/DIR_framework_anthillasp.squashfs.xz /framework/anthillasp");
            }

            public static void CreateUnits() {
                string folder = "/mnt/cdrom/Overlay/anthillasp";
                ConsoleLogger.Warn("Your anthillasp units be written in tmpfs!");
                Command.Launch("mount", "-t tmpfs tmpfs /mnt/cdrom/Overlay/anthillasp/");
                string path = Path.Combine(folder, "");
                if (!File.Exists(path)) {
                    using (StreamWriter sw = File.CreateText(path)) {
                        sw.WriteLine("[Unit]");
                        sw.WriteLine("");
                        sw.WriteLine("");
                    }
                }
                Command.Launch("chmod", "777 " + path);
            }
        }
    }
}
