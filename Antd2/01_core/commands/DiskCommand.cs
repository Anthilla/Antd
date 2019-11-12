using Antd2.cmds;
using Antd2.Init;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class DiskCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                {"show", Show },
            };

        static void Show(string[] args) {
            var blkid = Blkid.Get();
            foreach (var disk in blkid) {
                Console.WriteLine($"  {disk.Partition}\t{disk.Type} {disk.Uuid} {disk.Label}");
            }

            Console.WriteLine();
            var df = Df.Get();
            foreach (var disk in df) {
                Console.WriteLine($"  {disk.FS}\t{disk.Avail}/{disk.Blocks} at {disk.Mountpoint}");
            }

            Console.WriteLine();
            var lsblk = Lsblk.Get();
            foreach (var disk in lsblk) {
                Console.WriteLine($"  {disk.Name}\t{disk.Size} at {disk.Mountpoint}");
            }
        }
    }
}