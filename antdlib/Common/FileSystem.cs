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
using System.IO;

namespace antdlib {

    public static class FileSystem {

        /// <summary>
        /// If file exists return content else return empty string
        /// </summary>
        /// <param name="directory">file location</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string ReadFile(string path) {
            if (File.Exists(path)) {
                return File.ReadAllText(path);
            }
            else {
                ConsoleLogger.Warn("Path '{0}' doesn't exist", path);
                return String.Empty;
            }
        }

        public static string ReadFile(string directory, string filename) {
            string path = Path.Combine(directory, filename);
            if (File.Exists(path)) {
                return File.ReadAllText(path);
            }
            else {
                ConsoleLogger.Warn("File {0} doesn't exist in {1}", filename, directory);
                return String.Empty;
            }
        }

        public static void WriteFile(string path, string content) {
            using (StreamWriter sw = File.CreateText(path)) {
                sw.Write(content);
            }
        }

        public static void WriteFile(string directory, string filename, string content) {
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, filename);
            using (StreamWriter sw = File.CreateText(path)) {
                sw.Write(content);
            }
        }

        public static void CopyDirectory(string source, string destination) {
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(source, destination));
            }
            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(source, destination), true);
            }
        }
    }
}