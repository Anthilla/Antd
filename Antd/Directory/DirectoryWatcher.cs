using Antd.Log;
using System;
using System.IO;

namespace Antd {
    public class DirectoryWatcher {

        private string path;

        public DirectoryWatcher(string _path) {
            path = _path;
        }

        public void Watch() {
            FileSystemWatcher watcher = new FileSystemWatcher(path);
            //watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
            //   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = true;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e) {
            //<<<<<<<<<<TODO>>>>>>>>>>>>
            //if file = text lo posso leggere
            //tell to signalr
            //distingui r/rw/eccetera
            //recupera IDseriale della macchina
            //<<<<<<<<<<OOOO>>>>>>>>>>>>
            LogRepo.Create(Timestamp.Now, e.ChangeType.ToString(), e.FullPath);
            //Console.WriteLine(Timestamp.Now  + " File: " + e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e) {
            var o = e.OldName;
            var n = e.Name;
            LogRepo.Create(Timestamp.Now, e.ChangeType.ToString(), e.FullPath, e.OldName);
            //Console.WriteLine(Timestamp.Now + " File: {0} renamed to {1}", o, n);
        }
    }
}
