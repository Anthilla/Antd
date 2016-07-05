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
            Console.Title = "antdsh";
            Execute.RemounwRwOs();
            AntdshLogger.SetupFile();
            Directory.CreateDirectory(Parameter.AntdVersionsDir);
            Directory.CreateDirectory(Parameter.AntdTmpDir);

            if (args.Length > 0) {
                Command(args);
                return;
            }

            KeepAlive();
        }

        private static void KeepAlive() {
            while (_command != "quit" && _command != "exit" && _command != "close") {
                Console.Write($"{DateTime.Now.ToString("[dd-MM-yyyy] HH:mm")} > antdsh > ");
                _command = Console.ReadLine();
                if (string.IsNullOrEmpty(_command)) continue;
                AddCommand(_command);
                Command(_command.Trim().SplitToList(" ").ToArray());
            }
        }

        private static void Command(string[] command) {
            if (command[0] == "help") {
                Help();
            }
            else if (command[0] == "update") {
                Console.WriteLine("");
                Console.WriteLine(command.Length);
                Console.WriteLine("");
                if (command.Length > 1) {
                    Update.LaunchUpdateFor(command[1]);
                }
                else {
                    Update.Check();
                }
            }
            else if (command[0] == "start") {
                Start();
            }
            else if (command[0] == "stop") {
                Stop();
            }
            else if (command[0] == "restart") {
                Restart();
            }
            else if (command[0] == "history") {
                PrintHistory();
            }
            else {
                var result = Terminal.Execute(string.Join(" ", command));
                if (string.IsNullOrEmpty(result)) return;
                Console.Write(result);
                Console.WriteLine();
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

        private static bool IsAntdRunning() => Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep").Length > 0;

        private static void Start() {
            if (IsAntdRunning()) return;
            Console.WriteLine("Antd is not running, so we can start it.");
            Console.WriteLine($"Looking for antds in {Parameter.AntdVersionsDir}");
            var newestVersionFound = Execute.GetNewestVersion();
            if (newestVersionFound.Key != null) {
                Execute.LinkVersionToRunning(newestVersionFound.Key);
                Console.WriteLine($"New antd '{newestVersionFound.Key}' linked to running version");
                Console.WriteLine("Restarting services now...");
                Execute.RestartSystemctlAntdServices();
                if (IsAntdRunning()) {
                    Console.WriteLine("Antd is running now!");
                }
                else {
                    Console.WriteLine("Something went wrong starting antd... retrying starting it...");
                    StartLoop(newestVersionFound.Key);
                }
            }
            else {
                Console.WriteLine(
                    "There's no antd on this machine, you can try use update-url command to dowload the latest version...");
            }
        }

        private static int _startCount;
        private static void StartLoop(string versionToRun) {
            while (true) {
                _startCount++;
                Console.WriteLine($"Retry #{_startCount}");
                if (_startCount < 5) {
                    Execute.LinkVersionToRunning(versionToRun);
                    Console.WriteLine($"New antd '{versionToRun}' linked to running version");
                    Console.WriteLine("Restarting services now...");
                    Execute.RestartSystemctlAntdServices();
                    if (IsAntdRunning()) {
                        Console.WriteLine("Antd is running now!");
                    }
                    else {
                        Console.WriteLine("Something went wrong starting antd... retrying starting it...");
                        continue;
                    }
                }
                else {
                    Console.WriteLine("Error: too many retries...");
                }
                break;
            }
        }

        private static void Stop() {
            Console.WriteLine("Checking whether antd is running or not");
            if (!IsAntdRunning()) return;
            Console.WriteLine("Removing everything and stopping antd");
            Execute.StopServices();
            UmountAll();
            if (IsAntdRunning() == false) {
                Console.WriteLine("Antd has been stopped now!");
            }
            else {
                Console.WriteLine("Something went wrong starting antd, antdsh is retrying");
                StopLoop();
            }
        }

        private static int _stopCount;
        private static void StopLoop() {
            while (true) {
                _stopCount++;
                Console.WriteLine($"Retry #{_stopCount}");
                if (_stopCount < 5) {
                    Console.WriteLine("Removing everything and stopping antd.");
                    Execute.StopServices();
                    UmountAll();
                    if (IsAntdRunning() == false) {
                        Console.WriteLine("Antd has been stopped now!");
                    }
                    else {
                        Console.WriteLine("Something went wrong stopping antd... retrying stopping it...");
                        continue;
                    }
                }
                else {
                    Console.WriteLine("Error: too many retries...");
                }
                break;
            }
        }

        private static void Restart() {
            Console.WriteLine("Checking whether antd is running or not...");
            if (IsAntdRunning() == false) {
                Console.WriteLine("Cannot restart antd because it isn't running! Try the 'start' command instead!");
            }
            else {
                Stop();
                Start();
            }
        }

        private static void UmountAll() {
            Console.WriteLine("Unmounting Antd");
            while (true) {
                var r = Terminal.Execute("cat /proc/mounts | grep /antd");
                var f = Terminal.Execute("df | grep /cfg/antd");
                if (r.Length <= 0 && f.Length <= 0)
                    return;
                Terminal.Execute($"umount {Parameter.AntdCfg}");
                Terminal.Execute($"umount {Parameter.AntdCfgDatabase}");
                Terminal.Execute("umount /framework/antd");
            }
        }

        private static void IsRunning() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            Console.WriteLine(res.Length > 0 ? "Yes, is running." : "No.");
        }

        private static void CleanTmp() {
            Console.WriteLine("Cleaning tmp.");
            Execute.CleanTmp();
        }
    }
}
