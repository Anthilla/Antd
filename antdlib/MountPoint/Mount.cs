//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System;
using antdlib.Common;
using antdlib.Log;
using Common.Logging.Simple;

namespace antdlib.MountPoint {
    public class Mount {

        private static readonly string[] DefaultWorkingDirectories = { Parameter.AntdCfg };

        public static void WorkingDirectories() {
            foreach (var dir in DefaultWorkingDirectories) {
                var mntDir = SetDirsPath(dir);
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(mntDir);
                if (IsAlreadyMounted(dir))
                    continue;
                ConsoleLogger.Log($"mounting {mntDir}");
                SetBind(mntDir, dir);
            }
        }

        private static readonly string[] DefaultOverlayDirectories = {
            "/usr/share/.mono",
            "/var/lib/samba",
            "/var/log/journal",
            "/var/log/nginx",
            "/var/www/ca",
        };

        public static void OverlayDirectories() {
            foreach (var dir in DefaultOverlayDirectories) {
                Dir(dir);
                Terminal.Terminal.Execute($"rsync {Parameter.Overlay}/{dir} {dir}");
            }
        }

        public static void AllDirectories() {
            ConsoleLogger.Log("Checking for saved mounts information:");
            if (!MountRepository.Get().Any()) {
                ConsoleLogger.Log("No mounts information found...");
                ConsoleLogger.Log("I will load my default values!");
                foreach (var t in DefaultWorkingDirectories) {
                    MountRepository.Create(t, MountContext.Core, MountEntity.Directory);
                }
            }

            ConsoleLogger.Log("Checking existing directories status:");
            CheckCurrentStatus();

            ConsoleLogger.Log("Mounting directories");
            var directoryMounts = MountRepository.Get().Where(m => m.MountEntity == MountEntity.Directory).ToArray();
            var y = directoryMounts.Length == 1 ? "y" : "ies";
            ConsoleLogger.Log($"Mounting {directoryMounts.Length} director{y}:");
            foreach (var t in directoryMounts) {
                try {
                    var dir = t.Path.Replace("\\", "");
                    var mntDir = SetDirsPath(dir);
                    Directory.CreateDirectory(dir);
                    Directory.CreateDirectory(mntDir);
                    ConsoleLogger.Log($"{mntDir} -> {dir}");
                    if (IsAlreadyMounted(dir) == false) {
                        SetBind(mntDir, dir);
                    }
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }

            ConsoleLogger.Log("Mounting files");
            var fileMounts = MountRepository.Get().Where(m => m.MountEntity == MountEntity.File).ToArray();
            var s = fileMounts.Length == 1 ? "" : "s";
            ConsoleLogger.Log($"Mounting {fileMounts.Length} file{s}:");
            foreach (var t in fileMounts) {
                var file = t.Path.Replace("\\", "");
                var mntFile = SetFilesPath(file);
                if (mntFile == null)
                    throw new ArgumentNullException(nameof(mntFile));
                if (!System.IO.File.Exists(mntFile))
                    continue;
                var path = file.GetAllStringsButLast('/');
                var mntPath = mntFile.GetAllStringsButLast('/');
                Terminal.Terminal.Execute($"mkdir -p {path}");
                Terminal.Terminal.Execute($"mkdir -p {mntPath}");
                if (!System.IO.File.Exists(file)) {
                    Terminal.Terminal.Execute($"cp {mntFile} {file}");
                }
                ConsoleLogger.Log($"{mntFile} -> {file}");
                if (IsAlreadyMounted(file) == false) {
                    SetBind(mntFile, file);
                }
            }

            ConsoleLogger.Log("checking detected directories status");
            foreach (var t in directoryMounts) {
                CheckMount(t.Path);
            }
            ConsoleLogger.Log("restarting associated services");
            foreach (var srvc in from t in directoryMounts select t.AssociatedUnits into service where service.Count > 0 from srvc in service select srvc) {
                Terminal.Terminal.Execute($"systemctl restart {srvc}");
            }
        }

        public static void CheckCurrentStatus() {
            ConsoleLogger.Point("current status prima di  EnumerateDirectories");
            var directories = Directory.EnumerateDirectories(Parameter.RepoDirs, "DIR*", SearchOption.TopDirectoryOnly).ToArray();
            var y = directories.Length == 1 ? "y" : "ies";
            ConsoleLogger.Log($"{directories.Length} director{y} found in {Parameter.RepoDirs}");
            foreach (var t in directories) {
                var realPath = GetDirsPath(t);
                ConsoleLogger.Log($"{t} found, should be mounted under {realPath}");
                var mount = MountRepository.Get(realPath);
                if (mount == null) {
                    MountRepository.Create(realPath, MountContext.External, MountEntity.Directory);
                }
                ConsoleLogger.Log($"Further check on {realPath}...");
                if (Directory.Exists(realPath))
                    continue;
                try {
                    ConsoleLogger.Log($"{realPath} does not exists, copying content from {t}");
                    Terminal.Terminal.Execute($"mkdir -p {t}");
                    Terminal.Terminal.Execute($"mkdir -p {realPath}");
                    Terminal.Terminal.Execute($"cp {t} {realPath}");
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }

            var files = Directory.EnumerateFiles(Parameter.RepoDirs, "FILE*", SearchOption.TopDirectoryOnly).ToArray();
            var s = files.Length == 1 ? "" : "s";
            ConsoleLogger.Log($"{files.Length} file{s} found in {Parameter.RepoDirs}");
            foreach (var t in files) {
                var realPath = GetFilesPath(t);
                ConsoleLogger.Log($"{t} found, should be mounted under {realPath}");
                var mount = MountRepository.Get(realPath);
                if (mount == null) {
                    MountRepository.Create(realPath, MountContext.External, MountEntity.File);
                }
                ConsoleLogger.Log($"Further check on {realPath}...");
                if (System.IO.File.Exists(realPath))
                    continue;
                try {
                    ConsoleLogger.Log($"{realPath} does not exists, copying content from {t}");
                    var path = t.GetAllStringsButLast('/');
                    var mntPath = realPath.GetAllStringsButLast('/');
                    Terminal.Terminal.Execute($"mkdir -p {path}");
                    Terminal.Terminal.Execute($"mkdir -p {mntPath}");
                    Terminal.Terminal.Execute($"cp {t} {realPath}");
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                }
            }
        }

        public static void Check() {
            var mounts = MountRepository.Get();
            var mountModels = mounts as MountModel[] ?? mounts.ToArray();
            if (!mountModels.Any())
                return;
            foreach (var t in mountModels) {
                ConsoleLogger.Log($"{t.Path}:");
                CheckMount(t.Path);
            }
        }

        public static void Dir(string directory) {
            MountRepository.Create(directory, MountContext.External, MountEntity.Directory);
            var mntDir = SetDirsPath(directory);
            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(mntDir);
            SetBind(mntDir, directory);
            Check();
        }

        public static void File(string file) {
            MountRepository.Create(file, MountContext.External, MountEntity.File);
            var mntFile = SetFilesPath(file);
            SetBind(mntFile, file);
        }

        private static void CheckMount(string directory) {
            ConsoleLogger.Log($">>     check: {directory}");
            var isMntd = IsAlreadyMounted(directory);
            ConsoleLogger.Log($">>     is {directory} already mounted? {isMntd}");
            var mntDirectory = SetDirsPath(directory);
            var timestampNow = Timestamp.Now;
            DFP.Set(mntDirectory, timestampNow);
            DFP.Set(directory, timestampNow);
            var dirsTimestamp = DFP.GetTimestamp(mntDirectory);
            var dirsDfp = dirsTimestamp != null;
            var directoryTimestamp = DFP.GetTimestamp(directory);
            var directoryDfp = directoryTimestamp != null;
            if (isMntd && directoryTimestamp == "unauthorizedaccessexception" && dirsTimestamp == "unauthorizedaccessexception") {
                ConsoleLogger.Log("unauthorizedaccessexception");
                MountRepository.SetAsMountedReadOnly(directory);
            }
            else if (isMntd && dirsDfp && directoryDfp) {
                if (dirsTimestamp == directoryTimestamp) {
                    ConsoleLogger.Log("mounted");
                    MountRepository.SetAsMounted(directory, mntDirectory);
                }
                else {
                    ConsoleLogger.Log("mounted, but on a different directory");
                    MountRepository.SetAsDifferentMounted(directory);
                }
            }
            else if (isMntd == false && dirsDfp && directoryDfp == false) {
                ConsoleLogger.Log("not mounted");
                MountRepository.SetAsNotMounted(directory);
            }
            else if (isMntd && dirsDfp == false && directoryDfp) {
                ConsoleLogger.Log("tmp mounted");
                MountRepository.SetAsTmpMounted(directory);
            }
            else if (isMntd == false && dirsDfp == false && directoryDfp == false) {
                ConsoleLogger.Log("error");
                MountRepository.SetAsError(directory);
            }
            else {
                MountRepository.SetAsError(directory);
            }
            DFP.Delete(mntDirectory);
            DFP.Delete(directory);
        }

        private static void SetBind(string source, string destination) {
            ConsoleLogger.Log($"Check if {source} is already mounted...");
            if (IsAlreadyMounted(source, destination)) {
                ConsoleLogger.Point($"is already mounted {source} {destination}");
                ConsoleLogger.Log($"{source} is already mounted!");
            }
            else {
                ConsoleLogger.Log($"Mounting: {source}");
                ConsoleLogger.Point($"mount -o bind {source} {destination}");
                Terminal.Terminal.Execute($"mount -o bind {source} {destination}");
            }
        }

        public static string SetDirsPath(string source) {
            return $"{Parameter.RepoDirs}/DIR{source.Replace("_", "__").Replace("/", "_").Replace("\\", "/")}";
        }

        public static string GetDirsPath(string source) {
            var result0 = source.Replace(Parameter.RepoDirs, "").Replace("DIR", "").Replace("_", "/").Replace("__", "_");
            //todo fix this -> 1) sostituisco gli _ singoli con / poi __ con _
            var result1 = new Regex("[^_](_)[^_]").Replace(result0, "/");
            var result2 = new Regex("_{2,}").Replace(result1, "_");
            return result2.Replace("\\", "/").Replace("//", "/");
        }

        public static string SetFilesPath(string source) {
            return $"{Parameter.RepoDirs}/FILE{source.Replace("/", "_").Replace("\\", "/").Replace("__", "_")}";
        }

        public static string GetFilesPath(string source) {
            return source.Replace(Parameter.RepoDirs, "").Replace("FILE", "").Replace("_", "/").Replace("\\", "/").Replace("//", "/");
        }

        public static bool IsAlreadyMounted(string directory) {
            var df = Terminal.Terminal.Execute($"df | grep {directory}");
            var pm = Terminal.Terminal.Execute($"cat /proc/mounts | grep {directory}");
            return df.Length > 0 || pm.Length > 0;
        }

        public static bool IsAlreadyMounted(string source, string destination) {
            return IsAlreadyMounted(source) || IsAlreadyMounted(destination);
        }

        private static int _umount1Retry;
        public static void Umount(string directory) {
            if (IsAlreadyMounted(directory) && _umount1Retry < 5) {
                ConsoleLogger.Log($"umount, retry #{_umount1Retry}");
                Terminal.Terminal.Execute($"umount {directory}");
                _umount1Retry = _umount1Retry + 1;
                Umount(directory);
            }
            _umount1Retry = 0;
        }

        private static int _umount2Retry;
        public static void Umount(string source, string destination) {
            if (IsAlreadyMounted(source, destination) && _umount1Retry < 5) {
                ConsoleLogger.Log($"umount, retry #{_umount2Retry}");
                Terminal.Terminal.Execute($"umount {source}");
                Terminal.Terminal.Execute($"umount {destination}");
                _umount2Retry = _umount2Retry + 1;
                Umount(source, destination);
            }
            _umount1Retry = 0;
        }
    }
}
