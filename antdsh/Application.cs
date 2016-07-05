//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using antdlib.common;

namespace antdsh {
    internal class Application {
        private static string _command;
        private static readonly IDictionary<string, string> CommandList = new Dictionary<string, string>();

        private static void Main(string[] args) {
            Execute.RemounwRwOs();
            AntdshLogger.SetupFile();
            while (true) {
                Console.Title = "antdsh";
                Directory.CreateDirectory(Parameter.AntdVersionsDir);
                Directory.CreateDirectory(Parameter.AntdTmpDir);
                if (args.Length == 0) {
                    Console.Write($"{DateTime.Now.ToString("[dd-MM-yyyy] HH:mm")} > antdsh > ");
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

        private static void Command(string command) {
            if (command == "help") {
                Help();
            }
            else if (command == "start") {
                Shell.Start();
            }
            else if (command == "stop") {
                Shell.Stop();
            }
            else if (command == "restart") {
                Shell.Restart();
            }
            else if (command.StartsWith("update")) {
                var context = command.Split(' ');
                if (context.Length > 0) {
                    Update.LaunchUpdateFor(context[1]);
                }
                else {
                    Update.Check();
                }
            }
            else if (command == "history") {
                PrintHistory();
            }
            else if (command == "exit") {
                Shell.Exit();
            }
            else if (command == "") { }
            else {
                Shell.Execute(command);
            }
        }

        private static void Help() {
            Console.WriteLine("> Command List:");
            WriteHelp("help", "show this list");
            WriteHelp("start", "initialize a running version of antd");
            WriteHelp("stop", "stop any running version of antd");
            WriteHelp("restart", "restart antd related systemctl services and mounts");
            WriteHelp("update", "update the selected resource from the public repository, options are: antd,  antdsh, system, kernel");
            WriteHelp("history", "show the commands used in this antdsh session");
            WriteHelp("exit", "exit from antdsh");
            WriteHelp("", "any other command not listed here will be executed on this machine and you will get its return code");
        }

        private static void WriteHelp(string command, string description) {
            Console.WriteLine($"    {command}:");
            Console.WriteLine($"        {description};");
        }

        private static void AddCommand(string command) {
            if (command == "history")
                return;
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            CommandList.Add(new KeyValuePair<string, string>(timestamp, command));
        }

        private static void PrintHistory() {
            foreach (var cmd in CommandList) {
                Console.WriteLine(cmd.Value);
            }
        }
    }
}
