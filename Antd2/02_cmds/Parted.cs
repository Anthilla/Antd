using Antd2.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.cmds {

    /// <summary>
    /// https://linux.die.net/man/8/parted
    /// https://www.gnu.org/software/parted/manual/parted.html
    /// </summary>
    public static class Parted {

        private const string partedCommand = "parted";
        private const string flagOn = "on";
        private const string flagOff = "off";

        //public static IEnumerable<string> Print(string device) {
        //    var cmd = $"{partedCommand} -a optimal -s {device} -- print";
        //    Console.WriteLine(cmd);
        //    var lines = Bash.Execute(cmd);
        //    return lines;
        //}

        /// <summary>
        /// parted -m /dev/sdb print
        /// 
        /// /dev/sdb:4001GB:scsi:512:4096:gpt:ATA WDC WD40PURZ-85T:;
        /// 1:1049kB:4001GB:4001GB:zfs::;
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static PartedDiskModel Print(string device) {
            var cmd = $"{partedCommand} -m {device} -- print";
            Console.WriteLine(cmd);
            var lines = Bash.Execute(cmd).Skip(1).ToArray();

            var diskLine = lines[0];
            var diskInfo = ParsePartedLine_Disk(diskLine);

            var partitionInfo = lines.Skip(1)
                .Select(_ => ParsePartedLine_Partition(_))
                .ToArray();

            foreach (var pt in partitionInfo) {
                pt.Name = diskInfo.Name + pt.Name;
            }

            diskInfo.Partitions = partitionInfo;

            return diskInfo;
        }

        // /dev/sdb:4001GB:scsi:512:4096:gpt:ATA WDC WD40PURZ-85T:;
        // 0        1      2    3   4    5   6                    7
        private static PartedDiskModel ParsePartedLine_Disk(string line) {
            var arr = line.Split(new[] { ":" }, StringSplitOptions.None);
            return new PartedDiskModel(arr[0], arr[1], arr[2], arr[5], arr[6]);
        }

        // 1:1049kB:4001GB:4001GB:zfs::;
        // 0 1      2      3      4   56
        private static PartedPartitionModel ParsePartedLine_Partition(string line) {
            var arr = line.Split(new[] { ":" }, StringSplitOptions.None);
            return new PartedPartitionModel(arr[0], arr[1], arr[2], arr[4]);
        }

        // parted /dev/sdb -m print devices
        public static IEnumerable<string> GetDisks() {
            var cmd = $"{partedCommand} -l";
            Console.WriteLine(cmd);
            var lines = Bash.Execute(cmd)
                .Where(_ => _.StartsWith("Disk "))
                .Where(_ => _.Contains("/dev"))
                .Select(_ => _.Replace("Disk ", "").Trim().Split(':').FirstOrDefault().Trim())
                .ToArray();
            return lines;
        }

        /// <summary>
        /// parted -a optimal -s /dev/sdb -- mklabel gpt
        /// </summary>
        /// <param name="device"></param>
        /// <param name="diskLabel"></param>
        /// 
        public static void SetDiskLabel(string device, string diskLabel) {
            var cmd = $"{partedCommand} -a optimal -s {device} -- mklabel {diskLabel}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }

        /// <summary>
        /// parted -a optimal -s /dev/sdb -- mkpart primary NAME ext3 1MiB 78MiB
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partType"></param>
        /// <param name="partName"></param>
        /// <param name="fsType"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void SetPartition(string device, string partType, string partName, string fsType, string start, string end) {
            var cmd = $"{partedCommand} -a optimal -s {device} -- mkpart {partType} {partName} {fsType} {start} {end}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }

        public static void SetPartitionName(string device, string partitionNumber, string label) {
            var cmd = $"{partedCommand} -a optimal -s {device} -- name {partitionNumber} '{label}'";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }

        public static void RescuePartition(string device, string start, string end) {
            var cmd = $"{partedCommand} -a optimal -s {device} -- rescue {start} {end}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }

        public static void RemovePartition(string device, string partitionNumber) {
            var cmd = $"{partedCommand} -a optimal -s {device} -- rm {partitionNumber}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }

        public static void ResizePartition(string device, string partitionNumber, string end) {
            var cmd = $"{partedCommand} -a optimal -s {device} -- resizepart {partitionNumber} {end}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }

        public static void SetPartitionFlag(string device, string partitionNumber, string key, string value) {
            var cmd = $"{partedCommand} -a optimal -s {device} -- set {partitionNumber} {key} {value}";
            Console.WriteLine(cmd);
            var result = Bash.Execute(cmd);
            foreach (var l in result)
                Console.WriteLine(l);
        }


        /// <summary>
        /// (GPT) - Enable this to record that the selected partition is a GRUB BIOS partition.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetBiosGrubOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "bios_grub", flagOn);
        }
        public static void SetBiosGrubOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "bios_grub", flagOff);
        }


        /// <summary>
        /// (GPT) - this flag is used to tell special purpose software that the GPT partition may be bootable.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetLegacyBootOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "legacy_boot", flagOn);
        }
        public static void SetLegacyBootOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "legacy_boot", flagOff);
        }


        /// <summary>
        /// (Mac, MS-DOS, PC98) - should be enabled if you want to boot off the partition. The semantics vary between disk labels.
        /// For MS-DOS disk labels, only one partition can be bootable. If you are installing LILO on a partition that partition must be bootable. 
        /// For PC98 disk labels, all ext2 partitions must be bootable (this is enforced by Parted).
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetBootOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "boot", flagOn);
        }
        public static void SetBootOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "boot", flagOff);
        }

        /// <summary>
        /// (GPT) - This flag identifies partitions that contain Microsoft filesystems (NTFS or FAT).
        /// It may optionally be set on Linux filesystems to mimic the type of configuration created by parted 3.0 and earlier, 
        /// in which a separate Linux filesystem type code was not available on GPT disks. 
        /// This flag can only be removed within parted by replacing it with a competing flag, such as boot or msftres.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetMsftdataOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "msftdata", flagOn);
        }
        public static void SetMsftdataOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "msftdata", flagOff);
        }

        /// <summary>
        /// (MS-DOS,GPT) - This flag identifies a "Microsoft Reserved" partition, which is used by Windows.
        /// Note that this flag should not normally be set on Windows filesystem partitions (those that contain NTFS or FAT filesystems).
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetMsftresOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "msftres", flagOn);
        }
        public static void SetMsftresOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "msftres", flagOff);
        }

        /// <summary>
        /// (MS-DOS, GPT) - this flag identifies an Intel Rapid Start Technology partition.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetIrstOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "irst", flagOn);
        }
        public static void SetIrstOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "irst", flagOff);
        }

        /// <summary>
        /// (MS-DOS, GPT) - this flag identifies a UEFI System Partition. On GPT it is an alias for boot.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetEspOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "esp", flagOn);
        }
        public static void SetEspOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "esp", flagOff);
        }

        /// <summary>
        /// (MS-DOS) - this flag can be enabled to tell MS DOS, MS Windows 9x and MS Windows ME based operating systems to use Linear (LBA) mode
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetLbaOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "lba", flagOn);
        }
        public static void SetLbaOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "lba", flagOff);
        }

        /// <summary>
        /// (Mac) - this flag should be enabled if the partition is the root device to be used by Linux.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetRootOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "root", flagOn);
        }
        public static void SetRootOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "root", flagOff);
        }


        /// <summary>
        /// (Mac) - this flag should be enabled if the partition is the swap device to be used by Linux.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetSwapOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "swap", flagOn);
        }
        public static void SetSwapOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "swap", flagOff);
        }

        /// <summary>
        /// (MS-DOS, PC98) - this flag can be enabled to hide partitions from Microsoft operating systems.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetHiddenOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "hidden", flagOn);
        }
        public static void SetHiddenOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "hidden", flagOff);
        }

        /// <summary>
        /// (MS-DOS) - this flag can be enabled to tell linux the partition is a software RAID partition.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetRaidOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "raid", flagOn);
        }
        public static void SetRaidOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "raid", flagOff);
        }

        /// <summary>
        /// (MS-DOS) - this flag can be enabled to tell linux the partition is a physical volume.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetLVMOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "LVM", flagOn);
        }
        public static void SetLVMOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "LVM", flagOff);
        }

        /// <summary>
        /// (MS-DOS) - this flag can be enabled so that the partition can be used by the Linux/PA-RISC boot loader, palo.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetPALOOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "PALO", flagOn);
        }
        public static void SetPALOOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "PALO", flagOff);
        }

        /// <summary>
        /// (MS-DOS, GPT) - this flag can be enabled so that the partition can be used as a PReP
        /// boot partition on PowerPC PReP or IBM RS6K/CHRP hardware.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetPREPOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "PREP", flagOn);
        }
        public static void SetPREPOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "PREP", flagOff);
        }

        /// <summary>
        /// (MS-DOS) - Enable this to indicate that a partition can be used as a diagnostics / recovery partition.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="partitionNumber"></param>
        public static void SetDIAGOn(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "DIAG", flagOn);
        }
        public static void SetDIAGOff(string device, string partitionNumber) {
            SetPartitionFlag(device, partitionNumber, "DIAG", flagOff);
        }
    }
}
