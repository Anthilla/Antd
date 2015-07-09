using antdlib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    public class common {
        public static void ChangeRunningVersion(KeyValuePair<string, string> newestVersionFound, string linkedVersionValue) {
            Console.WriteLine("> Updating!");
            string fileToLink;
            if (newestVersionFound.Key.Contains(global.squashEndsWith)) {
                fileToLink = newestVersionFound.Key;
            }
            else if (newestVersionFound.Key.Contains(global.zipEndsWith)) {
                fileToLink = Path.GetFullPath(newestVersionFound.Key.Replace(global.zipStartsWith, global.squashStartsWith).Replace(global.zipEndsWith, global.squashEndsWith));
                Terminal.Execute("7z x " + Path.GetFullPath(newestVersionFound.Key));
                Terminal.Execute("mksquashfs " +
                    Path.GetFullPath(newestVersionFound.Key.Replace(global.zipEndsWith, "")) + " " +
                    fileToLink +
                    " -comp xz -Xbcj x86 -Xdict-size 75%");
            }
            else {
                Console.WriteLine("> Update failed unexpectedly");
                return;
            }
            Terminal.Execute("ln -s " + Path.GetFullPath(fileToLink) + " " + Path.GetFullPath(global.antdRunning));
            Terminal.Execute("systemctl restart antd-prepare.service");
            Terminal.Execute("systemctl restart framework-antd.mount");
            Terminal.Execute("systemctl antd-launcher.service");
            return;
        }
    }
}
