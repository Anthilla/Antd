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

        /// <summary>
        /// Apps			le app
        /// Config			configurazioni delle app
        /// DIRS			cartelle di sistema da overmountare
        /// Units			unit systemd & antdinit
        /// Scripts			"/usr/bin di antd"
        /// Kernel			
        /// System
        /// framework
        /// cfg
        /// runtime			antd non compresso se serve o script
        /// work			cartella di lavoro "temporanea"
        /// </summary>
        /// <param name="args"></param>
        public static void PrepareFunc(string[] args) {
            Console.WriteLine("  Preparing basic environment for antd installation");
            //Console.WriteLine("  Create and mount working directories");
            //Mount.WorkingAntdDirectories();

            Console.WriteLine("  Create working directories");
            Directory.CreateDirectory("/Antd/Apps");
            Directory.CreateDirectory("/Antd/Apps/Anthilla_Antd");
            Directory.CreateDirectory("/Antd/Apps/Anthilla_AntdUi");
            Directory.CreateDirectory("/Antd/Config");
            Directory.CreateDirectory("/Antd/Config/antd");
            Directory.CreateDirectory("/Antd/DIRS");
            Directory.CreateDirectory("/Antd/Units");
            Directory.CreateDirectory("/Antd/Scripts");
            Directory.CreateDirectory("/Antd/Kernel");
            Directory.CreateDirectory("/Antd/System");
            Directory.CreateDirectory("/Antd/framework");
            Directory.CreateDirectory("/Antd/cfg");
            Directory.CreateDirectory("/Antd/runtime");
            Directory.CreateDirectory("/Antd/work");

            Console.ForegroundColor = ConsoleColor.Green;
            var current = Directory.GetCurrentDirectory();
            Console.WriteLine("  " + current);
            Console.ForegroundColor = ConsoleColor.White;

            if (!File.Exists("/Antd/Config/antd/antd.toml")) {
                Console.WriteLine("  Copying default configuration file");
                File.Copy("antd.toml", "/Antd/Config/antd/antd.toml");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  WARN: this file '/Antd/Config/antd/antd.toml' must be edited!");
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
            var antdUnitPath = "/Antd/Units/anthillaUnits/app-antd-launcher.service";
            if (!File.Exists(antdUnitPath))
                File.Copy("Units/app-antd-launcher.service", antdUnitPath);
            Bash.Do($"systemctl enable {antdUnitPath}");


        }
    }
}