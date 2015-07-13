using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    class Program {
        static void Main(string[] args) {
            Directory.CreateDirectory(global.configDir);
            Directory.CreateDirectory(global.versionsDir);
            Directory.CreateDirectory(global.tmpDir);
            Console.WriteLine("> antdsh");
            if (args.Length == 0) {
                var input = Console.ReadLine();
                Command(input);
                Main(args);
            }
            else {
                Command(args[0]);
                shell.Exit();
            }
        }

        static void Command(string command) {
            if (command == "help") { Help(); }
            else if (command == "start") { shell.Start(); }
            else if (command == "update-check") { shell.UpdateCheck(); }
            else if (command == "update-launch") { shell.UpdateLaunch(); }
            else if (command == "update-url") { shell.UpdateFromUrl(); }
            else if (command == "update-select") { shell.UpdateSelect(); }
            else if (command == "reload-services") { shell.ReloadServices(); }
            else if (command == "reload-systemctl") { shell.ReloadSystemctl(); }
            else if (command == "stop-services") { shell.StopServices(); }
            else if (command == "isrunning") { shell.IsRunning(); }
            else if (command == "clean-tmp") { shell.CleanTmp(); }
            else if (command == "info") { shell.Info(); }
            else if (command == "exit") { shell.Exit(); }
            else { Console.WriteLine("> Command not found :)"); return; }
        }

        static void Help() {
            Console.WriteLine("> Command List:");
            Console.WriteLine(">     help:");
            Console.WriteLine(">         show the command list;");
            Console.WriteLine(">     start");
            Console.WriteLine(">         initialize a running version of antd;");
            Console.WriteLine(">     update-check");
            Console.WriteLine(">         check for the newest version of antd;");
            Console.WriteLine(">     update-launch");
            Console.WriteLine(">         update antd to its newest version;");
            Console.WriteLine(">     update-url");
            Console.WriteLine(">         update antd from an url,");
            Console.WriteLine(">         at the moment antd is downloaded from its Github repository;");
            Console.WriteLine(">     update-select");
            Console.WriteLine(">         select a running version from the ones listed;");
            Console.WriteLine(">     reload-services");
            Console.WriteLine(">         reload all antd related systemctl services and mounts;");
            Console.WriteLine(">     reload-systemctl");
            Console.WriteLine(">         reload systemctl daemon;");
            Console.WriteLine(">     stop-services");
            Console.WriteLine(">         stop all antd related systemctl services and mounts;");
            Console.WriteLine(">     isrunning");
            Console.WriteLine(">         check whether antd process is active or not;");
            Console.WriteLine(">     info");
        }
    }
}
