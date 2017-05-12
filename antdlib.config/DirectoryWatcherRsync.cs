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

using antdlib.common;
using antdlib.models;
using anthilla.commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.config {
    public class DirectoryWatcherRsync {
        private static List<RsyncObjectModel> _directories;

        private static List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        public static void Start(List<RsyncObjectModel> newDirectories = null) {
            var conf = RsyncConfiguration.Get();
            if(conf == null) {
                return;
            }
            _directories = conf.Directories;
            if(newDirectories != null) {
                _directories = newDirectories;
            }

            var paths = _directories.Select(_ => _.Type == "file" ? Path.GetDirectoryName(_.Source) : _.Source).ToArray();
            ConsoleLogger.Log("[watcher rsync] start");
            try {
                if(!_watchers.Any()) {
                    foreach(var w in _watchers) {
                        w.Dispose();
                    }
                }
                foreach(var path in paths) {
                    if(!Directory.Exists(path) && !File.Exists(path)) {
                        continue;
                    }
                    var fsw = new FileSystemWatcher(path) {
                        NotifyFilter = NotifyFilters.LastWrite,
                        IncludeSubdirectories = true,
                        EnableRaisingEvents = true
                    };

                    fsw.Changed += FileChanged;
                    _watchers.Add(fsw);
                }
            }
            catch(Exception ex) {
                ConsoleLogger.Log(ex.Message);
            }
        }

        private static void FileChanged(object source, FileSystemEventArgs e) {
            if(!_directories.Any()) {
                return;
            }
            ConsoleLogger.Log($"[watcher rsync] change at {e.FullPath}");
            var files = _directories.Where(_ => _.Source == e.FullPath).ToList();
            if(!files.Any()) {
                ConsoleLogger.Log($"[watcher rsync] applying {e.FullPath} change");
                var parent = Path.GetDirectoryName(e.FullPath);
                var dirs = _directories.Where(_ => _.Source == parent).ToList();
                if(!dirs.Any()) {
                    return;
                }
                foreach(var dir in dirs) {
                    if(dir.Type == "directory") {
                        var src = dir.Source.EndsWith("/") ? dir.Source : dir.Source + "/";
                        var dst = dir.Destination.EndsWith("/") ? dir.Destination : dir.Destination + "/";
                        ConsoleLogger.Log($"[watcher rsync] rsync, src {src}; dst {dst};");
                        CommandLauncher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                    }
                    if(dir.Type == "file") {
                        var src = dir.Source.EndsWith("/") ? dir.Source.TrimEnd('/') : dir.Source;
                        var dst = dir.Destination.EndsWith("/") ? dir.Destination.TrimEnd('/') : dir.Destination;
                        ConsoleLogger.Log($"[watcher rsync] rsync, src {src}; dst {dst};");
                        CommandLauncher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                    }
                }
            }
            foreach(var file in files) {
                if(file.Type == "directory") {
                    var src = file.Source.EndsWith("/") ? file.Source : file.Source + "/";
                    var dst = file.Destination.EndsWith("/") ? file.Destination : file.Destination + "/";
                    ConsoleLogger.Log($"[watcher rsync] rsync, src {src}; dst {dst};");
                    CommandLauncher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                }
                if(file.Type == "file") {
                    var src = file.Source.EndsWith("/") ? file.Source.TrimEnd('/') : file.Source;
                    var dst = file.Destination.EndsWith("/") ? file.Destination.TrimEnd('/') : file.Destination;
                    ConsoleLogger.Log($"[watcher rsync] rsync, src {src}; dst {dst};");
                    CommandLauncher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                }
            }
        }
    }
}