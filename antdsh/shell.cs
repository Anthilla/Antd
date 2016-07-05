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
using antdlib.common;
using static System.Console;

namespace antdsh {
    public class Shell {
        private static bool IsAntdRunning() => Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep").Length > 0;

        public static void Start() {
            if (IsAntdRunning()) return;
            WriteLine("Antd is not running, so we can start it.");
            WriteLine($"Looking for antds in {Parameter.AntdVersionsDir}");
            var newestVersionFound = antdsh.Execute.GetNewestVersion();
            if (newestVersionFound.Key != null) {
                antdsh.Execute.LinkVersionToRunning(newestVersionFound.Key);
                WriteLine($"New antd '{newestVersionFound.Key}' linked to running version");
                WriteLine("Restarting services now...");
                antdsh.Execute.RestartSystemctlAntdServices();
                if (IsAntdRunning()) {
                    WriteLine("Antd is running now!");
                }
                else {
                    WriteLine("Something went wrong starting antd... retrying starting it...");
                    StartLoop(newestVersionFound.Key);
                }
            }
            else {
                WriteLine(
                    "There's no antd on this machine, you can try use update-url command to dowload the latest version...");
            }
        }

        private static int _startCount;

        private static void StartLoop(string versionToRun) {
            while (true) {
                _startCount++;
                WriteLine($"Retry #{_startCount}");
                if (_startCount < 5) {
                    antdsh.Execute.LinkVersionToRunning(versionToRun);
                    WriteLine($"New antd '{versionToRun}' linked to running version");
                    WriteLine("Restarting services now...");
                    antdsh.Execute.RestartSystemctlAntdServices();
                    if (IsAntdRunning()) {
                        WriteLine("Antd is running now!");
                    }
                    else {
                        WriteLine("Something went wrong starting antd... retrying starting it...");
                        continue;
                    }
                }
                else {
                    WriteLine("Error: too many retries...");
                }
                break;
            }
        }

        public static void Stop() {
            WriteLine("Checking whether antd is running or not");
            if (!IsAntdRunning()) return;
            WriteLine("Removing everything and stopping antd");
            antdsh.Execute.StopServices();
            UmountAll();
            if (IsAntdRunning() == false) {
                WriteLine("Antd has been stopped now!");
            }
            else {
                WriteLine("Something went wrong starting antd, antdsh is retrying");
                StopLoop();
            }
        }

        private static int _stopCount;

        private static void StopLoop() {
            while (true) {
                _stopCount++;
                WriteLine($"Retry #{_stopCount}");
                if (_stopCount < 5) {
                    WriteLine("Removing everything and stopping antd.");
                    antdsh.Execute.StopServices();
                    UmountAll();
                    if (IsAntdRunning() == false) {
                        WriteLine("Antd has been stopped now!");
                    }
                    else {
                        WriteLine("Something went wrong stopping antd... retrying stopping it...");
                        continue;
                    }
                }
                else {
                    WriteLine("Error: too many retries...");
                }
                break;
            }
        }

        public static void Restart() {
            WriteLine("Checking whether antd is running or not...");
            if (IsAntdRunning() == false) {
                WriteLine("Cannot restart antd because it isn't running! Try the 'start' command instead!");
            }
            else {
                Stop();
                Start();
            }
        }

        public static void UmountAll() {
            WriteLine("Unmounting Antd");
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

        public static void IsRunning() {
            var res = Terminal.Execute("ps -aef | grep Antd.exe | grep -v grep");
            WriteLine(res.Length > 0 ? "Yes, is running." : "No.");
        }

        public static void CleanTmp() {
            WriteLine("Cleaning tmp.");
            antdsh.Execute.CleanTmp();
        }

        public static void Exit() {
            Environment.Exit(1);
        }

        public static void Execute(string command) {
            WriteLine(Terminal.Execute(command));
        }
    }
}
