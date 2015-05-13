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

using Antd.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Antd.MachineStatus {
    public class ConfigEtc {

        public static void Export(string filePath) { 
            string txt = GetFileText(filePath);
            SetFile(filePath, txt);
            //todo mount dir or file...
        }

        private static string GetFileText(string filePath) {
            return FileSystem.ReadFile(filePath);
        }

        private static void SetFile(string filePath, string content) {
            string p;
            string[] split = filePath.Split('/');
            string drive = split[0];
            if (drive.Contains(':')) {
                string[] resplit = filePath.Split('/').Skip(1).ToArray();
                p = string.Join("/", resplit);
            }
            else {
                p = string.Join("/", split);
            }
            string path = Path.Combine("/cfg", p);

            FileAttributes attr = File.GetAttributes(filePath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                //is directory -> create directory
                Directory.CreateDirectory(path);
            }
            else {
                //is file -> get directory, create directory
                string d = Path.GetDirectoryName(path);
                Directory.CreateDirectory(d);
                FileSystem.WriteFile(path, content);
            }
        }
    }
}
