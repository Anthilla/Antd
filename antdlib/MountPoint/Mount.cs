///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace antdlib.MountPoint {
    public class Mount {

        private static string[] defaultDirectories = new string[] {
                    Folder.Root,
                    Folder.Database
                };

        public static void WorkingDirectories() {
            ConsoleLogger.Info("  I try to mount these directories by default:");
            for (int i = 0; i < defaultDirectories.Length; i++) {
                ConsoleLogger.Info($"  > {defaultDirectories[i]}");
                var dir = defaultDirectories[i];
                var DIR = SetDIRSPath(dir);
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(DIR);
                if (IsAlreadyMounted(dir) == false) {
                    ConsoleLogger.Log($"    - mounting {DIR}");
                    SetBind(DIR, dir);
                }
                else {
                    ConsoleLogger.Log($"    - {DIR} already mounted");
                }
            }
        }

        public static void AllDirectories() {
            ConsoleLogger.Log("  Checking for saved mounts information:");
            if (MountRepository.Get().Length < 1) {
                ConsoleLogger.Log("    No mounts information found...");
                ConsoleLogger.Log("    I will load my default values!");
                for (int i = 0; i < defaultDirectories.Length; i++) {
                    MountRepository.Create(defaultDirectories[i], MountContext.Core, MountEntity.Directory);
                }
            }

            ConsoleLogger.Log("  Checking existing directories status:");
            CheckCurrentStatus();

            ConsoleLogger.Log("  Mounting directories...");
            var directoryMounts = MountRepository.Get().Where(m => m.MountEntity == MountEntity.Directory).ToArray();
            var y = (directoryMounts.Length == 1) ? "y" : "ies";
            ConsoleLogger.Log($"     Mounting {directoryMounts.Length} director{y}:");
            for (int i = 0; i < directoryMounts.Length; i++) {
                var dir = directoryMounts[i].Path.Replace("\\", "");
                var DIR = SetDIRSPath(dir);
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(DIR);
                ConsoleLogger.Info($"         {DIR} -> {dir}");
                if (IsAlreadyMounted(dir) == false) {
                    SetBind(DIR, dir);
                }
            }

            ConsoleLogger.Log("  Mounting files...");
            var fileMounts = MountRepository.Get().Where(m => m.MountEntity == MountEntity.File).ToArray();
            var s = (fileMounts.Length == 1) ? "" : "s";
            ConsoleLogger.Log($"     Mounting {fileMounts.Length} file{s}:");
            for (int i = 0; i < fileMounts.Length; i++) {
                var file = fileMounts[i].Path.Replace("\\", "");
                var FILE = SetFILESPath(file);
                if (!System.IO.File.Exists(FILE)) {
                    System.IO.File.Copy(file, FILE);
                }
                ConsoleLogger.Info($"         {FILE} -> {file}");
                if (IsAlreadyMounted(file) == false) {
                    SetBind(FILE, file);
                }
            }

            ConsoleLogger.Log($"     Checking detected directories status:");
            for (int i = 0; i < directoryMounts.Length; i++) {
                CheckMount(directoryMounts[i].Path);
            }
            ConsoleLogger.Log($"     Restartng associated systemd services:");
            for (int i = 0; i < directoryMounts.Length; i++) {
                var service = directoryMounts[i].AssociatedUnits;
                if (service.Count > 0) {
                    foreach (var srvc in service) {
                        Terminal.Execute($"systemctl restart {srvc}");
                    }
                }
            }
        }

        public static void CheckCurrentStatus() {
            var directories = Directory.EnumerateDirectories(Folder.Dirs, "DIR*", SearchOption.TopDirectoryOnly).ToArray();
            var y = (directories.Length == 1) ? "y" : "ies";
            ConsoleLogger.Log($"      {directories.Length} director{y} found in {Folder.Dirs}");
            for (int i = 0; i < directories.Length; i++) {
                var realPath = GetDIRSPath(directories[i]);
                ConsoleLogger.Log($"      {directories[i]} found, should be mounted under {realPath}");
                var mount = MountRepository.Get(realPath);
                if (mount == null) {
                    MountRepository.Create(realPath, MountContext.External, MountEntity.Directory);
                }
                ConsoleLogger.Log($"      Further check on {realPath}...");
                if (!Directory.Exists(realPath)) {
                    try {
                        ConsoleLogger.Log($"      {realPath} does not exists, copying content from {directories[i]}");
                        Terminal.Execute($"mkdir -p {directories[i]}");
                        Terminal.Execute($"mkdir -p {realPath}");
                        Terminal.Execute($"cp {directories[i]} {realPath}");
                        //FileSystem.CopyDirectory(directories[i], realPath);
                    }
                    catch (Exception) {
                    }
                }
            }

            var files = Directory.EnumerateFiles(Folder.Dirs, "FILE*", SearchOption.TopDirectoryOnly).ToArray();
            var s = (files.Length == 1) ? "" : "s";
            ConsoleLogger.Log($"      {files.Length} file{s} found in {Folder.Dirs}");
            for (int i = 0; i < files.Length; i++) {
                var realPath = GetFILESPath(files[i]);
                ConsoleLogger.Log($"      {files[i]} found, should be mounted under {realPath}");
                var mount = MountRepository.Get(realPath);
                if (mount == null) {
                    MountRepository.Create(realPath, MountContext.External, MountEntity.File);
                }
                ConsoleLogger.Log($"      Further check on {realPath}...");
                if (!System.IO.File.Exists(realPath)) {
                    try {
                        ConsoleLogger.Log($"      {realPath} does not exists, copying content from {files[i]}");
                        var path = files[i].GetAllStringsButLast('/');
                        var PATH = realPath.GetAllStringsButLast('/');
                        Terminal.Execute($"mkdir -p {path}");
                        Terminal.Execute($"mkdir -p {PATH}");
                        Terminal.Execute($"cp {files[i]} {realPath}");
                        //System.IO.File.Copy(files[i], realPath);
                    }
                    catch (Exception) {
                    }
                }
            }
        }

        public static void Check() {
            var mounts = MountRepository.Get();
            if (mounts.Length > 0) {
                for (int i = 0; i < mounts.Length; i++) {
                    ConsoleLogger.Log($"         {mounts[i].Path}:");
                    CheckMount(mounts[i].Path);
                }
            }
        }

        public static void Dir(string directory) {
            MountRepository.Create(directory, MountContext.External, MountEntity.Directory);
            var DIR = SetDIRSPath(directory);
            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(DIR);
            SetBind(DIR, directory);
            Check();
        }

        public static void File(string file) {
            MountRepository.Create(file, MountContext.External, MountEntity.File);
            var FILE = SetFILESPath(file);
            SetBind(FILE, file);
        }

        private static void CheckMount(string directory) {
            ConsoleLogger.Log($">>     check: {directory}");
            var isMntd = IsAlreadyMounted(directory);
            ConsoleLogger.Log($">>     is {directory} already mounted? {isMntd}");
            var mntDirectory = SetDIRSPath(directory);
            string timestampNow = Timestamp.Now;
            DFP.Set(mntDirectory, timestampNow);
            DFP.Set(directory, timestampNow);
            var dirsTimestamp = DFP.GetTimestamp(mntDirectory);
            bool dirsDFP = (dirsTimestamp == null) ? false : true;
            var directoryTimestamp = DFP.GetTimestamp(directory);
            bool directoryDFP = (directoryTimestamp == null) ? false : true;
            if (isMntd == true && directoryTimestamp == "unauthorizedaccessexception" && dirsTimestamp == "unauthorizedaccessexception") {
                ConsoleLogger.Log($"             unauthorizedaccessexception");
                MountRepository.SetAsMountedReadOnly(directory);
            }
            else if (isMntd == true && dirsDFP == true && directoryDFP == true) {
                if (dirsTimestamp == directoryTimestamp) {
                    ConsoleLogger.Success($"             mounted");
                    MountRepository.SetAsMounted(directory, mntDirectory);
                }
                else {
                    ConsoleLogger.Log($"             mounted, but on a different directory");
                    MountRepository.SetAsDifferentMounted(directory);
                }
            }
            else if (isMntd == false && dirsDFP == true && directoryDFP == false) {
                ConsoleLogger.Log($"             not mounted");
                MountRepository.SetAsNotMounted(directory);
            }
            else if (isMntd == true && dirsDFP == false && directoryDFP == true) {
                ConsoleLogger.Log($"             tmp mounted");
                MountRepository.SetAsTMPMounted(directory);
            }
            else if (isMntd == false && dirsDFP == false && directoryDFP == false) {
                ConsoleLogger.Log($"             error");
                MountRepository.SetAsError(directory);
            }
            else {
                ConsoleLogger.Warn($"             unknown error");
                MountRepository.SetAsError(directory);
            }
            DFP.Delete(mntDirectory);
            DFP.Delete(directory);
        }

        private static void SetBind(string source, string destination) {
            ConsoleLogger.Log($"    Check if {source} is already mounted...");
            if (IsAlreadyMounted(source, destination) == true) {
                ConsoleLogger.Log($"     {source} is already mounted!");
            }
            else {
                ConsoleLogger.Log($"      Mounting: {source}");
                Terminal.Execute($"mount -o bind {source} {destination}");
            }
        }

        public static string SetDIRSPath(string source) {
            return $"{Folder.Dirs}/DIR{source.Replace("_", "__").Replace("/", "_").Replace("\\", "/")}";
        }

        public static string GetDIRSPath(string source) {
            var result0 = source.Replace(Folder.Dirs, "").Replace("DIR", "").Replace("_", "/").Replace("__", "_");
            //todo fix this -> 1) sostituisco gli _ singoli con / poi __ con _
            var result1 = new Regex("[^_](_)[^_]").Replace(result0, "/");
            var result2 = new Regex("_{2,}").Replace(result1, "_");
            return result2.Replace("\\", "/").Replace("//", "/");
        }

        private static string SetLiveCDPath(string source) {
            return Path.Combine(Folder.LiveCd, source).Replace("\\", "/");
        }

        public static bool IsAlreadyMounted(string directory) {
            var df = Terminal.Execute($"df | grep w \"{directory}\"");
            var pm = Terminal.Execute($"cat /proc/mounts | grep w \"{directory}\"");
            return (df.Length > 0 || pm.Length > 0) ? true : false;
        }

        public static bool IsAlreadyMounted(string source, string destination) {
            var Sdf = Terminal.Execute($"df | grep -w \"{source}\"");
            var Spm = Terminal.Execute($"cat /proc/mounts | grep -w \"{source}\"");
            var Ddf = Terminal.Execute($"df | grep -w \"{destination}\"");
            var Dpm = Terminal.Execute($"cat /proc/mounts | grep -w \"{destination}\"");
            return (Sdf.Length > 0 || Spm.Length > 0 || Ddf.Length > 0 || Dpm.Length > 0) ? true : false;
        }

        public static string SetFILESPath(string source) {
            return $"{Folder.Dirs}/FILE{source.Replace("/", "_").Replace("\\", "/").Replace("__", "_")}";
        }

        public static string GetFILESPath(string source) {
            return source.Replace(Folder.Dirs, "").Replace("FILE", "").Replace("_", "/").Replace("\\", "/").Replace("//", "/");
        }

        private static int umount1Retry = 0;
        public static void Umount(string directory) {
            if (IsAlreadyMounted(directory) == true && umount1Retry < 5) {
                Terminal.Execute($"umount {directory}");
                umount1Retry = umount1Retry + 1;
                Umount(directory);
            }
            umount1Retry = 0;
        }

        private static int umount2Retry = 0;
        public static void Umount(string source, string destination) {
            if (IsAlreadyMounted(source, destination) == true && umount1Retry < 5) {
                Terminal.Execute($"umount {source}");
                Terminal.Execute($"umount {destination}");
                umount2Retry = umount2Retry + 1;
                Umount(source, destination);
            }
            umount1Retry = 0;
        }
    }
}
