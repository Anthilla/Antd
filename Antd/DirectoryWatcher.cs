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
using System.IO;
using antdlib.common;
using Antd.Database;

namespace Antd {
    public class DirectoryWatcher {
        public void StartWatching() {
            try {
                var paths = new RsyncRepository().GetDirectoriesToWatch();
                foreach (var path in paths) {
                    if (!Directory.Exists(path) && !File.Exists(path)) continue;
                    var watcher = new FileSystemWatcher(path) {
                        NotifyFilter =
                            NotifyFilters.LastAccess |
                            NotifyFilters.LastWrite |
                            NotifyFilters.FileName |
                            NotifyFilters.DirectoryName,
                        IncludeSubdirectories = true,
                    };
                    watcher.Changed += OnChanged;
                    watcher.Created += OnChanged;
                    watcher.Deleted += OnChanged;
                    watcher.Renamed += OnRenamed;
                    watcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception ex) {
                ConsoleLogger.Log(ex.Message);
            }
        }

        private static void OnChanged(object source, FileSystemEventArgs e) {
            new RsyncRepository().SyncDirectories(e.FullPath);
            ConsoleLogger.Log($"directory Watcher: {e.FullPath} {e.ChangeType}");
        }

        private static void OnRenamed(object source, RenamedEventArgs e) {
            new RsyncRepository().SyncDirectories(e.FullPath);
            ConsoleLogger.Log($"directory Watcher: {e.OldName} renamed to {e.Name}");
        }
    }
}