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
using System.Diagnostics;
using System.Linq;
using antdlib.Common;

namespace antdlib.Terminal {
    public class Terminal {
        public static string Execute(string command) {
            var output = string.Empty;
            var error = string.Empty;
            if (!AssemblyInfo.IsUnix)
                return output;
            var process = new Process {
                StartInfo = {
                    FileName = "bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            try {
                process.Start();
                using (var streamReader = process.StandardOutput) {
                    output = streamReader.ReadToEnd();
                }
                using (var streamReader = process.StandardError) {
                    error = streamReader.ReadToEnd();
                }
                process.WaitForExit();
                return output;
            }
            catch (Exception ex) {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Launching [{command}] has failed!");
                Console.WriteLine("Error message:");
                Console.WriteLine($"{ex.Message}");
                Console.WriteLine("-----------------------------------");
                return error;
            }
        }

        public static string Execute(string command, string dir) {
            var output = string.Empty;
            var error = string.Empty;
            if (!AssemblyInfo.IsUnix)
                return output;
            var process = new Process {
                StartInfo = {
                    FileName = "bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = dir
                }
            };
            try {
                process.Start();
                using (var streamReader = process.StandardOutput) {
                    output = streamReader.ReadToEnd();
                }
                using (var streamReader = process.StandardError) {
                    error = streamReader.ReadToEnd();
                }
                process.WaitForExit();
                return output;
            }
            catch (Exception ex) {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Launching [{command}] has failed!");
                Console.WriteLine("Error message:");
                Console.WriteLine($"{ex.Message}");
                Console.WriteLine("-----------------------------------");
                return error;
            }
        }

        public class MultiLine {
            public static string Execute(IEnumerable<string> commands) {
                var genericOutput = string.Empty;
                if (!AssemblyInfo.IsUnix)
                    return genericOutput;
                foreach (var command in commands) {
                    var process = new Process {
                        StartInfo = {
                            FileName = "bash",
                            Arguments = "-c \"" + command + "\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false
                        }
                    };
                    try {
                        process.Start();
                        using (var streamReader = process.StandardOutput) {
                            genericOutput += streamReader.ReadToEnd();
                        }
                        using (var streamReader = process.StandardError) {
                            genericOutput += streamReader.ReadToEnd();
                        }
                        process.WaitForExit();
                    }
                    catch (Exception ex) {
                        genericOutput += ex.Message;
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("{0} has failed", command);
                        Console.WriteLine("Error message:");
                        Console.WriteLine("{0}", ex.Message);
                        Console.WriteLine("-----------------------------------");
                    }
                }
                return genericOutput;
            }

            public static string Execute(IEnumerable<string> commands, string dir) {
                var genericOutput = string.Empty;
                if (!AssemblyInfo.IsUnix)
                    return genericOutput;
                foreach (var command in commands) {
                    var process = new Process {
                        StartInfo = {
                            FileName = "bash",
                            Arguments = "-c \"" + command + "\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            WorkingDirectory = dir
                        }
                    };
                    try {
                        process.Start();
                        using (var streamReader = process.StandardOutput) {
                            genericOutput += streamReader.ReadToEnd();
                        }
                        using (var streamReader = process.StandardError) {
                            genericOutput += streamReader.ReadToEnd();
                        }
                        process.WaitForExit();
                    }
                    catch (Exception ex) {
                        genericOutput += ex.Message;
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("{0} has failed", command);
                        Console.WriteLine("Error message:");
                        Console.WriteLine("{0}", ex.Message);
                        Console.WriteLine("-----------------------------------");
                    }
                }
                return genericOutput;
            }
        }

        public class Screen {

            public static string[] GetAll() {
                var list = new List<string>();
                var results = Terminal.Execute("screen -list").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (results.Count <= 2)
                    return list.ToArray();
                results.RemoveAt(results.Count - 1);
                results.RemoveAt(0);
                list.AddRange(from line in results where line.Contains("(") select line.RemoveWhiteSpace().Trim().Split(new[] { "(" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                return list.ToArray();
            }

            public static string Get(string screenName) {
                return Terminal.Execute($"screen -list | grep {screenName}").Trim();
            }

            public static void Wipe() {
                Terminal.Execute("screen -wipe");
            }

            public static void Kill(string screenName) {
                var s = Get(screenName);
                if (s.Length > 0) {
                    var proc = s.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToArray().First();
                    if (proc.Length > 0) {
                        Terminal.Execute($"kill -9 {proc}");
                    }
                }
                Wipe();
            }

            public static void KillAll() {
                foreach (var screen in GetAll()) {
                    Kill(screen);
                }
            }

            private static string SetScreenName(int num) {
                var name = $"antd-screen-{num.ToString("D2")}";
                return (Get(name).Length > 0) ? SetScreenName(num + 1) : name;
            }

            public static string Execute(string command) {
                var screend = $"screen -S antd-screen-{SetScreenName(0)} {command}";
                var output = string.Empty;
                var error = string.Empty;
                if (!AssemblyInfo.IsUnix)
                    return output;
                var process = new Process {
                    StartInfo = {
                        FileName = "bash",
                        Arguments = $"-c \"{screend}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };
                try {
                    process.Start();
                    using (var streamReader = process.StandardOutput) {
                        output = streamReader.ReadToEnd();
                    }
                    using (var streamReader = process.StandardError) {
                        error = streamReader.ReadToEnd();
                    }
                    process.WaitForExit();
                    return output;
                }
                catch (Exception ex) {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine($"Launching [{command}] has failed!");
                    Console.WriteLine("Error message:");
                    Console.WriteLine($"{ex.Message}");
                    Console.WriteLine("-----------------------------------");
                    return error;
                }
            }

            public static string Execute(string command, string dir) {
                var screend = $"screen -S antd-screen-{SetScreenName(0)} {command}";
                var output = string.Empty;
                var error = string.Empty;
                if (!AssemblyInfo.IsUnix)
                    return output;
                var process = new Process {
                    StartInfo = {
                        FileName = "bash",
                        Arguments = $"-c \"{screend}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        WorkingDirectory = dir
                    }
                };
                try {
                    process.Start();
                    using (var streamReader = process.StandardOutput) {
                        output = streamReader.ReadToEnd();
                    }
                    using (var streamReader = process.StandardError) {
                        error = streamReader.ReadToEnd();
                    }
                    process.WaitForExit();
                    return output;
                }
                catch (Exception ex) {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine($"Launching [{command}] has failed!");
                    Console.WriteLine("Error message:");
                    Console.WriteLine($"{ex.Message}");
                    Console.WriteLine("-----------------------------------");
                    return error;
                }
            }
        }

        public class Background {
            public static void Execute(string command) {
                if (!AssemblyInfo.IsUnix) return;
                var process = new Process {
                    StartInfo = {
                        FileName = "bash",
                        Arguments = $"-c \"{command} &\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    }
                };
                try {
                    process.Start();
                }
                catch (Exception ex) {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine($"Launching [{command}] has failed!");
                    Console.WriteLine("Error message:");
                    Console.WriteLine($"{ex.Message}");
                    Console.WriteLine("-----------------------------------");
                }
            }

            public static void Execute(string command, string dir) {
                if (!AssemblyInfo.IsUnix) return;
                var process = new Process {
                    StartInfo = {
                        FileName = "bash",
                        Arguments = $"-c \"{command} &\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        WorkingDirectory = dir
                    }
                };
                try {
                    process.Start();
                }
                catch (Exception ex) {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine($"Launching [{command}] has failed!");
                    Console.WriteLine("Error message:");
                    Console.WriteLine($"{ex.Message}");
                    Console.WriteLine("-----------------------------------");
                }
            }
        }
    }
}