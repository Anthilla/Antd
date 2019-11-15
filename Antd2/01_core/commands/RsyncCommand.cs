using Antd2.cmds;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class RsyncCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "sync", SyncFunc },
                { "archive", ArchiveFunc },
                { "custom", SyncCustomFunc },
            };

        public static void SyncFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var source = args[0];
            var destination = args[1];
            Rsync.Sync(source, destination);
        }

        public static void ArchiveFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var source = args[0];
            var destination = args[1];
            Rsync.SyncArchive(source, destination);
        }

        public static void SyncCustomFunc(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var options = args[0];
            var source = args[1];
            var destination = args[2];
            Rsync.SyncCustom(options, source, destination);
        }
    }
}