using anthilla.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.cmds {
    public static class Mount {

        private const string mountFileLocation = "/bin/mount";
        private const string umountFileLocation = "/bin/umount";
        private const string procMounts = "/proc/mounts";
        private const string bind = "-o bind";

        /// <summary>
        /// Ottiene la lista dei montaggi (es: /proc/mounts)
        /// </summary>
        /// <returns></returns>
        public static MountElement[] Get() {
            if(!File.Exists(procMounts)) {
                return new MountElement[0];
            }
            var result = File.ReadAllLines(procMounts);
            var mounts = new MountElement[result.Length];
            for(var i = 0; i < result.Length; i++) {
                var currentLine = result[i];
                var currentLineData = currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                mounts[i] = new MountElement() {
                    Active = true,
                    Bind = false,
                    Folder = currentLineData[0],
                    Device = currentLineData[0],
                    MountPoint = currentLineData[1],
                    FileSystem = currentLineData[2],
                    MountOptions = currentLineData[3]
                };
            }
            return mounts;
        }

        /// <summary>
        /// Ottiene la lista dei file e delle cartelle in /mnt/cdrom/DIRS e gestisce il loro montaggio
        /// </summary>
        public static void Set() {
            var running = Application.RunningConfiguration.Storage.Mounts;
            if(running.Length < 1) {
                return;
            }
            var directories = Directory.EnumerateDirectories(Parameter.RepoDirs, "DIR*", SearchOption.TopDirectoryOnly).ToArray();
            for(var i = 0; i < directories.Length; i++) {
                var currentDirectory = directories[i];
                var targetDirectory = MountHelper.ConvertDirectoryDirsPathToTarget(currentDirectory);
                if(MountHelper.IsAlreadyMounted(targetDirectory) == false) {
                    if(Directory.Exists(currentDirectory)) {
                        ConsoleLogger.Log($"[mount] {currentDirectory} -> {targetDirectory}");
                        Directory.CreateDirectory(targetDirectory);
                        MountWithBind(currentDirectory, targetDirectory);
                    }
                }
            }
            var files = Directory.EnumerateFiles(Parameter.RepoDirs, "FILE*", SearchOption.TopDirectoryOnly).ToArray();
            for(var i = 0; i < files.Length; i++) {
                var currentFile = files[i];
                var targetFile = MountHelper.ConvertFileDirsPathToTarget(currentFile);
                if(MountHelper.IsAlreadyMounted(targetFile) == false) {
                    ConsoleLogger.Log($"[mount] {currentFile} -> {targetFile}");
                    MountWithBind(currentFile, targetFile);
                }
            }
        }

        public static void MountSimple(string source, string destination) {
            var args = CommonString.Append(source, " ", destination);
            CommonProcess.Do(mountFileLocation, args);
        }

        public static void MountWithBind(string source, string destination) {
            var args = CommonString.Append(bind, " ", source, " ", destination);
            CommonProcess.Do(mountFileLocation, args);
        }

        public static void Umount(string mountPoint) {
            CommonProcess.Do(umountFileLocation, mountPoint);
        }

        /// <summary>
        /// Monta "automaticamente" una cartella per venire gestita da Antd, seguendo la struttura del sistema operativo di Anthilla
        /// </summary>
        /// <param name="mountPoint">In realtà è il percorso della cartella che vogliamo spostare in /mnt/cdrom/DIRS </param>
        public static void AutoMountDirectory(string mountPoint) {
            var source = MountHelper.ConvertDirectoryTargetPathToDirs(mountPoint);
            Directory.CreateDirectory(mountPoint);
            Directory.CreateDirectory(source);
            MountWithBind(source, mountPoint);
        }

        /// <summary>
        /// Monta "automaticamente" un file per venire gestito da Antd, seguendo la struttura del sistema operativo di Anthilla
        /// </summary>
        public static void AutoMountFile(string mountPoint) {
            var source = MountHelper.ConvertFileTargetPathToDirs(mountPoint);
            MountWithBind(source, mountPoint);
        }

        private static readonly string[] DefaultWorkingDirectories = { Parameter.AntdCfg, Parameter.EtcSsh, Parameter.Root };

        private static readonly Dictionary<string, string> DefaultWorkingDirectoriesWithOptions = new Dictionary<string, string> {
            {"/dev/shm", "-o remount,nodev,nosuid,mode=1777"},
            {"/hugepages", "-o mode=1770,gid=78 -t hugetlbfs hugetlbfs"},
            {"/sys/kernel/dlm", "-o default -t ocfs2_dlmfs dlm"}
        };

        public static void WorkingDirectories() {
            if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware") == false) {
                MountSimple("/mnt/cdrom/Kernel/active-firmware", "/lib64/firmware");
            }
            var kernelRelease = Bash.Execute("uname -r").Trim();
            var linkedRelease = Bash.Execute("file /mnt/cdrom/Kernel/active-modules").Trim();
            if(MountHelper.IsAlreadyMounted("/mnt/cdrom/Kernel/active-modules") == false &&
                linkedRelease.Contains(kernelRelease)) {
                var moduleDir = $"/lib64/modules/{kernelRelease}/";
                DirectoryWithAcl.CreateDirectory(moduleDir);
                MountSimple("/mnt/cdrom/Kernel/active-modules", moduleDir);
            }

            Bash.Execute("systemctl restart systemd-modules-load.service", false);
            foreach(var dir in DefaultWorkingDirectories) {
                var mntDir = MountHelper.ConvertDirectoryTargetPathToDirs(dir);
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(mntDir);
                if(MountHelper.IsAlreadyMounted(dir))
                    continue;
                ConsoleLogger.Log($"[mount] {mntDir} -> {dir}");
                MountWithBind(mntDir, dir);
            }
            foreach(var kvp in DefaultWorkingDirectoriesWithOptions) {
                if(MountHelper.IsAlreadyMounted(kvp.Key) == false) {
                    MountSimple(kvp.Value, kvp.Key);
                }
            }
        }
    }
}
