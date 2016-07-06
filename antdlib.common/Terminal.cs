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

namespace antdlib.common {
    public class Terminal {
        public string Execute(string command, string dir = "") {
            var output = string.Empty;
            var error = string.Empty;
            if (!Parameter.IsUnix)
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
            if (!string.IsNullOrEmpty(dir)) {
                process.StartInfo.WorkingDirectory = dir;
            }
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
                ConsoleLogger.Error($"Failed to execute '{command}': {ex.Message}");
                process.Close();
                return error;
            }
        }

        public string Execute(IEnumerable<string> commands, string dir = "") {
            var genericOutput = string.Empty;
            if (!Parameter.IsUnix)
                return genericOutput;
            foreach (var command in commands) {
                var process = new Process {
                    StartInfo = {
                            FileName = "bash",
                            Arguments = "-c \"" + command + "\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                        }

                };
                if (!string.IsNullOrEmpty(dir)) {
                    process.StartInfo.WorkingDirectory = dir;
                }
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
                    ConsoleLogger.Error($"Failed to execute '{command}': {ex.Message}");
                }
            }
            return genericOutput;
        }
    }
}