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

using Antd.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Antd {

    public class Command {

        public static CommandModel Launch(string file, string args) {
//#if WINDOWS
//            Console.WriteLine("Windows.");
//#else
//            Console.WriteLine("Not Windows.");
//#endif
            string output = string.Empty;
            string error = string.Empty;
            Process process = new Process {
                StartInfo = {
                    FileName = file,
                    Arguments = args,
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
                CommandModel command = new CommandModel {
                    input = new Tuple<string, string>(file, args),
                    date = DateTime.Now,
                    output = output,
                    outputTable = TextToList(output),
                    error = error,
                    errorTable = TextToList(error)
                };
                ConsoleLogger.Log("Launched {0} {1}", file, args);
                ConsoleLogger.Log("------------ Command output:");
                ConsoleLogger.Log("{0}", command.output);
                return command;
            }
            catch (Exception ex) {
                CommandModel command = new CommandModel {
                    error = ex.Message,
                    errorTable = TextToList(ex.Message)
                };
                Console.WriteLine("");
                ConsoleLogger.Warn("Launched {0} {1}", file, args);
                ConsoleLogger.Warn("------------ Error output:");
                ConsoleLogger.Warn("{0}", command.error);
                Console.WriteLine("");
                return command;
            }
        }

        public static CommandModel Launch(string file, string args, string dir) {
            string output = string.Empty;
            string error = string.Empty;
            Process process = new Process {
                StartInfo = {
                    FileName = file,
                    Arguments = args,
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
                CommandModel command = new CommandModel {
                    input = new Tuple<string, string>(file, args),
                    date = DateTime.Now,
                    output = output,
                    outputTable = TextToList(output),
                    error = error,
                    errorTable = TextToList(error)
                };
                return command;
            }
            catch (Exception ex) {
                CommandModel command = new CommandModel {
                    error = ex.Message,
                    errorTable = TextToList(ex.Message)
                };
                return command;
            }
        }

        public static List<string> TextToList(string text) {
            List<string> stringList = new List<string>();
            string[] rowDivider = new String[] { "\n" };
            string[] rowList = text.Split(rowDivider, StringSplitOptions.None).ToArray();
            foreach (string row in rowList) {
                if (!string.IsNullOrEmpty(row)) {
                    stringList.Add(row);
                }
            }

            return stringList;
        }
    }

    public class Terminal {

        public static string Execute(string command) {
            string output = string.Empty;
            string error = string.Empty;
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
                Console.WriteLine("{0} has failed", command);
                Console.WriteLine("Error message:");
                Console.WriteLine("{0}", ex.Message);
                Console.WriteLine("-----------------------------------");
                return error;
            }
        }

        public static string Execute(string command, string dir) {
            string output = string.Empty;
            string error = string.Empty;
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
                Console.WriteLine("{0} has failed", command);
                Console.WriteLine("Error message:");
                Console.WriteLine("{0}", ex.Message);
                Console.WriteLine("-----------------------------------");
                return error;
            }
        }
    }
}