using System.IO;
using System.Linq;
using anthilla.core;
using System.Collections.Generic;
using Antd.models;

namespace Antd.cmds {

    public class DirectoryWatcher {

    }

    public class SetupWatcher {

        private static FileSystemWatcher _fileSystemWatcher;

        private const string setupFilename = "setup.conf";
        private const string comment = "#";

        public static void Start() {
            if(_fileSystemWatcher != null) {
                return;
            }
            ConsoleLogger.Log($"[watcher] start on '{Const.AntdCfgSetup}'");
            _fileSystemWatcher = new FileSystemWatcher(Const.AntdCfgSetup) {
                NotifyFilter = NotifyFilters.LastWrite,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            _fileSystemWatcher.Changed += FileChanged;
        }

        private static void FileChanged(object source, FileSystemEventArgs e) {
            var fileName = Path.GetFileName(e.FullPath);
            if(CommonString.AreEquals(fileName, setupFilename) == false) {
                return;
            }
            if(!File.Exists(e.FullPath)) {
                return;
            }
            ConsoleLogger.Log($"[watcher] file '{e.FullPath}' changed");
            var result = File.ReadAllLines(e.FullPath).Where(_ => !_.Contains(comment)).ToArray();
            SetupCommands.Import(result);
            SetupCommands.Set();
        }
    }

    public class RsyncWatcher {

        private static RsyncObjectModel[] _directories;
        private static readonly List<FileSystemWatcher> Watchers = new List<FileSystemWatcher>();

        public static void Start() {
            var conf = Application.CurrentConfiguration.Services.Rsync;
            if(conf == null) {
                return;
            }
            _directories = conf.Directories;
            var paths = _directories.Select(_ => _.Type == "file" ? Path.GetDirectoryName(_.Source) : _.Source).ToArray();
            ConsoleLogger.Log("[watcher rsync] start");
            if(!Watchers.Any()) {
                foreach(var w in Watchers) {
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
                Watchers.Add(fsw);
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
                        Rsync.SyncDeleteAfter(src, dst);
                    }
                    if(dir.Type == "file") {
                        var src = dir.Source.EndsWith("/") ? dir.Source.TrimEnd('/') : dir.Source;
                        var dst = dir.Destination.EndsWith("/") ? dir.Destination.TrimEnd('/') : dir.Destination;
                        ConsoleLogger.Log($"[watcher rsync] rsync, src {src}; dst {dst};");
                        Rsync.SyncDeleteAfter(src, dst);
                    }
                }
            }
            foreach(var file in files) {
                if(file.Type == "directory") {
                    var src = file.Source.EndsWith("/") ? file.Source : file.Source + "/";
                    var dst = file.Destination.EndsWith("/") ? file.Destination : file.Destination + "/";
                    ConsoleLogger.Log($"[watcher rsync] rsync, src {src}; dst {dst};");
                    Rsync.SyncDeleteAfter(src, dst);
                }
                if(file.Type == "file") {
                    var src = file.Source.EndsWith("/") ? file.Source.TrimEnd('/') : file.Source;
                    var dst = file.Destination.EndsWith("/") ? file.Destination.TrimEnd('/') : file.Destination;
                    ConsoleLogger.Log($"[watcher rsync] rsync, src {src}; dst {dst};");
                    Rsync.SyncDeleteAfter(src, dst);
                }
            }
        }
    }

    public class LibvirtWatcher {

        private const string virshEtcDirectory = "/etc/libvirt/qemu";
        private const string clusterCfgFolder = "/cfg/antd/cluster";
        private const string destinationFolderName = "DIR_etc_libvirt_qemu";
        private static FileSystemWatcher _fileSystemWatcher;

        public void Start() {
            if(_fileSystemWatcher != null) {
                return;
            }
            ConsoleLogger.Log($"[watcher] start on '{virshEtcDirectory}'");
            _fileSystemWatcher = new FileSystemWatcher(virshEtcDirectory) {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            _fileSystemWatcher.Changed += FileChanged;
        }

        public void Stop() {
            if(_fileSystemWatcher == null) {
                return;
            }
            _fileSystemWatcher.Dispose();
        }

        /// <summary>
        /// Quando un file viene modificato lo invio agli altri nodi del cluster
        /// E verrà salvato nella cartella /cfg/antd/cluster/{machineUid}/DIR_etc_libvirt_qemu
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void FileChanged(object source, FileSystemEventArgs e) {
            var fileName = Path.GetFileName(e.FullPath);
            if(!File.Exists(e.FullPath)) {
                return;
            }
            ConsoleLogger.Log($"[watcher] file '{e.FullPath}' changed");
            var nodes = Application.CurrentConfiguration.Cluster.Nodes;
            for(var i = 0; i < nodes.Length; i++) {
                if(CommonString.AreEquals(nodes[i].MachineUid, Application.CurrentConfiguration.Host.MachineUid.ToString()) == true) {
                    continue;
                }
                var destinationPath = CommonString.Append(clusterCfgFolder, "/", Application.CurrentConfiguration.Host.MachineUid.ToString(), "/", destinationFolderName, "/", e.Name);
                ConsoleLogger.Log($"sync '{e.FullPath}' -> '{destinationPath}'");
                StorageClient.CreateFile(nodes[i], e.FullPath, destinationPath);
            }
        }
    }
}