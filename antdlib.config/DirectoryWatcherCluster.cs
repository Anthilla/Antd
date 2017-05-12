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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.config.shared;
using Nancy;

namespace antdlib.config {
    public class DirectoryWatcherCluster {

        private static FileSystemWatcher _fileSystemWatcher;

        public static void Start() {
            if(_fileSystemWatcher != null) {
                return;
            }
            ConsoleLogger.Log("[watcher config] start");
            try {
                _fileSystemWatcher = new FileSystemWatcher(Parameter.RepoDirs) {
                    NotifyFilter = NotifyFilters.LastWrite,
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true
                };
                _fileSystemWatcher.Changed += FileChanged;
            }
            catch(Exception ex) {
                ConsoleLogger.Log(ex.Message);
            }
        }

        public static void Stop() {
            ConsoleLogger.Log("[watcher config] stop");
            _fileSystemWatcher?.Dispose();
        }

        private static void FileChanged(object source, FileSystemEventArgs e) {
            var syncMachineConfiguration = ClusterConfiguration.Get();
            if(syncMachineConfiguration == null) {
                return;
            }
            var machines = syncMachineConfiguration.Select(_ => _.IpAddress).ToList();
            if(!machines.Any()) {
                return;
            }
            var extension = Path.GetExtension(e.FullPath);
            if(string.IsNullOrEmpty(extension)) {
                return;
            }
            if(e.Name.Contains("cluster")) {
                return;
            }
            if(e.Name.Contains(".bck")) {
                return;
            }
            if(e.Name.ToLower().Contains("dir_root")) {
                return;
            }
            if(e.Name.StartsWith(".")) {
                return;
            }
            ConsoleLogger.Log($"[watcher config] change at {e.FullPath}");
            var file = e.FullPath;
            var text = File.ReadAllText(file);
            if(string.IsNullOrEmpty(text)) {
                return;
            }
            var data = new Dictionary<string, string> {
                { "File", file },
                { "Content", text }
            };
            var app = new AppConfiguration().Get();
            foreach(var machine in machines) {
                ConsoleLogger.Log($"[watcher config] send config to {machine}");
                var status = new ApiConsumer().Post($"http://{machine}:{app.AntdUiPort}/cluster/accept", data);
                ConsoleLogger.Warn(status != HttpStatusCode.OK
                    ? $"[watcher config] sync with {machine} failed"
                    : $"[watcher config] sync with {machine} ok");
            }
        }
    }
}