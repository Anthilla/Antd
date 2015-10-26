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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdlib.Config {
    public class ConfigManagement {
        private static string configFolder = Folder.Config;

        public static IEnumerable<string> Contexts {
            get {
                return Directory.EnumerateDirectories(configFolder).Where(d => !d.StartsWith("disabled."));
            }
        }

        public static IEnumerable<string> GetContextFiles(string contextName) {
            try {
                var fullPath = Path.Combine(configFolder, contextName);
                if (!Contexts.Contains(fullPath)) {
                    throw new Exception();
                }
                return Directory.EnumerateFiles(fullPath, ".cfg", SearchOption.TopDirectoryOnly).Where(d => !d.StartsWith("disabled.")).OrderBy(f => f);
            }
            catch (Exception ex) {
                ConsoleLogger.Warn($"No files found for this configuration context: {contextName}");
                ConsoleLogger.Warn(ex.Message);
                return new List<string>() { };
            }
        }

        public static void ApplyForAll() {
            try {
                foreach (var context in Contexts) {
                    ApplyConfigForContext(context);
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
            }
        }

        public static void ApplyConfigForContext(string contextName) {
            try {
                var files = GetContextFiles(contextName);
                if (files.Count() < 1) {
                    throw new Exception();
                }
                foreach (var file in files) {
                    LaunchConfigurationForFile(file);
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Warn($"There is nothing to apply for this configuration context: {contextName}");
                ConsoleLogger.Warn(ex.Message);
            }
        }

        private static void LaunchConfigurationForFile(string filename) {
            try {
                if (!File.Exists(filename)) {
                    throw new Exception();
                }
                var lines = File.ReadAllLines(filename);
                if (lines.Count() < 1) {
                    throw new Exception();
                }
                foreach (var line in lines) {
                    try {
                        Terminal.Execute(line);
                    }
                    catch (Exception) {
                        ConsoleLogger.Warn($"Error while executing: {line}");
                    }
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Warn($"Cannot apply configuration stored in: {filename}");
                ConsoleLogger.Warn($"The file may not exists or it may be empty");
                ConsoleLogger.Warn(ex.Message);
            }
        }

        public static void SaveConfiguration(string contextName, string file, IEnumerable<string> lines) {
            try {
                if (lines.Count() < 1) {
                    throw new Exception();
                }
                var contextPath = Path.Combine(configFolder, contextName);
                if (!Directory.Exists(contextPath)) {
                    Directory.CreateDirectory(contextPath);
                }
                if (!file.EndsWith(".cfg")) {
                    file = file + ".cfg";
                }
                var filePath = Path.Combine(contextPath, file);
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                }
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex) {
                ConsoleLogger.Warn($"Cannot save {file} configuration for {contextName}");
                ConsoleLogger.Warn(ex.Message);
            }
        }
    }
}
