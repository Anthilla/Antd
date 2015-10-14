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

using antdlib.Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace antdlib {

    public class Terminal {

        public static string Execute(string command) {
            string output = string.Empty;
            string error = string.Empty;
            if (AssemblyInfo.IsUnix == true) {
                Process process = new Process {
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
                    using (StreamReader streamReader = process.StandardOutput) {
                        output = streamReader.ReadToEnd();
                    }
                    using (StreamReader streamReader = process.StandardError) {
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
            return output;
        }

        public static string Execute(string command, string dir) {
            string output = string.Empty;
            string error = string.Empty;
            if (AssemblyInfo.IsUnix == true) {
                Process process = new Process {
                    StartInfo = {
                    FileName = "bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = dir.ToString()
                }
                };
                try {
                    process.Start();
                    using (StreamReader streamReader = process.StandardOutput) {
                        output = streamReader.ReadToEnd();
                    }
                    using (StreamReader streamReader = process.StandardError) {
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
            return output;
        }

        public class MultiLine {

            public static string Execute(string[] commands) {
                string genericOutput = string.Empty;
                if (AssemblyInfo.IsUnix == true) {
                    foreach (var command in commands) {
                        Process process = new Process {
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
                            using (StreamReader streamReader = process.StandardOutput) {
                                genericOutput += streamReader.ReadToEnd();
                            }
                            using (StreamReader streamReader = process.StandardError) {
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
                }
                return genericOutput;
            }

            public static string Execute(string[] commands, string dir) {
                string genericOutput = string.Empty;
                if (AssemblyInfo.IsUnix == true) {
                    foreach (var command in commands) {
                        Process process = new Process {
                            StartInfo = {
                                FileName = "bash",
                                Arguments = "-c \"" + command + "\"",
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                     UseShellExecute = false,
                        WorkingDirectory = dir.ToString()
                            }
                        };
                        try {
                            process.Start();
                            using (StreamReader streamReader = process.StandardOutput) {
                                genericOutput += streamReader.ReadToEnd();
                            }
                            using (StreamReader streamReader = process.StandardError) {
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
                }
                return genericOutput;
            }
        }

        public class Screen {

            public static string[] GetAll() {
                var list = new List<string>() { };
                var results = Terminal.Execute("screen -list").Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (results.Count > 2) {
                    results.RemoveAt(results.Count - 1);
                    results.RemoveAt(0);
                    foreach (var line in results) {
                        if (line.Contains("(")) {
                            var screen = line.RemoveWhiteSpace().Trim().Split(new String[] { "(" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                            list.Add(screen);
                        }
                    }
                }
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
                    var proc = s.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries).ToArray().First();
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
                string output = string.Empty;
                string error = string.Empty;
                if (AssemblyInfo.IsUnix == true) {

                    Process process = new Process {
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
                        using (StreamReader streamReader = process.StandardOutput) {
                            output = streamReader.ReadToEnd();
                        }
                        using (StreamReader streamReader = process.StandardError) {
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
                return output;
            }

            public static string Execute(string command, string dir) {
                var screend = $"screen -S antd-screen-{SetScreenName(0)} {command}";
                string output = string.Empty;
                string error = string.Empty;
                if (AssemblyInfo.IsUnix == true) {

                    Process process = new Process {
                        StartInfo = {
                    FileName = "bash",
                    Arguments = $"-c \"{screend}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = dir.ToString()
                }
                    };
                    try {
                        process.Start();
                        using (StreamReader streamReader = process.StandardOutput) {
                            output = streamReader.ReadToEnd();
                        }
                        using (StreamReader streamReader = process.StandardError) {
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
                return output;
            }
        }
    }
}