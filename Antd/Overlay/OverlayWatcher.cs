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
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.config;

namespace Antd.Overlay {
    public class OverlayWatcher {
        public void StartWatching() {
            try {
                var path = Parameter.Overlay;
                var watcher = new FileSystemWatcher(path) {
                    NotifyFilter =
                        NotifyFilters.LastAccess |
                        NotifyFilters.LastWrite |
                        NotifyFilters.FileName |
                        NotifyFilters.DirectoryName,
                    IncludeSubdirectories = true
                };
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;
                watcher.EnableRaisingEvents = true;
            }
            catch(Exception ex) {
                ConsoleLogger.Log(ex.Message);
            }
        }

        public static Dictionary<string, string> ChangedDirectories { get; } = new Dictionary<string, string>();

        private static readonly Bash Bash = new Bash();

        private static readonly string[] Filter = {
            "cfg",
            "systemd",
            "etc",
            "journal"
        };

        private static void OnChanged(object source, FileSystemEventArgs e) {
            var directory = Path.GetDirectoryName(e.FullPath);
            if(directory != null && !ChangedDirectories.ContainsKey(directory) && !directory.ContainsAny(Filter)) {
                var du = Bash.Execute($"du -msh {directory}/").SplitToList("/").First();
                ChangedDirectories.Add(directory, du);
            }
        }

        private static void OnRenamed(object source, RenamedEventArgs e) {
            var directory = Path.GetDirectoryName(e.FullPath);
            if(directory != null && !ChangedDirectories.ContainsKey(directory) && !directory.ContainsAny(Filter)) {
                var du = Bash.Execute($"du -msh {directory}/").SplitToList("/").First();
                ChangedDirectories.Add(directory, du);
            }
        }

        private readonly MountManagement _mount = new MountManagement();

        public void SetOverlayDirectory(string overlayPath) {
            var overlayDir = Parameter.Overlay;
            var path = overlayPath.Replace(Parameter.Overlay, "");
            var dirsPath = MountHelper.SetDirsPath(path);
            Bash.Execute($"mkdir -p {dirsPath}", false);
            Bash.Execute($"rsync -aHA --delete-during {overlayDir}/ {dirsPath}/", false);
            Bash.Execute($"rm -fR {path}", false);
            _mount.Dir(path);
        }
    }
}