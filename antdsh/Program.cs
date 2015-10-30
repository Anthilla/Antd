///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using antdlib;
using antdlib.Boot;
using System;
using System.Collections.Generic;
using System.IO;

namespace antdsh {
    class Program {
        static string _command;
        static HashSet<Cmd> _commandList = new HashSet<Cmd>();

        private static void Main(string[] args) {
            while (true) {
                Console.Title = "antdsh";
                RepositoryCheck.CheckIfGlobalRepositoryIsWriteable();
                Directory.CreateDirectory(Folder.AntdVersionsDir);
                Directory.CreateDirectory(Folder.AntdTmpDir);
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
                    _command = Console.ReadLine();
                    if (_command != "") {
                        AddCommand(_command);
                    }
                    if (_command != null)
                        Command(_command.Trim());
                    continue;
                }
                Command(args[0]);
                Shell.Exit();
                break;
            }
        }

        static void Command(string command) {
            switch (command) {
                case "help":
                    Help();
                    break;
                case "start":
                    Shell.Start();
                    break;
                case "stop":
                    Shell.Stop();
                    break;
                case "restart":
                    Shell.Restart();
                    break;
                case "status":
                    Shell.Status();
                    break;
                case "umount-all":
                    Shell.UmountAll();
                    break;
                case "update":
                    Shell.UpdateFromPublicRepo();
                    break;
                case "update-check":
                    Shell.UpdateCheck();
                    break;
                case "update-launch":
                    Shell.UpdateLaunch();
                    break;
                case "update-select":
                    Shell.UpdateSelect();
                    break;
                case "reload-systemctl":
                    Shell.ReloadSystemctl();
                    break;
                case "isrunning":
                    Shell.IsRunning();
                    break;
                case "clean-tmp":
                    Shell.CleanTmp();
                    break;
                case "info":
                    Shell.Info();
                    break;
                case "history":
                    PrintHistory();
                    break;
                case "exit":
                    Shell.Exit();
                    break;
                case "":
                    return;
                default:
                    Shell.Execute(command);
                    break;
            }
        }

        static void Help() {
            Console.WriteLine("> Command List:");
            WriteHelp("help", "show this list");
            WriteHelp("start", "initialize a running version of antd");
            WriteHelp("stop", "stop any running version of antd");
            WriteHelp("restart", "restart antd related systemctl services and mounts");
            WriteHelp("status", "show antd status from systemctl");
            WriteHelp("update", "update antd from the public repository");
            WriteHelp("umount-all", "umount all antd directories recursively");
            WriteHelp("update-check", "check if a newer version of antd exists on this machine");
            WriteHelp("update-launch", "update antd to the newest version found on this machine");
            WriteHelp("update-select", "select a running version from the ones found on this machine");
            WriteHelp("reload-systemctl", "reload systemctl daemon");
            WriteHelp("isrunning", "check whether antd process is active or not");
            WriteHelp("clean-tmp", "remove every files and directories from tmp directory");
            WriteHelp("info", "generic command");
            WriteHelp("history", "show the commands used in this antdsh session");
            WriteHelp("exit", "exit from antdsh");
            WriteHelp("red-button", "delete permanently all antd(sh)-related files!");
            WriteHelp(" ", "any other command not listed here will be executed on this machine and you will get its return code");
        }

        static void WriteHelp(string command, string description) {
            Console.WriteLine("    {0}:", command);
            Console.WriteLine("        {0};", description);
        }

        static void AddCommand(string command) {
            if (command == "history")
                return;
            var cmd = new Cmd {
                Timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Command = command
            };
            _commandList.Add(cmd);
        }

        static void PrintHistory() {
            foreach (var cmd in _commandList) {
                Console.WriteLine(cmd.Command);
            }
        }

        public class Cmd {
            public string Timestamp { get; set; }
            public string Command { get; set; }
        }
    }
}
