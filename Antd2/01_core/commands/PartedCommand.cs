using Antd2.cmds;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class PartedCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                {"print", Print },
                {"mklabel", Mklabel },
                {"mkpart", SetPartition },
                {"name", SetName },
                {"rescue", Rescue },
                {"rm", Remove },
                {"resizepart", Resize },
                {"flag", Flag },
            };

        private static void Print(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var disk = args[0];
            foreach (var l in Parted.Print(disk))
                Console.WriteLine(l);
        }

        private static void Mklabel(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var disk = args[0];
            var label = args[2];
            Parted.SetDiskLabel(disk, label);
        }

        private static void SetPartition(string[] args) {
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

        private static void SetName(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var partitionNumber = args[1];
            var label = args[2];
            Parted.SetPartitionName(device, partitionNumber, label);
        }

        private static void Rescue(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var start = args[1];
            var end = args[2];
            Parted.RescuePartition(device, start, end);
        }

        private static void Remove(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var partitionNumber = args[1];
            Parted.RemovePartition(device, partitionNumber);
        }

        private static void Resize(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var device = args[0];
            var partitionNumber = args[1];
            var end = args[2];
            Parted.ResizePartition(device, partitionNumber, end);
        }

        private static void Flag(string[] args) {
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
    }
}