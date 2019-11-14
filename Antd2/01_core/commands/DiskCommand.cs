using Antd2.cmds;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class DiskCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                {"show", Show },
                {"create", Create },
                {"delete", Delete },
                {"label", AddLabel },
                {"format", Format },
            };

        private static void Show(string[] args) {
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

        private static void Create(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var disk = args[0];
            var size = args[1];
            Console.WriteLine($"  create new partition on {disk} ({size})");
            Fdisk.CreatePrimaryPartition(disk, size);
        }

        private static void Delete(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var disk = args[0];
            var number = args[1];
            Console.WriteLine($"  delete partition {disk}{number}");
            Fdisk.DeletePartition(disk, number);
        }

        private static void AddLabel(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var partition = args[0];
            var label = args[1];
            Console.WriteLine($"  partition {partition} LABEL={label}");
            Mkfs.Ext4.AddLabel(partition, label);
        }

        private static void Format(string[] args) {
            if (args.Length < 6) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var diskLabel = args[1];
            var partType = args[2];

            string
                partName,
                fsType,
                start,
                end;

            if (args.Length == 7) {
                partName = args[3];
                fsType = args[4];
                start = args[5];
                end = args[6];
            }
            else {
                partName = "";
                fsType = args[3];
                start = args[4];
                end = args[5];
            }

            Parted.FormatPartition(device, diskLabel, partType, partName, fsType, start, end);
        }
    }
}