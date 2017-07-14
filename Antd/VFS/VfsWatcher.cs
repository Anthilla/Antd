using antdlib.common;
using antdlib.models;
using anthilla.core;
using Kvpbase;
using KvpbaseSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.VFS {
    public class VfsWatcher {

        private List<FileSystemWatcher> _watchers;
        private List<NodeModel> _nodes;

        public void Start(Cluster.Configuration config, List<NodeModel> nodes) {
            var dirs = config.FileSystemMapping.Select(_ => _.LocalPath);
            foreach(var dir in dirs) {
                try {
                    var watcher = new FileSystemWatcher(dir) {
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
                    _watchers.Add(watcher);
                }
                catch(Exception ex) {
                    ConsoleLogger.Log(ex.Message);
                }
            }
            _nodes = nodes;
        }

        public void Stop() {
            foreach(var watcher in _watchers) {
                watcher.Dispose();
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e) {
            Sync(e.FullPath);
        }

        private void OnRenamed(object source, RenamedEventArgs e) {
            Sync(e.FullPath);
        }

        private void Sync(string filePath) {
            string fileType = MimeTypes.GetFromExtension(Path.GetExtension(filePath));
            var data = File.ReadAllBytes(filePath);
            string containerPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            foreach(var node in _nodes) {
                //prendere la porta giusta
                var client = new Client("default", "default", $"http://{node.PublicIp}:8080");
                if(client == null) {
                    continue;
                }
                VFS.VfsClient.CreateObject(client, containerPath, fileType, data, fileName);
            }
        }
    }
}
