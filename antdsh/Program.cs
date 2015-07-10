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
            }
            else {
                Command(args[0]);
            }
            Console.ReadLine();
        }

        static void Command(string command) {
            if (command == "help") { Help(); }
            else if (command == "start") { shell.Start(); }
            else if (command == "update-check") { shell.UpdateCheck(); }
            else if (command == "update-launch") { shell.UpdateLaunch(); }
            else if (command == "update-url") { shell.UpdateFromUrl(); }
            else if (command == "update-select") { shell.UpdateSelect(); }
            else if (command == "info") { shell.Info(); }
            else if (command == "set-directory-download") { shell.SetDirectoryDownload(); }
        }

        static void Help() {
            Console.WriteLine("> Try with these commands:");
            Console.WriteLine(">     update-check");
            Console.WriteLine(">     update-launch");
            Console.WriteLine(">     update-force");
            Console.WriteLine(">     update-git");
            Console.WriteLine(">     update-selectversion");
            Console.WriteLine(">     info");
            Console.WriteLine(">     set-directory-download");
            Console.WriteLine(">     help");
        }
    }
}
