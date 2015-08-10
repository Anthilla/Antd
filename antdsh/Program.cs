using System;
using System.Collections.Generic;
using System.IO;

namespace antdsh {
    class Program {
        static string command;
        static HashSet<cmd> commandList = new HashSet<cmd>() { };

        static void Main(string[] args) {
            Directory.CreateDirectory(global.versionsDir);
            Directory.CreateDirectory(global.tmpDir);
            if (args.Length == 0) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(DateTime.Now.ToString("[dd-MM-yyyy] HH:mm"));
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(" > ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("antdsh");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(" > ");
                Console.ResetColor();
                command = Console.ReadLine();
                if (command != "") { AddCommand(command); }
                Command(command.Trim());
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
            else if (command == "stop") { shell.Stop(); }
            else if (command == "restart") { shell.Restart(); }
            else if (command == "status") { shell.Status(); }
            else if (command == "umount-all") { shell.UmountAll(); }
            else if (command == "update-check") { shell.UpdateCheck(); }
            else if (command == "update-launch") { shell.UpdateLaunch(); }
            else if (command == "update-url") { shell.UpdateFromUrl(); }
            else if (command == "update-select") { shell.UpdateSelect(); }
            else if (command == "reload-systemctl") { shell.ReloadSystemctl(); }
            else if (command == "isrunning") { shell.IsRunning(); }
            else if (command == "clean-tmp") { shell.CleanTmp(); }
            else if (command == "info") { shell.Info(); }
            //else if (command == "progress") { shell.Progress(); }
            //else if (command == "clear") { Terminal.Execute("clear"); }
            else if (command == "history") { PrintHistory(); }
            //else if (command.StartsWith("test")) { Console.WriteLine($"testing: {command}"); }
            else if (command == "exit") { shell.Exit(); }
            else if (command == "") { return; }
            else { shell.Execute(command); }
        }

        static void Help() {
            Console.WriteLine("> Command List:");
            WriteHelp("help", "show this list");
            WriteHelp("start", "initialize a running version of antd");
            WriteHelp("stop", "stop any running version of antd");
            WriteHelp("restart", "restart antd related systemctl services and mounts");
            WriteHelp("status", "show antd status from systemctl");
            WriteHelp("umount-all", "umount all antd directories recursively");
            WriteHelp("update-check", "check if a newer version of antd exists on this machine");
            WriteHelp("update-launch", "update antd to the newest version found on this machine");
            WriteHelp("update-url", "update antd from an url");
            WriteHelp("update-select", "select a running version from the ones found on this machine");
            WriteHelp("reload-systemctl", "reload systemctl daemon");
            WriteHelp("isrunning", "check whether antd process is active or not");
            WriteHelp("clean-tmp", "remove every files and directories from tmp directory");
            WriteHelp("info", "generic command");
            WriteHelp("history", "show the commands used in this antdsh session");
            WriteHelp("exit", "exit from antdsh");
        }

        static void WriteHelp(string command, string description) {
            Console.WriteLine("    {0}:", command);
            Console.WriteLine("        {0};", description);
        }

        static void AddCommand(string command) {
            if (command != "history") {
                var cmd = new cmd() {
                    timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    command = command
                };
                commandList.Add(cmd);
            }
        }

        static void PrintHistory() {
            foreach (var cmd in commandList) {
                Console.WriteLine(cmd.command);
            }
            return;
        }

        public class cmd {
            public string timestamp { get; set; }

            public string command { get; set; }
        }

        //string[] items = { "nought", "one", "two", "three", "four" };

        //var item = items[4];

        //var pad1 = Enumerable.Repeat("", 1);
        //var pad2 = Enumerable.Repeat("", 2);

        //var padded = pad1.Concat(items);
        //var next1 = items.Concat(pad1);
        //var next2 = items.Skip(1).Concat(pad2);

        //var sandwiched =
        //    padded
        //        .Zip(next1, (previous, current) => new { previous, current })
        //        .Zip(next2, (pair, next) => new { pair.previous, pair.current, next })
        //        .FirstOrDefault(triplet => triplet.current == item);
    }
}
