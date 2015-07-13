using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    class Program {
        static void Main(string[] args) {
            Directory.CreateDirectory(global.versionsDir);
            Directory.CreateDirectory(global.tmpDir);
            if (args.Length == 0) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("> antdsh, waiting for a command: ");
                Console.ResetColor();
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
            else if (command == "progress") { shell.Progress(); }
            else { Console.WriteLine("> Command not found :)"); return; }
        }

        static void Help() {
            Console.WriteLine("> Command List:");
            WriteHelp("help", "show the command list");
            WriteHelp("start", "initialize a running version of antd");
            WriteHelp("update-check", "check for the newest version of antd");
            WriteHelp("update-launch", "update antd to its newest version");
            WriteHelp("update-url", "update antd from an url");
            WriteHelp("update-select", "select a running version from the ones listed");
            WriteHelp("reload-services", "reload all antd related systemctl services and mounts");
            WriteHelp("reload-systemctl", "reload systemctl daemon");
            WriteHelp("stop-services", "stop all antd related systemctl services and mounts");
            WriteHelp("isrunning", "check whether antd process is active or not");
            WriteHelp("clean-tmp", "remove every files and directories from tmp directory");
            WriteHelp("info", "generic");
            WriteHelp("exit", "exit from the application");
        }

        static void WriteHelp(string command, string description) {
            Console.WriteLine(">     {0}:", command);
            Console.WriteLine(">         {0};", description);
        }
    }
}
