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
using antd.commands;
using antdlib.common;
using Antd.Rsync;

namespace Antd {
    public class DirectoryWatcher {
        private readonly RsyncObjectModel[] _directoriesModel;
        private readonly string[] _paths;
        private FileSystemWatcher _fsw;

        public DirectoryWatcher(RsyncObjectModel[] paths) {
            _directoriesModel = paths;
            _paths = paths.Select(_ => _.Type == "file" ? Path.GetDirectoryName(_.Source) : _.Source).ToArray();
        }

        public void StartWatching() {
            ConsoleLogger.Log("[watcher] start");
            try {
                foreach(var path in _paths) {
                    if(!Directory.Exists(path) && !File.Exists(path)) {
                        continue;
                    }
                    _fsw = new FileSystemWatcher(path) {
                        NotifyFilter =
                            NotifyFilters.LastAccess |
                            NotifyFilters.LastWrite |
                            NotifyFilters.FileName |
                            NotifyFilters.DirectoryName,
                        IncludeSubdirectories = true,
                    };
                    _fsw.Changed += OnChanged;
                    _fsw.Created += OnChanged;
                    _fsw.Deleted += OnChanged;
                    _fsw.Renamed += OnRenamed;
                    _fsw.EnableRaisingEvents = true;
                }
            }
            catch(Exception ex) {
                ConsoleLogger.Log(ex.Message);
            }
        }

        public void Stop() {
            ConsoleLogger.Log("[watcher] stop");
            _fsw?.Dispose();
        }

        private void OnChanged(object source, FileSystemEventArgs e) {
            ConsoleLogger.Log($"[watcher] change at {e.FullPath}");
            var files = _directoriesModel.Where(_ => _.Source == e.FullPath).ToList();
            if(!files.Any()) {
                var parent = Path.GetDirectoryName(e.FullPath);
                var dirs = _directoriesModel.Where(_ => _.Source == parent).ToList();
                if(!dirs.Any()) {
                    return;
                }
                foreach(var dir in dirs) {
                    if(dir.Type == "directory") {
                        var src = dir.Source.EndsWith("/") ? dir.Source : dir.Source + "/";
                        var dst = dir.Destination.EndsWith("/") ? dir.Destination : dir.Destination + "/";
                        var launcher = new CommandLauncher();
                        launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                    }
                    if(dir.Type == "file") {
                        var src = dir.Source.EndsWith("/") ? dir.Source.TrimEnd('/') : dir.Source;
                        var dst = dir.Destination.EndsWith("/") ? dir.Destination.TrimEnd('/') : dir.Destination;
                        var launcher = new CommandLauncher();
                        launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                    }
                }
            }
            foreach(var file in files) {
                if(file.Type == "directory") {
                    var src = file.Source.EndsWith("/") ? file.Source : file.Source + "/";
                    var dst = file.Destination.EndsWith("/") ? file.Destination : file.Destination + "/";
                    var launcher = new CommandLauncher();
                    launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                }
                if(file.Type == "file") {
                    var src = file.Source.EndsWith("/") ? file.Source.TrimEnd('/') : file.Source;
                    var dst = file.Destination.EndsWith("/") ? file.Destination.TrimEnd('/') : file.Destination;
                    var launcher = new CommandLauncher();
                    launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                }
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e) {
            ConsoleLogger.Log($"[watcher] change at {e.FullPath}");
            var files = _directoriesModel.Where(_ => _.Source == e.FullPath).ToList();
            if(!files.Any()) {
                var parent = Path.GetDirectoryName(e.FullPath);
                var dirs = _directoriesModel.Where(_ => _.Source == parent).ToList();
                if(!dirs.Any()) {
                    return;
                }
                foreach(var dir in dirs) {
                    if(dir.Type == "directory") {
                        var src = dir.Source.EndsWith("/") ? dir.Source : dir.Source + "/";
                        var dst = dir.Destination.EndsWith("/") ? dir.Destination : dir.Destination + "/";
                        var launcher = new CommandLauncher();
                        launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                    }
                    if(dir.Type == "file") {
                        var src = dir.Source.EndsWith("/") ? dir.Source.TrimEnd('/') : dir.Source;
                        var dst = dir.Destination.EndsWith("/") ? dir.Destination.TrimEnd('/') : dir.Destination;
                        var launcher = new CommandLauncher();
                        launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                    }
                }
            }
            foreach(var file in files) {
                if(file.Type == "directory") {
                    var src = file.Source.EndsWith("/") ? file.Source : file.Source + "/";
                    var dst = file.Destination.EndsWith("/") ? file.Destination : file.Destination + "/";
                    var launcher = new CommandLauncher();
                    launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                }
                if(file.Type == "file") {
                    var src = file.Source.EndsWith("/") ? file.Source.TrimEnd('/') : file.Source;
                    var dst = file.Destination.EndsWith("/") ? file.Destination.TrimEnd('/') : file.Destination;
                    var launcher = new CommandLauncher();
                    launcher.Launch("rsync", new Dictionary<string, string> { { "$source", src }, { "$destination", dst } });
                }
            }
        }
    }
}