using antd.core;
using System;
using System.IO;

namespace Antd2.cmds {

    public class Qemu {

        private const string qemuImgCommand = "qemu-img";
        private const string qemuImgDiskExtension = ".qed";

        /// <summary>
        /// qemu-img create vhd001.qed 16G
        /// </summary>
        /// <param name="diskFilepath"></param>
        /// <param name="size"></param>
        public static void CreateVirtualDisk(string diskFilepath, string size) {
            if (!diskFilepath.EndsWith(qemuImgDiskExtension)) {
                diskFilepath = CommonString.Append(diskFilepath, qemuImgDiskExtension);
            }
            if (File.Exists(diskFilepath)) {
                Console.WriteLine($"[qemu] disk creation skipped because a disk with this name already exists");
                return;
            }
            var args = CommonString.Append("create ", diskFilepath, " -f qed ", size);
            var cmd = $"{qemuImgCommand} {args}";
            Bash.Do(cmd);
        }
    }
}


