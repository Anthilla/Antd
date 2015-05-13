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
    public static class ConfigEtc {

        public static void Export(string filePath) {
            string txt = GetFileText(filePath);
            SetFile(filePath, txt);
        }

        private static string GetFileText(string _filePath) {
            string filePath = _filePath.RemoveDriveLetter();
            string text = FileSystem.ReadFile(filePath);
            SaveOriginalConf(filePath, text);
            return text;
        }

        private static void SetFile(string _filePath, string content) {
            string filePath = _filePath.RemoveDriveLetter();
            string newPath = Path.Combine("/cfg", filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            FileSystem.WriteFile(newPath, content);
            Command.Launch("mount", newPath + " " + filePath);
        }

        private static void SaveOriginalConf(string fileName, string content) {
            ConfigFileModel model = new ConfigFileModel();
            model._Id = Guid.NewGuid().ToString();
            model.path = fileName;
            model.content = content;
            model.version = 0;
            model.timestamp = Timestamp.Now;
            DeNSo.Session.New.Set(model);
        }

        private static string RemoveDriveLetter(this String fullPath) {
            string p;
            string[] split = fullPath.Split('/');
            string drive = split[0];
            if (drive.Contains(':')) {
                string[] resplit = fullPath.Split('/').Skip(1).ToArray();
                p = string.Join("/", resplit);
            }
            else {
                p = string.Join("/", split);
            }
            return "/" + p;
        }
    }
}
