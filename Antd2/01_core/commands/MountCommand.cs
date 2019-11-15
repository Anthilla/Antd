using Antd2.cmds;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class MountCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "view", ViewFunc },
                { "simple", MountSimpleFunc },
                { "bind", MountBindFunc },
                { "umount", UmountFunc },
            };

        public static void ViewFunc(string[] args) {
            var mounts = Mount.Get();
            Console.WriteLine("  Device\tMountPoint\tFileSystem\t");
            Console.WriteLine();
            foreach (var m in mounts)
                Console.WriteLine($"  {m.Device}\t{m.MountPoint}\t{m.FileSystem}\t");
            Console.WriteLine();
        }

        public static void MountSimpleFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var source = args[0];
            var destination = args[1];
            Mount.MountSimple(source, destination);
        }

        public static void MountBindFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var source = args[0];
            var destination = args[1];
            Mount.MountWithBind(source, destination);
        }

        public static void UmountFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var source = args[0];
            Mount.Umount(source);
        }
    }
}