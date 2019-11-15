using Antd2.cmds;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class PartedCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                {"print", PrintFunc },
                {"mklabel", MklabelFunc },
                {"mkpart", SetPartitionFunc },
                {"name", SetNameFunc },
                {"rescue", RescueFunc },
                {"rm", RemoveFunc },
                {"resizepart", ResizeFunc },
                {"flag", FlagFunc },
                {"mkfs.ext4", MkfsExt4Func },
            };

        private static void PrintFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var disk = args[0];
            foreach (var l in Parted.Print(disk))
                Console.WriteLine(l);
        }

        private static void MklabelFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            Console.Write("  resetting the label may cause loss of data, continue this operation? (Y/n)");
            if (Console.ReadLine() != "Y")
                return;
            var disk = args[0];
            var label = args[1];
            Console.Write($"  you are setting '{label}' label on device {disk}, do you confirm this operation? (Y/n)");
            if (Console.ReadLine() != "Y")
                return;
            Parted.SetDiskLabel(disk, label);
        }

        private static void SetPartitionFunc(string[] args) {
            if (args.Length < 5) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var partType = args[1];

            string
                partName,
                fsType,
                start,
                end;

            if (args.Length == 6) {
                partName = args[2];
                fsType = args[3];
                start = args[4];
                end = args[5];
            }
            else {
                partName = "";
                fsType = args[2];
                start = args[3];
                end = args[4];
            }

            Parted.SetPartition(device, partType, partName, fsType, start, end);
        }

        private static void SetNameFunc(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var partitionNumber = args[1];
            var label = args[2];
            Parted.SetPartitionName(device, partitionNumber, label);
        }

        private static void RescueFunc(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var start = args[1];
            var end = args[2];
            Parted.RescuePartition(device, start, end);
        }

        private static void RemoveFunc(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            Console.Write("  removing this partition may cause loss of data, continue this operation? (Y/n)");
            if (Console.ReadLine() != "Y")
                return;
            var device = args[0];
            var partitionNumber = args[1];
            Console.Write($"  you trying to remove '{device}{partitionNumber}', do you confirm this operation? (Y/n)");
            if (Console.ReadLine() != "Y")
                return;
            Parted.RemovePartition(device, partitionNumber);
        }

        private static void ResizeFunc(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var partitionNumber = args[1];
            var end = args[2];
            Parted.ResizePartition(device, partitionNumber, end);
        }

        private static void FlagFunc(string[] args) {
            if (args.Length < 4) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var partitionNumber = args[1];
            var key = args[2];
            var value = args[3];
            Parted.SetPartitionFlag(device, partitionNumber, key, value);
        }

        private static void MkfsExt4Func(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            Console.Write("  setting a new label may cause loss of data, continue this operation? (Y/n)");
            if (Console.ReadLine() != "Y")
                return;
            var partition = args[0];
            var label = args[1];
            Console.Write($"  you are setting partition label '{label}' on partition {partition}, do you confirm this operation? (Y/n)");
            if (Console.ReadLine() != "Y")
                return;
            Mkfs.Ext4.AddLabel(partition, label);
        }

    }
}