using Antd2.cmds;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bash = Antd2.cmds.Bash;

namespace Antd2 {
    public class InstallCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "prepare", PrepareFunc },
            };

        public static void PrepareFunc(string[] args) {
            Console.WriteLine("  Preparing basic environment for antd installation");
            Console.WriteLine("  Create and mount working directories");
            Mount.WorkingAntdDirectories();

            if (!File.Exists("/cfg/antd/antd.toml")) {
                Console.WriteLine("  Copying default configuration file");
                File.Copy("antd.toml", "/cfg/antd/antd.toml");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  WARN: this file '/cfg/antd/antd.toml' must be edited!");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("  Create ssh key for root user");
            Bash.Do("echo -e n | ssh-keygen -t rsa -N '' -f /root/.ssh/id_rsa");


            Console.WriteLine("  Create default targets");
            Console.WriteLine("  Create antd.target");
            Target.CreateAntdTarget();
            Console.WriteLine("  Create applicative.target");
            Target.CreateApplicativeTarget();

            Console.WriteLine("  Create antd unit");
            var antdUnitPath = "/mnt/cdrom/Units/anthillaUnits/app-antd-launcher.service";
            if (!File.Exists(antdUnitPath))
                File.Copy("Units/app-antd-launcher.service", antdUnitPath);
            Bash.Do($"systemctl enable {antdUnitPath}");
        }
    }
}