﻿using Antd2.cmds;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class DiskCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                {"show", ShowFunc },
                {"create", CreateFunc },
                {"delete", DeleteFunc },
                {"label", AddLabelFunc },
            };

        private static void ShowFunc(string[] args) {
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
                foreach (var l in disk.Children)
                    Console.WriteLine($"  {l.Name}\t{l.Size} at {l.Mountpoint}");
            }
        }

        private static void CreateFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var disk = args[0];
            var size = args[1];
            Console.WriteLine($"  create new partition on {disk} ({size})");
            Fdisk.CreatePrimaryPartition(disk, size);
        }

        private static void DeleteFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var disk = args[0];
            var number = args[1];
            Console.WriteLine($"  delete partition {disk}{number}");
            Fdisk.DeletePartition(disk, number);
        }

        private static void AddLabelFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var partition = args[0];
            var label = args[1];
            Console.WriteLine($"  partition {partition} LABEL={label}");
            Mkfs.Ext4.AddLabel(partition, label);
        }
    }
}