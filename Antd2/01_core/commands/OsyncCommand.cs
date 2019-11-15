using Antd2.Sync;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class OsyncCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "file", SyncFileFunc },
                { "dir", SyncDirFunc },
            };

        public static void SyncFileFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var source = args[0];
            var destination = args[1];
            Console.WriteLine($"osync {source} {destination}");
            OSync.SyncFiles(source, destination);
        }

        public static void SyncDirFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var source = args[0];
            var destination = args[1];
            Console.WriteLine($"osync {source} {destination}");
            OSync.SyncDirectories(source, destination);
        }
    }
}